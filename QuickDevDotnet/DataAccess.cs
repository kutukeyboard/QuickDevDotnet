using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;
using QuickDevDotnet;

namespace QuickDevDotnet
{
	public class DataAccess
  {
		AppSecurity Security = new AppSecurity();
		
		enum DbType {SQLServer, MySQL};
		DbType CurrentDbType;
		string ConfigFile = Environment.CurrentDirectory + "/config.txt";
		static string MyConnectionString	="";
		public string MyServer = "";
		public string MyDatabase = "";
		public string MyUserID = "";
		public string MyPassword = "";

		public bool ConnectDatabase()
		{
			LoadConnection();

			if(MyServer == "" || MyDatabase == "" || MyUserID == "" || MyPassword == "") {return false;}

			DbConnection Connection;
			if(CurrentDbType == DbType.SQLServer)
			{
				MyConnectionString = "Server=" + MyServer + ";Database=" + MyDatabase + ";User Id=" + MyUserID + ";Password=" + MyPassword + ";";
				Connection = new SqlConnection(MyConnectionString);
			}
			else
			{
				MyConnectionString = "Server=" + MyServer + ";Database=" + MyDatabase + ";Uid=" + MyUserID + ";Pwd=" + MyPassword + ";";
				Connection = new MySqlConnection(MyConnectionString);
			}

			try
			{
				Connection.Open();
				Connection.Close();
				return true;
			}
			catch 
			{
				Connection.Close();
				return false;;
			}
		}

		public void LoadConnection()
		{
			try
			{
				;
				string[] StringArray = Security.DecryptText(File.ReadAllText(ConfigFile)).Split(Convert.ToChar(";"));
				if(StringArray[0] == "SQLServer")
				{
					CurrentDbType = DbType.SQLServer;
				} else {
					CurrentDbType = DbType.MySQL;
				}

				MyServer = StringArray[0];
				MyDatabase = StringArray[1];
				MyUserID = StringArray[2];
				MyPassword = StringArray[3];
			} catch{}
		}

		public void SaveConnection()
		{
			try
			{
				File.WriteAllText(ConfigFile,CurrentDbType.ToString() + ";" + MyServer + ";" + MyDatabase + ";" + MyUserID + ";" + MyPassword);
				Security.EncryptFile(ConfigFile);
			} catch{}
		}

		public string GetSingleValue(string MyQuery)
		{
			string MyValue = string.Empty;
			DbConnection Connection;
			DbCommand Command;

			if (CurrentDbType == DbType.SQLServer)
			{
				Connection = new SqlConnection(MyConnectionString);
				Command = new SqlCommand();
			}
			else
			{
				Connection = new MySqlConnection(MyConnectionString);
				Command = new MySqlCommand();
			}
			Command.Connection = Connection;
			Command.CommandText = MyQuery;

			try
			{
				Connection.Open();
				using (Command)
				{
					using (DbDataReader reader = Command.ExecuteReader())
					{
						reader.Read();
						MyValue = reader[0].ToString();
					}
				}
				Connection.Close();
			}
			catch
			{
				Connection.Close();
			}

			return MyValue;
		}

		public DataTable GetData(string MyQuery)
		{
			DataTable MyDataTable = new DataTable();

			DbConnection Connection;
			DbCommand Command;
			DbDataAdapter Adapter;

			if (CurrentDbType == DbType.SQLServer)
			{
				Connection = new SqlConnection(MyConnectionString);
				Command = new SqlCommand();
				Adapter = new SqlDataAdapter((SqlCommand)Command);
			}
			else
			{
				Connection = new MySqlConnection(MyConnectionString);
				Command = new MySqlCommand();
				Adapter = new MySqlDataAdapter((MySqlCommand)Command);
			}
			Command.Connection = Connection;
			Command.CommandText = MyQuery;

			try
			{
				Connection.Open();
				Adapter.Fill(MyDataTable);
				Connection.Close();
			}
			catch
			{
				Connection.Close();
			}

			return MyDataTable;
		}

		public void ExecQuery(string MyQuery)
		{
			DbConnection connection;
			DbCommand command;
			if (CurrentDbType == DbType.SQLServer)
			{
				connection = new SqlConnection(MyConnectionString);
				command = new SqlCommand();
			}
			else
			{
				connection = new MySqlConnection(MyConnectionString);
				command = new MySqlCommand();
			}
			command.Connection = connection;
			command.CommandText = MyQuery;

			try
			{
				connection.Open();
				using (command)
				{
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
			catch (Exception ex)
			{
				connection.Close();
				return;
			}
		}
		
		public void ExportData(string OutputFilePath, string MyQuery)
		{
			DbConnection connection;
			DbCommand command;
			if (CurrentDbType== DbType.SQLServer)
			{
				connection = new SqlConnection(MyConnectionString);
				command = new SqlCommand();
			}
			else
			{
				connection = new MySqlConnection(MyConnectionString);
				command = new MySqlCommand();
			}
			command.Connection = connection;
			command.CommandText = MyQuery;
			string Output = "";
			try
			{
				connection.Open();
				using (command)
				{
					using (DbDataReader reader = command.ExecuteReader())
					{
						for (int i = 0; i < reader.FieldCount; i++)
						{
							Output += reader.GetName(i).ToString();
							if (i < reader.FieldCount - 1)
								Output += ",";
						}
						Output += "\n";
						
						while (reader.Read())
						{
							for (int i = 0; i < reader.FieldCount; i++)
							{
								Output += reader.GetValue(i);
								if (i < reader.FieldCount - 1)
									Output += ",";
							}
							Output +="\n";
						}
					}
				}
				connection.Close();
				File.WriteAllText(OutputFilePath,Output);
			}
			catch (Exception ex)
			{
				connection.Close();
				return;
			}
		}

	}
}
