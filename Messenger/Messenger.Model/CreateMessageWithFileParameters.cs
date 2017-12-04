using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class CreateMessageWithFileParameters
    {
        public Guid userFromId { get; set; }
        public Guid groupToId { get; set; }
        public byte[] photo { get; set; }  
        public byte status { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
