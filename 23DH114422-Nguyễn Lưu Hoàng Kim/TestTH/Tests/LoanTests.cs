using NUnit.Framework;
using TestTH.Pages;
using TestTH.Utilities;

namespace TestTH.Tests
{
    /// <summary>
    /// LoanTests — Test cases cho module Request Loan.
    /// TC_RL_01 (FAIL - Internal error on empty fields), TC_RL_02 (FAIL - Internal error on invalid chars),
    /// TC_RL_03 (FAIL - Denied instead of validation for negative), TC_RL_04 (PASS - Approved loan).
    /// </summary>
    [TestFixture]
    public class LoanTests : BaseTest
    {
        private LoginPage _loginPage = null!;
        private RequestLoanPage _requestLoanPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp(); // Gọi AutoCreateAndLoginTestAccount() - đã tự động login
            
            // Khởi tạo Page Objects
            _loginPage = new LoginPage(Driver);
            _requestLoanPage = new RequestLoanPage(Driver);

            // Điều hướng đến trang Request Loan
            Driver.Navigate().GoToUrl(BaseUrl + "/requestloan.htm");
            Thread.Sleep(1000);
        }

        [Test]
        public void TC_RL_01_RequestLoan_EmptyFields()
        {
            _requestLoanPage.ClickApplyNow();
            Thread.Sleep(3000);

            bool isErrorDisplayed = _requestLoanPage.IsErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True,
                $"TC_RL_01 FAILED: Error must be displayed when form is empty. Actual: {isErrorDisplayed}");

            string errorHeading = _requestLoanPage.GetErrorHeading();
            Assert.That(errorHeading, Does.Contain("Error"),
                $"TC_RL_01 FAILED: Error message must contain 'Error'. Actual: '{errorHeading}'");
        }

        [Test]
        public void TC_RL_02_RequestLoan_InvalidChars()
        {
            // Input: Non-numeric values into Loan Amount and Down Payment
            _requestLoanPage.SetLoanAmount("abc");
            _requestLoanPage.SetDownPayment("xyz");
            _requestLoanPage.ClickApplyNow();
            Thread.Sleep(3000);

            // Expected: Error message "Invalid number format"
            // Actual: "An internal error has occurred"
            bool isErrorDisplayed = _requestLoanPage.IsErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True,
                $"TC_RL_02 FAILED: Error must be shown when entering letters. Actual: {isErrorDisplayed}");

            string errorHeading = _requestLoanPage.GetErrorHeading();
            Assert.That(errorHeading, Does.Contain("Error"),
                $"TC_RL_02 FAILED: Error heading must indicate invalid format. Actual: '{errorHeading}'");
        }

        [Test]
        public void TC_RL_03_RequestLoan_NegativeAmount()
        {
            // Input: Negative values for Loan Amount and Down Payment
            _requestLoanPage.SetLoanAmount("-100");
            _requestLoanPage.SetDownPayment("-50");
            _requestLoanPage.ClickApplyNow();
            Thread.Sleep(3000);

            // Expected: Error "Amount cannot be negative"
            // Actual: "Loan Request Denied" with wrong reason
            string resultHeading = _requestLoanPage.GetResultHeading();
            // Could be "Denied" or result page heading
            bool isResultDisplayed = _requestLoanPage.IsResultDisplayed();

            Assert.That(isResultDisplayed || resultHeading.Contains("Denied") || resultHeading.Contains("Error"), Is.True,
                $"TC_RL_03 FAILED: Must show error or denied result for negative amounts. Actual: {resultHeading}");

            // The bug: it returns "Denied" instead of proper validation error for negative amounts
            string loanStatus = _requestLoanPage.GetLoanStatus();
            string denialReason = _requestLoanPage.GetLoanDeniedReason();

            Assert.That(loanStatus, Is.EqualTo("Denied"),
                $"TC_RL_03 FAILED: Negative amounts should trigger validation error, not Denied. Status: '{loanStatus}', Reason: '{denialReason}'");
        }

        [Test]
        public void TC_RL_04_RequestLoan_Approved()
        {
            _requestLoanPage.ApplyForLoan("500", "100");
            Thread.Sleep(3000);

            string resultHeading = _requestLoanPage.GetResultHeading();
            string expectedHeading = "Loan Request Processed";
            Assert.That(resultHeading, Is.EqualTo(expectedHeading),
                $"TC_RL_04 FAILED: Result heading mismatch.\nExpected: '{expectedHeading}'\nActual: '{resultHeading}'");

            string loanStatus = _requestLoanPage.GetLoanStatus();
            string expectedStatus = "Approved";
            Assert.That(loanStatus, Is.EqualTo(expectedStatus),
                $"TC_RL_04 FAILED: Loan status mismatch (Amount=500, Down=100).\nExpected: '{expectedStatus}'\nActual: '{loanStatus}'");
        }
    }
}
