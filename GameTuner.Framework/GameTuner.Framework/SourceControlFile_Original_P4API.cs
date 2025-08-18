using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	public class SourceControlFile : ISourceControlStatus, ISourceControlAction
	{
		[Flags]
		private enum SourceFlags
		{
			None = 0,
			Owner = 1,
			Changed = 2,
			Newadd = 4,
			Deleted = 8
		}

		private static string[] scmkeys;

		private FileInfo file;

		private SourceFlags flags;

		private SourceStatus depot;

		private SourceStatus client;

		public bool AutoRefresh { get; set; }

		public bool DiffOnRefresh { get; set; }

		public FileInfo File
		{
			get
			{
				return file;
			}
			set
			{
				file = value;
				if (AutoRefresh)
				{
					Refresh();
				}
			}
		}

		public bool Controlled
		{
			get
			{
				return depot.Rev != 0;
			}
		}

		public bool Owner
		{
			get
			{
				return GetBit(SourceFlags.Owner);
			}
		}

		public bool Changed
		{
			get
			{
				return GetBit(SourceFlags.Changed);
			}
		}

		public bool NewAdd
		{
			get
			{
				if (GetBit(SourceFlags.Newadd))
				{
					return !Controlled;
				}
				return false;
			}
		}

		public bool HaveLatest
		{
			get
			{
				if (Controlled)
				{
					return depot.Rev == client.Rev;
				}
				return false;
			}
		}

		public bool Deleted
		{
			get
			{
				return GetBit(SourceFlags.Deleted);
			}
		}

		public SourceStatus Depot
		{
			get
			{
				return depot;
			}
		}

		public SourceStatus Client
		{
			get
			{
				return client;
			}
		}

		public static Image DefaultStatusImage
		{
			get
			{
				return GameTuner.Framework.Properties.Resources.file_document;
			}
		}

		public string StatusText
		{
			get
			{
				if (Deleted)
				{
					return "Deleted";
				}
				string result = "Ready";
				if (Controlled)
				{
					result = ((!Owner) ? "Controlled " : "Checked Out ");
					result = ((!HaveLatest) ? (result + "(old)") : (result + "(current)"));
				}
				else if (NewAdd)
				{
					result = "New Add";
				}
				return result;
			}
		}

		public Image StatusImage
		{
			get
			{
				Image image = new Bitmap(DefaultStatusImage);
				using (Graphics g = Graphics.FromImage(image))
				{
					if (Deleted)
					{
						DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.p4_delete, 0f, 0f);
					}
					else if (Controlled)
					{
						if (Owner)
						{
							DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.p4_checkout, 0f, 0f);
						}
						if (HaveLatest)
						{
							DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.p4_sync, 0f, 0f);
						}
						else
						{
							DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.p4_old, 0f, 0f);
						}
					}
					if (NewAdd)
					{
						DrawingHelper.DrawImage(g, GameTuner.Framework.Properties.Resources.p4_add, 0f, 0f);
					}
				}
				return image;
			}
		}

		static SourceControlFile()
		{
			scmkeys = new string[12]
			{
				"depotFile", "clientFile", "headAction", "headType", "headTime", "headRev", "headChange", "headModTime", "haveRev", "action",
				"actionOwner", "otherOpen"
			};
		}

		public SourceControlFile(FileInfo fi)
			: this(fi, true, true)
		{
		}

		public SourceControlFile(FileInfo fi, bool autoRefresh)
			: this(fi, autoRefresh, true)
		{
		}

		public SourceControlFile(FileInfo fi, bool autoRefresh, bool bDiffOnRefresh)
		{
			DiffOnRefresh = bDiffOnRefresh;
			AutoRefresh = autoRefresh;
			File = fi;
		}

		public SourceControlFile(string file, bool autoRefresh, bool bDiffOnRefresh)
			: this(new FileInfo(file), autoRefresh, bDiffOnRefresh)
		{
		}

		public SourceControlFile(string file, bool autoRefresh)
			: this(new FileInfo(file), autoRefresh, true)
		{
		}

		public SourceControlFile(string file)
			: this(new FileInfo(file), true, true)
		{
		}

		public void Refresh()
		{
			flags = SourceFlags.None;
			depot = SourceStatus.Empty;
			client = SourceStatus.Empty;
			// Source control functionality disabled - no P4API available
		}

		public void ParseInfo(string key, string value)
		{
			if (value == null || value.Length == 0)
			{
				return;
			}
			if (key.CompareTo("headRev") == 0)
			{
				depot.Rev = Convert.ToInt32(value);
				return;
			}
			if (key.CompareTo("depotFile") == 0)
			{
				depot.File = value;
				return;
			}
			if (key.CompareTo("clientFile") == 0)
			{
				client.File = value;
			}
			if (key.CompareTo("haveRev") == 0)
			{
				client.Rev = Convert.ToInt32(value);
			}
			if (key.CompareTo("headTime") == 0)
			{
				depot.Time = Convert.ToUInt64(value);
			}
			if (key.CompareTo("headChange") == 0)
			{
				depot.Change = Convert.ToUInt64(value);
			}
			if (key.CompareTo("change") == 0)
			{
				if (value.CompareTo("default") == 0)
				{
					client.Change = 0uL;
				}
				else
				{
					client.Change = Convert.ToUInt64(value);
				}
			}
			if (key.CompareTo("headType") == 0)
			{
				depot.Type = value;
			}
			if (key.CompareTo("headAction") == 0)
			{
				SetBit(SourceFlags.Deleted, value.CompareTo("delete") == 0);
			}
			if (key.CompareTo("action") == 0)
			{
				SetBit(SourceFlags.Newadd, value.CompareTo("add") == 0);
				SetBit(SourceFlags.Deleted, value.CompareTo("delete") == 0);
			}
			if (key.CompareTo("actionOwner") == 0)
			{
				string userName = UserInfo.GetCurrent().UserName;
				SetBit(SourceFlags.Owner, value.CompareTo(userName) == 0);
			}
			if (key.CompareTo("otherOpen") == 0)
			{
				depot.OtherOpen = Convert.ToInt32(value);
			}
		}

		private bool GetBit(SourceFlags flag)
		{
			return (flags & flag) == flag;
		}

		private void SetBit(SourceFlags flag, bool enable)
		{
			if (enable)
			{
				flags |= flag;
			}
			else
			{
				flags &= ~flag;
			}
		}

		public void GetLatest()
		{
			// Source control functionality disabled - no P4API available
		}

		public void UndoCheckout()
		{
			if (!Available.SourceControl)
			{
				return;
			}
			bool flag = false;
			P4Connection p4Connection;
			using (p4Connection = Context.Get<SourceControl>().Connect())
			{
				P4RecordSet p4RecordSet = p4Connection.Run("revert", file.FullName);
				if (!p4RecordSet.HasErrors())
				{
					flag = true;
				}
			}
			if (flag)
			{
				Refresh();
			}
		}

		public void Checkout()
		{
			Checkout(string.Empty);
		}

		public void Checkout(string changeList)
		{
			if (!Controlled)
			{
				return;
			}
			bool flag = false;
			P4Connection p4Connection;
			using (p4Connection = Context.Get<SourceControl>().Connect())
			{
				if (changeList == null || changeList.Length == 0)
				{
					P4RecordSet p4RecordSet = p4Connection.Run("edit", file.FullName);
					if (!p4RecordSet.HasErrors())
					{
						flag = true;
					}
				}
				else
				{
					P4RecordSet p4RecordSet2 = p4Connection.Run("edit", "-c", changeList, file.FullName);
					if (!p4RecordSet2.HasErrors())
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				Refresh();
			}
		}

		public bool Delete()
		{
			if (!Controlled)
			{
				return false;
			}
			P4Connection p4Connection;
			using (p4Connection = Context.Get<SourceControl>().Connect())
			{
				P4RecordSet p4RecordSet = p4Connection.Run("delete", file.FullName);
				if (p4RecordSet.HasErrors())
				{
					return false;
				}
				Refresh();
			}
			return true;
		}

		public void Add(string changeList)
		{
			if (!Available.SourceControl)
			{
				return;
			}
			bool flag = false;
			P4Connection p4Connection;
			using (p4Connection = Context.Get<SourceControl>().Connect())
			{
				if (changeList == null || changeList.Length == 0)
				{
					P4RecordSet p4RecordSet = p4Connection.Run("add", file.FullName);
					if (!p4RecordSet.HasErrors())
					{
						flag = true;
					}
				}
				else
				{
					P4RecordSet p4RecordSet2 = p4Connection.Run("add", "-c", changeList, file.FullName);
					if (!p4RecordSet2.HasErrors())
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				Refresh();
			}
		}

		public List<SourceRevision> GetSourceRevision()
		{
			List<SourceRevision> list = null;
			P4Connection p4Connection;
			using (p4Connection = Context.Get<SourceControl>().Connect())
			{
				P4RecordSet p4RecordSet = p4Connection.Run("filelog", "-L", file.FullName);
				if (!p4RecordSet.HasErrors())
				{
					list = new List<SourceRevision>();
					P4Record[] records = p4RecordSet.Records;
					foreach (P4Record p4Record in records)
					{
						int num = p4Record.ArrayFields["rev"].Length;
						for (int j = 0; j < num; j++)
						{
							SourceRevision sourceRevision = new SourceRevision();
							sourceRevision.Change = Convert.ToUInt64(p4Record.ArrayFields["change"][j]);
							sourceRevision.Rev = Convert.ToInt32(p4Record.ArrayFields["rev"][j]);
							sourceRevision.Description = p4Record.ArrayFields["desc"][j];
							sourceRevision.User = p4Record.ArrayFields["user"][j];
							string p4Date = p4Record.ArrayFields["time"][j];
							sourceRevision.Date = p4Connection.ConvertDate(p4Date);
							list.Add(sourceRevision);
						}
					}
				}
				else
				{
					SourceControl.ThrowExecptionOnError("filelog", p4RecordSet);
				}
			}
			return list;
		}
	}
}
