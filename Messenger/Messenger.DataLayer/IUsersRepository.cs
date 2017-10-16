using Messenger.Model;
using System;

namespace Messenger.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user, Files file);
        void Delete(string login);
        User Get(string login);
        void Update(User user);
        //void UpdatePhoto(string login);
    }
}

