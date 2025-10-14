using POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.PublishGoods;
using POVWebDomain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.Common.Options
{
    public class SaleModeTypeReturn
    {
        public List<CheckboxOption<int>> Options { get; set; }
        public List<SelectableOption<int>> SalesModeTypeDef { get; set; }
    }
}
