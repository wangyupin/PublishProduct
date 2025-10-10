namespace CityHubCore.Infrastructure.API {
    public class ResponseBase {
        public dynamic Result { get; set; }
        public Pagination Pagination { get; set; }
    }
}
