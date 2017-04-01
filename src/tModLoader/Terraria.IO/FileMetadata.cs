using System;
using System.IO;

namespace Terraria.IO
{
	public class FileMetadata
	{
		public const ulong MAGIC_NUMBER = 27981915666277746uL;
		public const int SIZE = 20;
		public FileType Type;
		public uint Revision;
		public bool IsFavorite;

		private FileMetadata()
		{
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(27981915666277746uL | (ulong)this.Type << 56);
			writer.Write(this.Revision);
			writer.Write((ulong)((long)(this.IsFavorite.ToInt() & 1)));
		}

		public void IncrementAndWrite(BinaryWriter writer)
		{
			this.Revision += 1u;
			this.Write(writer);
		}

		public static FileMetadata FromCurrentSettings(FileType type)
		{
			return new FileMetadata
			{
				Type = type,
				Revision = 0u,
				IsFavorite = false
			};
		}

		public static FileMetadata Read(BinaryReader reader, FileType expectedType)
		{
			FileMetadata fileMetadata = new FileMetadata();
			fileMetadata.Read(reader);
			if (fileMetadata.Type != expectedType)
			{
				throw new FileFormatException(string.Concat(new string[]
						{
							"Expected type \"",
							Enum.GetName(typeof(FileType), expectedType),
							"\" but found \"",
							Enum.GetName(typeof(FileType), fileMetadata.Type),
							"\"."
						}));
			}
			return fileMetadata;
		}

		private void Read(BinaryReader reader)
		{
			ulong num = reader.ReadUInt64();
			if ((num & 72057594037927935uL) != 27981915666277746uL)
			{
				throw new FileFormatException("Expected Re-Logic file format.");
			}
			byte b = (byte)(num >> 56 & 255uL);
			FileType fileType = FileType.None;
			FileType[] array = (FileType[])Enum.GetValues(typeof(FileType));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == (FileType)b)
				{
					fileType = array[i];
					break;
				}
			}
			if (fileType == FileType.None)
			{
				throw new FileFormatException("Found invalid file type.");
			}
			this.Type = fileType;
			this.Revision = reader.ReadUInt32();
			ulong num2 = reader.ReadUInt64();
			this.IsFavorite = ((num2 & 1uL) == 1uL);
		}
	}
}
