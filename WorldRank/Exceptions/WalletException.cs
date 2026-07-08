using System;
using System.Collections.Generic;
using System.Text;

namespace WorldRank.Exceptions
{
    public class WalletException : Exception
    {
        public WalletException(string message) : base("An error occurred in the wallet : " + message)
        {
            
        }
    }
}
