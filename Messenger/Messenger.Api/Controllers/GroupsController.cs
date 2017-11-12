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
    public class GroupsController : ApiController

    {
        private readonly IUsersRepository _usersRepository;
        private readonly IGroupsRepository _groupsRepository;
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";

        public GroupsController()
        {
            _usersRepository = new UsersRepository(ConnectionString);
            _groupsRepository = new GroupsRepository(ConnectionString, _usersRepository);
        }
        
        [HttpPost]
        [Route("api/group")]
        public Group CreateGroup([FromBody] GroupCreateParameters group)
        {
            var groupMembers = group.members;
            var groupName = group.name;
            return _groupsRepository.Create(groupMembers, groupName);
        }

        [HttpGet]
        [Route("api/group/{id}/messages")]
        public List<Message> GetMessages(Guid id)
        {
            return _groupsRepository.GetUsersMessagesInGroup(id);
        }


        /// <summary>
        /// Get group by id
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/group/{id}")]
        public List<User> GetMembers(Guid id)
        {
            return _groupsRepository.GetChatMembers(id);
        }

        /// <summary>
        /// Get group by id
        /// </summary>
        /// <param name="memberId">Member id</param>
        /// <param name="groupId">Group id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/group/{groupId}/user/{memberId}")]
        public void AddMember(Guid memberId, Guid groupId)//не работает
        {
            _groupsRepository.AddMember(memberId, groupId);
        }

        [HttpDelete]
        [Route("api/group/{groupId}")]
        public void Detete(Guid groupId)
        {
            _groupsRepository.DeleteChat(groupId);
        }
        
        
    }
}