using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

public class BasePage
{
    protected IWebDriver driver;
    protected WebDriverWait wait;

    public BasePage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    // Hàm đợi một Element hiển thị trước khi tương tác hoặc chụp ảnh
    protected void WaitForElement(By locator)
    {
        wait.Until(ExpectedConditions.ElementIsVisible(locator));
    }
}