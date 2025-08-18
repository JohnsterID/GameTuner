using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameTuner.Framework
{
	public class PackFile : IDisposable
	{
		public struct PackHeader
		{
			public int Version;

			public int FourCC;

			public bool FullPaths;

			public bool BigEndian;

			public int Count;

			public static readonly PackHeader Empty;

			static PackHeader()
			{
				Empty = default(PackHeader);
			}
		}

		public class PackFileInfo
		{
			public string Name { get; private set; }

			public uint Size { get; private set; }

			public uint Offset { get; private set; }

			public DateTime Date { get; private set; }

			public PackFile Owner { get; private set; }

			public PackFileInfo(PackFile owner, string name, uint size, uint offset, DateTime date)
			{
				Owner = owner;
				Name = name;
				Size = size;
				Offset = offset;
				Date = date;
			}

			public void Copy(PackFileInfo src)
			{
				Name = src.Name;
				Size = src.Size;
				Offset = src.Offset;
				Date = src.Date;
				Owner = src.Owner;
			}
		}

		public class PackFileInfoCollection : List<PackFileInfo>
		{
		}

		private class PackWriteInfo
		{
			public PackFileInfo info;

			public FileInfo fi;

			public byte[] name;

			public PackWriteInfo(PackFileInfo info, FileInfo fi, int pad)
			{
				this.info = info;
				this.fi = fi;
				name = new byte[pad];
				int length = fi.Name.Length;
				for (int i = 0; i < length; i++)
				{
					name[i] = (byte)fi.Name[i];
				}
			}
		}

		private class PackFileStream : Stream
		{
			private PackFileInfo file;

			private Stream stream;

			private long pos;

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override long Length
			{
				get
				{
					return file.Size;
				}
			}

			public override long Position
			{
				get
				{
					return pos;
				}
				set
				{
					if (value < 0 || value >= file.Size)
					{
						throw new IOException("Stream position out of range");
					}
					pos = value;
				}
			}

			public PackFileStream(Stream stream, PackFileInfo file)
			{
				pos = 0L;
				this.stream = stream;
				this.file = file;
			}

			public override void Flush()
			{
				stream.Flush();
			}

			protected override void Dispose(bool disposing)
			{
				stream.Close();
				base.Dispose(disposing);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (pos >= file.Size)
				{
					return 0;
				}
				if (stream.Position != pos + file.Offset)
				{
					stream.Position = pos + file.Offset;
				}
				int num = stream.Read(buffer, offset, Math.Min(count, (int)(file.Size - pos)));
				pos += num;
				return num;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				switch (origin)
				{
				case SeekOrigin.Begin:
					if (offset < 0 || offset >= file.Size)
					{
						throw new IOException("Seek position out of range");
					}
					pos = offset;
					break;
				case SeekOrigin.Current:
					if (pos + offset < 0 || pos + offset >= file.Size)
					{
						throw new IOException("Seek position out of range");
					}
					pos += offset;
					break;
				case SeekOrigin.End:
					if (pos - offset < 0 || pos - offset >= file.Size)
					{
						throw new IOException("Seek position out of range");
					}
					pos -= offset;
					break;
				}
				return pos;
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}
		}

		public const string PakMagic6 = "FPK_";

		public const string PakMagic7 = "FPK7";

		private DateTime lastDate;

		private PackHeader header;

		public bool IsValid
		{
			get
			{
				return Isvalid(header);
			}
		}

		public PackHeader Header
		{
			get
			{
				return header;
			}
		}

		public PackFileInfoCollection Files { get; private set; }

		public string Filename { get; private set; }

		public PackFile()
			: this(null)
		{
		}

		public PackFile(string fileName)
		{
			header = PackHeader.Empty;
			Files = new PackFileInfoCollection();
			if (!string.IsNullOrEmpty(fileName))
			{
				Open(fileName);
			}
		}

		public void Dispose()
		{
		}

		public void Open(string fileName)
		{
			Filename = fileName;
			ReadFileHeaders();
		}

		private void ReadFileHeaders()
		{
			Files.Clear();
			lastDate = new FileInfo(Filename).LastWriteTime;
			using (Stream input = new FileStream(Filename, FileMode.Open, FileAccess.Read))
			{
				BinaryReader reader = new BinaryReader(input);
				if (IsPackFile(reader, out header))
				{
					for (int i = 0; i < header.Count; i++)
					{
						ReadFileHeader(reader);
					}
				}
			}
		}

		public void Close()
		{
		}

		public Stream GetFileStream(string fileName)
		{
			string file = Path.GetFileName(fileName);
			PackFileInfo file2 = Files.Find((PackFileInfo a) => string.Compare(a.Name, file, true) == 0);
			return GetFileStream(file2);
		}

		public Stream GetFileStream(PackFileInfo file)
		{
			DateTime lastWriteTime = new FileInfo(Filename).LastWriteTime;
			if (lastWriteTime != lastDate)
			{
				ReadFileHeaders();
				PackFileInfo packFileInfo = Files.Find((PackFileInfo a) => string.Compare(a.Name, file.Name, true) == 0);
				if (packFileInfo == null)
				{
					return null;
				}
				file.Copy(packFileInfo);
				file = packFileInfo;
			}
			if (file != null)
			{
				Stream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
				return new PackFileStream(stream, file);
			}
			return null;
		}

		private static bool Isvalid(PackHeader hdr)
		{
			if (!MathEx.IsFourCC(hdr.FourCC, "FPK_"))
			{
				return MathEx.IsFourCC(hdr.FourCC, "FPK7");
			}
			return true;
		}

		private void ReadFileHeader(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(num);
			string name = Encoding.ASCII.GetString(bytes);
			int num2 = ((num + 3) & -4) - num;
			if (num2 != 0)
			{
				reader.BaseStream.Seek(num2, SeekOrigin.Current);
			}
			long num3 = reader.ReadInt64();
			uint size = reader.ReadUInt32();
			uint offset = reader.ReadUInt32();
			DateTime date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(num3);
			Files.Add(new PackFileInfo(this, name, size, offset, date));
		}

		private static int HeaderSize(int version)
		{
			int num = 0;
			switch (version)
			{
			case 6:
				num += 4;
				num += 4;
				num++;
				num++;
				num += 4;
				break;
			case 7:
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				break;
			}
			return num;
		}

		private static bool ReadHeader(BinaryReader reader, out PackHeader hdr)
		{
			bool result = false;
			hdr = PackHeader.Empty;
			int num = reader.ReadInt32();
			int num2 = reader.ReadInt32();
			switch (num)
			{
			case 6:
				if (MathEx.IsFourCC(num2, "FPK_"))
				{
					hdr.Version = num;
					hdr.FourCC = num2;
					hdr.FullPaths = reader.ReadBoolean();
					hdr.BigEndian = reader.ReadBoolean();
					hdr.Count = reader.ReadInt32();
					result = true;
				}
				break;
			case 7:
				if (MathEx.IsFourCC(num2, "FPK7"))
				{
					hdr.Version = num;
					hdr.FourCC = num2;
					hdr.FullPaths = false;
					hdr.BigEndian = false;
					reader.ReadUInt32();
					hdr.Count = reader.ReadInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					result = true;
				}
				break;
			}
			return result;
		}

		public static bool IsPackFile(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return false;
			}
			bool flag = false;
			using (Stream input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = new BinaryReader(input))
				{
					PackHeader hdr;
					return IsPackFile(reader, out hdr);
				}
			}
		}

		public static bool IsPackFile(BinaryReader reader, out PackHeader hdr)
		{
			hdr = PackHeader.Empty;
			bool result = false;
			try
			{
				result = ReadHeader(reader, out hdr);
			}
			catch (Exception exception)
			{
				ErrorHandling.Error(exception, ErrorLevel.Log);
				hdr = PackHeader.Empty;
			}
			return result;
		}

		public static void CreatePackFile(string fileName, IEnumerable<string> files)
		{
			List<PackWriteInfo> list = new List<PackWriteInfo>();
			uint num = 0u;
			uint num2 = (uint)HeaderSize(6);
			foreach (string file in files)
			{
				FileInfo fileInfo = new FileInfo(file);
				if (!fileInfo.Exists)
				{
					throw new FileNotFoundException(string.Format("Unable to find file {0}", Path.GetFileName(fileName)));
				}
				string name = fileInfo.Name;
				int num3 = (name.Length + 3) & -4;
				PackFileInfo info = new PackFileInfo(null, name, (uint)fileInfo.Length, num, fileInfo.LastWriteTime);
				list.Add(new PackWriteInfo(info, fileInfo, num3));
				int num4 = 0;
				num4 += 4;
				num4 += num3;
				num4 += 8;
				num4 += 4;
				num4 += 4;
				num2 += (uint)num4;
				num += (uint)(int)fileInfo.Length;
			}
			Stream stream = null;
			BinaryWriter binaryWriter = null;
			try
			{
				PackHeader empty = PackHeader.Empty;
				empty.Version = 6;
				empty.FourCC = MathEx.ToFourCC("FPK_");
				empty.FullPaths = false;
				empty.BigEndian = false;
				empty.Count = list.Count;
				stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				binaryWriter = new BinaryWriter(stream);
				WriteHeader(binaryWriter, empty);
				WriteFileInfo(binaryWriter, num2, list);
				WriteFiles(binaryWriter, list);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
			}
		}

		private static void WriteHeader(BinaryWriter writer, PackHeader header)
		{
			writer.Write(header.Version);
			writer.Write(header.FourCC);
			writer.Write(header.FullPaths);
			writer.Write(header.BigEndian);
			writer.Write(header.Count);
		}

		private static void WriteFileInfo(BinaryWriter writer, uint baseOffset, List<PackWriteInfo> packFiles)
		{
			foreach (PackWriteInfo packFile in packFiles)
			{
				long value = (long)(packFile.info.Date - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
				writer.Write(packFile.name.Length);
				writer.Write(packFile.name);
				writer.Write(value);
				writer.Write(packFile.info.Size);
				writer.Write(packFile.info.Offset + baseOffset);
			}
		}

		private static void WriteFiles(BinaryWriter writer, List<PackWriteInfo> packFiles)
		{
			byte[] buffer = new byte[1024];
			foreach (PackWriteInfo packFile in packFiles)
			{
				using (Stream stream = new FileStream(packFile.fi.FullName, FileMode.Open, FileAccess.Read))
				{
					int num = (int)packFile.info.Size;
					for (int num2 = stream.Read(buffer, 0, Math.Min(1024, num)); num2 > 0; num2 = ((num > 0) ? stream.Read(buffer, 0, Math.Min(1024, num)) : 0))
					{
						writer.Write(buffer, 0, num2);
						num -= num2;
					}
				}
			}
		}

		public static void ExtractFile(PackFileInfo kFile, string szDiskLocation)
		{
			using (Stream stream = kFile.Owner.GetFileStream(kFile))
			{
				using (FileStream fileStream = new FileStream(szDiskLocation, FileMode.Create))
				{
					for (int i = 0; i < stream.Length; i++)
					{
						fileStream.WriteByte((byte)stream.ReadByte());
					}
				}
			}
		}

		public void ExtractFiles(string szDirectory)
		{
			foreach (PackFileInfo file in Files)
			{
				ExtractFile(file, Path.Combine(szDirectory, file.Name));
			}
		}
	}
}
