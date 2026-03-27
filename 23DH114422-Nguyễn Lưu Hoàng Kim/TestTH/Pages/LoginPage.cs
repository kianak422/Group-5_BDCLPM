using OpenQA.Selenium;

namespace TestTH.Pages
{
    /// <summary>
    /// LoginPage đại diện cho trang đăng nhập ParaBank.
    /// Chứa các locator và action liên quan đến chức năng Login/Logout/Register.
    /// </summary>
    public class LoginPage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // Ô nhập username (nằm ở sidebar bên trái)
        private readonly By _txtUsername = By.Name("username");

        // Ô nhập password
        private readonly By _txtPassword = By.Name("password");

        // Nút "Log In"
        private readonly By _btnLogin = By.CssSelector("input.button[value='Log In']");

        // Link "Register" để chuyển đến trang đăng ký
        private readonly By _lnkRegister = By.LinkText("Register");

        // Link "Log Out" (hiển thị sau khi đăng nhập thành công)
        private readonly By _lnkLogout = By.LinkText("Log Out");

        // Thông báo chào mừng sau khi đăng nhập (VD: "Welcome John Smith")
        private readonly By _lblWelcome = By.CssSelector("#leftPanel p.smallText");

        // Panel chứa form đăng nhập (hiển thị khi chưa login hoặc sau logout)
        private readonly By _lblCustomerLogin = By.CssSelector("#leftPanel h2");

        // ==================== CONSTRUCTOR ====================

        /// <summary>
        /// Khởi tạo LoginPage với WebDriver instance.
        /// </summary>
        /// <param name="driver">WebDriver instance được truyền từ test class.</param>
        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Thực hiện đăng nhập với username và password.
        /// Nhập thông tin vào form và click nút Login.
        /// </summary>
        /// <param name="username">Tên đăng nhập.</param>
        /// <param name="password">Mật khẩu.</param>
        public void Login(string username, string password)
        {
            _driver.FindElement(_txtUsername).Clear();
            _driver.FindElement(_txtUsername).SendKeys(username);

            _driver.FindElement(_txtPassword).Clear();
            _driver.FindElement(_txtPassword).SendKeys(password);

            _driver.FindElement(_btnLogin).Click();
        }

        /// <summary>
        /// Click vào link "Register" để chuyển đến trang đăng ký tài khoản mới.
        /// </summary>
        public void ClickRegister()
        {
            _driver.FindElement(_lnkRegister).Click();
        }

        /// <summary>
        /// Click vào link "Log Out" để đăng xuất khỏi hệ thống.
        /// </summary>
        public void ClickLogout()
        {
            _driver.FindElement(_lnkLogout).Click();
        }

        // ==================== VERIFICATION METHODS ====================

        /// <summary>
        /// Kiểm tra nút "Log Out" có hiển thị hay không.
        /// Nếu hiển thị → đã đăng nhập thành công.
        /// </summary>
        /// <returns>true nếu nút Log Out visible, false nếu không.</returns>
        public bool IsLogoutButtonDisplayed()
        {
            try
            {
                return _driver.FindElement(_lnkLogout).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy text thông báo chào mừng sau khi đăng nhập (VD: "Welcome John Smith").
        /// </summary>
        /// <returns>Nội dung text chào mừng.</returns>
        public string GetWelcomeMessage()
        {
            return _driver.FindElement(_lblWelcome).Text;
        }

        /// <summary>
        /// Kiểm tra form đăng nhập (Customer Login) có hiển thị hay không.
        /// Nếu hiển thị → đã đăng xuất thành công hoặc chưa đăng nhập.
        /// </summary>
        /// <returns>true nếu form login visible, false nếu không.</returns>
        public bool IsLoginFormDisplayed()
        {
            try
            {
                var element = _driver.FindElement(_lblCustomerLogin);
                return element.Displayed && element.Text.Contains("Customer Login");
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
