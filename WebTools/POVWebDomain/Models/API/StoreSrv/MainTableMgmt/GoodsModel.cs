using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.DB.POVWeb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Goods
{
    public class GetGoodsHelpOffsetRequest
    {
        public string Q { get; set; } //SearchTerm
        public int Page { get; set; } //CurrentPage
        public int PerPage { get; set; } //Offset
        public string Sort { get; set; } //SortDirection
        public string SortColumn { get; set; }
        public string Mode { get; set; } //Mode all ...
        public AdvanceSearchRequest AdvanceRequest { get; set; }
        public SearchTerm SearchTerm {  get; set; }
    }
    
}
