using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using Task9.Clients;
using Task9.Utils;

namespace Task9
{
    public class UserServiceTests
    {
        private readonly UserServiceClient _userServiceClient = new UserServiceClient();
        private readonly UserGenerator _userGenerator = new UserGenerator();
        private readonly string _noElementsMessage = "Sequence contains no elements";

        [Test]
        public async Task T1_UserService_RegisterUser_WithEmptyFields_StatusCodeIs200AndIdMoreThan0()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("", "");
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert            
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, response.Status);
                Assert.IsTrue(response.Body > 0);
            });
        }
        [Test]
        public async Task T2_UserService_RegisterUser_WithUpperCase_StatusCodeIs200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("ISAAC", "AMORTEGUI");
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test]
        public async Task T3_UserService_RegisterUser_FieldIsOneCharacter_StatusCodeIs200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("A", "B");
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }
        [Test]
        public async Task T4_UserService_RegisterUser_FieldsLengthGreaterThan100Symbols_StatusCodeIs200()
        {
            //Precondition
            string longName = new string('A', 105);
            var request = _userGenerator.GenerateRegisterNewUserRequest(longName, longName);
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test]
        public async Task T5_UserService_RegisterUser_RegisterThreeUsers_IdIsAutoIncremented()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("i", "a");
            var request2 = _userGenerator.GenerateRegisterNewUserRequest("Test", "User");
            var request3 = _userGenerator.GenerateRegisterNewUserRequest("Test", "User");
            //Action
            var responseRegisterNewUser = await _userServiceClient.RegisterNewUser(request);
            var responseRegisterNewUser2 = await _userServiceClient.RegisterNewUser(request2);
            var responseRegisterNewUser3 = await _userServiceClient.RegisterNewUser(request3);
            //Assert

            Assert.Multiple(() =>
            {
                Assert.IsTrue(responseRegisterNewUser2.Body > responseRegisterNewUser.Body);
                Assert.IsTrue(responseRegisterNewUser3.Body > responseRegisterNewUser2.Body);
                Assert.IsTrue(responseRegisterNewUser3.Body > responseRegisterNewUser.Body);
            });
        }
        [Test]
        public async Task T6_UserService_RegisterUser_FieldsAreNull_StatusCodeIs500()
        {
            //Precondition            
            var request = _userGenerator.GenerateRegisterNewUserRequest(null, null);
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.Status);
        }
        [Test]
        public async Task T7_UserService_RegisterUser_SpecialCharacters_StatusCodeIs200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("Boeing-787!@", "Avianc@_1#$%");
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test]
        public async Task T8_UserService_RegisterUser_AfterUserIsDeletedAndNewUserRegistered_NewUserIdIsIncrementedByOne()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            //Action
            var responseUser1 = await _userServiceClient.RegisterNewUser(request);
            var deleteUser1 = await _userServiceClient.DeleteUser(responseUser1.Body);
            var responseUser2 = await _userServiceClient.RegisterNewUser(request);
            //Assert
            int newUserId = responseUser1.Body + 1;
            Assert.AreEqual(responseUser2.Body, newUserId);
        }
        [Test]
        public async Task T9_UserService_RegisterUser_FieldAreDigits_StatusCodeIs200()
        {
            //Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("55637346", "242345");
            //Action
            var response = await _userServiceClient.RegisterNewUser(request);
            //Assert  
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test]
        public async Task T10_UserService_GetUserStatus_UserExists_ReturnsFalseDefault()
        {
            // Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");

            // Action
            var response = await _userServiceClient.RegisterNewUser(request);
            var userStatusResponse = await _userServiceClient.GetUserStatus(response.Body, _noElementsMessage);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, userStatusResponse.Status);

            if (userStatusResponse.IsSuccess)
            {
                Assert.IsFalse(userStatusResponse.Body);
            }
            else if (userStatusResponse.Content == _noElementsMessage)
            {
                Assert.Fail("User status not found");
            }
            else
            {
                Assert.Fail("An error occurred: " + userStatusResponse.ErrorMessage);
            }
        }

        [Test]
        public async Task T11_UserService_GetUserStatus_UserDoesNotExist_Returns500()
        {
            // Action            
            var userStatusResponse = await _userServiceClient.GetUserStatus(0, _noElementsMessage);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, userStatusResponse.Status);
                Assert.AreEqual(_noElementsMessage, userStatusResponse.Content);
            });
        }

        [Test]
        public async Task T12_UserService_GetUserStatus_StatusChangedToTrue_StatusCodeIs200AndNewStatusTrue()
        {
            // Precondition
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");

            // Action
            var response = await _userServiceClient.RegisterNewUser(request);
            var userStatusResponse1 = await _userServiceClient.GetUserStatus(response.Body, _noElementsMessage);
            var setUserStatusResponse = await _userServiceClient.SetUserStatus(response.Body, true);
            var userStatusResponse2 = await _userServiceClient.GetUserStatus(response.Body, _noElementsMessage);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(HttpStatusCode.OK, userStatusResponse1.Status);
                Assert.IsFalse(userStatusResponse1.Body);
                Assert.AreEqual(HttpStatusCode.OK, userStatusResponse2.Status);
                Assert.IsTrue(userStatusResponse2.Body);
            });
        }
    }
}