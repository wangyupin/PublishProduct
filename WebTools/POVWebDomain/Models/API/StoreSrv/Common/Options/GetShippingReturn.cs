
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.Common.Options
{
    public class GetShippingReturn
    {
        public List<CheckboxOption<long>> Options { get; set; }
        public List<ShippingTypes> ShippingTypes { get; set; }
    }

    public class ShippingTypes
    {
        public long ID { get; set; }
        public bool Checked { get; set; } = false;
        public bool IsFree { get; set; } = false;
    }
}
