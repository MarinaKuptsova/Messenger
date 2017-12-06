using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;
using System.Windows;

namespace Messenger.DataLayer.Sql
{
    public class GroupsRepository : IGroupsRepository
    {
        private readonly string _connectionString;
        private readonly IUsersRepository _usersRepository;

        public GroupsRepository(string connectionString, IUsersRepository usersRepository)
        {
            _connectionString = connectionString;
            _usersRepository = usersRepository;
        }

        public void AddMember(Guid memberId, Guid groupId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert into UserGroup (GroupId, UserId) values (@GroupId, @UserId)";
                    command.Parameters.AddWithValue("@GroupId", groupId);
                    command.Parameters.AddWithValue("@UserId", memberId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Group Create(IEnumerable<Guid> members, string name)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var group = new Group
                    {
                        CreateDate = DateTime.Now,
                        Id = Guid.NewGuid(),
                        GroupName = name
                    };
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Groups (Id, CreateDate, GroupName) values (@Id, @CreateDate, @GroupName)";
                        command.Parameters.AddWithValue("@Id", group.Id);
                        command.Parameters.AddWithValue("@CreateDate", group.CreateDate);
                        command.Parameters.AddWithValue("@GroupName", group.GroupName);
                        command.ExecuteNonQuery();
                    }
                    foreach (var userId in members)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "insert into UserGroup(GroupId, UserId) values (@GroupId, @UserId)";
                            command.Parameters.AddWithValue("@GroupId", group.Id);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    return group;
                }
            }
        }

        public void DeleteChat(Guid groupId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "delete from UserGroup where GroupId=@groupId";
                        command.Parameters.AddWithValue("@groupId", groupId);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "delete from Groups where Id=@id";
                        command.Parameters.AddWithValue("@id", groupId);
                        command.ExecuteNonQuery();

                    }
                    transaction.Commit();
                }
                
            }
        }

        public void DeleteMember(Guid memberId, Guid groupId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "delete from UserGroup where UserId=@userId and GroupId=@groupId";
                    command.Parameters.AddWithValue("@userId", memberId);
                    command.Parameters.AddWithValue("@groupId", groupId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<User> GetChatMembers(Guid groupId)
        {
            List<User> listUsers = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "select ug.UserId as Id, u.FirstName, u.LastName, u.Password, u.Photo from UserGroup ug inner join Users u on ug.UserId=u.Id where ug.GroupId=@id;";
                    command.Parameters.AddWithValue("@id", groupId);
                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                User user = new User();
                                user.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                                user.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                user.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                user.Photo = reader.GetGuid(reader.GetOrdinal("Photo"));
                                user.Password = reader.GetString(reader.GetOrdinal("Password"));
                                listUsers.Add(user);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        
                    }
                }
            }
            return listUsers;
        }

        public List<Message> GetUsersMessagesInGroup(Guid groupId)
        {
            var usersMessages = new List<Message>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "select * from Message m " +
                        "left join Files f on m.AttachedFiles = f.Id " +
                        "where MessageToGroupId = @id order by SendTime";
                    command.Parameters.AddWithValue("@id", groupId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("AttachedFiles")))
                            {
                                var message = new Message()
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    MessageFromUserId = reader.GetGuid(reader.GetOrdinal("MessageFromUserId")),
                                    MessageToGroupId = reader.GetGuid(reader.GetOrdinal("MessageToGroupId")),
                                    SendTime = reader.GetDateTime(reader.GetOrdinal("SendTime")),
                                    AttachedFile = reader.GetGuid(reader.GetOrdinal("AttachedFiles")),
                                    Status = Convert.ToBoolean(reader.GetByte(reader.GetOrdinal("Status"))),
                                    AttachedFileName = String.Concat(reader.GetString(reader.GetOrdinal("Name")), reader.GetString(reader.GetOrdinal("Type"))),
                                    TextblockVisibility = Visibility.Collapsed,
                                    ButtonVisibility = Visibility.Visible,
                                    TextblockFileNameVisibility = Visibility.Visible,
                                    IsRead = Convert.ToBoolean(reader.GetByte(reader.GetOrdinal("IsRead")))
                                    
                                };
                                usersMessages.Add(message);
                            }
                            else
                            {
                                var message = new Message()
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                    MessageFromUserId = reader.GetGuid(reader.GetOrdinal("MessageFromUserId")),
                                    MessageToGroupId = reader.GetGuid(reader.GetOrdinal("MessageToGroupId")),
                                    SendTime = reader.GetDateTime(reader.GetOrdinal("SendTime")),
                                    Status = Convert.ToBoolean(reader.GetByte(reader.GetOrdinal("Status"))),
                                    ButtonVisibility = Visibility.Collapsed,
                                    TextblockFileNameVisibility = Visibility.Collapsed,
                                    TextblockVisibility = Visibility.Visible,
                                    IsRead = Convert.ToBoolean(reader.GetByte(reader.GetOrdinal("IsRead")))
                                };
                                usersMessages.Add(message);
                            }


                        }
                    }
                }
            }
            return usersMessages;
        }

    }
}
