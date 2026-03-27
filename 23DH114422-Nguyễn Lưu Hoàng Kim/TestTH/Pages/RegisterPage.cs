using OpenQA.Selenium;

namespace TestTH.Pages
{
    /// <summary>
    /// RegisterPage đại diện cho trang đăng ký tài khoản mới trên ParaBank.
    /// Chứa tất cả locator của form đăng ký và action RegisterUser().
    /// </summary>
    public class RegisterPage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // Thông tin cá nhân
        private readonly By _txtFirstName = By.Id("customer.firstName");
        private readonly By _txtLastName = By.Id("customer.lastName");

        // Thông tin địa chỉ
        private readonly By _txtAddress = By.Id("customer.address.street");
        private readonly By _txtCity = By.Id("customer.address.city");
        private readonly By _txtState = By.Id("customer.address.state");
        private readonly By _txtZipCode = By.Id("customer.address.zipCode");

        // Thông tin liên hệ
        private readonly By _txtPhone = By.Id("customer.phoneNumber");
        private readonly By _txtSsn = By.Id("customer.ssn");

        // Thông tin tài khoản đăng nhập
        private readonly By _txtUsername = By.Id("customer.username");
        private readonly By _txtPassword = By.Id("customer.password");
        private readonly By _txtConfirmPassword = By.Id("repeatedPassword");

        // Nút "Register"
        private readonly By _btnRegister = By.CssSelector("input.button[value='Register']");

        // Tiêu đề chào mừng sau khi đăng ký thành công (VD: "Welcome testuser_xxx")
        private readonly By _lblWelcomeTitle = By.CssSelector("#rightPanel h1.title");

        // Thông báo đăng ký thành công
        private readonly By _lblSuccessMessage = By.CssSelector("#rightPanel p");

        // ==================== CONSTRUCTOR ====================

        /// <summary>
        /// Khởi tạo RegisterPage với WebDriver instance.
        /// </summary>
        /// <param name="driver">WebDriver instance được truyền từ test class.</param>
        public RegisterPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Điền đầy đủ thông tin vào form đăng ký và submit.
        /// </summary>
        /// <param name="firstName">Tên.</param>
        /// <param name="lastName">Họ.</param>
        /// <param name="address">Địa chỉ đường.</param>
        /// <param name="city">Thành phố.</param>
        /// <param name="state">Bang/Tỉnh.</param>
        /// <param name="zipCode">Mã bưu điện.</param>
        /// <param name="phone">Số điện thoại.</param>
        /// <param name="ssn">Số an sinh xã hội (SSN).</param>
        /// <param name="username">Tên đăng nhập mới.</param>
        /// <param name="password">Mật khẩu.</param>
        /// <param name="confirmPassword">Xác nhận mật khẩu.</param>
        public void RegisterUser(
            string firstName,
            string lastName,
            string address,
            string city,
            string state,
            string zipCode,
            string phone,
            string ssn,
            string username,
            string password,
            string confirmPassword)
        {
            // Điền thông tin cá nhân
            _driver.FindElement(_txtFirstName).SendKeys(firstName);
            _driver.FindElement(_txtLastName).SendKeys(lastName);

            // Điền địa chỉ
            _driver.FindElement(_txtAddress).SendKeys(address);
            _driver.FindElement(_txtCity).SendKeys(city);
            _driver.FindElement(_txtState).SendKeys(state);
            _driver.FindElement(_txtZipCode).SendKeys(zipCode);

            // Điền thông tin liên hệ
            _driver.FindElement(_txtPhone).SendKeys(phone);
            _driver.FindElement(_txtSsn).SendKeys(ssn);

            // Điền tài khoản đăng nhập
            _driver.FindElement(_txtUsername).SendKeys(username);
            _driver.FindElement(_txtPassword).SendKeys(password);
            _driver.FindElement(_txtConfirmPassword).SendKeys(confirmPassword);

            // Click nút Register
            _driver.FindElement(_btnRegister).Click();
        }

        // ==================== VERIFICATION METHODS ====================

        /// <summary>
        /// Lấy tiêu đề chào mừng sau khi đăng ký thành công (VD: "Welcome testuser_xxx").
        /// </summary>
        /// <returns>Nội dung tiêu đề chào mừng.</returns>
        public string GetWelcomeTitle()
        {
            return _driver.FindElement(_lblWelcomeTitle).Text;
        }

        /// <summary>
        /// Lấy thông báo đăng ký thành công.
        /// Expected: "Your account was created successfully. You are now logged in."
        /// </summary>
        /// <returns>Nội dung thông báo thành công.</returns>
        public string GetSuccessMessage()
        {
            return _driver.FindElement(_lblSuccessMessage).Text;
        }
    }
}
