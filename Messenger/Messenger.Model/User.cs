using System;
using System.Drawing;
using System.Windows.Media.Imaging;

//using System.Windows.Media.Imaging;

namespace Messenger.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public Guid Photo { get; set; }
        public byte[] Ava { get; set; }
        public BitmapImage UserBitmapImage { get; set; }
    }
}

