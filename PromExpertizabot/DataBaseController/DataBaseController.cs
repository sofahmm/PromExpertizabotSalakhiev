using System.Net;
using System.Text;
using System;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using PromExpertizabot;

namespace Database
{
	internal class DataBaseController
	{
		public DataBaseController(string connectionString)
		{
			connectionstring = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		protected static string connectionstring = Program.connectionstring;

		public static long GetLastId(string tableName, string idColumn = "ID")
		{
			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var command = connection.CreateCommand();
				command.CommandText = $"SELECT MAX({idColumn}) FROM {tableName};";

				var result = command.ExecuteScalar();

				if (result != null && result != DBNull.Value)
				{
					return Convert.ToInt64(result);
				}

				return 0;
			}
		}

		public static List<string> GetAllTables()
		{
			
			var tables = new List<string>();

			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var command = connection.CreateCommand();
				command.CommandText = @"
				SELECT name 
				FROM sqlite_master 
				WHERE type='table' 
				AND name NOT LIKE 'sqlite_%'
				ORDER BY name;";

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						tables.Add(reader.GetString(0));
					}
				}
			}

			return tables;
		}

		public static void Insert(Dictionary<string, dynamic> data)
		{
			string tableName = data["TABLE_NAME"];
			data.Remove("TABLE_NAME");

			if (string.IsNullOrEmpty(tableName) || data == null || data.Count == 0)
			{
				throw new ArgumentException("Table name and data cannot be empty");
			}
			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var columns = string.Join(", ", data.Keys);
				var parameters = string.Join(", ", data.Keys.Select(k => "@" + k));

				var command = connection.CreateCommand();
				command.CommandText = $"INSERT INTO '{tableName}' ({columns}) VALUES ({parameters})";

				foreach (var item in data)
				{
					command.Parameters.AddWithValue("@" + item.Key, item.Value);
				}
				int affectedRows = command.ExecuteNonQuery();
				Log($"==DATA BASE INSERT== affected rows: {affectedRows}");
			}
		}

		public static List<Dictionary<string, dynamic>> Select(string tableName, Dictionary<string, dynamic> whereConditions = null)
		{
			if (string.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException("Table name cannot be empty");
			}

			var results = new List<Dictionary<string, dynamic>>();

			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var command = connection.CreateCommand();

				command.CommandText = $"SELECT * FROM {tableName}";

				if (whereConditions != null && whereConditions.Count > 0)
				{
					var whereClauses = whereConditions.Keys.Select(k => $"{k} = @{k}");
					command.CommandText += " WHERE " + string.Join(" AND ", whereClauses);

					foreach (var item in whereConditions)
					{
						command.Parameters.AddWithValue("@" + item.Key, item.Value);
					}
				}

				try
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var row = new Dictionary<string, dynamic>();

							for (int i = 0; i < reader.FieldCount; i++)
							{
								row[reader.GetName(i)] = reader.GetValue(i);
							}

							results.Add(row);
						}
					}
				}
				catch
				{
					//Console.WriteLine(command.CommandText);
					return results;
				}

				Log($"==DATA BASE SELECT== command: {command.CommandText}");
			}
			
			return results;
		}
		
		public static void Update(Dictionary<string, dynamic> data, Dictionary<string, dynamic> whereConditions)
		{
			string tableName = data["TABLE_NAME"];
			data.Remove("TABLE_NAME");
			if (string.IsNullOrEmpty(tableName) || data == null || data.Count == 0 || whereConditions == null || whereConditions.Count == 0)
			{
				throw new ArgumentException("Table name, data and conditions cannot be empty");
			}

			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var command = connection.CreateCommand();

				var setClauses = data.Keys.Select(k => $"{k} = @set_{k}");
				command.CommandText = $"UPDATE {tableName} SET {string.Join(", ", setClauses)}";

				var whereClauses = whereConditions.Keys.Select(k => $"{k} = @where_{k}");
				command.CommandText += " WHERE " + string.Join(" AND ", whereClauses);

				foreach (var item in data)
				{
					command.Parameters.AddWithValue("@set_" + item.Key, item.Value);
				}

				foreach (var item in whereConditions)
				{
					command.Parameters.AddWithValue("@where_" + item.Key, item.Value);
				}

				//Console.WriteLine(command.CommandText);
				int affectedRows = command.ExecuteNonQuery();
				//Log($"==DATA BASE UPDATE== affected rows: {affectedRows}");
			}
		}

		public static void Delete(string tableName, Dictionary<string, dynamic> whereConditions)
		{
			if (string.IsNullOrEmpty(tableName) || whereConditions == null || whereConditions.Count == 0)
			{
				//throw new ArgumentException("Table name and conditions cannot be empty");
			}

			using (var connection = new SQLiteConnection(connectionstring))
			{
				connection.Open();

				var command = connection.CreateCommand();

				command.CommandText = $"DELETE FROM {tableName}";

				var whereClauses = whereConditions.Keys.Select(k => $"{k} = @{k}");
				command.CommandText += " WHERE " + string.Join(" AND ", whereClauses);

				foreach (var item in whereConditions)
				{
					command.Parameters.AddWithValue("@" + item.Key, item.Value);
				}

				int affectedRows = command.ExecuteNonQuery();

				if (affectedRows == 0)
				{

				}
				
			  	Log($"==DATA BASE DELETE== affected rows: {affectedRows}");
			}
		}

		public static void Log(string message)
		{
			using (var connection = new SQLiteConnection(connectionstring))
			{
				/*connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "INSERT INTO LOG (MASSAGE, TIME_STAMP) VALUES (@message, @time)";
				command.Parameters.AddWithValue("@message", message);
				command.Parameters.AddWithValue("@time", System.DateTime.Now);
				command.ExecuteNonQuery();*/
			}
		}
	}
}