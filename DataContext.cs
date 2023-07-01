using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Responses.Base;

namespace Task9
{
    public class DataContext
    {
        public int UserId;

        public int SecondUserId;

        public int ThirdUserId;

        public CommonResponse<bool> GetUserStatusResponse;

        public readonly string NoElementsMessage = "Sequence contains no elements";

        public readonly string NotActiveErrorMessage = "not active user";

        public readonly string PrecisionDigitsErrorMessage = "Amount value must have precision 2 numbers after dot";

        public readonly string IncorrectAmountErrorMessage = "Amount cannot be '0'";

        public readonly int NonExistingUserId = 0;

        public CommonResponse<int> RegisterNewUserResponse;

        public CommonResponse<int> RegisterSecondUserResponse;

        public CommonResponse<int> RegisterThirdUserResponse;

        public CommonResponse<object> DeleteUserResponse;

        public CommonResponse<bool> SetUserStatusResponse;

        public CommonResponse<decimal> GetBalanceResponse;

        internal CommonResponse<Guid> ChargeResponse;
    }
}
