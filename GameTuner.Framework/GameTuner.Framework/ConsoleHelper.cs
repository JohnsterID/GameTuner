using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameTuner.Framework
{
	public static class ConsoleHelper
	{
		private static bool consoleAttached;

		public static void CreateConsole()
		{
			if (!consoleAttached)
			{
				IntPtr foregroundWindow = GetForegroundWindow();
				int lpdwProcessId;
				GetWindowThreadProcessId(foregroundWindow, out lpdwProcessId);
				Process processById = Process.GetProcessById(lpdwProcessId);
				if (processById.ProcessName == "cmd")
				{
					AttachConsole(processById.Id);
					consoleAttached = true;
				}
				else
				{
					AllocConsole();
				}
				ShowConsole(false);
			}
		}

		public static void ShowConsole(bool show)
		{
			NativeMethods.ShowWindow(GetConsoleWindow(), show ? 5 : 0);
		}

		public static void DestroyConsole()
		{
			if (consoleAttached)
			{
				FreeConsole();
				consoleAttached = false;
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeConsole();

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool AttachConsole(int dwProcessId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("Kernel32", SetLastError = true)]
		private static extern IntPtr GetConsoleWindow();
	}
}
