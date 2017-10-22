using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer.Sql
{
    class GroupsRepository : IGroupsRepository
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

        public void Create(IEnumerable<Guid> members, string name)
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
                            "insert into Group (Id, CreateDate, GroupName) values (@Id, @CreateDate, @GroupName)";
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
                            command.CommandText = "insert into UserGroup (GroupId, UserId) values (@GroupId, @UserId)";
                            command.Parameters.AddWithValue("@GroupId", group.Id);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void DeleteChat(Guid groupId)
        {
            throw new NotImplementedException();
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

        public IEnumerable<Group> GetUserChats(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
