using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class CreateMessageParameters
    {
        public string messageText { get; set; }
        public Guid userFromId { get; set; }
        public Guid groupToId { get; set; }
    }
}
