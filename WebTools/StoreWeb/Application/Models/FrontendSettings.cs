using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreWeb.Application.Models {
    public class FrontendSettings {
        public string SellBranch { get; set; }
        public string TerminalID { get; set; }
        public int HeartbeatIntervalPerSecond { get; set; }
        
    }
}
