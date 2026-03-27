using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using TestTH.Pages;

namespace TestTH.Utilities
{
    /// <summary>
    /// BaseTest — Class cơ sở cho tất cả test class.
    /// Kế thừa DriverFactory để sử dụng WebDriver.
    /// Tự động xử lý sau mỗi test case:
    ///   1. Kiểm tra kết quả test (PASS / FAIL)
    ///   2. Chụp screenshot tự động (cả PASS và FAIL)
    ///   3. Ghi kết quả ra file Excel (EPPlus)
    ///   4. Đóng browser (gọi base.TearDown)
    ///
    /// Cách sử dụng: Các test class chỉ cần kế thừa BaseTest thay vì DriverFactory.
    /// </summary>
    public class BaseTest : DriverFactory
    {
        private static readonly IReadOnlyDictionary<string, string> ExpectedResultByTestName =
            new Dictionary<string, string>
            {
                ["Login_ValidCredentials_Success"] = "Dang nhap thanh cong, hien thi Welcome va nut Log Out.",
                ["Logout_Success"] = "Dang xuat thanh cong, hien thi lai form Customer Login.",
                ["Register_ValidUser_Success"] = "Tao tai khoan thanh cong va tu dong dang nhap voi thong bao dung.",
                ["TransferFunds_Success"] = "Chuyen tien thanh cong, hien thi thong bao Transfer Complete!.",

                ["TC_FT_01_Find_ByTransactionId_ValidID"] = "Hien thi dung giao dich ung voi Transaction ID hop le.",
                ["TC_FT_02_Find_ByTransactionId_InvalidID"] = "Hien thi No transactions found hoac danh sach rong.",
                ["TC_FT_03_Find_ByTransactionId_EmptyID"] = "Bao loi yeu cau ID (Invalid transaction ID).",
                ["TC_FT_04_Find_ByDate_ValidDate"] = "Hien thi danh sach giao dich theo ngay hop le.",

                ["TC_UC_01_ProfileLoad_AutoLoadData"] = "Form Update Contact Info tu dong load dung thong tin hien tai.",
                ["TC_UC_02_ProfileUpdate_ValidData"] = "Cap nhat profile thanh cong, hien thi Profile Updated.",
                ["TC_UC_03_ProfileUpdate_EmptyFirstName"] = "Khong cho luu khi First Name rong, hien thi thong bao loi.",
                ["TC_UC_08_ProfileUpdate_EmptyZipCode"] = "Khong cho luu khi Zip Code rong, hien thi thong bao loi.",
                ["TC_UC_09_ProfileUpdate_AlphaZipCode_ShouldBeRejected"] = "Khong cho luu khi Zip Code chua ky tu chu, hien thi thong bao loi.",

                ["TC_RL_01_RequestLoan_EmptyFields"] = "Khi de trong truong bat buoc, he thong hien thi loi.",
                ["TC_RL_04_RequestLoan_Approved"] = "Yeu cau vay hop le duoc phe duyet (Approved)."
            };

        /// <summary>
        /// [SetUp] — Chuẩn bị trước mỗi test.
        /// DriverFactory.SetUp() đã được gọi trước đó (do NUnit gọi từ class cha lên).
        /// Tự động tạo và đăng nhập tài khoản test trước khi chạy test.
        /// </summary>
        [SetUp]
        public new void SetUp()
        {
            // Tự động tạo tài khoản và đăng nhập trước mỗi test
            AutoCreateAndLoginTestAccount();
        }

        /// <summary>
        /// [TearDown] — Xử lý sau mỗi test case:
        ///   1. Xác định PASS / FAIL từ NUnit TestContext
        ///   2. Chụp screenshot (cả PASS và FAIL)
        ///   3. Ghi kết quả vào Excel
        ///   4. Gọi base.TearDown() để đóng browser
        /// </summary>
        [TearDown]
        public new void TearDown()
        {
            // Lấy tên test hiện tại từ NUnit context
            string testName = TestContext.CurrentContext.Test.Name;

            // Lấy trạng thái test từ NUnit context
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

            string status;
            string screenshotPath = string.Empty;
            string expectedResult;
            string actualResult;

            string mappedExpected = GetMappedExpectedResult(testName);

            if (testStatus == TestStatus.Passed)
            {
                status = "PASS";
                expectedResult = mappedExpected;
                actualResult = "PASS - Ket qua thuc te phu hop voi expected result.";
            }
            else
            {
                status = "FAIL";

                // Lấy Expected Result và Actual Result từ NUnit assertion message
                string failMessage = TestContext.CurrentContext.Result.Message ?? "N/A";
                expectedResult = mappedExpected;
                actualResult = ExtractActual(failMessage);

                if (string.IsNullOrWhiteSpace(actualResult) || actualResult == "Khong xac dinh duoc ket qua thuc te.")
                {
                    actualResult = failMessage.Split('\n')[0].Trim();
                }
            }

            // Chụp screenshot cho cả PASS và FAIL
            try
            {
                screenshotPath = ScreenshotHelper.CaptureScreenshot(Driver, testName);
                TestContext.WriteLine($"[SCREENSHOT] {screenshotPath}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"[WARNING] Không thể chụp screenshot: {ex.Message}");
            }

            try
            {
                // Ghi kết quả test vào file Excel
                ExcelReportHelper.WriteResult(testName, expectedResult, actualResult, status, screenshotPath);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"[WARNING] Không thể ghi Excel: {ex.Message}");
            }

            // Gọi DriverFactory.TearDown() để đóng browser và giải phóng WebDriver
            base.TearDown();
        }

