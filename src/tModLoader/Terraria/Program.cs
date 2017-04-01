using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#if CLIENT
using System.Windows.Forms;
#endif
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.Social;
using Terraria.ModLoader;
using TerrariaApi.Server;

namespace Terraria
{
    public static class Program
    {
#if CLIENT
		public const bool IsServer = false;

#else
        public const bool IsServer = true;

#endif
        //public static Dictionary<string, string> LaunchParameters;
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
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"DEBUG 8923 Unhandled exception\n{e}");
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
#if WINDOWS
                MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
#else
				MethodInfo[] methods = type.GetMethods();
#endif
                MethodInfo[] array2 = methods;
                for (int j = 0; j < array2.Length; j++)
                {
                    MethodInfo methodInfo = array2[j];
                    if (!methodInfo.IsAbstract && !methodInfo.ContainsGenericParameters && methodInfo.GetMethodBody() != null)
                    {
#if WINDOWS
                        RuntimeHelpers.PrepareMethod(methodInfo.MethodHandle);
#else
						Program.JitForcedMethodCache = methodInfo.MethodHandle.GetFunctionPointer();
#endif
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
            //ServerApi.Hooks.InvokeGameInitializ/*e*/();
            Console.WriteLine("Test923");
            Program.LaunchParameters = Utils.ParseArguements(args);
            if (Program.LaunchParameters.ContainsKey("-savedirectory"))
            {
                Program.LaunchParameters["-savedirectory"] = Path.Combine(Program.LaunchParameters["-savedirectory"], "ModLoader");
            }
            if (LaunchParameters.ContainsKey("-build"))
            {
                ModCompile.BuildModCommandLine(LaunchParameters["-build"]);
                return;
            }
            ThreadPool.SetMinThreads(8, 8);
            LanguageManager.Instance.SetLanguage("English");
        }
//            using (Main main = new Main())
//            {
//                try
//                {
//                    SocialAPI.Initialize(null);
//                    LaunchInitializer.LoadParameters(main);
//                    Main.OnEnginePreload += delegate
//                    {
//                        Program.StartForceLoad();
//                    };
//#if CLIENT
//					main.Run();
//#else
//                    main.DedServ();
//#endif
//                }
                //catch (Exception e)
                //{
                //    Program.DisplayException(e);
                //}
            }
        }

//        private static void DisplayException(Exception e)
//        {
//            try
//            {
//                using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
//                {
//                    streamWriter.WriteLine(DateTime.Now);
//                    streamWriter.WriteLine(e);
//                    streamWriter.WriteLine("");
//                }
//#if CLIENT
//				MessageBox.Show(e.ToString(), "Terraria: Error");
//#else
//                Console.WriteLine("Server crash: " + DateTime.Now);
//                Console.WriteLine(e);
//                Console.WriteLine("");
//                Console.WriteLine("Please send crashlog.txt to support@terraria.org");
//#endif
//            }
//            catch
//            {
//            }
//        }
    