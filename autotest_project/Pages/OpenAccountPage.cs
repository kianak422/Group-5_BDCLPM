using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public class OpenAccountPage : BasePage
{
    public OpenAccountPage(IWebDriver driver) : base(driver) { }

    public void OpenAccount(string type)
    {
        // 1. Đợi dropdown loại tài khoản xuất hiện rồi mới chọn
        WaitForElement(By.Id("type"));
        var dropdown = driver.FindElement(By.Id("type"));
        new SelectElement(dropdown).SelectByText(type);

        // 2. MẸO QUAN TRỌNG: Đợi một chút để hệ thống ParaBank load ID tài khoản ở dropdown phía dưới
        Thread.Sleep(1000);

        // 3. Tìm nút và Click
        IWebElement btnOpen = driver.FindElement(By.XPath("//input[@value='Open New Account']"));
        btnOpen.Click();

        // 4. Đợi cho đến khi tiêu đề thành công xuất hiện (Xác nhận đã nhấn nút thành công)
        WaitForElement(By.XPath("//h1[contains(text(),'Account Opened!')]"));
    }
}