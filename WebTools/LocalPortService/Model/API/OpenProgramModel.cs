using System.ComponentModel.DataAnnotations;

namespace LocalPortService.Model.API
{
    public class OpenProgramRequest
    {
        [Required(ErrorMessage = "請輸入系統的key")]
        public string Key { get; set; }
    }
}
