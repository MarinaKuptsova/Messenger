using System;

namespace Messenger.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public string MessageText { get; set; }
        public int ParentMessageId { get; set; }
        public string MessageFrom { get; set; }
        public string MessageTo { get; set; }
        public DateTime SendTime { get; set; }
        public int AttachedFile { get; set; }
    }

}
