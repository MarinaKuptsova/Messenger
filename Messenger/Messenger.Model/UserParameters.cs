using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class UserParameters
    {
        public User user { get; set; }
        public byte[] photo { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
