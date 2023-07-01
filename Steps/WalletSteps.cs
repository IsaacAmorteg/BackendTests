using Task9.Clients;
using Task9.Utils;
using TechTalk.SpecFlow;

namespace Task9.Steps
{
    [Binding]
    public sealed class WalletSteps
    {
        private readonly UserServiceClient _userServiceClient = UserServiceClient.Instance;
        private readonly UserGenerator _userGenerator = new UserGenerator();
        private readonly WalletServiceClient _walletServiceClient = WalletServiceClient.Instance;
        private readonly BalanceChargeGenerator _balanceChargeGenerator = new BalanceChargeGenerator();

        private readonly DataContext _context;


        public WalletSteps(DataContext context)
        {
            _context = context;
        }


        [When(@"Get balance")]
        public async Task WhenGetBalance()
        {
            _context.GetBalanceResponse = await _walletServiceClient.GetBalance(_context.UserId);
        }

        [When(@"User is charged (.*)")]
        public async Task WhenUserIsCharged(double chargeAmount)
        {
            var request = _balanceChargeGenerator.GenerateBalanceChargeRequest(_context.UserId, chargeAmount);
            _context.ChargeResponse = await _walletServiceClient.BalanceCharge(request);
        }






    }
}