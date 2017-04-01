using System;
using System.Runtime.InteropServices;

namespace Terraria.Utilities
{
	public static class FileOperationAPIWrapper
	{
		[Flags]
		private enum FileOperationFlags : ushort
		{
			FOF_SILENT = 4,
			FOF_NOCONFIRMATION = 16,
			FOF_ALLOWUNDO = 64,
			FOF_SIMPLEPROGRESS = 256,
			FOF_NOERRORUI = 1024,
			FOF_WANTNUKEWARNING = 16384
		}

		private enum FileOperationType : uint
		{
			FO_MOVE = 1u,
			FO_COPY,
			FO_DELETE,
			FO_RENAME
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		private struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			[MarshalAs(UnmanagedType.U4)]
			public FileOperationAPIWrapper.FileOperationType wFunc;
			public string pFrom;
			public string pTo;
			public FileOperationAPIWrapper.FileOperationFlags fFlags;
			[MarshalAs(UnmanagedType.Bool)]
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern int SHFileOperation(ref FileOperationAPIWrapper.SHFILEOPSTRUCT FileOp);

		private static bool Send(string path, FileOperationAPIWrapper.FileOperationFlags flags)
		{
			bool result;
			try
			{
				FileOperationAPIWrapper.SHFILEOPSTRUCT sHFILEOPSTRUCT = new FileOperationAPIWrapper.SHFILEOPSTRUCT
				{
					wFunc = FileOperationAPIWrapper.FileOperationType.FO_DELETE,
					pFrom = path + '\0' + '\0',
					fFlags = FileOperationAPIWrapper.FileOperationFlags.FOF_ALLOWUNDO | flags
				};
				FileOperationAPIWrapper.SHFileOperation(ref sHFILEOPSTRUCT);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static bool Send(string path)
		{
			return FileOperationAPIWrapper.Send(path, FileOperationAPIWrapper.FileOperationFlags.FOF_NOCONFIRMATION | FileOperationAPIWrapper.FileOperationFlags.FOF_WANTNUKEWARNING);
		}

		public static bool MoveToRecycleBin(string path)
		{
			return FileOperationAPIWrapper.Send(path, FileOperationAPIWrapper.FileOperationFlags.FOF_SILENT | FileOperationAPIWrapper.FileOperationFlags.FOF_NOCONFIRMATION | FileOperationAPIWrapper.FileOperationFlags.FOF_NOERRORUI);
		}

		private static bool DeleteFile(string path, FileOperationAPIWrapper.FileOperationFlags flags)
		{
			bool result;
			try
			{
				FileOperationAPIWrapper.SHFILEOPSTRUCT sHFILEOPSTRUCT = new FileOperationAPIWrapper.SHFILEOPSTRUCT
				{
					wFunc = FileOperationAPIWrapper.FileOperationType.FO_DELETE,
					pFrom = path + '\0' + '\0',
					fFlags = flags
				};
				FileOperationAPIWrapper.SHFileOperation(ref sHFILEOPSTRUCT);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static bool DeleteCompletelySilent(string path)
		{
			return FileOperationAPIWrapper.DeleteFile(path, FileOperationAPIWrapper.FileOperationFlags.FOF_SILENT | FileOperationAPIWrapper.FileOperationFlags.FOF_NOCONFIRMATION | FileOperationAPIWrapper.FileOperationFlags.FOF_NOERRORUI);
		}
	}
}
