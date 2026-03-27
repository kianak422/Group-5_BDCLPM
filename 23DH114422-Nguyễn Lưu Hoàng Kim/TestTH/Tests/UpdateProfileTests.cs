using NUnit.Framework;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// UpdateProfileTests — 3 test cases (1 PASS + 2 FAIL).
    /// TC_UC_01 (PASS - Auto load), TC_UC_09 (FAIL - Alphabetic zip code accepted),
    /// TC_UC_10 (FAIL - Alphabetic phone accepted).
    /// </summary>
    [TestFixture]
    public class UpdateProfileTests : BaseTest
    {
        private LoginPage _loginPage = null!;
        private UpdateProfilePage _updateProfilePage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp(); // Gọi AutoCreateAndLoginTestAccount() - đã tự động login
            
            // Khởi tạo Page Objects
            _loginPage = new LoginPage(Driver);
            _updateProfilePage = new UpdateProfilePage(Driver);
            
            Thread.Sleep(1000);
        }

        [Test]
        public void TC_UC_01_ProfileLoad_AutoLoadData()
        {
            // Arrange - Đã đăng nhập qua base.SetUp()
            
            // Act
            _updateProfilePage.ClickUpdateContactInfoMenu();
            Thread.Sleep(1500);

            bool isAutoLoaded = _updateProfilePage.IsProfileDataAutoLoaded();
            Assert.That(isAutoLoaded, Is.True,
                $"TC_UC_01 FAILED: Form must auto-load current profile data. Actual: {isAutoLoaded}");
        }

        [Test]
        public void TC_UC_09_ProfileUpdate_AlphaZipCode_ShouldBeRejected()
        {
            _updateProfilePage.ClickUpdateContactInfoMenu();
            Thread.Sleep(1500);

            _updateProfilePage.SetZipCode("adcbd");
            _updateProfilePage.ClickUpdateProfile();
            Thread.Sleep(1000);

            bool isErrorDisplayed = _updateProfilePage.IsZipCodeErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True,
                $"TC_UC_09 FAILED: Zip Code with letters must be rejected and error shown. Actual: {isErrorDisplayed}");
        }

        [Test]
        public void TC_UC_10_ProfileUpdate_InvalidPhone()
        {
            _updateProfilePage.ClickUpdateContactInfoMenu();
            Thread.Sleep(1500);

            _updateProfilePage.SetPhone("phone");
            _updateProfilePage.ClickUpdateProfile();
            Thread.Sleep(1000);

            // Expected: Error message for invalid phone format
            // Actual: System accepts alphabetic characters and saves successfully
            bool isErrorDisplayed = _updateProfilePage.IsPhoneErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True,
                $"TC_UC_10 FAILED: Phone with letters 'phone' must be rejected and error shown. Actual: Error displayed={isErrorDisplayed}");

            string errorMsg = _updateProfilePage.GetPhoneErrorMessage();
            Assert.That(errorMsg, Does.Contain("Phone") | Does.Contain("format") | Does.Contain("invalid"),
                $"TC_UC_10 FAILED: Error message must related to phone format. Actual: '{errorMsg}'");
        }
    }
}
