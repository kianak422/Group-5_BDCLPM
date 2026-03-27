using OpenQA.Selenium;

namespace TestTH.Pages
{
    /// <summary>
    /// FindTransactionsPage đại diện cho trang tìm kiếm giao dịch.
    /// Hỗ trợ tìm kiếm theo Transaction ID và theo Date.
    /// </summary>
    public class FindTransactionsPage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // --- Tìm theo Transaction ID ---
        private readonly By _lnkFindTransactions = By.LinkText("Find Transactions");
        private readonly By _txtTransactionId = By.Id("transactionId");
        private readonly By _lblIdError = By.XPath("//*[contains(normalize-space(),'Invalid transaction ID')]");

        // --- Tìm theo Date ---
        private readonly By _txtTransactionDate = By.Id("transactionDate");
        private readonly By _lblDateError = By.XPath("//*[contains(normalize-space(),'Invalid date format')]");

        // --- Kết quả ---
        private readonly By _lblTransactionResultHeading = By.XPath("//h1[contains(normalize-space(),'Transaction Results')]");
        private readonly By _lblInternalErrorHeading = By.XPath("//h1[contains(normalize-space(),'Error!')]");
        private readonly By _lblNoTransactionFound = By.XPath("//*[contains(normalize-space(),'No transactions found')]");
        private readonly By _lnkTransactionResult = By.XPath("//a[contains(@href,'transaction.htm?id=')]");

        // ==================== CONSTRUCTOR ====================

        public FindTransactionsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Mở trang Find Transactions từ menu sidebar để tránh redirect sai trang.
        /// </summary>
        public void OpenFromMenu()
        {
            _driver.FindElement(_lnkFindTransactions).Click();
        }

        /// <summary>
        /// Kiểm tra đã ở đúng trang Find Transactions hay chưa.
        /// </summary>
        public bool IsFindTransactionsPageDisplayed()
        {
            try
            {
                return _driver.FindElement(_txtTransactionId).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm giao dịch theo Transaction ID.
        /// </summary>
        public void SearchByTransactionId(string transactionId)
        {
            _driver.FindElement(_txtTransactionId).Clear();
            _driver.FindElement(_txtTransactionId).SendKeys(transactionId);
            _driver.FindElement(_txtTransactionId).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Click nút Find by ID mà không nhập dữ liệu.
        /// </summary>
        public void ClickFindById()
        {
            _driver.FindElement(_txtTransactionId).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Tìm kiếm giao dịch theo Date.
        /// </summary>
        public void SearchByDate(string date)
        {
            _driver.FindElement(_txtTransactionDate).Clear();
            _driver.FindElement(_txtTransactionDate).SendKeys(date);
            _driver.FindElement(_txtTransactionDate).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// Tìm kiếm giao dịch theo Date Range (From Date và To Date).
        /// </summary>
        public void SearchByDateRange(string fromDate, string toDate)
        {
            // Tìm ô "From Date" và nhập dữ liệu
            var txtFromDate = _driver.FindElement(By.Id("fromDate"));
            if (txtFromDate != null)
            {
                txtFromDate.Clear();
                txtFromDate.SendKeys(fromDate);
            }

            // Tìm ô "To Date" và nhập dữ liệu
            var txtToDate = _driver.FindElement(By.Id("toDate"));
            if (txtToDate != null)
            {
                txtToDate.Clear();
                txtToDate.SendKeys(toDate);
            }

            // Click nút "Find by Date Range"
            var btnFindByDateRange = _driver.FindElement(By.CssSelector("input.button[value*='Date Range']"));
            if (btnFindByDateRange != null)
            {
                btnFindByDateRange.Click();
            }
        }

        // ==================== VERIFICATION METHODS ====================

        /// <summary>
        /// Kiểm tra bảng kết quả giao dịch có hiển thị hay không.
        /// </summary>
        public bool IsTransactionResultDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblTransactionResultHeading).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra trang lỗi nội bộ có hiển thị hay không.
        /// </summary>
        public bool IsInternalErrorDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblInternalErrorHeading).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra thông báo không tìm thấy giao dịch có hiển thị hay không.
        /// </summary>
        public bool IsNoTransactionsFoundDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblNoTransactionFound).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra danh sách kết quả có ít nhất 1 transaction link.
        /// </summary>
        public bool HasTransactionRows()
        {
            return _driver.FindElements(_lnkTransactionResult).Count > 0;
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi Transaction ID.
        /// </summary>
        public string GetIdErrorMessage()
        {
            return _driver.FindElement(_lblIdError).Text;
        }

        /// <summary>
        /// Kiểm tra thông báo lỗi ID có hiển thị hay không.
        /// </summary>
        public bool IsIdErrorDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblIdError).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy màu CSS của thông báo lỗi ID (verify màu đỏ).
        /// </summary>
        public string GetIdErrorColor()
        {
            return _driver.FindElement(_lblIdError).GetCssValue("color");
        }

        /// <summary>
        /// Lấy nội dung thông báo lỗi Date.
        /// </summary>
        public string GetDateErrorMessage()
        {
            return _driver.FindElement(_lblDateError).Text;
        }

        /// <summary>
        /// Kiểm tra thông báo lỗi Date có hiển thị hay không.
        /// </summary>
        public bool IsDateErrorDisplayed()
        {
            try
            {
                return _driver.FindElement(_lblDateError).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy màu CSS của thông báo lỗi Date (verify màu đỏ).
        /// </summary>
        public string GetDateErrorColor()
        {
            return _driver.FindElement(_lblDateError).GetCssValue("color");
        }
    }
}
