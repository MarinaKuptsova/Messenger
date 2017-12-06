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

        public Message Create(string messageText, Guid userFromId, Guid groupToId, bool status)
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
                    SendTime = DateTime.Now,
                    Status = status,
                    IsRead = false
                };
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                            "insert into Message (Id, MessageText, MessageFromUserId, MessageToGroupId, SendTime, Status, IsRead) " +
                            "values (@id, @text, @from, @to, @sendtime, @status, @isread)";
                    command.Parameters.AddWithValue("@id", message.Id);
                    command.Parameters.AddWithValue("@text", message.MessageText);
                    command.Parameters.AddWithValue("@from", message.MessageFromUserId);
                    command.Parameters.AddWithValue("@to", message.MessageToGroupId);
                    command.Parameters.AddWithValue("@sendtime", message.SendTime);
                    command.Parameters.AddWithValue("@status", Convert.ToByte(message.Status));
                    command.Parameters.AddWithValue("@isread", Convert.ToByte(message.IsRead));
                    command.ExecuteNonQuery();
                }
                return message;
            }
        }

       

        public Message CreateWithFile(Guid userFromId, Guid groupToId, byte[] photo, bool status, string name, string type)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    Files file = new Files();
                    file.Id = Guid.NewGuid();
                    file.Owner = userFromId;
                    file.UserFile = photo;
                    var message = new Message()
                    {
                        Id = Guid.NewGuid(),
                        MessageFromUserId = userFromId,
                        MessageToGroupId = groupToId,
                        SendTime = DateTime.Now,
                        AttachedFile = file.Id, 
                        Status = status,
                        IsRead = false
                    };
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Files (Id, Owner, UserFile, Name, Type) values (@id, @owner, @userfile, @name, @type)";
                        command.Parameters.AddWithValue("@id", file.Id);
                        command.Parameters.AddWithValue("@owner", file.Owner);
                        command.Parameters.AddWithValue("@userfile", file.UserFile);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@type", type);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Message (Id, MessageFromUserId, MessageToGroupId, SendTime, AttachedFiles, Status, IsRead) " +
                            "values (@id, @from, @to, @sendtime, @files, @status, @isread)";
                        command.Parameters.AddWithValue("@id", message.Id);
                        command.Parameters.AddWithValue("@from", message.MessageFromUserId);
                        command.Parameters.AddWithValue("@to", message.MessageToGroupId);
                        command.Parameters.AddWithValue("@sendtime", message.SendTime);
                        command.Parameters.AddWithValue("@files", message.AttachedFile);
                        command.Parameters.AddWithValue("@status", Convert.ToByte(message.Status));
                        command.Parameters.AddWithValue("@isread", Convert.ToByte(message.IsRead));
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

        public void UpdateIsRead(Guid messageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "update Message set IsRead=1 where Id=@id";
                    command.Parameters.AddWithValue("@id", messageId);
                    command.ExecuteNonQuery();
                }
            }
        }

        
    }
}
