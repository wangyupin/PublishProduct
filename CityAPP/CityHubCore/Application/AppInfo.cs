using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application {
    public class AppInfo {
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string AppVer { get; set; }
        public string StartDate { get; set; }
        public string LicenseQuantity { get; set; }
        public string DefaultUID { get; set; }
        public string DefaultPSW { get; set; }
        public string StockImg { get; set; }
        public string StockImgExcel { get; set; }
        public string GoodsSize { get; set; }
        public string DefaultEUID { set; get; } //員工
        public string DefaultEPSW { set; get; }  //員工密碼
        public string HasWebSite { get; set; }
    }
}
