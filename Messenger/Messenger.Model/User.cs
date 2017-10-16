using System;

namespace Messenger.Model
{
    public class User
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int Photo { get; set; }
        public bool IsActive { get; set; }
        public string UserGroup { get; set; }
    }
}

