using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TestTH.Utilities
{
    /// <summary>
    /// DriverFactory chịu trách nhiệm khởi tạo và quản lý vòng đời của WebDriver.
    /// Sử dụng NUnit [SetUp] / [TearDown] để tự động mở và đóng trình duyệt cho mỗi test.
    /// Các test class chỉ cần kế thừa DriverFactory để sử dụng WebDriver.
    /// </summary>
    public class DriverFactory
    {
        // WebDriver instance dùng chung cho các test trong class con
        protected IWebDriver Driver { get; private set; } = null!;

        // URL gốc của ứng dụng ParaBank
        protected const string BaseUrl = "https://parabank.parasoft.com/parabank";

        /// <summary>
        /// [SetUp] - Chạy trước mỗi test case.
        /// Khởi tạo ChromeDriver với các tùy chọn tối ưu cho automation testing.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Cấu hình Chrome options
            var chromeOptions = new ChromeOptions();

            // Chạy ở chế độ maximize để thấy đầy đủ giao diện
            chromeOptions.AddArgument("--start-maximized");

            // Tắt thông báo "Chrome is being controlled by automated test software"
            chromeOptions.AddExcludedArgument("enable-automation");

            // Tắt các popup không cần thiết
            chromeOptions.AddArgument("--disable-notifications");
            chromeOptions.AddArgument("--disable-popup-blocking");

            // Khởi tạo ChromeDriver
            Driver = new ChromeDriver(chromeOptions);

            // Thiết lập implicit wait mặc định (10 giây)
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Thiết lập page load timeout (30 giây)
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// [TearDown] - Chạy sau mỗi test case.
        /// Đóng trình duyệt và giải phóng tài nguyên WebDriver.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            try
            {
                if (Driver != null)
                {
                    // Đóng tất cả các tab/window và thoát trình duyệt
                    Driver.Quit();
                }
            }
            catch (Exception)
            {
                // Bỏ qua exception khi quit (trình duyệt có thể đã đóng)
            }
            finally
            {
                try
                {
                    // Giải phóng tài nguyên
                    Driver?.Dispose();
                }
                catch (Exception)
                {
                    // Bỏ qua exception khi dispose
                }

                // Đặt Driver = null để tránh double-dispose
                Driver = null!;
            }
        }
    }
}
