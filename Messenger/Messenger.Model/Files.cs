using System;

namespace Messenger.Model
{
    public class Files
    {
        public Guid Id { get; set; }
        public Guid Owner { get; set; }
        public byte[] UserFile { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

}


