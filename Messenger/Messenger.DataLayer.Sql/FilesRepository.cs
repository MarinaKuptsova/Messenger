using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.Sql
{
    public class FilesRepository : IFilesRepository

    {
        private readonly string _connectionString;
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public FilesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Files GetFileFromId(Guid id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Files where Id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Файл с Id {id} не найден");
                        var file = new Files()
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Owner = reader.GetGuid(reader.GetOrdinal("Owner")),
                            UserFile = (byte[])reader["UserFile"],
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Type = reader.GetString(reader.GetOrdinal("Type"))
                        };
                        return file;
                    }
                }
            }
        }
    }
}
