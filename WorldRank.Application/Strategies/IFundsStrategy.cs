using System;
using System.Collections.Generic;
using System.Text;
using WorldRank.Domain.Entities;

namespace WorldRank.Application.Strategies
{
    public interface IFundsStrategy
    {
        FundsOperation Operation { get; }
        void Execute(Wallet wallet, decimal amount);
    }
}
