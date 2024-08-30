using Wallet.Shared.Domain.Constant;

namespace Wallet.Shared.Result;
public class BaseResponse
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "Successful operation.";
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public object? Data { get; set; }

    public BaseResponse Fail(Error error)
    {
        IsSuccess = false;
        ErrorCode = error.ErrorCode;
        ErrorMessage = error.ErrorMessage;
        Message = string.Empty;
        return this;
    }
}

public class BaseResponse<T> : BaseResponse
{
    public new T? Data { get; set; }

    public new  BaseResponse<T> Fail(Error error)
    {
        base.Fail(error);
        return this;
    }
}