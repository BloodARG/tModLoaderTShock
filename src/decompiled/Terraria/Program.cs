using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria
{
	public static class Program
	{
		public const bool IsServer = false;

		public static Dictionary<string, string> LaunchParameters = new Dictionary<string, string>();

		private static int ThingsToLoad = 0;

		private static int ThingsLoaded = 0;

		public static bool LoadedEverything = false;

		public static IntPtr JitForcedMethodCache;

		public static float LoadedPercentage
		{
			get
			{
				if (Program.ThingsToLoad == 0)
				{
					return 1f;
				}
				return (float)Program.ThingsLoaded / (float)Program.ThingsToLoad;
			}
		}

		public static void StartForceLoad()
		{
			if (!Main.SkipAssemblyLoad)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(Program.ForceLoadThread));
				return;
			}
			Program.LoadedEverything = true;
		}

		public static void ForceLoadThread(object ThreadContext)
		{
			Program.ForceLoadAssembly(Assembly.GetExecutingAssembly(), true);
			Program.LoadedEverything = true;
		}

		private static void ForceJITOnAssembly(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo[] array2 = methods;
				for (int j = 0; j < array2.Length; j++)
				{
					MethodInfo methodInfo = array2[j];
					if (!methodInfo.IsAbstract && !methodInfo.ContainsGenericParameters && methodInfo.GetMethodBody() != null)
					{
						RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
					}
				}
				Program.ThingsLoaded++;
			}
		}

		private static void ForceStaticInitializers(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (!type.IsGenericType)
				{
					RuntimeHelpers.RunClassConstructor(type.TypeHandle);
				}
			}
		}

		private static void ForceLoadAssembly(Assembly assembly, bool initializeStaticMembers)
		{
			Program.ThingsToLoad = assembly.GetTypes().Length;
			Program.ForceJITOnAssembly(assembly);
			if (initializeStaticMembers)
			{
				Program.ForceStaticInitializers(assembly);
			}
		}

		private static void ForceLoadAssembly(string name, bool initializeStaticMembers)
		{
			Assembly assembly = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				if (assemblies[i].GetName().Name.Equals(name))
				{
					assembly = assemblies[i];
					break;
				}
			}
			if (assembly == null)
			{
				assembly = Assembly.Load(name);
			}
			Program.ForceLoadAssembly(assembly, initializeStaticMembers);
		}

		public static void LaunchGame(string[] args)
		{
			Program.LaunchParameters = Utils.ParseArguements(args);
			ThreadPool.SetMinThreads(8, 8);
			LanguageManager.Instance.SetLanguage("English");
			using (Main main = new Main())
			{
				try
				{
					SocialAPI.Initialize(null);
					LaunchInitializer.LoadParameters(main);
					Main.OnEnginePreload += delegate
					{
						Program.StartForceLoad();
					};
					main.Run();
				}
				catch (Exception e)
				{
					Program.DisplayException(e);
				}
			}
		}

		private static void DisplayException(Exception e)
		{
			try
			{
				using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
				{
					streamWriter.WriteLine(DateTime.Now);
					streamWriter.WriteLine(e);
					streamWriter.WriteLine("");
				}
				MessageBox.Show(e.ToString(), "Terraria: Error");
			}
			catch
			{
			}
		}
	}
}
