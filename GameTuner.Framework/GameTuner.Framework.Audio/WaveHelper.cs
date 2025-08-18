using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GameTuner.Framework.Audio
{
	public static class WaveHelper
	{
		public static int DeviceCount
		{
			get
			{
				return NativeMethods.waveOutGetNumDevs();
			}
		}

		public static NativeMethods.WaveCaps[] GetDeviceCaps()
		{
			int deviceCount = DeviceCount;
			NativeMethods.WaveCaps[] array = new NativeMethods.WaveCaps[deviceCount];
			for (int i = 0; i < deviceCount; i++)
			{
				NativeMethods.waveOutGetDevCaps(new IntPtr(i), out array[i], (uint)Marshal.SizeOf(typeof(NativeMethods.WaveCaps)));
			}
			return array;
		}

		private static string ReadChunk(BinaryReader reader)
		{
			byte[] array = new byte[4];
			reader.Read(array, 0, array.Length);
			return Encoding.ASCII.GetString(array);
		}

		public static WaveHeaderResult ReadHeader(string fileName, out NativeMethods.WaveFormat format)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return ReadHeader(stream, out format);
			}
		}

		public static WaveHeaderResult ReadHeader(Stream stream, out NativeMethods.WaveFormat format)
		{
			format = default(NativeMethods.WaveFormat);
			BinaryReader binaryReader = new BinaryReader(stream);
			if (ReadChunk(binaryReader) != "RIFF")
			{
				return WaveHeaderResult.NotRiff;
			}
			binaryReader.ReadInt32();
			if (ReadChunk(binaryReader) != "WAVE")
			{
				return WaveHeaderResult.NotWave;
			}
			while (ReadChunk(binaryReader) != "fmt ")
			{
				long offset = binaryReader.ReadInt32();
				stream.Seek(offset, SeekOrigin.Current);
			}
			int num = binaryReader.ReadInt32();
			if (num < 16)
			{
				return WaveHeaderResult.BadChunk;
			}
			long position = stream.Position;
			format.wFormatTag = binaryReader.ReadInt16();
			format.nChannels = binaryReader.ReadInt16();
			format.nSamplesPerSec = binaryReader.ReadInt32();
			format.nAvgBytesPerSec = binaryReader.ReadInt32();
			format.nBlockAlign = binaryReader.ReadInt16();
			format.wBitsPerSample = binaryReader.ReadInt16();
			stream.Seek(position + num, SeekOrigin.Begin);
			while (ReadChunk(binaryReader) != "data")
			{
				long offset2 = binaryReader.ReadInt32();
				stream.Seek(offset2, SeekOrigin.Current);
			}
			binaryReader.ReadInt32();
			return WaveHeaderResult.Ok;
		}

		public static byte[] LoadWave(string fileName, out NativeMethods.WaveFormat format, out WaveHeaderResult result)
		{
			byte[] result2 = null;
			using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				result = ReadHeader(stream, out format);
				if (result == WaveHeaderResult.Ok)
				{
					using (BinaryReader binaryReader = new BinaryReader(stream))
					{
						long num = stream.Length - stream.Position;
						result2 = binaryReader.ReadBytes((int)num);
					}
				}
			}
			return result2;
		}
	}
}
