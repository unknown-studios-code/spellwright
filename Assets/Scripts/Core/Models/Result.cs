namespace Spellwright.Core.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Data { get; }
        public ErrorCode ErrorCode { get; }
        public string ErrorMessage { get; }

        private Result(bool isSuccess, T data, ErrorCode errorCode, string errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, ErrorCode.None, null);
        }

        public static Result<T> Failure(ErrorCode errorCode, string errorMessage)
        {
            return new Result<T>(false, default, errorCode, errorMessage);
        }
    }
}
