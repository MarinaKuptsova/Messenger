using Messenger.Model;
using System;

namespace Messenger.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user, Files file);
        void Delete(Guid id);
        User Get(Guid id);
        void Update(User user);
        //void UpdatePhoto(Guid id);
        //Files GetUsersFiles(Guid id) ид юзера
    }
}

