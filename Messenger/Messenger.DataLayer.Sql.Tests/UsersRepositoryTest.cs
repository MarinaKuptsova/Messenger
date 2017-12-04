using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messenger.Model;

namespace Messenger.DataLayer.Sql.Tests
{
    [TestClass]
    public class UsersRepositoryTest
    {
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempGroups = new List<Guid>();

        [TestMethod]
        public void ShouldCreateUser()
        {
            //arrange
            var user = new User
            {
                FirstName = "test1",
                Password = "password",
                LastName = "test2"
            };
            byte[] photo = new byte[0];
            string name = "photo";
            string type = ".jpg";
            //act
            var repository = new UsersRepository(ConnectionString);
            var result = repository.Create(user, photo, name, type);

            _tempUsers.Add(result.Id);
            
            //asserts
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.Password, result.Password);
            Assert.AreEqual(user.Photo, result.Photo);
            
        }

        

        [TestMethod]
        public void ShouldGetUser()
        {
            var user = new User
            {
                FirstName = "test1",
                Password = "password",
                LastName = "test2"
            };
            byte[] photo = new byte[0];
            string name = "photo";
            string type = ".jpg";
            var repositiry = new UsersRepository(ConnectionString);
            var newUser = repositiry.Create(user, photo, name, type);

            _tempUsers.Add(newUser.Id);
            var result = repositiry.Get(newUser.Id);

            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.Password, result.Password);
            Assert.AreEqual(user.Photo, result.Photo);
            
        }

        [TestMethod]
        public void ShouldUpdateUser()
        {
            var user = new User
            {
                FirstName = "test1",
                Password = "password",
                LastName = "test2"
            };
            User testUser = new User
            {
                FirstName = "testUser"
            };
            byte[] photo = new byte[0];
            string name = "photo";
            string type = ".jpg";
            var repository = new UsersRepository(ConnectionString);
            var newUser = repository.Create(user, photo, name, type);

            _tempUsers.Add(newUser.Id);

            var result = repository.Update(newUser, testUser);
             

            Assert.AreEqual(testUser.FirstName, result.FirstName);
            
        }

        [TestMethod]
        public void ShouldGetUserChats()
        {
            const string name = "чат";
            const string name2 = "чат2";
            var user1 = new User()
            {
                FirstName = "test1",
                Password = "password1",
                LastName = "test2"
            };
            var user2 = new User()
            {
                FirstName = "test3",
                Password = "password2",
                LastName = "test3"
            };
            var user3 = new User()
            {
                FirstName = "test3",
                Password = "password2",
                LastName = "test3"
            };
            byte[] photo = new byte[0];
            string nameF = "photo";
            string type = ".jpg";
            var repository = new UsersRepository(ConnectionString);
            var result1 = repository.Create(user1, photo, nameF, type);
            var result2 = repository.Create(user2, photo, nameF, type);
            var result3 = repository.Create(user3, photo, nameF, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);
            _tempUsers.Add(result3.Id);

            IEnumerable<Guid> members1 = new Guid[] { result1.Id, result2.Id };
            IEnumerable<Guid> members2 = new Guid[] { result1.Id, result2.Id, result3.Id };

            var chatRepository = new GroupsRepository(ConnectionString, repository);
            var groupResult1 = chatRepository.Create(members1, name);
            var groupResult2 = chatRepository.Create(members2, name2);

            _tempGroups.Add(groupResult1.Id);
            _tempGroups.Add(groupResult2.Id);

            List<Group> userGroups = repository.GetUserChats(result1.Id);

            var res = userGroups.Find(x => x.Id == groupResult1.Id);
            Assert.AreEqual(groupResult1.Id, res.Id);

            res = userGroups.Find(x => x.Id == groupResult2.Id);
            Assert.AreEqual(groupResult2.Id, res.Id);

        }

        [TestMethod]
        public void ShouldLoginUser()
        {
            var user = new User
            {
                FirstName = "test",
                Password = "password",
                LastName = "test2345"
            };
            byte[] photo = new byte[0];
            string name = "photo";
            string type = ".jpg";
            var usersRepository = new UsersRepository(ConnectionString);
            var newUser = usersRepository.Create(user, photo, name, type);

            _tempUsers.Add(user.Id);

            var testUser = new User
            {
                FirstName = "test",
                Password = "password",
                LastName = "test2345"
            };

            var result = usersRepository.Login(testUser.FirstName, testUser.LastName, testUser.Password);

            Assert.AreEqual(newUser.Id, result.Id);
            Assert.AreEqual(newUser.Photo, result.Photo);
            Assert.AreEqual(newUser.FirstName, result.FirstName);
        }

        [TestCleanup]
        public void Clean()
        {
            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString).Delete(id);
            foreach (var id in _tempGroups)
                new GroupsRepository(ConnectionString, new UsersRepository(ConnectionString)).DeleteChat(id);
        }

    }
}
