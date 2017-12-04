using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messenger.Model;

namespace Messenger.DataLayer.Sql.Tests
{
    [TestClass]
    public class GroupsRepositoryTest
    {
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";
        private readonly List<Guid> _tempGroups = new List<Guid>();
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempMessages = new List<Guid>();

        [TestMethod]
        public void ShouldCreateGroup()
        {
            const string name = "чат";
            string nameF = "photo";
            string type = ".jpg";
            var group = new Group()
            {
                GroupName = name
            };
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
            byte[] photo = new byte[0];
            var repository = new UsersRepository(ConnectionString);
            var result1 = repository.Create(user1, photo, nameF, type);
            var result2 = repository.Create(user2, photo, nameF, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);

            IEnumerable<Guid> members = new Guid[] {result1.Id, result2.Id};
            
            var chatRepository = new GroupsRepository(ConnectionString, repository);
            var groupResult = chatRepository.Create(members, name);

            _tempGroups.Add(groupResult.Id);
            
            Assert.AreEqual(group.GroupName, groupResult.GroupName);
        }

        

        [TestMethod]
        public void ShouldGetChatMembers()
        {
            const string name = "чат";
            string nameF = "photo";
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
            byte[] photo = new byte[0];
            var repository = new UsersRepository(ConnectionString);
            var result1 = repository.Create(user1, photo, nameF, type);
            var result2 = repository.Create(user2, photo, nameF, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);

            IEnumerable<Guid> members = new Guid[] { result1.Id, result2.Id };
           
            var chatRepository = new GroupsRepository(ConnectionString, repository);
            var groupResult = chatRepository.Create(members, name);

            _tempGroups.Add(groupResult.Id);

            var resultUsers = chatRepository.GetChatMembers(groupResult.Id);

            var res = resultUsers.Find(x => x.Id == result1.Id);
            Assert.AreEqual(result1.Id, res.Id);

            res = resultUsers.Find(x => x.Id == result2.Id);
            Assert.AreEqual(result2.Id, res.Id);
        }
        
        [TestMethod]
        public void ShouldAddMember()
        {
            const string name = "чат";
            string nameF = "photo";
            string type = ".jpg";
            var user1 = new User()
            {
                FirstName = "test1",
                Password = "password1",
                LastName = "testtest2"
            };
            var user2 = new User()
            {
                FirstName = "test2",
                Password = "password2",
                LastName = "testtest2"
            };
            var user3 = new User()
            {
                FirstName = "test3",
                Password = "password3",
                LastName = "testtest3"
            };
            byte[] photo = new byte[0];
            var repository = new UsersRepository(ConnectionString);
            var result1 = repository.Create(user1, photo, nameF, type);
            var result2 = repository.Create(user2, photo, nameF, type);
            var result3 = repository.Create(user3, photo, nameF, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);
            _tempUsers.Add(result3.Id);

            IEnumerable<Guid> members = new Guid[] { result1.Id, result2.Id };

            var chatRepository = new GroupsRepository(ConnectionString, repository);
            var groupResult = chatRepository.Create(members, name);

            _tempGroups.Add(groupResult.Id);

            chatRepository.AddMember(result3.Id, groupResult.Id);

            var result = chatRepository.GetChatMembers(groupResult.Id);
            var resultUser = result.Find(x => x.Id == result3.Id);

            Assert.AreEqual(result3.Id, resultUser.Id);
        }

        [TestMethod]
        public void ShouldDeleteMember()
        {
            const string name = "чат";
            string nameF = "photo";
            string type = ".jpg";
            var user1 = new User()
            {
                FirstName = "test1",
                Password = "password1",
                LastName = "testtest2"
            };
            var user2 = new User()
            {
                FirstName = "test2",
                Password = "password2",
                LastName = "testtest2"
            };
            var user3 = new User()
            {
                FirstName = "test3",
                Password = "password3",
                LastName = "testtest3"
            };
            byte[] photo = new byte[0];
            var repository = new UsersRepository(ConnectionString);
            var result1 = repository.Create(user1, photo, nameF, type);
            var result2 = repository.Create(user2, photo, nameF, type);
            var result3 = repository.Create(user3, photo, nameF, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);
            _tempUsers.Add(result3.Id);

            IEnumerable<Guid> members = new Guid[] { result1.Id, result2.Id, result3.Id };

            var chatRepository = new GroupsRepository(ConnectionString, repository);
            var groupResult = chatRepository.Create(members, name);

            _tempGroups.Add(groupResult.Id);

            chatRepository.DeleteMember(result1.Id, groupResult.Id);

            var resultMembers = chatRepository.GetChatMembers(groupResult.Id);

            var result = resultMembers.Find(x => x.Id == result1.Id);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ShouldGetMessagesInGroup()
        {
            string messageText1 = "Привет. Как дела?";
            string messageText2 = "Привет. Нормально.";

            byte status = 1;
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
            string name = "photo";
            string type = ".jpg";
            var usersRepository = new UsersRepository(ConnectionString);
            var result1 = usersRepository.Create(user1, photo, name, type);
            var result2 = usersRepository.Create(user2, photo, name, type);

            _tempUsers.Add(result1.Id);
            _tempUsers.Add(result2.Id);

            var chatRepository = new GroupsRepository(ConnectionString, usersRepository);
            IEnumerable<Guid> members = new Guid[] { result1.Id, result2.Id };
            var groupResult = chatRepository.Create(members, "чатик");

            _tempGroups.Add(groupResult.Id);

            var messageRepository = new MessageRepository(ConnectionString);
            var messageResult1 = messageRepository.CreateWithFile(result1.Id, groupResult.Id, photo, status, name, type);
            var messageResult2 = messageRepository.Create(messageText2, result2.Id, groupResult.Id, status);

            _tempMessages.Add(messageResult1.Id);
            _tempMessages.Add(messageResult2.Id);

            var messages = new List<Message>();
            messages = chatRepository.GetUsersMessagesInGroup(groupResult.Id);

            var result = messages.Find(x => x.Id == messageResult1.Id);
            Assert.AreEqual(messageResult1.Id, result.Id);
            Assert.AreEqual(messageResult1.AttachedFile, result.AttachedFile);

            result = messages.Find(x => x.Id == messageResult2.Id);
            Assert.AreEqual(messageResult2.Id, result.Id);
            Assert.AreEqual(messageResult2.AttachedFile, result.AttachedFile);
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
