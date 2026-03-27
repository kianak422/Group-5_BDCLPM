using OpenQA.Selenium;

namespace TestTH.Utilities
{
    /// <summary>
    /// ScreenshotHelper — Chụp ảnh màn hình khi test FAIL.
    /// Sử dụng Selenium ITakesScreenshot để capture toàn bộ trang.
    /// Ảnh được lưu dưới dạng PNG với tên: TestName_yyyyMMdd_HHmmss.png
    /// </summary>
    public static class ScreenshotHelper
    {
        // Thư mục lưu screenshot khi test FAIL
        private static readonly string ScreenshotDirectory =
            @"C:\Users\Kim\OneDrive\Pictures\Screenshots";

        /// <summary>
        /// Chụp screenshot từ WebDriver và lưu vào thư mục đã cấu hình.
        /// </summary>
        /// <param name="driver">WebDriver instance đang chạy</param>
        /// <param name="testName">Tên test case (dùng đặt tên file)</param>
        /// <returns>Đường dẫn đầy đủ tới file screenshot đã lưu</returns>
        public static string CaptureScreenshot(IWebDriver driver, string testName)
        {
            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(ScreenshotDirectory))
            {
                Directory.CreateDirectory(ScreenshotDirectory);
            }

            // Tạo tên file: TestName_yyyyMMdd_HHmmss.png
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{testName}_{timestamp}.png";
            string filePath = Path.Combine(ScreenshotDirectory, fileName);

            // Chụp screenshot thông qua interface ITakesScreenshot
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(filePath);

            return filePath;
        }
    }
}
