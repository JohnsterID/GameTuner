using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GameTuner.Framework
{
	public static class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct HDITEM
		{
			public uint mask;

			public int cxy;

			public string pszText;

			public IntPtr hbm;

			public int cchTextMax;

			public int fmt;

			public IntPtr lParam;

			public int iImage;

			public int iOrder;

			public uint type;

			public IntPtr pvFilter;

			public uint state;
		}

		public struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;

			public void SetRect(Rectangle rect)
			{
				left = rect.Left;
				top = rect.Top;
				right = rect.Right;
				bottom = rect.Bottom;
			}
		}

		public struct SCROLLINFO
		{
			public int cbSize;

			public int fMask;

			public int nMin;

			public int nMax;

			public int nPage;

			public int nPos;

			public int nTrackPos;

			public static SCROLLINFO Empty
			{
				get
				{
					SCROLLINFO sCROLLINFO = default(SCROLLINFO);
					sCROLLINFO.cbSize = Marshal.SizeOf(sCROLLINFO);
					return sCROLLINFO;
				}
			}
		}

		public enum ScrollBarDirection
		{
			SB_HORZ,
			SB_VERT,
			SB_CTL,
			SB_BOTH
		}

		[Flags]
		public enum ScrollInfoMask
		{
			SIF_RANGE = 1,
			SIF_PAGE = 2,
			SIF_POS = 4,
			SIF_DISABLENOSCROLL = 8,
			SIF_TRACKPOS = 0x10,
			SIF_ALL = 0x17
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class WNDCLASS
		{
			public int style;

			public WndProc lpfnWndProc;

			public int cbClsExtra;

			public int cbWndExtra;

			public IntPtr hInstance;

			public IntPtr hIcon;

			public IntPtr hCursor;

			public IntPtr hbrBackground;

			public string lpszMenuName;

			public string lpszClassName;

			public WNDCLASS()
			{
				hInstance = IntPtr.Zero;
				hIcon = IntPtr.Zero;
				hCursor = IntPtr.Zero;
				hbrBackground = IntPtr.Zero;
			}
		}

		public struct MSG
		{
			public IntPtr hwnd;

			public int message;

			public IntPtr wParam;

			public IntPtr lParam;

			public int time;

			public int pt_x;

			public int pt_y;
		}

		private struct MARGINS
		{
			public int leftWidth;

			public int rightWidth;

			public int topHeight;

			public int bottomHeight;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class WNDCLASS_I
		{
			public int style;

			public IntPtr lpfnWndProc;

			public int cbClsExtra;

			public int cbWndExtra;

			public IntPtr hInstance;

			public IntPtr hIcon;

			public IntPtr hCursor;

			public IntPtr hbrBackground;

			public IntPtr lpszMenuName;

			public IntPtr lpszClassName;

			public WNDCLASS_I()
			{
				hInstance = IntPtr.Zero;
				hIcon = IntPtr.Zero;
				hCursor = IntPtr.Zero;
				hbrBackground = IntPtr.Zero;
				lpszMenuName = IntPtr.Zero;
				lpszClassName = IntPtr.Zero;
			}
		}

		public struct COPYDATASTRUCT
		{
			public uint dwData;

			public int cbData;

			public IntPtr lpData;
		}

		public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

		public enum WaveFormats
		{
			Pcm = 1,
			Float = 3
		}

		[Flags]
		public enum WaveFileFormat : uint
		{
			WAVE_FORMAT_1M08 = 1u,
			WAVE_FORMAT_1S08 = 2u,
			WAVE_FORMAT_1M16 = 4u,
			WAVE_FORMAT_1S16 = 8u,
			WAVE_FORMAT_2M08 = 0x10u,
			WAVE_FORMAT_2S08 = 0x20u,
			WAVE_FORMAT_2M16 = 0x40u,
			WAVE_FORMAT_2S16 = 0x80u,
			WAVE_FORMAT_4M08 = 0x100u,
			WAVE_FORMAT_4S08 = 0x200u,
			WAVE_FORMAT_4M16 = 0x400u,
			WAVE_FORMAT_4S16 = 0x800u,
			WAVE_FORMAT_44M08 = 0x100u,
			WAVE_FORMAT_44S08 = 0x200u,
			WAVE_FORMAT_44M16 = 0x400u,
			WAVE_FORMAT_44S16 = 0x800u,
			WAVE_FORMAT_48M08 = 0x1000u,
			WAVE_FORMAT_48S08 = 0x2000u,
			WAVE_FORMAT_48M16 = 0x4000u,
			WAVE_FORMAT_48S16 = 0x8000u,
			WAVE_FORMAT_96M08 = 0x10000u,
			WAVE_FORMAT_96S08 = 0x20000u,
			WAVE_FORMAT_96M16 = 0x40000u,
			WAVE_FORMAT_96S16 = 0x80000u
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public struct WaveCaps
		{
			public ushort wMid;

			public ushort wPid;

			public uint vDriverVersion;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;

			public WaveFileFormat dwFormats;

			public ushort wChannels;

			public ushort wReserved1;

			public uint dwSupport;
		}

		public struct WaveFormat
		{
			public short wFormatTag;

			public short nChannels;

			public int nSamplesPerSec;

			public int nAvgBytesPerSec;

			public short nBlockAlign;

			public short wBitsPerSample;

			public short cbSize;

			public WaveFormat(int rate, int bits, int channels)
			{
				wFormatTag = 1;
				nChannels = (short)channels;
				nSamplesPerSec = rate;
				wBitsPerSample = (short)bits;
				cbSize = 0;
				nBlockAlign = (short)(channels * (bits / 8));
				nAvgBytesPerSec = nSamplesPerSec * nBlockAlign;
			}
		}

		public struct WaveHdr
		{
			public IntPtr lpData;

			public int dwBufferLength;

			public int dwBytesRecorded;

			public IntPtr dwUser;

			public int dwFlags;

			public int dwLoops;

			public IntPtr lpNext;

			public int reserved;
		}

		public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHdr wavhdr, int dwParam2);

		public const int PM_NOREMOVE = 0;

		public const int PM_REMOVE = 1;

		public const int PM_NOYIELD = 2;

		public const int WS_VSCROLL = 2097152;

		public const int WS_HSCROLL = 1048576;

		public const int WM_CREATE = 1;

		public const int WM_DESTROY = 2;

		public const int WM_MOVE = 3;

		public const int WM_SIZE = 5;

		public const int WM_ACTIVATE = 6;

		public const int WM_SETFONT = 48;

		public const int WM_COPYDATA = 74;

		public const int WM_NCCALCSIZE = 131;

		public const int WM_NCPAINT = 133;

		public const int WM_HSCROLL = 276;

		public const int WM_VSCROLL = 277;

		public const int WM_USER = 1024;

		public const int WM_APP = 32768;

		public const int HDS_BUTTONS = 2;

		public const int HDS_HOTTRACK = 4;

		public const int HDS_HIDDEN = 8;

		public const int HDS_FULLDRAG = 128;

		public const int HDS_FLAT = 512;

		public const int HDS_CHECKBOXES = 1024;

		public const int HDS_NOSIZING = 2048;

		public const int HDS_OVERFLOW = 4096;

		public const int HDM_FIRST = 4608;

		public const int HDM_INSERTITEMW = 4618;

		public const int HDI_WIDTH = 1;

		public const int HDI_TEXT = 2;

		public const int SWP_NOSIZE = 1;

		public const int SWP_NOMOVE = 2;

		public const int SWP_NOZORDER = 4;

		public const int SWP_NOREDRAW = 8;

		public const int SWP_NOACTIVATE = 16;

		public const int SWP_FRAMECHANGED = 32;

		public const int SWP_SHOWWINDOW = 64;

		public const int SWP_HIDEWINDOW = 128;

		public const int SWP_NOCOPYBITS = 256;

		public const int SWP_NOOWNERZORDER = 512;

		public const int SWP_NOSENDCHANGING = 1024;

		public const int SW_HIDE = 0;

		public const int SW_SHOWNORMAL = 1;

		public const int SW_NORMAL = 1;

		public const int SW_SHOWMINIMIZED = 2;

		public const int SW_SHOWMAXIMIZED = 3;

		public const int SW_MAXIMIZE = 3;

		public const int SW_SHOWNOACTIVATE = 4;

		public const int SW_SHOW = 5;

		public const int SW_MINIMIZE = 6;

		public const int SW_SHOWMINNOACTIVE = 7;

		public const int SW_SHOWNA = 8;

		public const int SW_RESTORE = 9;

		public const int SW_SHOWDEFAULT = 10;

		public const int SW_FORCEMINIMIZE = 11;

		public const int HWND_TOP = 0;

		public const int HWND_BOTTOM = 1;

		public const int HWND_TOPMOST = -1;

		public const int HWND_NOTOPMOST = -2;

		public const int MMSYSERR_NOERROR = 0;

		public const int MM_WOM_OPEN = 955;

		public const int MM_WOM_CLOSE = 956;

		public const int MM_WOM_DONE = 957;

		public const int CALLBACK_FUNCTION = 196608;

		public const int TIME_MS = 1;

		public const int TIME_SAMPLES = 2;

		public const int TIME_BYTES = 4;

		public static readonly IntPtr INVALID_HANDLE_VALUE;

		public static readonly HandleRef NullHandleRef;

		public static bool MessagePending
		{
			get
			{
				MSG msg;
				return PeekMessage(out msg, IntPtr.Zero, 0u, 0u, 0u);
			}
		}

		static NativeMethods()
		{
			INVALID_HANDLE_VALUE = new IntPtr(-1);
			NullHandleRef = new HandleRef(null, IntPtr.Zero);
		}

		public static int HIWORD(IntPtr param)
		{
			return (int)(((long)param >> 16) & 0xFFFF);
		}

		public static int LOWORD(IntPtr param)
		{
			return (int)((long)param & 0xFFFF);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string modName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetClassInfo(HandleRef hInst, string lpszClass, [In][Out] WNDCLASS_I wc);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern short RegisterClass(WNDCLASS wc);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern short UnregisterClass(string lpClassName, HandleRef hInstance);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern IntPtr GetProcAddress(HandleRef hModule, string lpProcName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateWindowEx(int exStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, HandleRef hWndParent, HandleRef hMenu, HandleRef hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyWindow(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr FindWindow(IntPtr className, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowScrollBar(IntPtr hwnd, int wBar, bool bShow);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsIconic(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PeekMessage(out MSG msg, IntPtr wnd, uint filterMin, uint filterMax, uint removeMsg);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr wnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		public static void ShowNoActivate(Form form, bool show)
		{
			ShowNoActivate(form, show, false);
		}

		public static void ShowNoActivate(Form form, bool show, bool topmost)
		{
			int value = (topmost ? (-1) : 0);
			int num = ((!show) ? 128 : 0);
			SetWindowPos(form.Handle, new IntPtr(value), 0, 0, 0, 0, 0x13 | num);
			form.Visible = show;
		}

		public static bool ExtendFrameIntoClientArea(Control control, int leftEdge, int rightEdge, int topEdge, int bottomEdge)
		{
			MARGINS pMarInset = new MARGINS
			{
				leftWidth = leftEdge,
				rightWidth = rightEdge,
				topHeight = topEdge,
				bottomHeight = bottomEdge
			};
			return DwmExtendFrameIntoClientArea(control.Handle, ref pMarInset) == 0;
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int index);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern int RegisterWindowMessage(string msg);

		[DllImport("User32.dll")]
		public static extern short GetAsyncKeyState(int keyCode);

		[DllImport("User32.dll")]
		public static extern IntPtr GetFocus();

		public static void Send_WM_COPYDATA(HandleRef hWnd, string sData)
		{
			COPYDATASTRUCT cOPYDATASTRUCT = default(COPYDATASTRUCT);
			cOPYDATASTRUCT.dwData = 0u;
			cOPYDATASTRUCT.cbData = sData.Length;
			cOPYDATASTRUCT.lpData = Marshal.StringToHGlobalAnsi(sData);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(COPYDATASTRUCT)));
			Marshal.StructureToPtr(cOPYDATASTRUCT, intPtr, true);
			SendMessage(hWnd, 74, IntPtr.Zero, intPtr);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(cOPYDATASTRUCT.lpData);
		}

		public static string Recieve_WM_COPYDATA(ref Message message)
		{
			if (message.Msg == 74)
			{
				COPYDATASTRUCT cOPYDATASTRUCT = (COPYDATASTRUCT)Marshal.PtrToStructure(message.LParam, typeof(COPYDATASTRUCT));
				return Marshal.PtrToStringAnsi(cOPYDATASTRUCT.lpData, cOPYDATASTRUCT.cbData);
			}
			return string.Empty;
		}

		[DllImport("winmm.dll")]
		public static extern int waveOutGetNumDevs();

		[DllImport("winmm.dll")]
		public static extern int waveOutGetDevCaps(IntPtr uDeviceID, out WaveCaps pwoc, uint cbwoc);

		[DllImport("winmm.dll")]
		public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);

		[DllImport("winmm.dll")]
		public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);

		[DllImport("winmm.dll")]
		public static extern int waveOutReset(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveOutClose(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveOutPause(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveOutRestart(IntPtr hWaveOut);

		[DllImport("winmm.dll")]
		public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetPitch(IntPtr hWaveOut, int dwPitch);

		[DllImport("winmm.dll")]
		public static extern int waveOutGetPitch(IntPtr hWaveOut, out int dwPitch);

		[DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint waveOutGetErrorText(int mmrError, StringBuilder pszText, uint cchText);

		public static string waveOutGetErrorText(int mmrError)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (waveOutGetErrorText(mmrError, stringBuilder, (uint)stringBuilder.Capacity) == 0)
			{
				return stringBuilder.ToString();
			}
			return "Unknown Error " + mmrError;
		}
	}
}
