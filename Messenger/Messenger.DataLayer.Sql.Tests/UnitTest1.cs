using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messenger.Model;

namespace Messenger.DataLayer.Sql.Tests
{
    [TestClass]
    public class UsersRepositoryTest
    {
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";
        

        [TestMethod]
        public void ShouldCreateUser()
        {
            //arrange
            var user = new User
            {
                FirstName = "Marina",
                IsActive = 1,
                Password = "password",
                LastName = "kupchik"
            };
            var file = new Files
            {
                Name = "Marina",
                Size = 10
            };
            
            //act
            var repository = new UsersRepository(ConnectionString);
            var result = repository.Create(user, file);
            
            //asserts
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.IsActive, result.IsActive);
            Assert.AreEqual(user.Password, result.Password);
            Assert.AreEqual(user.Photo, result.Photo);

            ShouldGetUser(result);
        }

        

        [TestMethod]
        public void ShouldGetUser(User user)
        {
            var repositiry = new UsersRepository(ConnectionString);
            var result = repositiry.Get(user.Id);

            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.Password, result.Password);
            Assert.AreEqual(user.IsActive, result.IsActive);
            Assert.AreEqual(user.Photo, result.Photo);

            ShouldUpdateUser(user);
        }

        [TestMethod]
        public void ShouldUpdateUser(User user)
        {
            User testUser = new User
            {
                FirstName = "testUser"
            };
            user.FirstName = "testUser";
            var repository = new UsersRepository(ConnectionString);
            repository.Update(user);
            var result = repository.Get(user.Id);

            Assert.AreEqual(testUser.FirstName, result.FirstName);

            ShouldDeleteUser(result);
        }

        [TestMethod]
        public void ShouldDeleteUser(User user)
        {
            var repository = new UsersRepository(ConnectionString);

            repository.Delete(user.Id);
            var result = repository.Get(user.Id);

            Assert.AreEqual(null, result);
        }

        

    }
}
