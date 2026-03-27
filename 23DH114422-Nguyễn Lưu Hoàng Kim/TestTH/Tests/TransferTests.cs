using NUnit.Framework;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// TransferTests chứa Smoke Test cho chức năng chuyển tiền.
    /// TC3: Chuyển tiền thành công giữa 2 tài khoản.
    /// </summary>
    [TestFixture]
    public class TransferTests : BaseTest
    {
        private LoginPage _loginPage = null!;
        private TransferFundsPage _transferPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp(); // Gọi AutoCreateAndLoginTestAccount()
            _loginPage = new LoginPage(Driver);
            _transferPage = new TransferFundsPage(Driver);
        }

        /// <summary>
        /// TC3: TransferFunds_Success
        /// Mục tiêu: Chuyển tiền giữa 2 tài khoản với số tiền hợp lệ.
        /// Expected: Hiển thị thông báo "Transfer Complete!" sau khi chuyển thành công.
        /// </summary>
        [Test]
        public void TransferFunds_Success()
        {
            // Arrange - Đã đăng nhập qua base.SetUp() với auto-generated account
            // Chờ dashboard load xong sau auto-login
            Thread.Sleep(2000);

            // Act - Truy cập trang Transfer Funds qua URL và chuyển tiền
            Driver.Navigate().GoToUrl(BaseUrl + "/transfer.htm");

            // Đợi trang load xong (dropdown accounts cần thời gian load)
            Thread.Sleep(2000);

            _transferPage.TransferMoney("100");

            // Đợi kết quả hiển thị
            Thread.Sleep(2000);

            // Assert - Kiểm tra thông báo chuyển tiền thành công chính xác
            string actualMessage = _transferPage.GetSuccessMessage();
            string expectedMessage = "Transfer Complete!";
            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                $"FAILED: Transfer success message mismatch.\nExpected: '{expectedMessage}'\nActual: '{actualMessage}'");;
        }
    }
}
