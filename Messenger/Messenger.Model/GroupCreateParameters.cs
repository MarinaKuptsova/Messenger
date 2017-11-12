using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class GroupCreateParameters
    {
        public IEnumerable<Guid> members { get; set; }
        public string name { get; set; }
    }
}
