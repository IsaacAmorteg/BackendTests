using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task9.Clients;
using Task9.Utils;

namespace Task9
{
    public class WalletServiceTests
    {
        private readonly WalletServiceClient _walletServiceClient = new WalletServiceClient();
        private readonly BalanceChargeGenerator _balanceChargeGenerator = new BalanceChargeGenerator();
        private readonly UserServiceClient _userServiceClient = new UserServiceClient();
        private readonly UserGenerator _userGenerator = new UserGenerator();
        private readonly int _nonExistingUserId = 0;
        private readonly string _notActiveErrorMessage = "not active user";

        
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
    }
}
//Precondition
//Action
//Assert