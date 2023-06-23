using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task9.Models.Requests;

namespace Task9.Utils
{
    public class BalanceChargeGenerator
    {
        public BalanceChargeRequest GenerateBalanceChargeRequest(int userId, double amount)
        {
            return new BalanceChargeRequest()
            {
                userId = userId,
                amount = amount
            };
        }
    }
}
