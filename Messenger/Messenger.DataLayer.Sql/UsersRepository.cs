using System;
using Messenger.Model;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;

namespace Messenger.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Create(User user, byte[] photo, string name, string type)
        {
            logger.Debug("Создание пользователя");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException ex)
                {
                    logger.Error("Не удается подключить к базе...{0}", ex.Message);
                }
                
                var file = new Files();
                using (var transaction = connection.BeginTransaction())
                {
                    file.Id = Guid.NewGuid();
                    user.Id = Guid.NewGuid();
                    user.Photo = file.Id;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Users (Id, FirstName, LastName, Password) values (@Id, @FirstName, @LastName, @Password)";
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.ExecuteNonQuery();
                    }
                    
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "insert into Files (Id, Owner, UserFile, Name, Type)" +
                                              "values (@Id, @Owner, @UserFile, @name, @type)";
                        command.Parameters.AddWithValue("@Id", file.Id);
                        command.Parameters.AddWithValue("@Owner", user.Id);
                        command.Parameters.AddWithValue("@UserFile", photo);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@type", type);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "update Users set Photo=@FileId where Id=@Id";
                        command.Parameters.AddWithValue("@FileId", file.Id);
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                
            }
            return user;
        }

        public void Delete(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "update Users set Photo=null where Id=@userId";
                        command.Parameters.AddWithValue("@userId", id);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "delete from Message where MessageFromUserId=@id";
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "delete from Files where Owner=@Id";
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "delete from UserGroup where UserId=@id";
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "delete from Users where Id =@userId";
                        command.Parameters.AddWithValue("@userId", id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                
            }
        }

        public User Get(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users u join Files f on u.Photo = f.Id where u.Id = @id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с Id {id} не найден");
                        var user = new User()
                        {
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Photo = reader.GetGuid(reader.GetOrdinal("Photo")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Ava = (byte[])reader["UserFile"]
                        };

                        return user;

                    }
                }
            }
        }

        public User Update(User user, User newUser)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    if (newUser.FirstName == null) newUser.FirstName = user.FirstName;
                    if (newUser.LastName == null) newUser.LastName = user.LastName;
                    if (newUser.Password == null) newUser.Password = user.Password;
                    
                    
                    
                    command.CommandText =
                        "update Users set FirstName=@FirstName, LastName=@LastName, Password=@Password where Id=@Id";
                    command.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                    command.Parameters.AddWithValue("@LastName", newUser.LastName);
                    command.Parameters.AddWithValue("@Password", newUser.Password);
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }
                return newUser;
            }
        }

        public List<Group> GetUserChats(Guid userId)
        {
            List<Group> groups = new List<Group>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "select ug.GroupId as Id, g.GroupName, g.CreateDate from UserGroup ug inner join Groups g on ug.GroupId=g.Id where ug.UserId=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                var group = new Group();
                                group.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                                group.GroupName = reader.GetString(reader.GetOrdinal("GroupName"));
                                group.CreateDate = reader.GetDateTime(reader.GetOrdinal("CreateDate"));
                                groups.Add(group);
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            return groups;
        }

        public User Login(string FirstName, string LastName, string Password)
        {
            logger.Debug("Аутентификация...");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    logger.Error("Не удаётся подключиться к базе...{0}", ex.Message);
                }
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users u " +
                                          "join Files f on u.Photo=f.Id " +
                                          "where FirstName=@name and LastName=@lastname and Password=@password";
                    command.Parameters.AddWithValue("@name", FirstName);
                    command.Parameters.AddWithValue("@lastname", LastName);
                    command.Parameters.AddWithValue("@password", Password);
                    logger.Info("Получение данных...");
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            logger.Error("Пользователь с таким именем и паролем не найден...");
                            return null;
                        }
                        var user = new User()
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Photo = reader.GetGuid(reader.GetOrdinal("Photo")),
                            FirstName = FirstName,
                            LastName = LastName,
                            Password = Password,
                            Ava = (byte[])reader["UserFile"]
                        };
                        return user;
                    }
                }
            }
        }

        public List<User> GetAllUsers()
        {
            var AllUsers = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                logger.Debug("Соединение с базой...");
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users u join Files f on u.Photo=f.Id";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User()
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Photo = reader.GetGuid(reader.GetOrdinal("Photo")),
                                Ava = (byte[])reader["UserFile"]
                            };
                            AllUsers.Add(user);
                        }
                    }
                }
            }
            return AllUsers;
        }
    }
}

