using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class ErrorHandling
	{
		private static string sm_sAppName = "Unknown App";

		private static string sm_sAppVersion = string.Empty;

		private static IErrorReportForm sm_kErrorForm = null;

		public static bool ShowErrorMessages = true;

		private static List<string> sm_ErrorReportRecipients = new List<string>();

		private static List<string> sm_ErrorReportAttachments = new List<string>();

		public static string AppName
		{
			get
			{
				return sm_sAppName;
			}
			set
			{
				sm_sAppName = value;
			}
		}

		public static string AppVersion
		{
			get
			{
				return sm_sAppVersion;
			}
			set
			{
				sm_sAppVersion = value;
			}
		}

		public static IErrorReportForm ErrorReportForm
		{
			get
			{
				return sm_kErrorForm;
			}
			set
			{
				sm_kErrorForm = value;
			}
		}

		public static List<string> ErrorReportRecipients
		{
			get
			{
				return sm_ErrorReportRecipients;
			}
		}

		public static List<string> ErrorReportAttachments
		{
			get
			{
				return sm_ErrorReportAttachments;
			}
		}

		public static void CatchUnhandledExceptions()
		{
			try
			{
				Application.ThreadException += ThreadExceptionHandler;
				AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			}
			catch (Exception exception)
			{
				Error(exception, "Exception throw while setting unhandled exception handlers!", ErrorLevel.SendReport);
			}
		}

		public static void Error(Exception exception, ErrorLevel level)
		{
			Error(exception, string.Empty, level);
		}

		public static void Error(Exception exception, string sNote, ErrorLevel level)
		{
			try
			{
				LogException(exception, sNote, ValidationResultLevel.Error);
				switch (level)
				{
				case ErrorLevel.ShowMessage:
					if (string.IsNullOrEmpty(sNote))
					{
						sNote = ((exception == null) ? "null exception" : exception.Message);
					}
					ShowErrorMessage(exception, sNote);
					break;
				case ErrorLevel.SendReport:
					ErrorReport(exception, sNote);
					break;
				}
			}
			catch (Exception exception2)
			{
				try
				{
					LogException(exception2);
					ErrorReport(exception2, "Occurred during error handling.\nOriginal error:\n" + GetErrorReportText(exception, string.Empty));
				}
				catch
				{
				}
			}
		}

		public static string GetErrorReportText(Exception exception, string sNote)
		{
			return GetErrorReportText(exception, sNote, string.Empty);
		}

		public static string GetErrorReportText(Exception exception, string sNote, string sUserComments)
		{
			try
			{
				string text;
				string text2;
				if (UserInfo.InHouse)
				{
					try
					{
						UserInfo current = UserInfo.GetCurrent();
						text = current.UserName;
						text2 = current.FullName;
					}
					catch
					{
						text = string.Empty;
						text2 = "Unkown User";
					}
				}
				else
				{
					text = "Unkown";
					text2 = "Not an in-house user.";
				}
				string text3;
				string text4;
				try
				{
					text3 = Environment.MachineName.ToString();
					text4 = string.Empty;
					OperatingSystem oSVersion = Environment.OSVersion;
					if (oSVersion.Platform == PlatformID.Win32S)
					{
						text4 = "Windows 3.1";
					}
					else if (oSVersion.Platform == PlatformID.WinCE)
					{
						text4 = "Windows CE";
					}
					else if (oSVersion.Platform == PlatformID.Xbox)
					{
						text4 = "XBox";
					}
					else if (oSVersion.Platform == PlatformID.MacOSX)
					{
						text4 = "Mac OSX";
					}
					else if (oSVersion.Platform == PlatformID.Unix)
					{
						text4 = "Unix";
					}
					else if (oSVersion.Platform == PlatformID.Win32Windows)
					{
						switch (oSVersion.Version.Minor)
						{
						case 0:
							text4 = "Windows 95";
							break;
						case 10:
							text4 = "Windows 98";
							break;
						case 90:
							text4 = "Windows ME";
							break;
						}
					}
					else if (oSVersion.Platform == PlatformID.Win32NT)
					{
						if (oSVersion.Version.Major < 5)
						{
							text4 = "Windows NT";
						}
						else if (oSVersion.Version.Major == 5)
						{
							switch (oSVersion.Version.Minor)
							{
							case 0:
								text4 = "Windows 2000";
								break;
							case 1:
								text4 = "Windows XP";
								break;
							case 2:
								text4 = "Windows 2003";
								break;
							}
						}
						else if (oSVersion.Version.Major == 6)
						{
							switch (oSVersion.Version.Minor)
							{
							case 0:
								text4 = "Windows Vista";
								break;
							case 1:
								text4 = "Windows 2008";
								break;
							case 2:
								text4 = "Windows 7";
								break;
							}
						}
					}
					text4 = ((!(text4 == string.Empty)) ? (text4 + " (" + oSVersion.ServicePack + ")") : oSVersion.VersionString);
				}
				catch
				{
					text4 = "Unkown";
					text3 = "Unkown";
				}
				string text5 = (ReflectionHelper.Is64Bit ? "64 bit" : "32 bit");
				string text6 = string.Concat("Application: ", AppName, "\nUser: ", text, " (", text2, ")\nTime: ", DateTime.Today.DayOfWeek, " ", DateTime.Now.ToString(), "\n\nRuntime: ", text5, "\nOS: ", text4, "\nMachine: ", text3, "\n\n");
				if (!string.IsNullOrEmpty(sNote))
				{
					text6 = text6 + "Note:\n" + sNote + "\n\n";
				}
				if (!string.IsNullOrEmpty(sUserComments))
				{
					text6 = text6 + "User Comments:\n" + sUserComments + "\n\n";
				}
				return text6 + GetExceptionReportText(exception);
			}
			catch (Exception exception2)
			{
				LogException(exception2);
				return string.Empty;
			}
		}

		private static string GetExceptionReportText(Exception exception)
		{
			try
			{
				if (exception == null)
				{
					return string.Empty;
				}
				string text = "Exception: " + exception.GetType().ToString() + "\nSource: " + exception.Source + "\nThread: " + Thread.CurrentThread.Name + "\nDescription: " + exception.Message + "\n\nStack Trace:\n" + exception.StackTrace;
				if (exception.InnerException != null)
				{
					text = text + "\n\nInner Exception:\n\n" + GetExceptionReportText(exception.InnerException);
				}
				return text;
			}
			catch (Exception exception2)
			{
				LogException(exception2);
				return string.Empty;
			}
		}

		private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
		{
			if (e != null)
			{
				UnhandledException(e.Exception);
			}
		}

		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			if (e != null)
			{
				UnhandledException(e.ExceptionObject as Exception);
			}
		}

		private static void UnhandledException(Exception e)
		{
			if (e == null)
			{
				Error(e, "Null exception sent to unhandled exception handler", ErrorLevel.SendReport);
			}
			else if (ReflectionHelper.IsType<OutOfMemoryException>(e.GetType()))
			{
				Error(e, "The application failed due to an \"Out of Memory\" exception.", ErrorLevel.ShowMessage);
			}
			else
			{
				Error(e, string.Empty, ErrorLevel.SendReport);
			}
			try
			{
				Application.Exit();
			}
			catch
			{
				Application.ExitThread();
			}
		}

		private static void LogException(Exception exception)
		{
			LogException(exception, string.Empty, ValidationResultLevel.Error);
		}

		private static void LogException(Exception exception, string sNote, ValidationResultLevel level)
		{
			ExceptionLogger.Log(exception, ReflectionHelper.GetDisplayName(exception), sNote, level);
		}

		private static void ShowErrorMessage(Exception exception, string sMessage)
		{
			if (ShowErrorMessages)
			{
				ErrorMessage errorMessage = new ErrorMessage();
				errorMessage.Error = exception;
				errorMessage.Message = sMessage;
				errorMessage.ShowDialog();
			}
		}

		private static void ErrorReport(Exception exception, string sNote)
		{
			string text = GetErrorReportText(exception, sNote);
			try
			{
				if (ErrorReportForm == null)
				{
					ErrorReportForm = new ErrorReportWnd();
				}
				ErrorReportForm.ErrorReport = text;
				ErrorReportForm.ShowDialog();
				text = ErrorReportForm.ErrorReport;
				if (!string.IsNullOrEmpty(ErrorReportForm.Comments))
				{
					text = GetErrorReportText(exception, sNote, ErrorReportForm.Comments);
				}
			}
			catch (Exception exception2)
			{
				LogException(exception2);
			}
			if (UserInfo.InHouse)
			{
				string text2 = "Unknown user";
				try
				{
					UserInfo current = UserInfo.GetCurrent();
					text2 = current.FullName;
				}
				catch (Exception exception3)
				{
					LogException(exception3);
				}
				Mail.Attachments.AddRange(ErrorReportAttachments);
				string sSubject = AppName + " Error Report From " + text2;
				string sMessageBody = text;
				if (sm_ErrorReportRecipients.Count > 0 && Mail.SendMessage(sm_ErrorReportRecipients, sSubject, sMessageBody, MailPriority.High))
				{
					MessageBox.Show("The error report has been send successfully!", "Error Report Sent");
					return;
				}
				try
				{
					MessageBox.Show("Failed to send error report by e-mail!", "Can't send error report");
					CopyableTextViewer copyableTextViewer = new CopyableTextViewer();
					copyableTextViewer.Text = "Error Report";
					copyableTextViewer.DisplayedText = text;
					copyableTextViewer.ShowDialog();
					return;
				}
				catch (Exception exception4)
				{
					LogException(exception4);
					return;
				}
			}
			string text3 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\GameTuner Error Report.txt";
			MessageBox.Show("Error reporting system not available.\n\nYour error will be saved to:\n" + text3 + "\n\nThank you for using GameTuner!", "Error Report Not Sent");
			try
			{
				StreamWriter streamWriter = File.CreateText(text3);
				string[] array = text.Split('\n');
				string[] array2 = array;
				foreach (string value in array2)
				{
					streamWriter.WriteLine(value);
				}
				streamWriter.Close();
			}
			catch
			{
				MessageBox.Show("That didn't work either.", ":-(");
			}
		}
	}
}
