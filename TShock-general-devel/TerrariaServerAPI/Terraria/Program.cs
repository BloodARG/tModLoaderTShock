using System;
using System.Collections.Generic;
using Terraria;
using TerrariaApi;
using TerrariaApi.Server;

namespace Terraria
{
	public static class Program
	{
		public const bool IsServer = true;

		public static Dictionary<string, string> LaunchParameters;
  //      public static void Main(string[] args)
		//{
		//	AppDomain.CurrentDomain.UnhandledException += UnhandledException;
		//	try
		//	{
		//		InitialiseInternals();
		//		ServerApi.Hooks.AttachOTAPIHooks(args);

		//		// avoid any Terraria.Main calls here or the heaptile hook will not work.
		//		// this is because the hook is executed on the Terraria.Main static constructor,
		//		// and simply referencing it in this method will trigger the constructor.
		//		StartServer(args);

		//		ServerApi.DeInitialize();
		//	}
		//	catch (Exception ex)
		//	{
		//		ServerApi.LogWriter.ServerWriteLine("Server crashed due to an unhandled exception:\n" + ex, TraceLevel.Error);
		//	}
		//}

		//static void StartServer(string[] args)
		//{
		//	if (args.Any(x => x == "-skipassemblyload"))
		//	{
		//		Terraria.Main.SkipAssemblyLoad = true;
		//	}

		//	Terraria.WindowsLaunch.Main(args);
		//}

		/// <summary>
		/// TShock sets up its own unhandled exception handler; this one is just to catch possible
		/// startup exceptions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"DEBUG Unhandled exception\n{e}");
		}
	}
}