        private string GetMappedExpectedResult(string testName)
        {
            if (ExpectedResultByTestName.TryGetValue(testName, out string? expected))
            {
                return expected;
            }

            return "Expected result chua duoc dinh nghia cho test nay.";
        }

        /// <summary>
        /// Trích xuất "Expected" từ NUnit assertion message.
        /// NUnit thường format: "  Expected: ..."
        /// </summary>
        private string ExtractExpected(string message)
        {
            foreach (var line in message.Split('\n'))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("Expected:"))
                    return ExtractContentAfterColon(trimmed);
            }
            return message.Split('\n')[0].Trim();
        }

        /// <summary>
        /// Trích xuất "Actual" từ NUnit assertion message.
        /// NUnit thường format: "  But was: ..."
        /// </summary>
        private string ExtractActual(string message)
        {
            foreach (var line in message.Split('\n'))
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("But was:") || trimmed.StartsWith("Actual:"))
                    return ExtractContentAfterColon(trimmed);
            }
            return "Khong xac dinh duoc ket qua thuc te.";
        }

        /// <summary>
        /// Tach phan noi dung sau dau ':' de bo tien to nhu "Expected:" / "But was:".
        /// </summary>
        private string ExtractContentAfterColon(string text)
        {
            int colonIndex = text.IndexOf(':');
            if (colonIndex < 0 || colonIndex == text.Length - 1)
            {
                return text.Trim();
            }

            return text[(colonIndex + 1)..].Trim();
        }

        /// <summary>
        /// Tự động tạo tài khoản test với username/password ngẫu nhiên và đăng nhập.
        /// Mỗi lần chạy test sẽ tạo một tài khoản mới với credential ngẫu nhiên.
        /// </summary>
        private void AutoCreateAndLoginTestAccount()
        {
            try
            {
                // Tạo username và password ngẫu nhiên
                var credentials = GenerateRandomCredentials();
                string username = credentials.Username;
                string password = credentials.Password;
                
                // Thông tin user cơ bản
                string firstName = "Test";
                string lastName = "User";
                
                // Dữ liệu địa chỉ mặc định cho registration
                string address = "123 Main Street";
                string city = "TestCity";
                string state = "CA";
                string zipCode = "12345";
                string phone = "5551234567";
                string ssn = "123456789";

                // Truy cập trang Register
                Driver.Navigate().GoToUrl(BaseUrl + "/register.htm");

                // Tạo RegisterPage instance
                var registerPage = new RegisterPage(Driver);

                // Điền form Registration và submit
                registerPage.RegisterUser(
                    firstName,
                    lastName,
                    address,
                    city,
                    state,
                    zipCode,
                    phone,
                    ssn,
                    username,
                    password,
                    password  // Confirm password cùng với password
                );

                // Chờ đăng ký hoàn tất
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Url.Contains("index.htm") || d.Url.Contains("overview.htm"));

                TestContext.WriteLine($"[AUTO-REGISTRATION] Tài khoản '{username}' đã được tạo và đăng nhập thành công!");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, log cảnh báo
                TestContext.WriteLine($"[AUTO-REGISTRATION] Cảnh báo: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo username và password ngẫu nhiên.
        /// Format: testuser_<random> / testpass_<random>
        /// </summary>
        private (string Username, string Password) GenerateRandomCredentials()
        {
            string randomId = Guid.NewGuid().ToString().Substring(0, 8);
            string username = $"testuser_{randomId}";
            string password = $"testpass_{randomId}";
            return (username, password);
        }
    }
}
