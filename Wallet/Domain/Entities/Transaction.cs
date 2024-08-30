using Wallet.Shared.Domain;

namespace Wallet.Domain.Entities;
public class Transaction : IEntity
{
    public int Id { get; set; }
    public StatusType Status { get; set; }
    public DateTime TransactionDate { get; set; }
    public ProcessType ProcessType { get; set; }
    public int ProcessSubType { get; set; }
    public decimal Amount { get; set; }
    public string? ToWalletNumber { get; set; }
    public string? FromWalletNumber { get; set; }
    public string? ToAccountNumber { get; set; }
    public string ProcessGuid { get; set; } = Guid.NewGuid().ToString("N");
}
