using Wallet.Shared.Domain.Enums;
using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Domain.Dtos.Wallet;

public class WalletResponseDto
{
    public string Email { get; set; }
    public string Msisdn { get; set; }
    public StatusType Status { get; set; }
    public string WalletNumber { get; set; }
    public int Id { get; set; }

    public WalletResponseDto Map(WalletEntity wallet)
    {
        Email = wallet.Email;
        Msisdn = wallet.Msisdn;
        Status = wallet.Status;
        WalletNumber = wallet.WalletNumber;
        Id = wallet.Id;
        return this;
    }
};