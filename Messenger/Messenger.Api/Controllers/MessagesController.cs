using Messenger.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Messenger.DataLayer.Sql;
using Messenger.Model;

namespace Messenger.Api.Controllers
{
    public class MessagesController : ApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IMessageRepository _messageRepository;
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";

        public MessagesController()
        {
            _usersRepository = new UsersRepository(ConnectionString);
            _groupsRepository = new GroupsRepository(ConnectionString, _usersRepository);
            _messageRepository = new MessageRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/message")]
        public Message Create([FromBody] CreateMessageParameters messages)
        {
            var messageText = messages.messageText;
            var userFromId = messages.userFromId;
            var groupToId = messages.groupToId;
            return _messageRepository.Create(messageText, userFromId, groupToId);
        }

        [HttpDelete]
        [Route("api/message/{id}")]
        public void Delete(Guid id)
        {
            _messageRepository.Delete(id);
        }
    }
}