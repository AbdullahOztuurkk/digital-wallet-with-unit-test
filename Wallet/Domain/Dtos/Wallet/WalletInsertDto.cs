using FluentValidation;

namespace Wallet.Domain.Dtos.Wallet;
public record WalletInsertDto(string Email, string Msisdn);

public class WalletInsertDtoValidator : AbstractValidator<WalletInsertDto>
{
    public WalletInsertDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(x => MailAddress.TryCreate(x, out _))
            .WithMessage(SystemError.E_0001.ErrorCode);

        RuleFor(x => x.Msisdn)
            .NotEmpty()
            .Matches(@"^\d{10}$")
            .WithMessage(SystemError.E_0001.ErrorCode);
    }
}
