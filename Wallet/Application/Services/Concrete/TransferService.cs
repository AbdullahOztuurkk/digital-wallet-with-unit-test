namespace Wallet.Application.Services.Concrete;
public class TransferService : ITransferService
{
    private readonly WalletDbContext _context;

    public TransferService(WalletDbContext walletDbContext)
    {
        this._context = walletDbContext;
    }

    public async Task<BaseResponse<PreTransferResponseDto>> PreTransfer(PreTransferRequestDto request)
    {
        var response = new BaseResponse<PreTransferResponseDto>();

        var fromWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == request.FromWalletNumber && x.Status != StatusType.Passive);
        if(fromWallet == null)
        {
            return response.Fail(SystemError.E_0005);
        }

        if(fromWallet.Status == StatusType.Blocked)
        {
            return response.Fail(SystemError.E_0009);
        }

        if (request.TransferType == TransferType.Wallet2Wallet)
        {
            var toWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.WalletNumber == request.ToWalletNumber && x.Status != StatusType.Passive);
            if (toWallet == null)
            {
                return response.Fail(SystemError.E_0005);
            }
        }
        else if( request.TransferType == TransferType.Wallet2Account)
        {
            //ToAccount check logic
        }

        if(fromWallet.Balance < request.Amount)
        {
            return response.Fail(SystemError.E_0010);
        }

        var transaction = new Transaction
        {
            Amount = request.Amount,
            FromWalletNumber = request.FromWalletNumber,
            ProcessType = ProcessType.BalanceTransfer,
            ProcessSubType = (int)request.TransferType,
            ToAccountNumber = request.ToAccountNumber,
            ToWalletNumber = request.ToWalletNumber,
            TransactionDate = DateTime.UtcNow.AddHours(3),
            Status = StatusType.Pending,
        };

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        response.Data = new PreTransferResponseDto { ProcessGuid = transaction.ProcessGuid };

        return response;
    }

    public async Task<BaseResponse> Wallet2Account(Wallet2AccountRequestDto request)
    {
        var response = new BaseResponse();

        var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.ProcessGuid == request.ProcessGuid);
        if(transaction == null)
        {
            return response.Fail(SystemError.E_0001);
        }

        if(transaction.TransactionDate.AddMinutes(2) > DateTime.UtcNow.AddHours(3))
        {
            return response.Fail(SystemError.E_0011);
        }

        if(transaction.Status != StatusType.Pending)
        {
            return response.Fail(SystemError.E_0012);
        }

        if(transaction.ToAccountNumber != request.MatchAccountNumber)
        {
            return response.Fail(SystemError.E_0013);
        }

        if( transaction.ProcessType != ProcessType.BalanceTransfer && transaction.ProcessSubType == (int)TransferType.Wallet2Account)
        {
            return response.Fail(SystemError.E_0014);
        }

        //Transfer money to account from wallet logic

        transaction.Status = StatusType.Success;

        _context.Transactions.Update(transaction);
        return response;
    }

    public async Task<BaseResponse> Wallet2Wallet(Wallet2WalletRequestDto request)
    {
        var response = new BaseResponse();

        var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.ProcessGuid == request.ProcessGuid);
        if (transaction == null)
        {
            return response.Fail(SystemError.E_0001);
        }

        if (transaction.TransactionDate.AddMinutes(2) > DateTime.UtcNow.AddHours(3))
        {
            return response.Fail(SystemError.E_0011);
        }

        if (transaction.Status != StatusType.Pending)
        {
            return response.Fail(SystemError.E_0012);
        }

        if (transaction.Amount != request.MatchAmount)
        {
            return response.Fail(SystemError.E_0013);
        }

        if (transaction.ProcessType != ProcessType.BalanceTransfer && transaction.ProcessSubType == (int)TransferType.Wallet2Wallet)
        {
            return response.Fail(SystemError.E_0014);
        }

        //Transfer wallet to wallet logic

        transaction.Status = StatusType.Success;

        _context.Transactions.Update(transaction);
        return response;
    }
}
