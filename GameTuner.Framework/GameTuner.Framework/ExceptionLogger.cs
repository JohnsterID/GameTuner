using System;

namespace GameTuner.Framework
{
	public static class ExceptionLogger
	{
		public class ExceptionEntry
		{
			public Exception Exception { get; private set; }

			public DateTime Time { get; private set; }

			public string Caption { get; private set; }

			public string Note { get; private set; }

			public ValidationResultLevel Level { get; private set; }

			public ExceptionEntry(Exception e, string caption, string note, ValidationResultLevel level)
			{
				Exception = e;
				Time = DateTime.Now;
				Note = note;
				Level = level;
				Caption = caption;
			}
		}

		public static object LockObject { get; private set; }

		public static ListEvent<ExceptionEntry> ExceptionLog { get; private set; }

		static ExceptionLogger()
		{
			LockObject = new object();
			ExceptionLog = new ListEvent<ExceptionEntry>();
		}

		public static void Log(string caption, string note)
		{
			Log(null, caption, note, ValidationResultLevel.None);
		}

		public static void Log(string caption, string note, ValidationResultLevel level)
		{
			Log(null, caption, note, level);
		}

		public static void Log(Exception e)
		{
			Log(e, ReflectionHelper.GetDisplayName(e), "", ValidationResultLevel.Error);
		}

		public static void Log(Exception e, string note)
		{
			Log(e, ReflectionHelper.GetDisplayName(e), note, ValidationResultLevel.Error);
		}

		public static void Log(Exception e, string caption, string note, ValidationResultLevel level)
		{
			lock (LockObject)
			{
				ExceptionLog.Add(new ExceptionEntry(e, caption, note, level));
			}
		}
	}
}
