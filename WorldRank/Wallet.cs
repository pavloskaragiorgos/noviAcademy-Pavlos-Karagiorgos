public class Wallet
{
    public decimal Balance { get; private set; }
    public enum Currency
    {
        USD,
        EUR
    }
    public bool IsBlocked { get; private set; }
    private Guid Id { get; set; }

    public Wallet(decimal initialBalance, Currency currency)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Balance cannot be negative.");
        Balance = initialBalance;
        IsBlocked = false;
    }
    public void addPlayerId(Guid playerId)
    {
        Id = playerId;
    }

}