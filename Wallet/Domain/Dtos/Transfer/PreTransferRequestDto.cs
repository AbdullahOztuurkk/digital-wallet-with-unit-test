using FluentValidation;

namespace Wallet.Domain.Dtos.Transfer;
public class PreTransferRequestDto
{
    public decimal Amount { get; set; }
    public TransferType TransferType { get; set; }
    public string? ToWalletNumber { get; set; }
    public string? FromWalletNumber { get; set; }
    public string? ToAccountNumber { get; set; }

    public class PreTransferRequestDtoValidator : AbstractValidator<PreTransferRequestDto>
    {
        public PreTransferRequestDtoValidator()
        {
            RuleFor(x => x.FromWalletNumber)
                .NotEmpty()
                .Must(x => long.TryParse(x, out _) && x.Length == 11)
                .WithMessage(SystemError.E_0005.ErrorMessage);

            RuleFor(x => x.Amount)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(SystemError.E_0008.ErrorMessage);

            When(x => x.TransferType == TransferType.Wallet2Wallet, () =>
            {
                RuleFor(x => x.ToWalletNumber)
                    .NotEmpty()
                    .Must(x => Int64.TryParse(x, out _))
                    .WithMessage(SystemError.E_0006.ErrorMessage);
            });

            When(x => x.TransferType == TransferType.Wallet2Account, () =>
            {
                RuleFor(x => x.ToAccountNumber)
                    .NotEmpty()
                    .Must(x => Int64.TryParse(x, out _))
                    .WithMessage(SystemError.E_0007.ErrorMessage);
            });
        }
    }
}

public class PreTransferResponseDto
{
    public string ProcessGuid { get; set; }
}
