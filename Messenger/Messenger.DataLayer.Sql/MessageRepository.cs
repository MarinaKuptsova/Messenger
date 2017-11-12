using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer.Sql
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Message Create(string messageText, Guid userFromId, Guid groupToId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var message = new Message()
                {
                    Id = Guid.NewGuid(),
                    MessageText = messageText,
                    MessageFromUserId = userFromId,
                    MessageToGroupId = groupToId,
                    SendTime = DateTime.Now
                };
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                            "insert into Message (Id, MessageText, MessageFromUserId, MessageToGroupId, SendTime) " +
                            "values (@id, @text, @from, @to, @sendtime)";
                    command.Parameters.AddWithValue("@id", message.Id);
                    command.Parameters.AddWithValue("@text", message.MessageText);
                    command.Parameters.AddWithValue("@from", message.MessageFromUserId);
                    command.Parameters.AddWithValue("@to", message.MessageToGroupId);
                    command.Parameters.AddWithValue("@sendtime", message.SendTime);
                    command.ExecuteNonQuery();
                }
                return message;
            }
        }

        public Message CreateWithFile(string messageText, Guid userFromId, Guid groupToId, Files file)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    file.Id = Guid.NewGuid();
                    file.Owner = userFromId;
                    var message = new Message()
                    {
                        Id = Guid.NewGuid(),
                        MessageFromUserId = userFromId,
                        MessageToGroupId = groupToId,
                        MessageText = messageText,
                        SendTime = DateTime.Now,
                        AttachedFile = file.Id
                    };
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Files (Id, Owner) values (@id, @owner)";
                        command.Parameters.AddWithValue("@id", file.Id);
                        command.Parameters.AddWithValue("@owner", file.Owner);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Message (Id, MessageText, MessageFromUserId, MessageToGroupId, SendTime, AttachedFiles) " +
                            "values (@id, @text, @from, @to, @sendtime, @files)";
                        command.Parameters.AddWithValue("@id", message.Id);
                        command.Parameters.AddWithValue("@text", message.MessageText);
                        command.Parameters.AddWithValue("@from", message.MessageFromUserId);
                        command.Parameters.AddWithValue("@to", message.MessageToGroupId);
                        command.Parameters.AddWithValue("@sendtime", message.SendTime);
                        command.Parameters.AddWithValue("@files", message.AttachedFile);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return message;
                }
            }
        }

        public void Delete(Guid messageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "delete from Message where Id=@id";
                    command.Parameters.AddWithValue("@id", messageId);
                    command.ExecuteNonQuery();
                }
            }
        }

        
    }
}
