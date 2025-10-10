using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.DashBoard.Announcement
{
    public class GetAnnouncementDataRequest
    {
        public string Q { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Sort { get; set; }
        public string SortColumn { get; set; }
        public string Mode { get; set; }
        public AdvanceSearchRequest AdvanceRequest { get; set; }
        public SearchTerm SearchTerm { get; set; }
    }

    public class GetViewAnnouncementDataRequest
    {
        public string AnnouncementID { get; set; }
    }

    public class AddAnnouncementDataRequest
    {
        public string Subject { get; set; }
        public string PostingTime { get; set; }
        public string PostingUnit { get; set; }
        public string PostingPerson { get; set; }
        public string PostingContent { get; set; }
        public string CheckTop { get; set; }
        public int Views { get; set; }
    }

    public class UpdAnnouncementDataRequest
    {
        public string AnnouncementID { get; set; }
        public string Subject { get; set; }
        public string PostingContent { get; set; }
        public string CheckTop { get; set; }
    }

    public class ID
    {
        public string AnnouncementID { get; set; }
    }
    public class DelAnnouncementDataRequest
    {
        public List<ID> DelList { get; set; }
    }

    public class UpdViewsRequest
    {
        public string AnnouncementID { get; set; }
        public int Views { get; set;}
    }

    public class AnnouncementContentEditRequest
    {
        public string AnnouncementID { get; set; }
    }
}
