using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public bool Status { get; set; }
        public Visibility TextblockVisibility { get; set; }
        public Visibility TextblockFileNameVisibility { get; set; }
        public Visibility ButtonVisibility { get; set; }
        public string AttachedFileName { get; set; }
        public BitmapImage OwnerAva { get; set; }
        public string OwnerName { get; set; }
        public HorizontalAlignment Orientation { get; set; }
        public bool IsRead { get; set; }
        
    }

}
