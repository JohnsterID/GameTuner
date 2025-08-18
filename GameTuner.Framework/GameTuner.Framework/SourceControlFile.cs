using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
	/// <summary>
	/// Stub implementation of SourceControlFile with P4API dependencies removed.
	/// All source control operations are disabled and return safe defaults.
	/// </summary>
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
			get { return file; }
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
			get { return false; } // Always false - no source control
		}

		public bool Owner
		{
			get { return false; } // Always false - no source control
		}

		public bool Changed
		{
			get { return false; } // Always false - no source control
		}

		public bool NewAdd
		{
			get { return false; } // Always false - no source control
		}

		public bool HaveLatest
		{
			get { return true; } // Always true - assume we have latest
		}

		public bool Deleted
		{
			get { return false; } // Always false - no source control
		}

		public SourceStatus Depot
		{
			get { return depot; }
		}

		public SourceStatus Client
		{
			get { return client; }
		}

		public static Image DefaultStatusImage
		{
			get { return Properties.Resources.ver_none; }
		}

		public string StatusText
		{
			get { return "No Source Control"; }
		}

		public Image StatusImage
		{
			get { return DefaultStatusImage; }
		}

		// Constructors
		public SourceControlFile(FileInfo fi) : this(fi, false) { }

		public SourceControlFile(FileInfo fi, bool autoRefresh) : this(fi, autoRefresh, false) { }

		public SourceControlFile(FileInfo fi, bool autoRefresh, bool bDiffOnRefresh)
		{
			file = fi;
			AutoRefresh = autoRefresh;
			DiffOnRefresh = bDiffOnRefresh;
			flags = SourceFlags.None;
			depot = SourceStatus.Empty;
			client = SourceStatus.Empty;
		}

		public SourceControlFile(string file, bool autoRefresh, bool bDiffOnRefresh) 
			: this(new FileInfo(file), autoRefresh, bDiffOnRefresh) { }

		public SourceControlFile(string file, bool autoRefresh) 
			: this(new FileInfo(file), autoRefresh, false) { }

		public SourceControlFile(string file) 
			: this(new FileInfo(file), false, false) { }

		// Stub methods - all source control operations disabled
		public void Refresh()
		{
			flags = SourceFlags.None;
			depot = SourceStatus.Empty;
			client = SourceStatus.Empty;
			// Source control functionality disabled - no P4API available
		}

		public void ParseInfo(string key, string value)
		{
			// No-op: source control info parsing disabled
		}

		public void GetLatest()
		{
			// Source control functionality disabled - no P4API available
		}

		public void UndoCheckout()
		{
			// Source control functionality disabled - no P4API available
		}

		public void Checkout()
		{
			// Source control functionality disabled - no P4API available
		}

		public void Checkout(string changeList)
		{
			// Source control functionality disabled - no P4API available
		}

		public bool Delete()
		{
			// Source control functionality disabled - no P4API available
			return false;
		}

		public void Add(string changeList)
		{
			// Source control functionality disabled - no P4API available
		}

		public List<SourceRevision> GetSourceRevision()
		{
			// Return empty list since we have no source control connection
			return new List<SourceRevision>();
		}
	}
}