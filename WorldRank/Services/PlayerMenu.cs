using System;
using WorldRank.Application.Services;
using AppPlayerService = WorldRank.Application.Services.PlayerService;

namespace WorldRank.Console.Services
{
    internal class PlayerMenu
    {
        private readonly AppPlayerService _playerService;

        public PlayerMenu(AppPlayerService playerService)
        {
            _playerService = playerService;
        }
        public void AddPlayer()
        {
            System.Console.Write("Name: ");
            var name = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                System.Console.WriteLine("Name cannot be empty.");
                return;
            }

            System.Console.Write("Score: ");
            var scoreInput = System.Console.ReadLine();
            if (!int.TryParse(scoreInput, out var score))
            {
                System.Console.WriteLine("Score must be a whole number.");
                return;
            }

            _playerService.AddPlayerAsync(name, score).GetAwaiter().GetResult();
            System.Console.WriteLine("Player added successfully.");
        }

        public void ListPlayers()
        {
            var all = _playerService.ListPlayersAsync().GetAwaiter().GetResult();

            if (all.Count == 0)
            {
                System.Console.WriteLine("No players registered.");
                return;
            }

            foreach (var player in all)
                System.Console.WriteLine(player);
        }

        public void ListPlayersByScore()
        {
            var groups = _playerService.ListPlayersByScoreAsync().GetAwaiter().GetResult();

            if (groups.Count == 0)
            {
                System.Console.WriteLine("No players registered.");
                return;
            }

            foreach (var group in groups)
            {
                System.Console.WriteLine($"Score {group.Key}:");
                foreach (var player in group)
                    System.Console.WriteLine($"  {player}");
            }
        }

        public void FindPlayerByName()
        {
            System.Console.Write("Search by name: ");
            var term = System.Console.ReadLine() ?? string.Empty;

            var player = _playerService.FindPlayerByNameAsync(term).GetAwaiter().GetResult();

            System.Console.WriteLine(player is null ? "No player found." : player.ToString());
        }

        public void FindPlayerById()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var player = _playerService.FindPlayerByIdAsync(playerId.Value).GetAwaiter().GetResult();

            System.Console.WriteLine(player is null ? "No player found." : player.ToString());
        }

        public void DeletePlayer()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            _playerService.DeletePlayerAsync(playerId.Value).GetAwaiter().GetResult();
            System.Console.WriteLine("Player deleted (if it existed).");
        }

    }
}