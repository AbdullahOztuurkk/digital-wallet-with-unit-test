using Wallet.Shared.Domain;

namespace Wallet.Domain.Entities;
public class Wallet : IEntity
{
    public int Id { get; set; }
    public string WalletNumber { get; set; }
    public string? Msisdn { get; set; }
    public required string Email { get; set; }
    public StatusType Status { get; set; } = StatusType.Active;
    public decimal Balance { get; set; } = 0M;
}
