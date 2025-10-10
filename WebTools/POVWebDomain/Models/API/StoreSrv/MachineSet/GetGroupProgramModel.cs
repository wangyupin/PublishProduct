using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.MachineSet
{
    public class GetGroupProgramRequest
    {
        [Required(ErrorMessage = "請輸入群組名稱")]
        public string GroupName { get; set; }
    }
}
