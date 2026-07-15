using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;

namespace WorldRank.Api.Dtos;

public record CreateWalletRequest(int PlayerId, Currency Currency, decimal Balance);

public record DepositRequest(decimal Amount);

public record WithdrawRequest(decimal Amount);

public record UpdateBalanceRequest(decimal NewBalance);

public record ApplyFundsOperationRequest(FundsOperation Operation, decimal Amount);