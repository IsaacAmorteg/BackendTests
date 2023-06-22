using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Requests;

namespace Task9.Utils
{
    public class UserGenerator
    {
        public RegisterNewUserRequest GenerateRegisterNewUserRequest(string firstName, string lastName)
        {
            return new RegisterNewUserRequest()
            {
                firstName = firstName,
                lastName = lastName
            };
        }
    }
}
