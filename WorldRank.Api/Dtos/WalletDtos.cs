using WorldRank.Domain.Enums;

namespace WorldRank.Api.Dtos;

public record CreateWalletRequest(int PlayerId, Currency Currency, decimal Balance);

public record DepositRequest(decimal Amount);