using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldRank.Exceptions
{
    public class InsufficientFundsException : WalletException
    {
        public InsufficientFundsException() : base("Insufficient funds in the wallet.")
        {
        }
    }
}
