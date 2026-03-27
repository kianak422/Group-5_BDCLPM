using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;

namespace TestTH.Utilities
{
    /// <summary>
    /// ExcelReportHelper — Ghi kết quả test vào file Excel bằng EPPlus.
    /// File báo cáo: D:\Dowload\Book1.xlsx
    /// Các cột: TestName | Expected Result | Actual Result | Status | Screenshot
    /// Screenshot được nhúng trực tiếp vào ô Excel (cả PASS và FAIL).
    /// Mỗi lần ghi sẽ append thêm dòng mới vào sheet hiện tại.
    /// Nếu file chưa tồn tại → tạo mới với header.
    /// </summary>
    public static class ExcelReportHelper
    {
        // Đường dẫn file Excel báo cáo
        private static readonly string ReportFilePath = @"D:\TestTH\TESTEXEL.xlsx";
        private const string ReportWorksheetName = "Test Results";

        // Lock object để tránh xung đột khi nhiều test ghi đồng thời
        private static readonly object _lock = new object();

        // Chiều cao mặc định của dòng chứa ảnh (pixel)
        private const int ImageHeight = 80;

        // Chiều rộng cột Screenshot (pixel)
        private const int ImageColumnWidth = 40;

        /// <summary>
        /// Static constructor — Thiết lập license EPPlus (bắt buộc từ v5+).
        /// </summary>
        static ExcelReportHelper()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Ghi một dòng kết quả test vào file Excel.
        /// Screenshot được nhúng trực tiếp vào ô Excel (cả PASS và FAIL).
        /// </summary>
        /// <param name="testName">Tên test case</param>
        /// <param name="expectedResult">Kết quả mong đợi</param>
        /// <param name="actualResult">Kết quả thực tế</param>
        /// <param name="status">Kết quả: PASS hoặc FAIL</param>
        /// <param name="screenshotPath">Đường dẫn screenshot</param>
        public static void WriteResult(string testName, string expectedResult, string actualResult, string status, string screenshotPath)
        {
            lock (_lock)
            {
                // Tạo thư mục nếu chưa tồn tại
                string? directory = Path.GetDirectoryName(ReportFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                FileInfo fileInfo = new FileInfo(ReportFilePath);

                using (var package = new ExcelPackage(fileInfo))
                {
                    // Luôn ghi vào sheet chuyên dụng để tránh dính format/template lạ từ Sheet1.
                    var worksheet = package.Workbook.Worksheets[ReportWorksheetName]
                                    ?? package.Workbook.Worksheets.Add(ReportWorksheetName);

                    EnsureHeaderExists(worksheet);

                    // Tìm dòng trống tiếp theo để ghi dữ liệu
                    int nextRow = Math.Max((worksheet.Dimension?.End.Row ?? 1) + 1, 2);

                    // Ghi dữ liệu vào các cột
                    // Cột 1: TestName | Cột 2: Expected Result | Cột 3: Actual Result | Cột 4: Status | Cột 5: Screenshot
                    worksheet.Cells[nextRow, 1].Value = testName;
                    worksheet.Cells[nextRow, 2].Value = expectedResult;
                    worksheet.Cells[nextRow, 3].Value = actualResult;
                    worksheet.Cells[nextRow, 4].Value = status;

                    // ── Nhúng ảnh trực tiếp vào cột Screenshot (cột 5) ──
                    if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                    {
                        // Tạo tên duy nhất cho ảnh trong workbook
                        string pictureName = $"Screenshot_{nextRow}_{DateTime.Now:HHmmssfff}";

                        // Thêm ảnh vào worksheet
                        var picture = worksheet.Drawings.AddPicture(pictureName, new FileInfo(screenshotPath));

                        // Đặt vị trí ảnh vào ô (nextRow, cột 5) — cột E
                        // EPPlus dùng 0-indexed cho cột trong SetPosition: cột 5 → index 4
                        picture.SetPosition(nextRow - 1, 2, 4, 5);

                        // Đặt kích thước ảnh vừa với ô
                        picture.SetSize(120, ImageHeight);

                        // Tăng chiều cao dòng để hiển thị ảnh
                        worksheet.Row(nextRow).Height = ImageHeight * 0.75; // px → pt (1pt ≈ 0.75px)
                    }
                    else
                    {
                        // Không có screenshot → ghi text "N/A"
                        worksheet.Cells[nextRow, 5].Value = "N/A";
                    }

                    // Tô màu status: xanh lá cho PASS, đỏ cho FAIL
                    var statusCell = worksheet.Cells[nextRow, 4];
                    if (status == "PASS")
                    {
                        statusCell.Style.Font.Color.SetColor(Color.Green);
                        statusCell.Style.Font.Bold = true;
                    }
                    else if (status == "FAIL")
                    {
                        statusCell.Style.Font.Color.SetColor(Color.Red);
                        statusCell.Style.Font.Bold = true;
                    }

                    // Auto-fit cột (trừ cột Screenshot)
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();

                    // Đặt chiều rộng cố định cho cột Screenshot
                    worksheet.Column(5).Width = ImageColumnWidth;

                    // Lưu file
                    package.Save();
                }
            }
        }

        private static void EnsureHeaderExists(ExcelWorksheet worksheet)
        {
            bool hasHeader = worksheet.Cells[1, 1].Text == "TestName"
                && worksheet.Cells[1, 2].Text == "Expected Result"
                && worksheet.Cells[1, 3].Text == "Actual Result"
                && worksheet.Cells[1, 4].Text == "Status"
                && worksheet.Cells[1, 5].Text == "Screenshot";

            if (!hasHeader)
            {
                CreateHeader(worksheet);
            }
        }

        /// <summary>
        /// Tạo header row cho file Excel mới.
        /// Header: TestName | Expected Result | Actual Result | Status | Screenshot
        /// </summary>
        private static void CreateHeader(ExcelWorksheet worksheet)
        {
            // Ghi tên cột
            worksheet.Cells[1, 1].Value = "TestName";
            worksheet.Cells[1, 2].Value = "Expected Result";
            worksheet.Cells[1, 3].Value = "Actual Result";
            worksheet.Cells[1, 4].Value = "Status";
            worksheet.Cells[1, 5].Value = "Screenshot";

            // Style header: bold, nền xám, viền
            using (var headerRange = worksheet.Cells[1, 1, 1, 5])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
        }
    }
}
