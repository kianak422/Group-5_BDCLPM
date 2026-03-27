using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// FindTransactionTests — 3 test cases (1 FAIL + 2 FAIL).
    /// TC_FT_02 (FAIL - Invalid ID), TC_FT_06 (FAIL - Invalid date format), 
    /// TC_FT_08 (FAIL - Invalid date range logic).
    /// </summary>
    [TestFixture]
    public class FindTransactionTests : BaseTest
    {
        private LoginPage _loginPage = null!;
        private FindTransactionsPage _findTransactionsPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp(); // Gọi AutoCreateAndLoginTestAccount() - đã tự động login
            
            // Khởi tạo Page Objects
            _loginPage = new LoginPage(Driver);
            _findTransactionsPage = new FindTransactionsPage(Driver);

            // Chờ trang dashboard load xong sau khi login
            Thread.Sleep(2000);

            // Chờ menu link "Find Transactions" xuất hiện và có thể click
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(d => d.FindElement(By.LinkText("Find Transactions")).Displayed);
            }
            catch
            {
                TestContext.WriteLine("[WARNING] Find Transactions link không hiển thị trong 10 giây.");
            }

            // Điều hướng đến trang Find Transactions qua menu để tránh redirect không ổn định
            _findTransactionsPage.OpenFromMenu();
            Thread.Sleep(2000);

            // Chờ page Find Transactions load xong
            try
            {
                wait.Until(d => _findTransactionsPage.IsFindTransactionsPageDisplayed());
            }
            catch
            {
                TestContext.WriteLine("[ERROR] Timeout: Find Transactions page không hiển thị sau 10 giây.");
            }

            Assert.That(_findTransactionsPage.IsFindTransactionsPageDisplayed(), Is.True,
                "SETUP FAILED: Không thể mở trang Find Transactions - bài test không thể chạy được.");
        }

        [Test]
        public void TC_FT_02_Find_ByTransactionId_InvalidID()
        {
            _findTransactionsPage.SearchByTransactionId("99999999");
            Thread.Sleep(2000);

            bool isNoTransactionsFound = _findTransactionsPage.IsNoTransactionsFoundDisplayed();
            bool hasNoRows = !_findTransactionsPage.HasTransactionRows();
            bool isValid = isNoTransactionsFound || hasNoRows;
            Assert.That(isValid, Is.True,
                $"TC_FT_02 FAILED: With invalid ID, must show 'No transactions found' or empty list. Found='No transactions': {isNoTransactionsFound}, Empty list: {hasNoRows}");
        }

        [Test]
        public void TC_FT_06_Find_ByDate_InvalidFormat()
        {
            // Input invalid date format: "13-45-2023" (month 13, day 45 don't exist)
            _findTransactionsPage.SearchByDate("13-45-2023");
            Thread.Sleep(2000);

            // Expected: Error message should be displayed
            // Actual: System does not show error, accepts the data
            bool isDateErrorDisplayed = _findTransactionsPage.IsDateErrorDisplayed();
            Assert.That(isDateErrorDisplayed, Is.True,
                $"TC_FT_06 FAILED: With invalid date format '13-45-2023', error must be shown. Actual: Error displayed={isDateErrorDisplayed}");

            string errorMsg = _findTransactionsPage.GetDateErrorMessage();
            Assert.That(errorMsg, Does.Contain("Invalid"),
                $"TC_FT_06 FAILED: Error message must contain 'Invalid'. Actual: '{errorMsg}'");
        }

        [Test]
        public void TC_FT_08_Find_ByDateRange_InvalidLogic()
        {
            // Input: From Date after To Date (From: "11-30-2023", To: "11-01-2023")
            _findTransactionsPage.SearchByDateRange("11-30-2023", "11-01-2023");
            Thread.Sleep(2000);

            // Expected: Error message "From date must be before To date"
            // Actual: Returns empty result, no error shown
            bool isErrorDisplayed = _findTransactionsPage.IsDateErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True,
                $"TC_FT_08 FAILED: When From > To, error must be shown. Actual: Error displayed={isErrorDisplayed}");

            string errorMsg = _findTransactionsPage.GetDateErrorMessage();
            Assert.That(errorMsg, Does.Contain("before") | Does.Contain("valid"),
                $"TC_FT_08 FAILED: Error message must explain date range logic. Actual: '{errorMsg}'");
        }
    }
}
