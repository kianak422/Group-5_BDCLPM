using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace AutoTest.Pages
{
    public class RegisterPage
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void Open()
        {
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/register.htm");
        }

        public void Register(string first, string last, string street, string city, string state, string zip, string phone, string ssn, string user, string pass, string confirmPass)
        {
            wait.Until(d => d.FindElement(By.Id("customer.firstName")));

            if (!string.IsNullOrEmpty(first)) driver.FindElement(By.Id("customer.firstName")).SendKeys(first);
            if (!string.IsNullOrEmpty(last)) driver.FindElement(By.Id("customer.lastName")).SendKeys(last);
            if (!string.IsNullOrEmpty(street)) driver.FindElement(By.Id("customer.address.street")).SendKeys(street);
            if (!string.IsNullOrEmpty(city)) driver.FindElement(By.Id("customer.address.city")).SendKeys(city);
            if (!string.IsNullOrEmpty(state)) driver.FindElement(By.Id("customer.address.state")).SendKeys(state);
            if (!string.IsNullOrEmpty(zip)) driver.FindElement(By.Id("customer.address.zipCode")).SendKeys(zip);
            if (!string.IsNullOrEmpty(phone)) driver.FindElement(By.Id("customer.phoneNumber")).SendKeys(phone);
            if (!string.IsNullOrEmpty(ssn)) driver.FindElement(By.Id("customer.ssn")).SendKeys(ssn);

            if (!string.IsNullOrEmpty(user)) driver.FindElement(By.Id("customer.username")).SendKeys(user);
            if (!string.IsNullOrEmpty(pass)) driver.FindElement(By.Id("customer.password")).SendKeys(pass);
            if (!string.IsNullOrEmpty(confirmPass)) driver.FindElement(By.Id("repeatedPassword")).SendKeys(confirmPass);

            driver.FindElement(By.XPath("//input[@value='Register']")).Click();
        }
    }
}
