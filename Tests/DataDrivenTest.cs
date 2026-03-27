using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AutoTest.Pages;
using AutoTest.Utils;
using System;

namespace AutoTest.Tests
{
    public class DataDrivenTest : BaseTest
    {
        [Test]
        public void Run_All_30_TestCases()
        {
            string[] tcList =
            {
                // ===== F1 =====
                "TC_F1_01","TC_F1_02","TC_F1_03","TC_F1_04","TC_F1_05","TC_F1_06","TC_F1_07","TC_F1_08",

                // ===== F2 =====
                "TC_F2_01","TC_F2_02","TC_F2_03","TC_F2_04","TC_F2_05","TC_F2_06","TC_F2_07","TC_F2_09",

                // ===== F3 =====
                "TC_F3_01","TC_F3_02","TC_F3_03","TC_F3_04","TC_F3_05","TC_F3_06","TC_F3_07","TC_F3_08","TC_F3_09",
                "TC_F3_10","TC_F3_11","TC_F3_12"
            };

            var login = new LoginPage(driver);
            var register = new RegisterPage(driver);
            var account = new AccountPage(driver);

            foreach (var tc in tcList)
            {
                try
                {
                    switch (tc)
                    {
                        // ================= F1 =================

                        case "TC_F1_01":
                            register.Open();
                            register.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "12345678", "123456", "jh01", "123456", "123456");
                            Check("Welcome", tc);
                            break;

                        case "TC_F1_02":
                            register.Open();
                            register.Register("", "Smith", "", "", "", "", "", "", "jh02", "123456", "123456");
                            Check("First name is required", tc);
                            break;

                        case "TC_F1_03":
                            register.Open();
                            register.Register("John", "Smith", "", "", "", "", "", "", "jh03", "123456", "654321");
                            Check("did not match", tc);
                            break;

                        case "TC_F1_04":
                            register.Open();
                            register.Register("John", "Smith", "75 Bui Duong Lich", "Ho Chi Minh", "Viet Nam", "123456", "0363758505", "123456", "", "123456", "123456");
                            Check("Username is required", tc);
                            break;

                        case "TC_F1_05":
                            register.Open();
                            register.Register("John", "Smith", "", "", "", "", "", "", "jh12", "", "");
                            Check("Password is required", tc);
                            break;

                        case "TC_F1_06":
                            register.Open();
                            register.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "12345678", "123456", "jh01", "123456", "123456");
                            Check("Welcome", tc);
                            break;

                        case "TC_F1_07":
                            register.Open();
                            register.Register("John", "Smith", "123 Queens", "New York", "USA", "123456", "abc123", "123456", "jh02", "123456", "123456");
                            Check("Welcome", tc);
                            break;

                        case "TC_F1_08":
                            register.Open();
                            register.Register("John", "Smith", "123 Queens", "New york", "USA", "abc", "12345678", "123456", "jh03", "123456", "123456");
                            Check("Welcome", tc);
                            break;

                        // ================= F2 =================

                        case "TC_F2_01":
                            login.Open();
                            login.Login("jh01", "123456");
                            Check("Accounts Overview", tc);
                            break;

                        case "TC_F2_02":
                            login.Open();
                            login.Login("jh01", "999999");
                            Check("could not be verified", tc);
                            break;

                        case "TC_F2_03":
                            login.Open();
                            login.Login("", "");
                            Check("Please enter a username and password", tc);
                            break;

                        case "TC_F2_04":
                            login.Open();
                            login.Login("fakeUser", "123456");
                            Check("could not be verified", tc);
                            break;

                        case "TC_F2_05":
                            login.Open();
                            login.Login("@@@", "@@@");
                            Check("could not be verified", tc);
                            break;

                        case "TC_F2_06":
                            login.Open();
                            login.Login("' OR '1'='1", "' OR '1'='1");
                            CheckNot("Accounts Overview", tc);
                            break;

                        case "TC_F2_07":
                            login.Open();
                            login.Login("jh01", "123456");
                            driver.Navigate().Refresh();
                            Check("Accounts Overview", tc);
                            break;

                        case "TC_F2_09":
                            login.Open();
                            login.Login("jh01", "123456");
                            login.Logout();
                            Check("Customer Login", tc);
                            break;

                        // ================= F3 =================

                        default:
                            login.Open();
                            login.Login("jh01", "123456");

                            if (tc == "TC_F3_03" || tc == "TC_F3_09" || tc == "TC_F3_10" || tc == "TC_F3_11")
                            {
                                account.ClickAccount("13455");
                                Check("Account Details", tc);
                            }
                            else if (tc == "TC_F3_05" || tc == "TC_F3_06")
                            {
                                account.ClickAccount("17562");
                                account.ClickTransaction("Funds Transfer Received");
                                Check("Transaction Details", tc);
                            }
                            else if (tc == "TC_F3_02")
                            {
                                account.ClickAccount("13455");
                                driver.Navigate().Back();
                                Check("Accounts Overview", tc);
                            }
                            else if (tc == "TC_F3_12")
                            {
                                account.ClickAdminPage();
                                account.SubmitDatabaseReset();
                                account.ClickAccountsOverview();
                                Check("Accounts Overview", tc);
                            }
                            else
                            {
                                Check("Accounts Overview", tc);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    string img = ScreenshotHelper.Capture(driver, tc);
                    Console.WriteLine($"[FAIL] {tc}: Exception: {ex.Message}. Screenshot: {img}");
                }
            }
        }

        // ===== HELPER =====

        private void Check(string text, string tc)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(d => d.PageSource.Contains(text));
                Console.WriteLine($"[PASS] {tc}: Found text '{text}'");
            }
            catch (WebDriverTimeoutException)
            {
                string img = ScreenshotHelper.Capture(driver, tc);
                Console.WriteLine($"[FAIL] {tc}: Text '{text}' not found. Screenshot: {img}");
            }
        }

        private void CheckNot(string text, string tc)
        {
            if (!driver.PageSource.Contains(text))
                Console.WriteLine($"[PASS] {tc}: Text '{text}' not found as expected");
            else
            {
                string img = ScreenshotHelper.Capture(driver, tc);
                Console.WriteLine($"[FAIL] {tc}: Unexpected text '{text}' found. Screenshot: {img}");
            }
        }
    }
}
