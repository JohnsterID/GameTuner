using System;
using System.Data.SqlClient;

namespace GameTuner.Framework
{
	public static class Database
	{
		private static string connectionString;

		public static string ConnectionString
		{
			get
			{
				return connectionString;
			}
			set
			{
				connectionString = value;
			}
		}

		public static SqlConnection Connect
		{
			get
			{
				if (connectionString == null || connectionString == "")
				{
					throw new Exception("Connection string has not been set");
				}
				return new SqlConnection(connectionString);
			}
		}

		public static string GetStr(string name, SqlDataReader dr)
		{
			int ordinal = dr.GetOrdinal(name);
			if (dr.IsDBNull(ordinal))
			{
				return "";
			}
			return dr[name].ToString().Trim();
		}

		public static bool GetBool(string name, SqlDataReader dr)
		{
			int ordinal = dr.GetOrdinal(name);
			if (dr.IsDBNull(ordinal))
			{
				return false;
			}
			return dr.GetBoolean(ordinal);
		}

		public static int GetInt(string name, SqlDataReader dr)
		{
			int ordinal = dr.GetOrdinal(name);
			if (dr.IsDBNull(ordinal))
			{
				return 0;
			}
			return dr.GetInt32(ordinal);
		}

		public static int QueryUniqueId(string field, string table, SqlConnection conn, SqlTransaction t)
		{
			int result = 1;
			SqlDataReader sqlDataReader = null;
			string cmdText = "SELECT max(" + field + ") FROM " + table;
			try
			{
				SqlCommand sqlCommand = new SqlCommand(cmdText, conn, t);
				sqlDataReader = sqlCommand.ExecuteReader();
				if (sqlDataReader.HasRows && sqlDataReader.Read())
				{
					result = (sqlDataReader.IsDBNull(0) ? 1 : (sqlDataReader.GetInt32(0) + 1));
				}
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
			return result;
		}

		public static int QueryExistence(SqlCommand cmd)
		{
			int result = 0;
			SqlDataReader sqlDataReader = null;
			try
			{
				sqlDataReader = cmd.ExecuteReader();
				if (sqlDataReader.HasRows && sqlDataReader.Read())
				{
					result = ((!sqlDataReader.IsDBNull(0)) ? sqlDataReader.GetInt32(0) : 0);
				}
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
			return result;
		}

		public static int QueryExistence(string query, SqlConnection conn, SqlTransaction t)
		{
			int result = 0;
			SqlDataReader sqlDataReader = null;
			try
			{
				SqlCommand sqlCommand = new SqlCommand(query, conn, t);
				sqlDataReader = sqlCommand.ExecuteReader();
				if (sqlDataReader.HasRows && sqlDataReader.Read())
				{
					result = ((!sqlDataReader.IsDBNull(0)) ? sqlDataReader.GetInt32(0) : 0);
				}
			}
			finally
			{
				if (sqlDataReader != null)
				{
					sqlDataReader.Close();
				}
			}
			return result;
		}
	}
}
