
using System;
using System.Collections.Generic;
using POVWebDomain.Models.Common;

namespace POVWebDomain.Models.API.StoreSrv.Common.Options
{
    public class GetPaymentReturn
    {
        public List<CheckboxOption<int>> Options { get; set; }
        public List<SelectableOption<int>> PayTypes { get; set; }
    }
}
