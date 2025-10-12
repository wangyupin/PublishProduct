using System;

namespace POVWebDomain.Common
{
    public class Result<T> : Result
    {
        private readonly T _data;

        public T Data => IsSuccess
            ? _data
            : throw new InvalidOperationException("無法取得失敗結果的資料");

        private Result(T data, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _data = data;
        }

        public static Result<T> Success(T data)
            => new Result<T>(data, true, Error.None);

        public static new Result<T> Failure(Error error)
            => new Result<T>(default, false, error);

        public static new Result<T> Failure(string errorMessage)
            => new Result<T>(default, false, Error.FromMessage(errorMessage));

        // 便利方法:從另一個 Result 轉換
        public static Result<T> Failure(Result result)
            => new Result<T>(default, false, result.Error);
    }
}