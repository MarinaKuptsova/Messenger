using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public static async Task<User> CreateUser(User user)
        {
            HttpResponseMessage response;
            response = await _client.PostAsJsonAsync("api/user",
                new User()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    Photo = user.Photo
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
    }
}
