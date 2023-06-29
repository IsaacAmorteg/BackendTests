using Task9.Clients;
using Task9.Utils;
using TechTalk.SpecFlow;

namespace Task9.Steps
{
    [Binding]
    public sealed class UserSteps
    {
        private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;
        private readonly UserGenerator _userGenerator = new UserGenerator();

        private readonly DataContext _context;        
        

        public UserSteps(DataContext context)
        {
            _context = context;
        }

        [Given(@"New user is created")]
        public async Task GivenNewUserIsCreated()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"Second user is created")]
        public async Task GivenSecondUserIsCreated()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("I", "A");
            _context.RegisterSecondUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.SecondUserId = _context.RegisterSecondUserResponse.Body;
        }


        [When(@"User is deleted")]
        public async Task WhenUserIsDeleted()
        {
            _context.DeleteUserResponse = await _userServiceClient.DeleteUser(_context.UserId);
        }


        [Given(@"A non existing user")]
        public void GivenANonExistingUser()
        {
            _context.UserId = _context.NonExistingUserId;
        }

        [Given(@"Three New Valid Users Created")]
        public async Task GivenThreeNewValidUsersCreated()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("TEST", "USER");
            var request2 = _userGenerator.GenerateRegisterNewUserRequest("TEST", "USER");
            var request3 = _userGenerator.GenerateRegisterNewUserRequest("TEST", "USER");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.RegisterSecondUserResponse = await _userServiceClient.RegisterNewUser(request2);
            _context.RegisterThirdUserResponse = await _userServiceClient.RegisterNewUser(request3);
            _context.UserId = _context.RegisterNewUserResponse.Body;
            _context.SecondUserId = _context.RegisterSecondUserResponse.Body;
            _context.ThirdUserId = _context.RegisterThirdUserResponse.Body;
        }


        [Given(@"New user with empty fields is created")]
        public async Task GivenNewUserWithEmptyFieldsIsCreated()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("", "");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with Upper Case fields")]
        public async Task GivenNewUserWithUpperCaseFields()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("TEST", "USER");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with null fields")]
        public async Task GivenNewUserWithNullFields()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest(null, null);
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with digit fields")]
        public async Task GivenNewUserWithDigitFields()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("55637346", "242345");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with Special Character fields")]
        public async Task GivenNewUserWithSpecialCharacterFields()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("Boeing-787!@", "Avianc@_1#$%");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with One Character fields")]
        public async Task GivenNewUserWithOneCharacterFields()
        {
            var request = _userGenerator.GenerateRegisterNewUserRequest("t", "U");
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }

        [Given(@"New user with Fields length greater than 100")]
        public async Task GivenNewUserWithFieldsLengthGreaterThan100()
        {
            string longName = new string('A', 105);
            var request = _userGenerator.GenerateRegisterNewUserRequest(longName, longName);
            _context.RegisterNewUserResponse = await _userServiceClient.RegisterNewUser(request);
            _context.UserId = _context.RegisterNewUserResponse.Body;
        }



        [When(@"Change user IsActive status to true")]
        public async Task WhenChangeUserIsActiveStatusToTrue()
        {
           _context.SetUserStatusToTrueResponse = await _userServiceClient.SetUserStatus(_context.UserId, true);
        }

        [When(@"Change user IsActive status to false")]
        public async Task WhenChangeUserIsActiveStatusToFalse()
        {
            _context.SetUserStatusToFalseResponse = await _userServiceClient.SetUserStatus(_context.UserId, false);
        }


        [When(@"Get user status")]
        public async Task WhenGetUserStatus()
        {
            _context.GetUserStatusResponse = await _userServiceClient.GetUserStatus(_context.UserId, _context.NoElementsMessage);            
        }

    }
}