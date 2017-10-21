using System;
using Messenger.Model;
using System.Data.SqlClient;

namespace Messenger.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Create(User user, Files file)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    file.Id = Guid.NewGuid();
                    user.Id = Guid.NewGuid();
                    user.Photo = file.Id;
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText =
                            "insert into Users (Id, FirstName, LastName, Password, IsActive) values (@Id, @FirstName, @LastName, @Password, @IsActive)";
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@IsActive", user.IsActive);
                        command.ExecuteNonQuery();
                    }
                    
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "insert into Files (Id, Name, Size, Owner)" +
                                              "values (@Id, @Name, @Size, @Owner)";
                        command.Parameters.AddWithValue("@Id", file.Id);
                        command.Parameters.AddWithValue("@Name", file.Name);
                        command.Parameters.AddWithValue("@Size", file.Size);
                        command.Parameters.AddWithValue("@Owner", user.Id);
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
                        command.CommandText = "update Users set Photo=null";
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
                    command.CommandText = "select * from Users where Id =@Id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с Id {id} не найден");
                        return new User
                        {
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName"))  ,
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            IsActive = reader.GetInt32(reader.GetOrdinal("IsActive")),
                            Photo = reader.GetGuid(reader.GetOrdinal("Photo")),
                            Password = reader.GetString(reader.GetOrdinal("Password"))
                        };
                    }
                }
            }
        }

        public void Update(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "update Users set FirstName=@FirstName, LastName=@LastName, Password=@Password, IsActive=@IsActive where Id=@Id";
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}

