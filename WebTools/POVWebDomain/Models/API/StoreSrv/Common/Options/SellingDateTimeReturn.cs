using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.Common.Options
{
    public class SellingDateTimeReturn
    {
        public List<Option<int>> Options { get; set; }
        public Option<int> SellingDateTime { get; set; }
    }
}
