using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.Client.DataAccess
{
    public static class DataAccess
    {
        private readonly static HttpClient _client;
        
        static DataAccess()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:63509/") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<User> CreateUser(UserParameters userParam)
        {

            HttpResponseMessage response;
            response = await _client.PostAsJsonAsync("api/user",
               new UserParameters()
               {
                   photo = userParam.photo,
                   user = userParam.user,
                   name = userParam.name,
                   type = userParam.type
               });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<User>();
            }
            return null;
        }

        public static async Task<User> LoginUser(string FirstName, string LastName, string Password)
        {
            var path = "api/user/" + FirstName + "/" + LastName + "/" + Password;
            var response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<User>();
            }
            return null;
        }

        public static async Task<Group> CreateChat(GroupCreateParameters group)
        {
            var response = await _client.PostAsJsonAsync("api/group", new GroupCreateParameters()
            {
                members = group.members,
                name = group.name
            });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Group>();
            }
            return null;
        }

        public static async Task<List<User>> GetAllUsers()
        {
            var response = await _client.GetAsync("api/user").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<User>>().ConfigureAwait(false);
            }
            return null;
        }

        public static async Task<List<Group>> GetUsersChats(Guid userId)
        {
            var path = "api/user/" + userId + "/groups";
            var response = await _client.GetAsync(path).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Group>>().ConfigureAwait(false);
            }
            return null;
        }

        public static async Task<List<Message>> GetUsersMessagesInGroup(Guid groupId)
        {
            var path = "api/group/" + groupId + "/messages";
            var response = await _client.GetAsync(path).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Message>>().ConfigureAwait(false);
            }
            return null;
        }

        public static async Task<Message> CreateMessage(CreateMessageParameters message)
        {
            var response = await _client.PostAsJsonAsync("api/message", new CreateMessageParameters()
            {
                messageText = message.messageText,
                userFromId = message.userFromId,
                groupToId = message.groupToId,
                status = message.status
            });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Message>();
            }
            return null;
        }

        public static async Task<User> GetUser(Guid id)
        {
            var path = "api/user/" + id;
            var response = await _client.GetAsync(path).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<User>().ConfigureAwait(false);
            }
            return null;
        }

        public static async Task<Message> CreateMessageWithFile(CreateMessageWithFileParameters message)
        {
            var response = await _client.PostAsJsonAsync("api/messages", new CreateMessageWithFileParameters()
            {
                userFromId = message.userFromId,
                groupToId = message.groupToId,
                photo = message.photo,
                status = message.status,
                name = message.name,
                type = message.type
            });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Message>();
            }
            return null;
        }

        public static async Task<Files> GetFileFromId(Guid id)
        {
            var path = "api/file/" + id;
            var response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Files>();
            }
            return null;
        }

        public static async Task<User> UpdateUser(UpdateParameters users)
        {
            var path = "api/user";
            var response = await _client.PutAsJsonAsync(path, new UpdateParameters()
            {
                user = users.user,
                newUser = users.newUser
            });
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<User>();
            }
            return null;
        }

        public static async Task<HttpStatusCode> DeleteMessage(Guid id)
        {
            var response = await _client.DeleteAsync($"api/message/{id}");
            return response.StatusCode;
        }

        public static async Task<HttpStatusCode> UpdateMessage(Guid id)
        {
            var IsRead = 1;
            var response = await _client.PutAsJsonAsync($"api/message/{id}", IsRead).ConfigureAwait(false);
            return response.StatusCode;
        }
    }
}
