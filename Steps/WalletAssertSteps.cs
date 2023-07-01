using NUnit.Framework;
using System.Net;
using TechTalk.SpecFlow;

namespace Task9.Steps
{
    [Binding]
    internal class WalletAssertSteps
    {
        private readonly DataContext _context;

        public WalletAssertSteps(DataContext context)
        {
            _context = context;
        }

        [Then(@"Get user balance response code is (.*)")]
        public void ThenGetUserBalanceResponseCodeIs(string expected)
        {
            if (expected == "OK")
            {
                Assert.AreEqual(HttpStatusCode.OK, _context.GetBalanceResponse.Status);
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, _context.GetBalanceResponse.Status);
            }
            
        }

        [Then(@"Get user balance response body is error message")]
        public void ThenGetUserBalanceResponseBodyIsErrorMessage()
        {
            Assert.AreEqual(_context.NotActiveErrorMessage, _context.GetBalanceResponse.Content);
        }


        [Then(@"Get user balance is expected (.*)")]
        public void ThenGetUserBalanceIsExpected(decimal expected)
        {
            Assert.AreEqual(expected, _context.GetBalanceResponse.Body);
        }

        [Then(@"Charge response code is (.*)")]
        public void ThenChargeResponseCodeIs(string expected)
        {
            if (expected == "OK")
            {
                Assert.AreEqual(HttpStatusCode.OK, _context.ChargeResponse.Status);
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, _context.ChargeResponse.Status);
            }
        }

        [Then(@"Charge response body is (.*)")]
        public void ThenChargeResponseBodyIs(string expected)
        {
            if (expected == "GUID")
            {
                Assert.AreNotEqual(Guid.Empty, _context.ChargeResponse.Body);
            }
            else
            {
                Assert.AreEqual(Guid.Empty, _context.ChargeResponse.Body);
            }
        }

        [Then(@"Charge response content is not active user")]
        public void ThenChargeResponseContentIsNotActiveUser()
        {
            Assert.AreEqual(_context.NotActiveErrorMessage, _context.ChargeResponse.Content);
        }

        [Then(@"Charge response content is precision error")]
        public void ThenChargeResponseContentIsPrecisionError()
        {
            Assert.AreEqual(_context.PrecisionDigitsErrorMessage, _context.ChargeResponse.Content);
        }

        [Then(@"Charge response content is incorrect amount error")]
        public void ThenChargeResponseContentIsIncorrectAmountError()
        {
            Assert.AreEqual(_context.IncorrectAmountErrorMessage, _context.ChargeResponse.Content);
        }




    }
}