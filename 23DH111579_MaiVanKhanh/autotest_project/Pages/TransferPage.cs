using OpenQA.Selenium;

public class TransferPage : BasePage
{
    public TransferPage(IWebDriver driver) : base(driver) { }

    public void Transfer(string amount)
    {
        driver.FindElement(By.Id("amount")).SendKeys(amount);
        Thread.Sleep(500); // Đợi dropdown load account (đặc thù của ParaBank)
        driver.FindElement(By.XPath("//input[@value='Transfer']")).Click();

        // Đợi kết quả hiển thị
        WaitForElement(By.XPath("//h1[contains(text(),'Transfer Complete!')]"));
    }
}