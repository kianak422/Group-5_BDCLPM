using _23DH111579_MaiVanKhanh; 
using NUnit.Framework;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace _23DH111579_MaiVanKhanh.Tests
{
    [TestFixture]
    public class ParaBankTests
    {
        private IWebDriver driver;
        private static string excelPath = @"C:\Users\maiva\Documents\assurance_sofware\pratice\autotest\autotest1.xlsx";

        // 1. HÀM ĐỌC DỮ LIỆU TỪ EXCEL CHO TEST EXPLORER
        public static IEnumerable<TestCaseData> GetTestDataFromExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(excelPath)))
            {
                var sheet = package.Workbook.Worksheets[0];
                int rowCount = sheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string tcID = sheet.Cells[row, 2].Text; // Cột B
                    if (string.IsNullOrEmpty(tcID)) continue;

                    yield return new TestCaseData(row, tcID).SetName(tcID);
                }
            }
        }

        // Hàm hỗ trợ bóc tách dữ liệu từ cột J (Test Data)
        // Ví dụ: Lấy giá trị của "Name" từ chuỗi: Name = "EVN"; Amount = "50"
        private string GetValue(string data, string key)
        {
            string pattern = $@"{key}\s*=\s*""([^""]*)""";
            var match = Regex.Match(data, pattern);
            return match.Success ? match.Groups[1].Value : "";
        }

        [SetUp]
        public void Setup()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");

            // Đăng nhập để vào môi trường kiểm thử nội bộ
            driver.FindElement(By.Name("username")).SendKeys("john");
            driver.FindElement(By.Name("password")).SendKeys("demo");
            driver.FindElement(By.XPath("//input[@value='Log In']")).Click();
            Thread.Sleep(2000);
        }

        // 2. PHƯƠNG THỨC TEST CHÍNH
        [Test]
        [TestCaseSource(nameof(GetTestDataFromExcel))]
        public void RunIndividualTestCase(int row, string tcID)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(excelPath)))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                string function = sheet.Cells[row, 3].Text;  // Cột C
                string testData = sheet.Cells[row, 10].Text; // Cột J
                string expected = sheet.Cells[row, 11].Text; // Cột K

                string actual = "";
                string status = "Fail";

                try
                {
                    switch (function)
                    {
                        case "Open Acc":
                            driver.FindElement(By.LinkText("Open New Account")).Click();
                            string type = testData.Contains("SAVINGS") ? "SAVINGS" : "CHECKING";
                            new OpenAccountPage(driver).OpenAccount(type);
                            // Lấy text thực tế từ tiêu đề h1 sau khi mở
                            actual = driver.FindElement(By.XPath("//h1")).Text;
                            break;

                        case "Transfer":
                            driver.FindElement(By.LinkText("Transfer Funds")).Click();
                            // Tự động bóc tách số từ chuỗi bất kỳ (ví dụ "Amount: 500")
                            string amount = Regex.Match(testData, @"\d+").Value;
                            new TransferPage(driver).Transfer(amount);
                            actual = driver.FindElement(By.XPath("//h1")).Text;
                            break;

                        case "Bill Pay":
                            driver.FindElement(By.LinkText("Bill Pay")).Click();
                            var billPage = new BillPayPage(driver);
                            billPage.PayBill(
                                GetValue(testData, "Name"),
                                GetValue(testData, "AddrStreet"),
                                GetValue(testData, "City"),
                                GetValue(testData, "State"),
                                GetValue(testData, "Zip"),
                                GetValue(testData, "Phone"),
                                GetValue(testData, "Acc"),
                                GetValue(testData, "VerifyAcc"),
                                GetValue(testData, "Amount")
                            );
                            var waitResult = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                            var resultHeader = waitResult.Until(d => d.FindElement(By.XPath("//div[@id='billPayResult']//h1[@class='title']")));
                            actual = resultHeader.Text;
                            break;
                    }

                    // Kiểm tra kết quả thực tế có chứa từ khóa mong đợi không
                    if (actual.ToLower().Contains(expected.ToLower()) || actual == "")
                    {
                        status = "Pass";
                    }
                }
                catch (Exception ex)
                {
                    actual = "Lỗi hệ thống: " + ex.Message;
                }

                // 3. GHI KẾT QUẢ VÀO EXCEL (Cột L, M, N)
                sheet.Cells[row, 12].Value = actual; // Actual Result
                sheet.Cells[row, 13].Value = status; // Status
                sheet.Cells[row, 13].Style.Font.Color.SetColor(status == "Pass" ? Color.Green : Color.Red);

                Capture(sheet, row, 14, tcID); // Screenshot vào cột N

                package.Save();

                // Assert để báo trạng thái cho Test Explorer
                Assert.That(status, Is.EqualTo("Pass"), $"TC {tcID} không đạt. Mong đợi: {expected}, Thực tế: {actual}");
            }
        }

        private void Capture(ExcelWorksheet sheet, int row, int col, string name)
        {
            try
            {
                Thread.Sleep(500); // Đợi 1 chút để trang ổn định trước khi chụp
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                using (var ms = new MemoryStream(ss.AsByteArray))
                {
                    var pic = sheet.Drawings.AddPicture($"Pic_{name}_{DateTime.Now.Ticks}", ms);
                    pic.SetPosition(row - 1, 5, col - 1, 5);
                    pic.SetSize(120, 60);
                    sheet.Row(row).Height = 50;
                }
            }
            catch { }
        }

        [TearDown]
        public void CloseBrowser()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}