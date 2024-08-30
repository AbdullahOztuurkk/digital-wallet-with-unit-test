namespace Wallet.Application.Services.Abstract;
public interface ITransferService
{
    Task<BaseResponse<PreTransferResponseDto>> PreTransfer(PreTransferRequestDto request);
    Task<BaseResponse> Wallet2Wallet(Wallet2WalletRequestDto request);
    Task<BaseResponse> Wallet2Account(Wallet2AccountRequestDto request);
}
