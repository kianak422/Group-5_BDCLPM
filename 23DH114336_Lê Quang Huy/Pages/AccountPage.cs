using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutoTest.Pages
{
    public class AccountPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public AccountPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public bool IsOverview()
        {
            try {
                return wait.Until(d => d.PageSource.Contains("Accounts Overview"));
            } catch { return false; }
        }

        public void ClickAccount(string accountId)
        {
            wait.Until(d => d.FindElement(By.LinkText(accountId))).Click();
        }

        public void ClickAccountsOverview()
        {
            wait.Until(d => d.FindElement(By.LinkText("Accounts Overview"))).Click();
        }

        public bool IsDetail()
        {
            try {
                return wait.Until(d => d.PageSource.Contains("Account Details"));
            } catch { return false; }
        }

        public void ClickTransaction(string transactionText)
        {
            wait.Until(d => d.FindElement(By.LinkText(transactionText))).Click();
        }

        public void ClickTransactionCell()
        {
            wait.Until(d => d.FindElement(By.XPath("//form[@id='activityForm']/table/tbody/tr[3]/td"))).Click();
        }

        public void ClickOpenNewAccount()
        {
            wait.Until(d => d.FindElement(By.LinkText("Open New Account"))).Click();
        }

        public void ClickAdminPage()
        {
            wait.Until(d => d.FindElement(By.LinkText("Admin Page"))).Click();
        }

        public void SubmitOpenAccount()
        {
            wait.Until(d => d.FindElement(By.XPath("//input[@value='Open New Account']"))).Click();
        }

        public void SubmitDatabaseReset()
        {
            // Thử click vào nút có nội dung "Clean Database" hoặc dùng XPath linh hoạt hơn
            try {
                wait.Until(d => d.FindElement(By.XPath("//button[text()='Clean Database']"))).Click();
            } catch {
                wait.Until(d => d.FindElement(By.CssSelector("button[value='CLEAN']"))).Click();
            }
        }

        public bool IsTransactionDetail()
        {
            try {
                return wait.Until(d => d.PageSource.Contains("Transaction Details"));
            } catch { return false; }
        }
    }
}
