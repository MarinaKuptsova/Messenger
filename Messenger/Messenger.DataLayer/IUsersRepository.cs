using Messenger.Model;
using System;
using System.Collections.Generic;

namespace Messenger.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user, byte[] photo, string name, string type);
        void Delete(Guid id);
        User Get(Guid id);
        User Update(User user, User newUser);
        List<Group> GetUserChats(Guid userId);
        User Login(string FirstName, string LastName, string Password);
        List<User> GetAllUsers();
        //void UpdatePhoto(Guid idUser);
        //Files GetUsersFiles(Guid idUser) 
    }
}

