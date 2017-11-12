using System;

namespace Messenger.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public Guid MessageFromUserId { get; set; }
        public Guid MessageToGroupId { get; set; }
        public DateTime SendTime { get; set; }
        public Guid AttachedFile { get; set; }
    }

}
