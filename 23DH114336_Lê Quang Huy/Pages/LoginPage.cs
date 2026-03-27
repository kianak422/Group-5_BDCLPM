using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutoTest.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void Open()
        {
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/");
        }

        public void Login(string user, string pass)
        {
            var userField = wait.Until(d => d.FindElement(By.Name("username")));
            userField.Clear();
            userField.SendKeys(user);

            var passField = wait.Until(d => d.FindElement(By.Name("password")));
            passField.Clear();
            passField.SendKeys(pass);

            var loginBtn = wait.Until(d => d.FindElement(By.CssSelector("input[value='Log In']")));
            loginBtn.Click();
        }

        public void Logout()
        {
            var logoutLink = wait.Until(d => d.FindElement(By.LinkText("Log Out")));
            logoutLink.Click();
        }
    }
}
