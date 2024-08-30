namespace Wallet.Application.Services.Abstract;
public interface IWalletService
{
    Task<BaseResponse<WalletResponseDto>> GetByWalletNumberAsync(string walletNumber);
    Task<BaseResponse<WalletResponseDto>> CreateWalletAsync(WalletInsertDto request);
    Task<BaseResponse<WalletResponseDto>> SetPassive(SetPassiveDto request);
    Task<BaseResponse<WalletResponseDto>> Freeze(string walletNumber);
    Task<BaseResponse<WalletResponseDto>> UnFreeze(string walletNumber);

}
