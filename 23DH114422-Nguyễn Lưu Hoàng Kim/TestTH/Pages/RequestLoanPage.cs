using OpenQA.Selenium;

namespace TestTH.Pages
{
    /// <summary>
    /// RequestLoanPage đại diện cho trang yêu cầu vay vốn.
    /// Hỗ trợ nhập Loan Amount, Down Payment, Apply, và verify kết quả.
    /// </summary>
    public class RequestLoanPage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // Ô nhập số tiền muốn vay
        private readonly By _txtLoanAmount = By.Id("amount");

        // Ô nhập số tiền trả trước (down payment)
        private readonly By _txtDownPayment = By.Id("downPayment");

        // Nút "Apply Now"
        private readonly By _btnApplyNow = By.CssSelector("input.button[value='Apply Now']");

        // Tiêu đề kết quả ("Loan Request Processed")
        private readonly By _lblResultHeading = By.CssSelector("#requestLoanResult h1.title");

        // Trạng thái loan ("Approved" / "Denied")
        private readonly By _lblLoanStatus = By.Id("loanStatus");

        // Tiêu đề trang lỗi khi để trống fields
        private readonly By _lblErrorHeading = By.CssSelector("#requestLoanError h1.title");

        // Thông báo lý do từ chối
        private readonly By _lblLoanDeniedReason = By.Id("loanRequestDeniedReason");

        // ==================== CONSTRUCTOR ====================

        public RequestLoanPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Gửi yêu cầu vay vốn với số tiền vay và số tiền trả trước.
        /// </summary>
        public void ApplyForLoan(string loanAmount, string downPayment)
        {
            _driver.FindElement(_txtLoanAmount).Clear();
            _driver.FindElement(_txtLoanAmount).SendKeys(loanAmount);

            _driver.FindElement(_txtDownPayment).Clear();
            _driver.FindElement(_txtDownPayment).SendKeys(downPayment);

            _driver.FindElement(_btnApplyNow).Click();
        }

        /// <summary>
        /// Click nút Apply Now mà không nhập dữ liệu.
        /// </summary>
        public void ClickApplyNow()
        {
            _driver.FindElement(_btnApplyNow).Click();
        }

        /// <summary>
        /// Nhập số tiền vay vào ô Loan Amount.
        /// </summary>
        public void SetLoanAmount(string amount)
        {
            _driver.FindElement(_txtLoanAmount).Clear();
            _driver.FindElement(_txtLoanAmount).SendKeys(amount);
        }

        /// <summary>
        /// Nhập số tiền trả trước vào ô Down Payment.
        /// </summary>
        public void SetDownPayment(string downPayment)
        {
            _driver.FindElement(_txtDownPayment).Clear();
            _driver.FindElement(_txtDownPayment).SendKeys(downPayment);
        }

        // ==================== VERIFICATION METHODS ====================

        /// <summary>
        /// Lấy tiêu đề kết quả loan (VD: "Loan Request Processed").
        /// </summary>
        public string GetResultHeading()
        {
            return _driver.FindElement(_lblResultHeading).Text;
        }

        /// <summary>
        /// Lấy trạng thái loan (VD: "Approved" hoặc "Denied").
        /// </summary>
        public string GetLoanStatus()
        {
            return _driver.FindElement(_lblLoanStatus).Text;
        }

        /// <summary>
        /// Kiểm tra kết quả loan có hiển thị hay không.
        /// </summary>
        public bool IsResultDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblResultHeading).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy tiêu đề lỗi khi gửi form không hợp lệ.
        /// </summary>
        public string GetErrorHeading()
        {
            return _driver.FindElement(_lblErrorHeading).Text;
        }

        /// <summary>
        /// Kiểm tra lỗi có hiển thị hay không.
        /// </summary>
        public bool IsErrorDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblErrorHeading).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy lý do từ chối loan (VD: "We cannot grant a loan in that amount with your available funds").
        /// </summary>
        public string GetLoanDeniedReason()
        {
            try
            {
                return _driver.FindElement(_lblLoanDeniedReason).Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }
    }
}
