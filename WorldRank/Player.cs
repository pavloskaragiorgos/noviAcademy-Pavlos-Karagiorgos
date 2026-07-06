public class Player
{
    public Guid Id { get; } 
    public string Name { get; set; }
    public int Score { get; private set; }
    public Player(string name) // constructor of the Player class
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name cannot be null or whitespace.", nameof(name));

        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Score = 0;
    }

    public void AddScore(int points)
    {
        if (points < 0) throw new ArgumentOutOfRangeException(nameof(points));
        Score += points;
    }


}