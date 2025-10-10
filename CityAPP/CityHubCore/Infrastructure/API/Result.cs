namespace CityHubCore.Infrastructure.API {
    public class FormatResultModel<T> {
        public static ResultModel<T> Success(T Data) {
            return new ResultModel<T>() {
                Succeeded = true,
                Data = Data
            };
        }

        public static ResultModel<T> Failure(T errors) {
            return new ResultModel<T>() {
                Succeeded = false,
                Data = errors
            };
        }
    }

    public class ResultModel<T> {
        public bool Succeeded { get; set; }

        public T Data { get; set; }

    }
}
