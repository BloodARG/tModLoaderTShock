using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Terraria.Utilities
{
	public class CrashDump
	{
		internal enum MINIDUMP_TYPE
		{
			MiniDumpNormal,
			MiniDumpWithDataSegs,
			MiniDumpWithFullMemory,
			MiniDumpWithHandleData = 4,
			MiniDumpFilterMemory = 8,
			MiniDumpScanMemory = 16,
			MiniDumpWithUnloadedModules = 32,
			MiniDumpWithIndirectlyReferencedMemory = 64,
			MiniDumpFilterModulePaths = 128,
			MiniDumpWithProcessThreadData = 256,
			MiniDumpWithPrivateReadWriteMemory = 512,
			MiniDumpWithoutOptionalData = 1024,
			MiniDumpWithFullMemoryInfo = 2048,
			MiniDumpWithThreadInfo = 4096,
			MiniDumpWithCodeSegs = 8192
		}

		[DllImport("dbghelp.dll")]
		private static extern bool MiniDumpWriteDump(IntPtr hProcess, int ProcessId, IntPtr hFile, CrashDump.MINIDUMP_TYPE DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallackParam);

		public static void Create()
		{
			DateTime dateTime = DateTime.Now.ToLocalTime();
			string path = string.Concat(new string[]
				{
#if CLIENT
				"Terraria ",
#else
					"TerrariaServer",
#endif
					Main.versionNumber,
					" ",
					dateTime.Year.ToString("D4"),
					"-",
					dateTime.Month.ToString("D2"),
					"-",
					dateTime.Day.ToString("D2"),
					" ",
					dateTime.Hour.ToString("D2"),
					"_",
					dateTime.Minute.ToString("D2"),
					"_",
					dateTime.Second.ToString("D2"),
					".dmp"
				});
			CrashDump.Create(path);
		}

		public static void CreateFull()
		{
			DateTime dateTime = DateTime.Now.ToLocalTime();
			string path = string.Concat(new string[]
				{
#if CLIENT
				"DMP-FULL Terraria ",
#else
					"DMP-FULL TerrariaServer",
#endif
					Main.versionNumber,
					" ",
					dateTime.Year.ToString("D4"),
					"-",
					dateTime.Month.ToString("D2"),
					"-",
					dateTime.Day.ToString("D2"),
					" ",
					dateTime.Hour.ToString("D2"),
					"_",
					dateTime.Minute.ToString("D2"),
					"_",
					dateTime.Second.ToString("D2"),
					".dmp"
				});
			using (FileStream fileStream = File.Create(path))
			{
				Process currentProcess = Process.GetCurrentProcess();
				CrashDump.MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle.DangerousGetHandle(), CrashDump.MINIDUMP_TYPE.MiniDumpWithFullMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public static void Create(string path)
		{
			bool flag = Program.LaunchParameters.ContainsKey("-fulldump");
			using (FileStream fileStream = File.Create(path))
			{
				Process currentProcess = Process.GetCurrentProcess();
				CrashDump.MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle.DangerousGetHandle(), flag ? CrashDump.MINIDUMP_TYPE.MiniDumpWithFullMemory : CrashDump.MINIDUMP_TYPE.MiniDumpWithIndirectlyReferencedMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}
