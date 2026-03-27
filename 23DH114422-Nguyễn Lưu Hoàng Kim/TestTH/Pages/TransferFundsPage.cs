using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestTH.Pages
{
    /// <summary>
    /// TransferFundsPage đại diện cho trang chuyển tiền giữa các tài khoản.
    /// Chứa locator cho số tiền, dropdown tài khoản nguồn/đích, và nút Transfer.
    /// </summary>
    public class TransferFundsPage
    {
        private readonly IWebDriver _driver;

        // ==================== LOCATORS ====================

        // Ô nhập số tiền cần chuyển
        private readonly By _txtAmount = By.Id("amount");

        // Dropdown chọn tài khoản nguồn (chuyển từ)
        private readonly By _ddlFromAccount = By.Id("fromAccountId");

        // Dropdown chọn tài khoản đích (chuyển đến)
        private readonly By _ddlToAccount = By.Id("toAccountId");

        // Nút "Transfer" để thực hiện chuyển tiền
        private readonly By _btnTransfer = By.CssSelector("input.button[value='Transfer']");

        // Tiêu đề thông báo chuyển tiền thành công
        private readonly By _lblTransferComplete = By.CssSelector("#showResult h1.title");

        // ==================== CONSTRUCTOR ====================

        /// <summary>
        /// Khởi tạo TransferFundsPage với WebDriver instance.
        /// </summary>
        /// <param name="driver">WebDriver instance được truyền từ test class.</param>
        public TransferFundsPage(IWebDriver driver)
        {
            _driver = driver;
        }

        // ==================== ACTIONS ====================

        /// <summary>
        /// Thực hiện chuyển tiền với số tiền được chỉ định.
        /// Sử dụng tài khoản mặc định đã được chọn sẵn trong dropdown.
        /// </summary>
        /// <param name="amount">Số tiền cần chuyển (VD: "100", "250.50").</param>
        public void TransferMoney(string amount)
        {
            // Nhập số tiền
            _driver.FindElement(_txtAmount).Clear();
            _driver.FindElement(_txtAmount).SendKeys(amount);

            // Click nút Transfer
            _driver.FindElement(_btnTransfer).Click();
        }

        /// <summary>
        /// Chọn tài khoản nguồn (chuyển từ) theo giá trị hiển thị.
        /// </summary>
        /// <param name="accountNumber">Số tài khoản nguồn.</param>
        public void SelectFromAccount(string accountNumber)
        {
            var selectElement = new SelectElement(_driver.FindElement(_ddlFromAccount));
            selectElement.SelectByText(accountNumber);
        }

        /// <summary>
        /// Chọn tài khoản đích (chuyển đến) theo giá trị hiển thị.
        /// </summary>
        /// <param name="accountNumber">Số tài khoản đích.</param>
        public void SelectToAccount(string accountNumber)
        {
            var selectElement = new SelectElement(_driver.FindElement(_ddlToAccount));
            selectElement.SelectByText(accountNumber);
        }

        /// <summary>
        /// Lấy tiêu đề thông báo sau khi chuyển tiền (VD: "Transfer Complete!").
        /// </summary>
        public string GetSuccessMessage()
        {
            return _driver.FindElement(_lblTransferComplete).Text;
        }
    }
}
