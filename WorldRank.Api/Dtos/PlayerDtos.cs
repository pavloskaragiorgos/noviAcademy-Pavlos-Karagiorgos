using WorldRank.Domain.Entities;

namespace WorldRank.Api.Dtos;

public record CreatePlayerRequest(string Name, int Score);

public record PlayerScoreGroup(int Score, List<Player> Players);