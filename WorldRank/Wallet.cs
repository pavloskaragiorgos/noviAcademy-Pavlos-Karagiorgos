using WorldRank;

public class Wallet
{
    public decimal Balance { get; private set; }
    public Currency Currency { get; private set; }
    public bool IsBlocked { get; private set; }
    private Guid Id { get; set; }

    public Wallet(decimal initialBalance, Currency currency)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Balance cannot be negative.");
        Balance = initialBalance;
        Currency = currency;
        IsBlocked = false;
    }
    public void addPlayerId(Guid playerId)
    {
        Id = playerId;
    }

    public void setBalance(decimal balance)
    {
        if (balance < 0)
            return;
        Balance = balance;
    }

}