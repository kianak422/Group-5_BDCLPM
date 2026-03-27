using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoTest
{
    public class BaseTest
    {
        protected IWebDriver driver = null!;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                    driver.Dispose();
                }
                finally
                {
                    driver = null!;
                }
            }
        }
    }
}