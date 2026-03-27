using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// RegisterTests chứa Functional Test cho chức năng đăng ký tài khoản.
    /// TC4: Đăng ký tài khoản mới thành công.
    /// </summary>
    [TestFixture]
    public class RegisterTests : DriverFactory
    {
        private LoginPage _loginPage = null!;
        private RegisterPage _registerPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();  // Call DriverFactory.SetUp() to initialize driver
            _loginPage = new LoginPage(Driver);
            _registerPage = new RegisterPage(Driver);
        }

        /// <summary>
        /// TC4: Register_ValidUser_Success
        /// Mục tiêu: Đăng ký tài khoản mới với đầy đủ thông tin hợp lệ.
        /// Expected: Thông báo "Your account was created successfully. You are now logged in."
        /// </summary>
        [Test]
        public void Register_ValidUser_Success()
        {
            // Arrange - Tạo username duy nhất để tránh trùng lặp
            string uniqueId = Guid.NewGuid().ToString("N")[..10];
            string newUsername = $"testuser_{uniqueId}";
            string password = "Test@123";

            // Act - Truy cập trang đăng ký
            Driver.Navigate().GoToUrl(BaseUrl + "/index.htm");
            _loginPage.ClickRegister();

            // Điền form đăng ký với dữ liệu hợp lệ
            _registerPage.RegisterUser(
                firstName: "Test",
                lastName: "User",
                address: "123 Main Street",
                city: "Ho Chi Minh",
                state: "HCM",
                zipCode: "70000",
                phone: "0901234567",
                ssn: "123-45-6789",
                username: newUsername,
                password: password,
                confirmPassword: password
            );

            // Assert 1 - Kiểm tra tiêu đề chào mừng chứa "Welcome" và username
            string welcomeTitle = _registerPage.GetWelcomeTitle();
            Assert.That(welcomeTitle, Does.Contain("Welcome"),
                $"FAILED: Welcome title must contain 'Welcome'. Actual: '{welcomeTitle}'");
            Assert.That(welcomeTitle, Does.Contain(newUsername),
                $"FAILED: Welcome title must contain username '{newUsername}'. Actual: '{welcomeTitle}'");

            // Assert 2 - Kiểm tra thông báo đăng ký thành công chính xác
            string successMessage = _registerPage.GetSuccessMessage();
            string expectedMessage = "Your account was created successfully. You are now logged in.";
            Assert.That(successMessage, Is.EqualTo(expectedMessage),
                $"FAILED: Success message mismatch.\nExpected: '{expectedMessage}'\nActual: '{successMessage}'");
        }
    }
}
