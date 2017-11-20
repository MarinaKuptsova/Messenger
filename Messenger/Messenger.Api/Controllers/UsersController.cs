using Messenger.DataLayer;
using Messenger.DataLayer.Sql;
using Messenger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Messenger.Api.Controllers
{
    public class UsersController : ApiController

    {
        private readonly IUsersRepository _usersRepository;
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Messenger;MultipleActiveResultSets=True;User Id=Marina; Password=1234Ma";


        public UsersController()
        {
            _usersRepository = new UsersRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/user")]
        public User Create([FromBody] User user)
        {
            return _usersRepository.Create(user);
        }

        [HttpGet]
        [Route("api/user")]
        public List<User> GetAllUsers()
        {
            return _usersRepository.GetAllUsers();
        }
        
        [HttpGet]
        [Route("api/user/{id}")]
        public User Get(Guid id)
        {
            return _usersRepository.Get(id);
        }

        [HttpGet]
        [Route("api/user/{id}/groups")]
        public List<Group> GetChats(Guid id)
        {
            return _usersRepository.GetUserChats(id);
        }

        [HttpPut]
        [Route("api/user")]
        public void Update([FromBody]UpdateParameters users)////////////
        {
            User user = users.user;
            User newUser = users.newUser;
            _usersRepository.Update(user, newUser);
        }

        [HttpDelete]
        [Route("api/user/{id}")]
        public void Delete(Guid id)
        {
            _usersRepository.Delete(id);
        }

        /// <summary>
        /// Get group by id
        /// </summary>
        /// <param name="FirstName">FirstName</param>
        /// <param name="LastName">LastName</param>
        /// <param name="Password">Password</param>
        /// <returns></returns>

        [HttpGet]
        [Route("api/user/{FirstName}/{LastName}/{Password}")]
        public User Login(string FirstName, string LastName, string Password)
        {
            return _usersRepository.Login(FirstName, LastName, Password);
        }
    }
}