using WalletEntity = Wallet.Domain.Entities.Wallet;

namespace Wallet.Application.Services.Concrete;
public class WalletService : IWalletService
{
    private readonly IBaseDbContext _context;

    public WalletService(IBaseDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<WalletResponseDto>> CreateWalletAsync(WalletInsertDto request)
    {
        var response = new BaseResponse<WalletResponseDto>();

        var wallet = new WalletEntity()
        {
            Email = request.Email,
            Msisdn = request.Msisdn,
            Status = StatusType.Active,
            WalletNumber = Guid.NewGuid().ToString(),
        };

        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();

        response.Data = new WalletResponseDto().Map(wallet);

        return response;
    }

    public async Task<BaseResponse<WalletResponseDto>> Freeze(string walletNumber)
    {
        var response = new BaseResponse<WalletResponseDto>();

        var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == walletNumber && x.Status != StatusType.Passive);
        if (wallet == null)
        {
            return response.Fail(SystemError.E_0003);
        }

        if(wallet.Status == StatusType.Blocked)
        {
            return response.Fail(SystemError.E_0004);
        }

        wallet.Status = StatusType.Blocked;

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();

        response.Data = new WalletResponseDto().Map(wallet);

        return response;
    }

    public async Task<BaseResponse<WalletResponseDto>> GetByWalletNumberAsync(string walletNumber)
    {
        var response = new BaseResponse<WalletResponseDto>();

        var exist = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == walletNumber && x.Status != StatusType.Passive);
        if (exist == null)
        {
            return response.Fail(SystemError.E_0003);
        }

        response.Data = new WalletResponseDto().Map(exist);

        return response;
    }

    public async Task<BaseResponse<WalletResponseDto>> SetPassive(SetPassiveDto request)
    {
        var response = new BaseResponse<WalletResponseDto>();

        var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == request.WalletNumber && x.Status != StatusType.Passive);
        if (wallet == null)
        {
            return response.Fail(SystemError.E_0003);
        }

        wallet.Status = StatusType.Passive;

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();

        response.Data = new WalletResponseDto().Map(wallet);

        return response;
    }

    public async Task<BaseResponse<WalletResponseDto>> UnFreeze(string walletNumber)
    {
        var response = new BaseResponse<WalletResponseDto>();

        var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == walletNumber && x.Status == StatusType.Blocked);
        if (wallet == null)
        {
            return response.Fail(SystemError.E_0003);
        }

        wallet.Status = StatusType.Active;

        _context.Wallets.Update(wallet);
        await _context.SaveChangesAsync();

        response.Data = new WalletResponseDto().Map(wallet);

        return response;
    }
}
