using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messenger.Model;

namespace Messenger.DataLayer.Sql.Tests
{
    [TestClass]
    public class MessageRepositoryTests
    {
        private readonly string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";
        private readonly List<Guid> _tempGroups = new List<Guid>();
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempMessages = new List<Guid>();

        [TestMethod]
        public void ShouldCreateMessage()
        {
            string messageText = "Привет. Как дела?";
            bool status = false;
            string name = "photo";
            string type = ".jpg";
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
            var file3 = new Files();
            byte[] photo = new byte[0];
            var usersRepository = new UsersRepository(ConnectionString);
            var result1 = usersRepository.Create(user1, photo, name, type);
            var result2 = usersRepository.Create(user2, photo, name, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);

            var chatRepository = new GroupsRepository(ConnectionString, usersRepository);
            IEnumerable<Guid> members = new Guid[] {result1.Id, result2.Id}; 
            var groupResult = chatRepository.Create(members, "чатик");

            _tempGroups.Add(groupResult.Id);

            var messageRepository = new MessageRepository(ConnectionString);
            var messageResult1 = messageRepository.CreateWithFile(result1.Id, groupResult.Id, photo, status, name, type);
            var messageResult2 = messageRepository.Create(messageText, result2.Id, groupResult.Id, status);

            _tempMessages.Add(messageResult1.Id);
            _tempMessages.Add(messageResult2.Id);

            Assert.AreEqual(result1.Id, messageResult1.MessageFromUserId);
            Assert.AreEqual(file3.Owner, result1.Id);

            Assert.AreEqual(result2.Id, messageResult2.MessageFromUserId);
        }

       

        [TestCleanup]
        public void Clean()
        {
            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString).Delete(id);
            foreach (var groupId in _tempGroups)
                new GroupsRepository(ConnectionString, new UsersRepository(ConnectionString)).DeleteChat(groupId);
            foreach (var messageId in _tempMessages)
                new MessageRepository(ConnectionString).Delete(messageId);
        }
    
    }
}
