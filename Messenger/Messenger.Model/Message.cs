using System;

namespace Messenger.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public Guid ParentMessageId { get; set; }
        public Guid MessageFrom { get; set; }
        public Guid MessageTo { get; set; }
        public DateTime SendTime { get; set; }
        public Guid AttachedFile { get; set; }
    }

}
