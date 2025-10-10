namespace CityHubCore.Infrastructure.API {
    public class Pagination {
        /// <summary>
        /// 起啟位置
        /// </summary>
        public int OffSet { get; set; }
        /// <summary>
        /// 第幾頁
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 總共筆數
        /// </summary>
        public int TotalRows { get; set; }
    }
}
