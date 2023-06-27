using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task9.Clients;
using Task9.Utils;

namespace Task9.Tests
{
    public class WalletServiceTests
    {
        private readonly WalletServiceClient _walletServiceClient = new WalletServiceClient();
        private readonly BalanceChargeGenerator _balanceChargeGenerator = new BalanceChargeGenerator();
        private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;
        private readonly UserGenerator _userGenerator = new UserGenerator();
        private readonly int _nonExistingUserId = 0;
        private readonly string _notActiveErrorMessage = "not active user";
        private readonly string _precisionDigitsErrorMessage = "Amount value must have precision 2 numbers after dot";
        private readonly string _incorrectAmountErrorMessage = "Amount cannot be '0'";


        [Test]
        public async Task T1_WalletService_GetBalance_NewUser_ReturnsNotActiveUser500()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);

            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, getBalanceResponse.Status);
                Assert.AreEqual(_notActiveErrorMessage, getBalanceResponse.Content);
            });

        }
        [Test]
        public async Task T2_WalletService_GetBalance_NonExistingUser_ReturnsNotActiveUser500()
        {
            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(_nonExistingUserId);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, getBalanceResponse.Status);
                Assert.AreEqual(_notActiveErrorMessage, getBalanceResponse.Content);
            });
        }
        [Test]
        public async Task T3_WalletService_GetBalance_NoTransactionsActiveUser_ReturnsBalanceZeroStatus200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            //Assert
            var expected = 0;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expected, getBalanceResponse.Body);
            });
        }
        [TestCase(10, 10.0)]
        [TestCase(0.01, 0.01)]
        [TestCase(9999999.99, 9999999.99)]
        [TestCase(10000000, 10000000.0)]
        public async Task T4_WalletService_GetBalance_IsCharged_ReturnsCorrectBalancesStatus200(double amountCharged, decimal expectedBalance)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amountCharged);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);

            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [TestCase(-10, "0")]
        [TestCase(-0.01, "0")]
        [TestCase(-9999999.99, "0")]
        [TestCase(-10000000.01, "0")]
        public async Task T4_4_WalletService_GetBalance_IsChargedNonSufficientFunds_ReturnsBalanceZeroStatus200(double amountCharged, decimal expectedBalance)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amountCharged);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });
        }

        [Test]
        public async Task T5_WalletService_GetBalance_MultipleTransactionsCharged_ReturnsCorrectBalanceStatus200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 10, 20.5, 30, -15 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 45.5;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [Test]
        public async Task T6_WalletService_GetBalance_MultipleTransactionsCharged_ReturnsBalanceZeroStatus200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 10, 20.5, 30, -10, -20, -30, -0.5 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 0.0;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }

        [Test]
        public async Task T7_WalletService_GetBalance_MultipleTransactionsCharged_ReturnsBalanceZeroDecimalOne()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 10, 20.5, 30, -10, -20, -30, -0.4 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 0.1;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [Test]
        public async Task T8_WalletService_GetBalance_MultipleTransactionsChargedNonSufficientFunds_BalanceIsLastBalanceWithOKChargePerformed()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 10, 20.5, 30, -10, -20, -30, -0.4, -0.8, -9999999.91 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 0.1;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [Test]
        public async Task T9_WalletService_GetBalance_MultipleTransactionsCharged_OverallBalance9999999_99()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 1000000, 1000000, 1000000, 1000000, 1000000, 1000000, 1000000, 1000000, 1000000, 999999.99 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 9999999.99;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [Test]
        public async Task T10_WalletService_GetBalance_MultipleTransactionsCharged_OverallBalance10000000()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 1000000, -1000000, 10000000 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var expectedBalance = 10000000.0;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });

        }
        [Test]
        public async Task T11_WalletService_GetBalance_MultipleTransactionsStatusToInactive_Returns500()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            double[] amountsCharged = { 10, 20.5, 30, -15 };

            foreach (double amount in amountsCharged)
            {
                var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, amount);
                await _walletServiceClient.BalanceCharge(chargeRequest);
            }

            await _userServiceClient.SetUserStatus(response.Body, false);
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);

            //Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, getBalanceResponse.Status);

        }
        [Test]
        public async Task T12_WalletService_GetBalance_NoTransactionsChargeNegativeBalance_Return200ZeroBalance()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            double chargeAmount = -100;
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Action
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            double expectedBalance = 0;
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, getBalanceResponse.Status);
                Assert.AreEqual(expectedBalance, getBalanceResponse.Body);
            });
        }
        [TestCase(10, 5)]
        [TestCase(10000.01, 526.2)]
        [TestCase(23450.65, 5)]
        public async Task T13_WalletService_Charge_PositiveBalanceAndPositiveCharge_ReturnsTransactionIdCode200(double initialBalance, double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, initialBalance);
            var chargeResponse1 = await _walletServiceClient.BalanceCharge(chargeRequest);
            var chargeRequest2 = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse2 = await _walletServiceClient.BalanceCharge(chargeRequest2);
            //Assert            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse1.Status);
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse2.Status);
                Assert.AreNotEqual(Guid.Empty, chargeResponse2.Body);
            });
        }
        [TestCase(-10)]
        [TestCase(-1000)]
        public async Task T14_WalletService_Charge_ZeroBalanceAndNegativeCharge_ReturnsTransactionIdEmptyCode500AndBodyMessage(double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Assert            
            var getBalanceResponse = await _walletServiceClient.GetBalance(response.Body);
            var contentErrorMessage = $"User have '{getBalanceResponse.Body}', you try to charge '{chargeAmount}.0'.";

            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(contentErrorMessage, chargeResponse.Content);
            });
        }

        [TestCase(-10, -5)]
        [TestCase(-1000, -300.30)]
        [TestCase(-0.5, -0.1)]
        public async Task T15_WalletService_Charge_NegativeBalanceAndNegativeCharge_ReturnsTransactionIdEmptyCode500(double initialBalance, double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, initialBalance);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            var chargeRequest2 = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse2 = await _walletServiceClient.BalanceCharge(chargeRequest2);
            //Assert            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse2.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse2.Body);

            });
        }

        [TestCase(10, -5)]
        [TestCase(1000, -300.30)]
        [TestCase(1000, -0.01)]
        [TestCase(0.5, -0.1)]
        public async Task T16_WalletService_Charge_PositiveBalanceNegativeCharge_ReturnsTransactionIDStatusCode200(double initialBalance, double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, initialBalance);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            var chargeRequest2 = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse2 = await _walletServiceClient.BalanceCharge(chargeRequest2);
            //Assert            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse.Status);
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse2.Status);
                Assert.AreNotEqual(Guid.Empty, chargeResponse2.Body);
            });
        }

        [TestCase(5)]
        [TestCase(0.01)]
        public async Task T17_WalletService_Charge_ZeroBalancePositiveCharge_ReturnsTransactionIDStatusCode200(double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse.Status);
                Assert.AreNotEqual(Guid.Empty, chargeResponse.Body);
            });
        }

        [TestCase(145)]
        [TestCase(-23.5)]
        public async Task T18_WalletService_Charge_InactiveUserCharge_ReturnsTransactionIdEmptyCode500AndBodyMessage(double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(_notActiveErrorMessage, chargeResponse.Content);
            });
        }
        [TestCase(145)]
        [TestCase(-23.5)]
        public async Task T19_WalletService_Charge_NonExistingUser_ReturnsTransactionIdEmptyCode500AndBodyMessage(double chargeAmount)
        {
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(_nonExistingUserId, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(_notActiveErrorMessage, chargeResponse.Content);
            });
        }

        [TestCase(10)]
        [TestCase(3410)]
        [TestCase(11030)]
        public async Task T20_WalletService_Charge_PositiveBalanceExceedNegativeBalance_ReturnsTransactionIdEmptyCode500(double initialBalance)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, initialBalance);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            var chargeRequest2 = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, -initialBalance - 0.01);
            var chargeResponse2 = await _walletServiceClient.BalanceCharge(chargeRequest2);
            var balanceResponse2 = await _walletServiceClient.GetBalance(response.Body);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse2.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse2.Body);
            });
        }

        [TestCase(100, -100.01)]
        [TestCase(2341.10, -5000.51)]
        public async Task T21_WalletService_Charge_PositiveBalanceExceedBalanceCharge_ReturnsTransactionIdEmptyCode500AndBodyMessage(double initialBalance, double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, initialBalance);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            var chargeRequest2 = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse2 = await _walletServiceClient.BalanceCharge(chargeRequest2);
            var balanceResponse2 = await _walletServiceClient.GetBalance(response.Body);
            //Assert            
            CultureInfo culture = CultureInfo.InvariantCulture;
            var expectedErrorMessage = $"User have '{balanceResponse2.Body.ToString("0.0", culture)}', you try to charge '{chargeAmount.ToString("0.00", culture)}'.";
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, chargeResponse.Status);
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse2.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse2.Body);
                Assert.AreEqual(expectedErrorMessage, chargeResponse2.Content);
            });
        }

        [TestCase(10000000.01)]
        [TestCase(999999999.35)]

        public async Task T22_WalletService_Charge_ZeroBalancePositiveCharge_ReturnsTransactionIdEmptyCode500AndBodyMessage(double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);

            //Assert
            CultureInfo culture = CultureInfo.InvariantCulture;
            var expectedErrorMessage = $"After this charge balance could be '{chargeAmount.ToString("0.00", culture)}', maximum user balance is '10000000'";
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(expectedErrorMessage, chargeResponse.Content);
            });
        }

        [TestCase(0.001)]
        [TestCase(210.011)]
        public async Task T23_WalletService_Charge_TwoDigitsAfterDecimal_ReturnsTransactionIdEmptyCode500AndBodyMessage(double chargeAmount)
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(_precisionDigitsErrorMessage, chargeResponse.Content);
            });
        }
        [Test]
        public async Task T24_WalletService_Charge_AmountZero_ReturnsTransactionIdEmptyCode500AndBodyMessage()
        {
            //Precondition
            var chargeAmount = 0;
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            var response = await _userServiceClient.RegisterNewUser(request);
            await _userServiceClient.SetUserStatus(response.Body, true);
            //Action
            var chargeRequest = _balanceChargeGenerator.GenerateBalanceChargeRequest(response.Body, chargeAmount);
            var chargeResponse = await _walletServiceClient.BalanceCharge(chargeRequest);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, chargeResponse.Status);
                Assert.AreEqual(Guid.Empty, chargeResponse.Body);
                Assert.AreEqual(_incorrectAmountErrorMessage, chargeResponse.Content);
            });
        }
    }
}
//Precondition
//Action
//Assert