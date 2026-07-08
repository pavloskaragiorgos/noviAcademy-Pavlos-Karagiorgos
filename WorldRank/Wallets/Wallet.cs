using WorldRank.Console.Enums;
using WorldRank.Exceptions;

namespace WorldRank.Console
{
	public class Wallet
	{
		public decimal Balance { get; private set; }
		public Currency Currency;
		public bool IsBlocked;

		public Wallet(decimal balance, Currency currency, bool isBlocked)
		{
			Balance = balance;
			Currency = currency;
			IsBlocked = false;
		}

		public void SetBalance(decimal balance)
		{
			if (balance < 0)
			{
				return;
			}
			Balance = balance;
		}

		public void Withdraw(decimal amount)
        {
            if (IsBlocked)
            {
                throw new InvalidOperationException("Wallet is blocked.");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal amount must be positive.");
            }
            if (Balance < amount)
            {
                throw new InsufficientFundsException();
            }
            Balance -= amount;
        }

        public override string ToString()
		{
			return "Balance -> " + Balance + " Currency ->" + Currency + " IsBlocked -> " + IsBlocked;
		}
	}
}