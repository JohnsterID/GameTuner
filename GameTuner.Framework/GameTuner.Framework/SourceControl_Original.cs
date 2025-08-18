using System;
using System.Collections.Generic;
using System.IO;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class SourceControl : ISourceControl
	{
		private class SourceControlLabel : ISourceControlLabel
		{
			public string Name { get; private set; }

			public DateTime Date { get; private set; }

			public SourceControlLabel(string name, DateTime date)
			{
				Name = name;
				Date = date;
			}
		}

		private string password;

		private string client;

		public string Port { get; set; }

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = value;
			}
		}

		public string Client
		{
			get
			{
				return client;
			}
			set
			{
				client = value;
			}
		}

		public bool IsConnected
		{
			get
			{
				bool result = true;
				try
				{
					using (Connect())
					{
					}
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}

		public SourceControl()
			: this("", "")
		{
		}

		public SourceControl(string client)
			: this(client, "")
		{
		}

		public SourceControl(string client, string password)
		{
			this.client = client;
			this.password = password;
			Port = GameTuner.Framework.Properties.Resources.P4Port;
		}

		public P4Connection Connect()
		{
			P4Connection p4Connection = new P4Connection();
			p4Connection.Port = Port;
			p4Connection.Password = password;
			p4Connection.Client = client;
			p4Connection.Connect();
			return p4Connection;
		}

		public void Submit(string sDescription, string[] asFiles)
		{
			P4Connection p4Connection = Connect();
			P4PendingChangelist p4PendingChangelist = p4Connection.CreatePendingChangelist(sDescription);
			foreach (string text in asFiles)
			{
				p4Connection.Run("reopen", "-c", p4PendingChangelist.Number.ToString(), text);
			}
			p4PendingChangelist.Submit();
		}

		public string GetLocalPathFromDepot(string depot)
		{
			string text = "";
			P4Connection p4Connection;
			using (p4Connection = Connect())
			{
				P4RecordSet p4RecordSet = p4Connection.Run("where", depot);
				if (!p4RecordSet.HasErrors() && p4RecordSet.Records.Length > 0 && p4RecordSet.Records[p4RecordSet.Records.Length - 1].Fields.ContainsKey("path"))
				{
					text = p4RecordSet.Records[p4RecordSet.Records.Length - 1]["path"];
					text.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				}
				else
				{
					ThrowExecptionOnError("where", p4RecordSet);
				}
			}
			return text;
		}

		public bool IsSourceControlPath(string szPath)
		{
			if (szPath == null)
			{
				return false;
			}
			return szPath.StartsWith("//depot", StringComparison.OrdinalIgnoreCase);
		}

		public static void ThrowExecptionOnError(string command, P4RecordSet result)
		{
			if (result.HasErrors())
			{
				string text = string.Format("P4 '{0}' command failed:", command);
				string[] errors = result.Errors;
				foreach (string text2 in errors)
				{
					text = text + "\n  " + text2;
				}
				throw new Exception(text);
			}
		}

		public List<ISourceControlLabel> CollectLabels()
		{
			return CollectLabels("");
		}

		public List<ISourceControlLabel> CollectLabels(string pattern)
		{
			List<ISourceControlLabel> list = null;
			P4Connection p4Connection;
			using (p4Connection = Connect())
			{
				P4RecordSet p4RecordSet = p4Connection.Run("labels");
				if (!p4RecordSet.HasErrors())
				{
					list = new List<ISourceControlLabel>();
					P4Record[] records = p4RecordSet.Records;
					foreach (P4Record p4Record in records)
					{
						string text = p4Record.Fields["label"];
						string p4Date = p4Record.Fields["Update"];
						if (string.IsNullOrEmpty(pattern) || text.Contains(pattern))
						{
							SourceControlLabel item = new SourceControlLabel(text, p4Connection.ConvertDate(p4Date));
							list.Add(item);
						}
					}
				}
				else
				{
					ThrowExecptionOnError("labels", p4RecordSet);
				}
			}
			return list;
		}
	}
}
