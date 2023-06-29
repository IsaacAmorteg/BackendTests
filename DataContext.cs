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

        public readonly int NonExistingUserId = 0;

        public CommonResponse<int> RegisterNewUserResponse;

        public CommonResponse<int> RegisterSecondUserResponse;

        public CommonResponse<int> RegisterThirdUserResponse;

        public CommonResponse<object> DeleteUserResponse;

        public CommonResponse<bool> SetUserStatusToTrueResponse;

        public CommonResponse<bool> SetUserStatusToFalseResponse;
    }
}
