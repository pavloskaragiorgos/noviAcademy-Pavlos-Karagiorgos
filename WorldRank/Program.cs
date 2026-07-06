List<Player> players = new List<Player>() ;

while (true)
{
    Console.WriteLine("\n--- Player Menu ---");
    Console.WriteLine("1. Add player");
    Console.WriteLine("2. List players");
    Console.WriteLine("3. Find by name");
    Console.WriteLine("4. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddPlayer(players);
            break;
        case "2":   
            ListPlayers(players); 
            break;
        case "3":
            FindPlayerByName(players); 
            break;
        case "4":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid option, try again.");
            break;
    }
}

static void AddPlayer(List<Player> players)
{
    Console.WriteLine("Give Player Name: ");
    string? name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Invalid input.");
        return;
    }
    players.Add(new Player(name));
}

static void ListPlayers(List<Player> players)
{
    if (players.Count == 0)
    {
        Console.WriteLine("No players found.");
        return;
    }
    Console.WriteLine("\n--- Player List --- \n");
    foreach (var player in players)
    {
        Console.WriteLine($"ID: {player.Id}, Name: {player.Name}, Score: {player.Score}");
    }
}

static void FindPlayerByName(List<Player> players)
{
    if (players.Count == 0)
    {
        Console.WriteLine("No players found.");
        return;
    }

    Console.WriteLine("Enter player name to search: ");
    string? name = Console.ReadLine();
    if(string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Invalid input.");
        return;
    }

    var matches = players.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
    if (matches.Count == 0)
    {

        Console.WriteLine("No player found with that name.");
        return;
    }
    Console.WriteLine(" Player(s) found: ");
    foreach (var player in matches)
    {
        Console.WriteLine($"ID: {player.Id}, Name: {player.Name}, Score: {player.Score}");
    }

}




