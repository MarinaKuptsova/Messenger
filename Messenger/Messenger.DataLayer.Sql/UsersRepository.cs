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
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "insert into User (First_Name, Last_Name, Login, Password, Is_Active, Photo, User_Group) " +
                                              "values ('" + user.FirstName + "', '" + user.LastName + "', '" +
                                              user.Login + "', '" + user.Password + "', '" +
                                              user.IsActive + "', '" + user.Photo + "', '" + user.UserGroup + "')";
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        file.Id = Guid.NewGuid();
                        command.CommandText = "insert into Files (Id, Name, Size, Owner)" +
                                              "values ('" + file.Id + "', '" + file.Name + "', '" +
                                              file.Size + "', '" + file.Owner + "' )";
                        command.ExecuteNonQuery();
                    }
                        transaction.Commit();
                }
                
            }
            return user;
        }

        public void Delete(string login)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "delete from User where Login = '"+ login +"'";
                        command.ExecuteNonQuery();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "delete from Files " +
                                              "where Id = (select Photo from User where Login = '"+ login +"') ";
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                
            }
        }

        public User Get(string login)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from User where Login = '" + login + "'";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с Login {login} не найден");
                        return new User
                        {
                            FirstName = Convert.ToString(reader["First_Name"]),
                            LastName = Convert.ToString(reader["Last_Name"]),
                            Login = Convert.ToString(reader["Login"]),
                            IsActive = Convert.ToBoolean(reader["Is_Active"]),
                            Photo = Convert.ToInt16(reader["Photo"]),
                            Password = Convert.ToString(reader["Password"]),
                            UserGroup = Convert.ToString(reader["User_Group"])
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
                        "update User(First_Name, Last_Name, Login, Password, Is_Active, User_Group)" +
                        "values ('" + user.FirstName + "', '" + user.LastName + "', '" + user.Login + "', '"
                        + user.Password + "', '" + user.IsActive + "', '" + user.UserGroup +
                        "')" +
                        "where Login = '" + user.Login + "'";
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}

