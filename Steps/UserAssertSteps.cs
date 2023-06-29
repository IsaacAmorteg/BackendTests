using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Responses.Base;
using TechTalk.SpecFlow;

namespace Task9.Steps
{
    [Binding]
    internal class UserAssertSteps
    {
        private readonly DataContext _context;

        public UserAssertSteps(DataContext context)
        {
            _context = context;
        }

        [Then(@"Register new user response Status code is OK")]
        public void ThenRegisterNewUserResponseStatusCodeIsOK()
        {
            Assert.AreEqual(HttpStatusCode.OK, _context.RegisterNewUserResponse.Status);
        }

        [Then(@"Register new user response Status code is Internal Server Error")]
        public void ThenRegisterNewUserResponseStatusCodeIsInternalServerError()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, _context.RegisterNewUserResponse.Status);
        }


        [Then(@"Register new user response body is Id greater than zero")]
        public void ThenRegisterNewUserResponseBodyIsIdGreaterThanZero()
        {
            Assert.IsTrue(_context.UserId > 0);
        }

        [Then(@"User Id is auto incremented for three registered users")]
        public void ThenUserIdIsAutoIncrementedForThreeRegisteredUsers()
        {            
            Assert.Multiple(() =>
            {
                Assert.IsTrue(_context.SecondUserId > _context.UserId);
                Assert.IsTrue(_context.ThirdUserId > _context.SecondUserId);
                Assert.IsTrue(_context.ThirdUserId > _context.UserId);

            });
        }

        [Then(@"User Id is auto incremented for two users")]
        public void ThenUserIdIsAutoIncrementedForTwoUsers()
        {                      
           Assert.IsTrue(_context.SecondUserId > _context.UserId);                
                       
        }


        [Then(@"Get user status response Status Code is OK")]
        public void ThenGetUserStatusResponseStatusCodeIsOK()
        {            
            Assert.AreEqual(HttpStatusCode.OK, _context.GetUserStatusResponse.Status);
        }

        [Then(@"Get user status response Status Code is 500")]
        public void ThenGetUserStatusResponseStatusCodeIs500()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, _context.GetUserStatusResponse.Status);
        }

        [Then(@"Get user status response body is error message - NonExistingUser")]
        public void ThenGetUserStatusResponseBodyIsErrorMessage_NonExistingUser()
        {
            Assert.AreEqual(_context.NoElementsMessage, _context.GetUserStatusResponse.Content);
        }


        [Then(@"User status is true")]
        public void ThenUserStatusIsTrue()
        {           
            Assert.IsTrue(_context.GetUserStatusResponse.Body);
        }

        [Then(@"User status is false")]
        public void ThenUserStatusIsFalse()
        {
            Assert.IsFalse(_context.GetUserStatusResponse.Body);
        }

        [Then(@"Set user status to true response is Internal Server Error")]
        public void ThenSetUserStatusToTrueResponseIsInternalServerError()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, _context.SetUserStatusToTrueResponse.Status);
        }

        [Then(@"Set user status to true response body is error message - NonExistingUser")]
        public void ThenSetUserStatusToTrueResponseBodyIsErrorMessage_NonExistingUser()
        {
            Assert.AreEqual(_context.NoElementsMessage, _context.SetUserStatusToTrueResponse.Content);
        }

        [Then(@"Set user status to false response is Internal Server Error")]
        public void ThenSetUserStatusToFalseResponseIsInternalServerError()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, _context.SetUserStatusToFalseResponse.Status);
        }

        [Then(@"Set user status to false response body is error message - NonExistingUser")]
        public void ThenSetUserStatusToFalseResponseBodyIsErrorMessage_NonExistingUser()
        {
            Assert.AreEqual(_context.NoElementsMessage, _context.SetUserStatusToFalseResponse.Content);
        }

        [Then(@"Set user status to true response is OK")]
        public void ThenSetUserStatusToTrueResponseIsOK()
        {
            Assert.AreEqual(HttpStatusCode.OK, _context.SetUserStatusToTrueResponse.Status);
        }
        

        [Then(@"Set user status to false response is OK")]
        public void ThenSetUserStatusToFalseResponseIsOK()
        {
            Assert.AreEqual(HttpStatusCode.OK, _context.SetUserStatusToFalseResponse.Status);
        }

        [Then(@"Delete User Response is OK")]
        public void ThenDeleteUserResponseIsOK()
        {
            Assert.AreEqual(HttpStatusCode.OK, _context.DeleteUserResponse.Status);
        }

        [Then(@"Delete user response is Internal Server Error")]
        public void ThenDeleteUserResponseIsInternalServerError()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, _context.DeleteUserResponse.Status);
        }

        [Then(@"Delete user response body is Error Message-NonExistingUser")]
        public void ThenDeleteUserResponseBodyIsErrorMessage_NonExistingUser()
        {
            Assert.AreEqual(_context.NoElementsMessage, _context.DeleteUserResponse.Content);
        }









    }
}
