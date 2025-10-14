using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.Common
{
    public class SelectableOption<T>
    {
        public T ID { get; set; }
        public bool Checked { get; set; } = false;
    }
}
