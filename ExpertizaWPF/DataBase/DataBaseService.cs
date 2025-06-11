using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ExpertizaWPF.DataBase
{
    public class DataBaseService
    {
        private const string connectionString = "Data Source=C:\\Users\\ASUS\\Desktop\\PromExpertizabot\\PromExpertizabot\\bin\\Debug\\net9.0\\config\\DataBase.db;Version=3;ReadOnly=False;";
        public List<Application> GetAllAplications()
        {
            var applications = new List<Application>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT a.ID, u.USER_NAME AS UserName,y.FIRST_NAME || ' ' || y.LAST_NAME AS Executor,p.NAME AS df, a.TYPE, a.DESCRIPTION, a.STATUS, a.DATE_CREATE, a.DATE_CLOSE
            FROM Applications a
            LEFT JOIN Users u ON a.USER = u.TG_ID
            LEFT JOIN Users y ON a.EXECUTOR = y.TG_ID
            LEFT JOIN Users o ON a.USER = o.TG_ID
            LEFT JOIN Objects p ON o.OBJECT_ID = p.ID";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            User = reader["UserName"].ToString(),
                            Executor = reader["Executor"].ToString(),
                            Type = reader["TYPE"].ToString(),
                            Object_Id = reader["df"].ToString(),
                            Description = reader["DESCRIPTION"].ToString(),
                            Status = reader["STATUS"].ToString(),
                            Date_create = reader["DATE_CREATE"].ToString(),
                            Date_close = reader["DATE_CLOSE"].ToString()
                        });
                    }
                }
            }
            return applications;
        }
    }
}
