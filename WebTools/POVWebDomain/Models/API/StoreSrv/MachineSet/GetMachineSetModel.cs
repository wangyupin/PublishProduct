using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.MachineSet
{
    public class GetMachineSetRequest
    {
        public string SellBranch { get; set; }
        public string TerminalID { get; set; }
    }

    public class EmployeeLoginRequest
    {
        [Required]
        public string EmpId { get; set; }
        [Required]
        public string EmpPassword { get; set; }
    }

    public class GetBookRemarkRequest
    {
        [Required]
        public string EmpId { get; set; }
    }

    public class UpdBookRemarkRequest
    {
        [Required]
        public string EmpId { get; set; }
        [Required]
        public int ProgramID { get; set; }
        [Required]
        public bool Flag { get; set; }
    }

    public class FrontModule
    {
        public List<Permission> Permission { get; set; }
    }
    public class Permission
    {
        public int ProgramID { get; set; }
        public string Program_Name { get; set; }
    }
}
