using static Wallet;

interface IPlayer
{
   public Guid Id { get; }
   public string Name { get; }
   public int Score { get; }
   Dictionary<Currency, Wallet> Wallets { get; set; }
}