using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

public class BillPayPage : BasePage
{
    public BillPayPage(IWebDriver driver) : base(driver) { }

    public void PayBill(string name, string addrStreet, string addrCity, string state, string zipCode, string phone, string account, string verifyAcc, string amount)
    {
        // 1. Nhập liệu
        driver.FindElement(By.Name("payee.name")).SendKeys(name ?? "");
        driver.FindElement(By.Name("payee.address.street")).SendKeys(addrStreet ?? "");
        driver.FindElement(By.Name("payee.address.city")).SendKeys(addrCity ?? "");
        driver.FindElement(By.Name("payee.address.state")).SendKeys(state ?? "");
        driver.FindElement(By.Name("payee.address.zipCode")).SendKeys(zipCode ?? "");
        driver.FindElement(By.Name("payee.phoneNumber")).SendKeys(phone ?? "");
        driver.FindElement(By.Name("payee.accountNumber")).SendKeys(account ?? "");
        driver.FindElement(By.Name("verifyAccount")).SendKeys(verifyAcc ?? "");
        driver.FindElement(By.Name("amount")).SendKeys(amount ?? "");

        // 2. Nghỉ một chút để đảm bảo các script của ParaBank nhận đủ dữ liệu
        Thread.Sleep(500);

        // 3. Click nút gửi
        driver.FindElement(By.XPath("//input[@value='Send Payment']")).Click();

        // 4. Đợi cho đến khi vùng kết quả xuất hiện
        // Mình dùng trực tiếp 'wait' từ BasePage (nếu bạn đã định nghĩa) hoặc tạo mới ở đây
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        // Đợi div kết quả hiện ra (ID này bao phủ toàn bộ vùng thông báo thành công)
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("billPayResult")));
    }
}