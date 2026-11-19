namespace SchoolApi.DTOs.Common;

// Every response from the API has this shape:
// { "statusCode": 200, "isSuccess": true, "result": { ... }, "error": null }
public class BaseResponse<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Result { get; set; }
    public string? Error { get; set; }

    // Ready-made responses, so we don't fill the 4 fields by hand every time
    public static BaseResponse<T> Success(T result, int statusCode = StatusCodes.Status200OK)
    {
        return new BaseResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Result = result
        };
    }

    public static BaseResponse<T> Failure(int statusCode, string error)
    {
        return new BaseResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Error = error
        };
    }
}
