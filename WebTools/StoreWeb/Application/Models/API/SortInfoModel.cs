using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWeb.Application.Models.API.SortInfoModel {
    public class GetSortListRequest {
        public string Sort01ID_Like { get; set; }
        public string Sort01ID_From { get; set; }
        public string Sort01ID_To { get; set; }
        public string Sort01Name_Like { get; set; }
        public string OrderBy { get; set; }
    }
}
