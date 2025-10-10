using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.DashBoard
{
    public class GetPeersSaleEstimateDataRequest
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string Year { get; set; }
    }
    public class CheckInvoiceRequest
    {
        public string SellStore { get; set; }
        public string SellBranch { get; set; }
        public string TerminalID { get; set; }
        public string InvTerm { get; set; }
    }

    public class GetInvoiceRequest
    {
        public string StoreID { get; set; }
        public string TerminalID { get; set; }
        public string InvTerm { get; set; }
    }
    public class GetCompanyTemplateDataRequest
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
    }
}
