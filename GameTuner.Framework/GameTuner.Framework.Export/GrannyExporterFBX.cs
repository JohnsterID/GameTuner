using System;
using System.Runtime.InteropServices;

namespace GameTuner.Framework.Export
{
	public class GrannyExporterFBX
	{
		public static ProgressUpdateDelegate ProgressUpdate { get; set; }

		public static bool ExportFBXFile(string szInputFilename, string szOutputFilename, string szAnimDefFile)
		{
			IntPtr intPtr = new IntPtr(0);
			if (ProgressUpdate != null)
			{
				intPtr = Marshal.GetFunctionPointerForDelegate(ProgressUpdate);
			}
			GCHandle gCHandle = GCHandle.Alloc(intPtr);
			bool result = ((!ReflectionHelper.Is64Bit) ? ExportFBXFileWin32(szInputFilename, szOutputFilename, szAnimDefFile, intPtr) : ExportFBXFilex64(szInputFilename, szOutputFilename, szAnimDefFile, intPtr));
			gCHandle.Free();
			return result;
		}

		[DllImport("GrannyExporterFBXWin32.dll", EntryPoint = "ExportFBXFile")]
		private static extern bool ExportFBXFileWin32(string szInputFilename, string szOutputFilename, string szAnimDefFile, IntPtr ProgressFunction);

		[DllImport("GrannyExporterFBXx64.dll", EntryPoint = "ExportFBXFile")]
		private static extern bool ExportFBXFilex64(string szInputFilename, string szOutputFilename, string szAnimDefFile, IntPtr ProgressFunction);
	}
}
