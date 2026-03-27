using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AutoTest.Pages;
using AutoTest.Utils;
using System;

namespace AutoTest.Tests
{
    public class TestScripts : BaseTest
    {
        private void VerifyText(string expectedText, string tcName, string passMessage)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(d => d.PageSource.Contains(expectedText));
                Console.WriteLine($"[PASS] {tcName}: {passMessage}");
            }
            catch (WebDriverTimeoutException)
            {
                string screenshot = ScreenshotHelper.Capture(driver, tcName);
                Console.WriteLine($"[FAIL] {tcName}: Text '{expectedText}' not found. URL: {driver.Url}. Screenshot: {screenshot}");
                Assert.Fail($"Text '{expectedText}' not found at {driver.Url}. Screenshot: {screenshot}");
            }
        }

        // ================= F1 - REGISTRATION =================

        [Test]
        public void TC_F1_01_Register_Valid_FullFields()
        {
            var page = new RegisterPage(driver);
            page.Open();
            // Đăng ký jh01 như mẫu KR-exported
            page.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "12345678", "123456", "jh01", "123456", "123456");
            VerifyText("Welcome", "TC_F1_01", "Tạo tài khoản thành công");
        }

        [Test]
        public void TC_F1_02_Register_Missing_FirstName()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("", "Smith", "", "", "", "", "", "", "jh02", "123456", "123456");
            VerifyText("First name is required", "TC_F1_02", "Hiển thị lỗi đúng");
        }

        [Test]
        public void TC_F1_03_Register_Password_Mismatch()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("John", "Smith", "", "", "", "", "", "", "jh03", "123456", "654321");
            VerifyText("did not match", "TC_F1_03", "Hiển thị lỗi");
        }

        [Test]
        public void TC_F1_04_Register_Missing_FirstName_FullAddress()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("", "Smith", "75 Bui Duong Lich", "Ho Chi Minh", "Viet Nam", "123456", "0363758505", "123456", "jh01", "123456", "123456");
            VerifyText("First name is required", "TC_F1_04", "Pass");
        }

        [Test]
        public void TC_F1_05_Register_Missing_AddressInfo()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("john", "smith", "", "", "", "", "", "", "jh05", "", "");
            VerifyText("Address is required", "TC_F1_05", "Pass");
        }

        [Test]
        public void TC_F1_06_Register_Valid_State_USE()
        {
            var page = new RegisterPage(driver);
            page.Open();
            // Đăng ký jh01 lại (có thể gây lỗi "already exists" nhưng vẫn đúng theo logic template)
            page.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "12345678", "123456", "jh01", "123456", "123456");
            VerifyText("Welcome", "TC_F1_06", "Pass");
        }

        [Test]
        public void TC_F1_07_Register_Alphanumeric_Phone()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "abc123", "123456", "jh02", "123456", "123456");
            VerifyText("Welcome", "TC_F1_07", "Pass");
        }

        [Test]
        public void TC_F1_08_Register_Alphanumeric_Zip()
        {
            var page = new RegisterPage(driver);
            page.Open();
            page.Register("John", "Smith", "123 Queens", "New york", "USA", "abc", "12345678", "123456", "jh03", "123456", "123456");
            VerifyText("Welcome", "TC_F1_08", "Pass");
        }

        // ================= F2 - LOGIN =================

        [Test]
        public void TC_F2_01_Login_Success()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            VerifyText("Accounts Overview", "TC_F2_01", "Login OK");
        }

        [Test]
        public void TC_F2_02_Login_Wrong_Password()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "99999999");
            VerifyText("could not be verified", "TC_F2_02", "Hiển thị lỗi");
        }

        [Test]
        public void TC_F2_03_Login_Empty()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("", "");
            VerifyText("Please enter a username and password", "TC_F2_03", "Hiển thị lỗi");
        }

        [Test]
        public void TC_F2_04_Login_Fake_User()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("faek user", "123456");
            VerifyText("could not be verified", "TC_F2_04", "Pass");
        }

        [Test]
        public void TC_F2_05_Login_Special_Chars()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("@@@", "@@@");
            VerifyText("could not be verified", "TC_F2_05", "Pass");
        }

        [Test]
        public void TC_F2_06_Login_SQL_Injection()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("OR'1'='1", "OR'1'='1");
            VerifyText("could not be verified", "TC_F2_06", "Pass");
        }

        [Test]
        public void TC_F2_07_Login_And_Navigate_Overview()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/overview.htm");
            VerifyText("Accounts Overview", "TC_F2_07", "Pass");
        }

        [Test]
        public void TC_F2_09_Login_And_Logout()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            login.Logout();
            VerifyText("Customer Login", "TC_F2_09", "Pass");
        }

        // ================= F3 - ACCOUNT =================

        [Test]
        public void TC_F3_01_Account_Overview_Click()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccountsOverview();
            VerifyText("Accounts Overview", "TC_F3_01", "Pass");
        }

        [Test]
        public void TC_F3_02_Account_Overview_Navigate_Back()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("13455");
            driver.Navigate().Back();
            VerifyText("Accounts Overview", "TC_F3_02", "Pass");
        }

        [Test]
        public void TC_F3_03_Account_Activity_Detail()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("13455");
            VerifyText("Account Details", "TC_F3_03", "Pass");
        }

        [Test]
        public void TC_F3_04_Open_New_Account_Flow()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickOpenNewAccount();
            account.SubmitOpenAccount();
            account.ClickAccountsOverview();
            VerifyText("Accounts Overview", "TC_F3_04", "Pass");
        }

        [Test]
        public void TC_F3_05_Transaction_Details_View()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("17562");
            account.ClickTransaction("Funds Transfer Received");
            VerifyText("Transaction Details", "TC_F3_05", "Pass");
        }

        [Test]
        public void TC_F3_06_Transaction_Details_Click_Cell()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("17562");
            account.ClickTransaction("Funds Transfer Received");
            account.ClickTransactionCell();
            VerifyText("Transaction Details", "TC_F3_06", "Pass");
        }

        [Test]
        public void TC_F3_07_Account_Overview_Navigation()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/overview.htm");
            VerifyText("Accounts Overview", "TC_F3_07", "Pass");
        }

        [Test]
        public void TC_F3_08_Account_Overview_Direct_Link()
        {
            var login = new LoginPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/overview.htm");
            VerifyText("Accounts Overview", "TC_F3_08", "Pass");
        }

        [Test]
        public void TC_F3_09_Account_Activity_Navigation()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("13455");
            VerifyText("Account Activity", "TC_F3_09", "Pass");
        }

        [Test]
        public void TC_F3_10_Account_Activity_Direct_Link()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAccount("13455");
            VerifyText("Account Activity", "TC_F3_10", "Pass");
        }

        [Test]
        public void TC_F3_11_Open_New_Account_Repeat()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickOpenNewAccount();
            account.SubmitOpenAccount();
            account.ClickAccountsOverview();
            driver.Navigate().Refresh();
            VerifyText("Accounts Overview", "TC_F3_11", "Pass");
        }

        [Test]
        public void TC_F3_12_Admin_Database_Reset()
        {
            var login = new LoginPage(driver);
            var account = new AccountPage(driver);
            login.Open();
            login.Login("jh01", "123456");
            account.ClickAdminPage();
            account.SubmitDatabaseReset();
            account.ClickAccountsOverview();
            VerifyText("Accounts Overview", "TC_F3_12", "Pass");
        }
    }
}
