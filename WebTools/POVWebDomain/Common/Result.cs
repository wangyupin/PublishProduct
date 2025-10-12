namespace POVWebDomain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error ?? Error.None;
        }

        public static Result Success() => new Result(true, Error.None);

        public static Result Failure(Error error) => new Result(false, error);

        public static Result Failure(string errorMessage)
            => new Result(false, Error.FromMessage(errorMessage));
    }
}