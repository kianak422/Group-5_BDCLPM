using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// LoginTests chứa các Smoke Test cho chức năng đăng nhập/đăng xuất.
    /// TC1: Login thành công với dữ liệu hợp lệ từ JSON.
    /// TC2: Logout thành công sau khi đăng nhập.
    /// </summary>
    [TestFixture]
    public class LoginTests : DriverFactory
    {
        private LoginPage _loginPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();  // Call DriverFactory.SetUp() to initialize driver
            _loginPage = new LoginPage(Driver);
        }

        /// <summary>
        /// TC1: Login_ValidCredentials_Success
        /// Mục tiêu: Đăng nhập thành công với username/password hợp lệ từ file JSON.
        /// Expected: Nút "Log Out" hiển thị và có thông báo "Welcome".
        /// </summary>
        [Test]
        public void Login_ValidCredentials_Success()
        {
            // Arrange - Lấy thông tin user từ file JSON
            string username = JsonReader.GetUserField("username");
            string password = JsonReader.GetUserField("password");

            // Act - Truy cập trang chủ và đăng nhập
            Driver.Navigate().GoToUrl(BaseUrl + "/index.htm");
            _loginPage.Login(username, password);

            // Assert 1 - Kiểm tra nút Log Out hiển thị (xác nhận đã login thành công)
            bool isLogoutDisplayed = _loginPage.IsLogoutButtonDisplayed();
            Assert.That(isLogoutDisplayed, Is.True,
                $"FAILED: Logout button should be displayed. Actual: {isLogoutDisplayed}");

            // Assert 2 - Kiểm tra thông báo chào mừng có chứa "Welcome"
            string welcomeMessage = _loginPage.GetWelcomeMessage();
            Assert.That(welcomeMessage, Does.Contain("Welcome"),
                $"FAILED: Welcome message must contain 'Welcome'. Actual message: '{welcomeMessage}'");
        }

        /// <summary>
        /// TC2: Logout_Success
        /// Mục tiêu: Đăng nhập rồi đăng xuất, kiểm tra quay về trang Customer Login.
        /// Expected: Form login hiển thị lại sau khi logout.
        /// </summary>
        [Test]
        public void Logout_Success()
        {
            // Arrange - Đăng nhập trước
            string username = JsonReader.GetUserField("username");
            string password = JsonReader.GetUserField("password");

            Driver.Navigate().GoToUrl(BaseUrl + "/index.htm");
            _loginPage.Login(username, password);

            // Act - Click Logout
            _loginPage.ClickLogout();

            // Assert - Kiểm tra form đăng nhập "Customer Login" hiển thị lại
            bool isLoginFormDisplayed = _loginPage.IsLoginFormDisplayed();
            Assert.That(isLoginFormDisplayed, Is.True,
                $"FAILED: Login form must be displayed after logout. Actual: {isLoginFormDisplayed}");
        }
    }
}
