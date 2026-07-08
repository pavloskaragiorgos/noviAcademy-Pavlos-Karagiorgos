namespace WorldRank;

public class Player : IPlayer
{

    public Dictionary<Currency, Wallet> Wallets { get; set; } = new Dictionary<Currency, Wallet>();

    public Guid Id { get; }

    public string Name { get; }

    public int Score { get => throw new NotImplementedException(); private set => throw new NotImplementedException(); }

    public Player(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
    }

    public void UpdateScore(int newScore)
    {
        if (newScore < 0)
            throw new ArgumentOutOfRangeException(nameof(newScore), "Score cannot be negative.");

        Score = newScore;
    }

    public override string ToString() =>
            $"[{Id}] {Name} - Score: {Score}";

    public void AddWallet(Currency currency, Wallet wallet)
    {
        if (Wallets.ContainsKey(currency))
            throw new InvalidOperationException($"Wallet for currency {currency} already exists.");
        Wallets[currency] = wallet;
        wallet.addPlayerId(Id);
    }

}

