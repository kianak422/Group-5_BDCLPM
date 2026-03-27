using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestTH.Pages
{
    /// <summary>
    /// UpdateProfilePage đại diện cho trang cập nhật thông tin liên hệ.
    /// Hỗ trợ cập nhật Address, validate First Name và Zip Code.
    /// </summary>
    public class UpdateProfilePage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // Link menu điều hướng "Update Contact Info" (sidebar trái)
        private readonly By _lnkUpdateContactInfo = By.LinkText("Update Contact Info");

        // Ô nhập First Name
        private readonly By _txtFirstName = By.Id("customer.firstName");

        // Ô nhập Last Name
        private readonly By _txtLastName = By.Id("customer.lastName");

        // Ô nhập Address (đường)
        private readonly By _txtAddress = By.Id("customer.address.street");

        // Ô nhập City
        private readonly By _txtCity = By.Id("customer.address.city");

        // Ô nhập State
        private readonly By _txtState = By.Id("customer.address.state");

        // Ô nhập Zip Code
        private readonly By _txtZipCode = By.Id("customer.address.zipCode");

        // Ô nhập Phone
        private readonly By _txtPhone = By.Id("customer.phoneNumber");

        // Nút "Update Profile"
        private readonly By _btnUpdateProfile = By.CssSelector("input.button[value='Update Profile']");

        // Thông báo lỗi validation
        private readonly By _lblFirstNameError = By.XPath("//*[normalize-space()='First name is required.' or normalize-space()='First name is required']");
        private readonly By _lblZipCodeError = By.XPath("//*[normalize-space()='Zip Code is required.' or normalize-space()='Zip Code is required']");
        private readonly By _lblPhoneError = By.XPath("//*[contains(normalize-space(),'Phone')]");

        // Tiêu đề thông báo cập nhật thành công
        private readonly By _lblSuccessHeading = By.XPath("//h1[contains(normalize-space(),'Profile Updated')]");

        // ==================== CONSTRUCTOR ====================

        public UpdateProfilePage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Click vào link "Update Contact Info" trên menu sidebar trái để mở trang.
        /// </summary>
        public void ClickUpdateContactInfoMenu()
        {
            _driver.FindElement(_lnkUpdateContactInfo).Click();
        }

        /// <summary>
        /// Cập nhật địa chỉ mới và submit form.
        /// </summary>
        public void UpdateAddress(string address)
        {
            _driver.FindElement(_txtAddress).Clear();
            _driver.FindElement(_txtAddress).SendKeys(address);
            _driver.FindElement(_btnUpdateProfile).Click();
        }

        /// <summary>
        /// Xóa nội dung ô First Name để test validation.
        /// Chờ đợi ô textbox có chứa dữ liệu rồi mới xóa bằng Ctrl+A → Backspace.
        /// </summary>
        public void ClearFirstName()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            // Bước 1: Chờ field có dữ liệu (form load xong)
            wait.Until(d =>
            {
                var value = d.FindElement(_txtFirstName).GetAttribute("value");
                return !string.IsNullOrEmpty(value);
            });

            // Bước 2: Xóa field và verify đã xóa thật sự (retry nếu chưa trống)
            wait.Until(d =>
            {
                var element = d.FindElement(_txtFirstName);
                element.Click(); // Đảm bảo focus vào field
                element.SendKeys(Keys.Control + "a");
                element.SendKeys(Keys.Delete);
                var value = element.GetAttribute("value");
                return string.IsNullOrEmpty(value); // true khi đã xóa hết
            });
        }

        /// <summary>
        /// Xóa nội dung ô Zip Code để test validation.
        /// Chờ đợi ô textbox có chứa dữ liệu rồi mới xóa bằng Ctrl+A → Backspace.
        /// </summary>
        public void ClearZipCode()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                var value = d.FindElement(_txtZipCode).GetAttribute("value");
                return !string.IsNullOrEmpty(value);
            });

            wait.Until(d =>
            {
                var element = d.FindElement(_txtZipCode);
                element.Click();
                element.SendKeys(Keys.Control + "a");
                element.SendKeys(Keys.Delete);
                var value = element.GetAttribute("value");
                return string.IsNullOrEmpty(value);
            });
        }

        /// <summary>
        /// Nhập giá trị mới vào ô Zip Code.
        /// </summary>
        public void SetZipCode(string zipCode)
        {
            _driver.FindElement(_txtZipCode).Clear();
            _driver.FindElement(_txtZipCode).SendKeys(zipCode);
        }

        /// <summary>
        /// Nhập giá trị mới vào ô Phone.
        /// </summary>
        public void SetPhone(string phone)
        {
            _driver.FindElement(_txtPhone).Clear();
            _driver.FindElement(_txtPhone).SendKeys(phone);
        }

        /// <summary>
        /// Kiểm tra các trường chính đã được auto-load dữ liệu.
        /// </summary>
        public bool IsProfileDataAutoLoaded()
        {
            return IsFieldPopulated(_txtFirstName)
                && IsFieldPopulated(_txtLastName)
                && IsFieldPopulated(_txtAddress)
                && IsFieldPopulated(_txtCity)
                && IsFieldPopulated(_txtState)
                && IsFieldPopulated(_txtZipCode)
                && IsFieldPopulated(_txtPhone);
        }

        /// <summary>
        /// Click nút Update Profile.
        /// </summary>
        public void ClickUpdateProfile()
        {
            _driver.FindElement(_btnUpdateProfile).Click();
        }

        // ==================== VERIFICATION METHODS ====================

        /// <summary>
        /// Lấy tiêu đề thông báo cập nhật thành công (VD: "Profile Updated").
        /// </summary>
        public string GetSuccessHeading()
        {
            return _driver.FindElement(_lblSuccessHeading).Text;
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi First Name.
        /// </summary>
        public string GetFirstNameErrorMessage()
        {
            return _driver.FindElement(_lblFirstNameError).Text;
        }

        /// <summary>
        /// Kiểm tra lỗi First Name có hiển thị hay không.
        /// </summary>
        public bool IsFirstNameErrorDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(d => d.FindElement(_lblFirstNameError));
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy màu CSS của thông báo lỗi First Name (verify màu đỏ).
        /// </summary>
        public string GetFirstNameErrorColor()
        {
            return _driver.FindElement(_lblFirstNameError).GetCssValue("color");
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi Zip Code.
        /// </summary>
        public string GetZipCodeErrorMessage()
        {
            return _driver.FindElement(_lblZipCodeError).Text;
        }

        /// <summary>
        /// Kiểm tra lỗi Zip Code có hiển thị hay không.
        /// </summary>
        public bool IsZipCodeErrorDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(d => d.FindElement(_lblZipCodeError));
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy màu CSS của thông báo lỗi Zip Code.
        /// </summary>
        public string GetZipCodeErrorColor()
        {
            return _driver.FindElement(_lblZipCodeError).GetCssValue("color");
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi Phone.
        /// </summary>
        public string GetPhoneErrorMessage()
        {
            return _driver.FindElement(_lblPhoneError).Text;
        }

        /// <summary>
        /// Kiểm tra lỗi Phone có hiển thị hay không.
        /// </summary>
        public bool IsPhoneErrorDisplayed()
        {
            try
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(d => d.FindElement(_lblPhoneError));
                return element.Displayed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsFieldPopulated(By locator)
        {
            try
            {
                var value = _driver.FindElement(locator).GetAttribute("value");
                return !string.IsNullOrWhiteSpace(value);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
