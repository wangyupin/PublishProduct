using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.MachineSet
{
    public class UpdHeartbeatRequest {
        public string SellBranch { get; set; }
        public string TerminalID { get; set; }
        public string Status { get; set; }
    }

    public class UpdPIDStatusRequest {
        public bool IsBusy { get; set; }
        public bool IsFueling { get; set; }
        public int Volume { get; set; }
        public int Amount { get; set; }
    }
}
