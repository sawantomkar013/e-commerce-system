namespace OrderService.Interface.API.Commands;

public class Response<TResponse, TErrorCodeEnum> : IResponse
    where TErrorCodeEnum : Enum
{
    public bool Success { get; set; }

    public TErrorCodeEnum ErrorCode { get; set; }

    public TResponse Value { get; set; }

    public static Response<TResponse, TErrorCodeEnum> Ok(TResponse response)
    {
        return new Response<TResponse, TErrorCodeEnum>
        {
            Success = true,
            Value = response,
        };
    }

    public static Response<TResponse, TErrorCodeEnum> Error(TErrorCodeEnum errorCode)
    {
        return new Response<TResponse, TErrorCodeEnum>
        {
            Success = false,
            ErrorCode = errorCode,
        };
    }
}
