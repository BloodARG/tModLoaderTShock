using System;
using System.Diagnostics;
using Terraria.Social;

namespace Terraria.Initializers
{
	public static class LaunchInitializer
	{
		public static void LoadParameters(Main game)
		{
			LaunchInitializer.LoadSharedParameters(game);
#if CLIENT
			LaunchInitializer.LoadClientParameters(game);
#else
			LaunchInitializer.LoadServerParameters(game);
#endif
		}

		private static void LoadSharedParameters(Main game)
		{
			string path;
			if ((path = LaunchInitializer.TryParameter(new string[]
				{
					"-loadlib"
				})) != null)
			{
				game.loadLib(path);
			}
			string s;
			int listenPort;
			if ((s = LaunchInitializer.TryParameter(new string[]
				{
					"-p",
					"-port"
				})) != null && int.TryParse(s, out listenPort))
			{
				Netplay.ListenPort = listenPort;
			}
		}

		private static void LoadClientParameters(Main game)
		{
			string iP;
			if ((iP = LaunchInitializer.TryParameter(new string[]
				{
					"-j",
					"-join"
				})) != null)
			{
				game.AutoJoin(iP);
			}
			string serverPassword;
			if ((serverPassword = LaunchInitializer.TryParameter(new string[]
				{
					"-pass",
					"-password"
				})) != null)
			{
				Netplay.ServerPassword = serverPassword;
				game.AutoPass();
			}
			if (LaunchInitializer.HasParameter(new string[]
				{
					"-host"
				}))
			{
				game.AutoHost();
			}
		}

		private static void LoadServerParameters(Main game)
		{
			try
			{
				string s;
				if ((s = LaunchInitializer.TryParameter(new string[]
					{
						"-forcepriority"
					})) != null)
				{
					Process currentProcess = Process.GetCurrentProcess();
					int num;
					if (int.TryParse(s, out num))
					{
						switch (num)
						{
							case 0:
								currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
								break;
							case 1:
								currentProcess.PriorityClass = ProcessPriorityClass.High;
								break;
							case 2:
								currentProcess.PriorityClass = ProcessPriorityClass.AboveNormal;
								break;
							case 3:
								currentProcess.PriorityClass = ProcessPriorityClass.Normal;
								break;
							case 4:
								currentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
								break;
							case 5:
								currentProcess.PriorityClass = ProcessPriorityClass.Idle;
								break;
							default:
								currentProcess.PriorityClass = ProcessPriorityClass.High;
								break;
						}
					}
					else
					{
						currentProcess.PriorityClass = ProcessPriorityClass.High;
					}
				}
				else
				{
					Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
				}
			}
			catch
			{
			}
			string s2;
			int netPlayers;
			if ((s2 = LaunchInitializer.TryParameter(new string[]
				{
					"-maxplayers",
					"-players"
				})) != null && int.TryParse(s2, out netPlayers))
			{
				game.SetNetPlayers(netPlayers);
			}
			string serverPassword;
			if ((serverPassword = LaunchInitializer.TryParameter(new string[]
				{
					"-pass",
					"-password"
				})) != null)
			{
				Netplay.ServerPassword = serverPassword;
			}
			string s3;
			int lang;
			if ((s3 = LaunchInitializer.TryParameter(new string[]
				{
					"-lang"
				})) != null && int.TryParse(s3, out lang))
			{
				Lang.lang = lang;
			}
			string worldName;
			if ((worldName = LaunchInitializer.TryParameter(new string[]
				{
					"-worldname"
				})) != null)
			{
				game.SetWorldName(worldName);
			}
			string newMOTD;
			if ((newMOTD = LaunchInitializer.TryParameter(new string[]
				{
					"-motd"
				})) != null)
			{
				game.NewMOTD(newMOTD);
			}
			string modPath;
			if ((modPath = LaunchInitializer.TryParameter(new string[]
				{
					"-modpath"
				})) != null)
			{
				ModLoader.ModLoader.modPath = modPath;
			}
			string banFilePath;
			if ((banFilePath = LaunchInitializer.TryParameter(new string[]
				{
					"-banlist"
				})) != null)
			{
				Netplay.BanFilePath = banFilePath;
			}
			if (LaunchInitializer.HasParameter(new string[]
				{
					"-autoshutdown"
				}))
			{
				game.EnableAutoShutdown();
			}
			if (LaunchInitializer.HasParameter(new string[]
				{
					"-secure"
				}))
			{
				Netplay.spamCheck = true;
			}
			string worldSize;
			if ((worldSize = LaunchInitializer.TryParameter(new string[]
				{
					"-autocreate"
				})) != null)
			{
				game.autoCreate(worldSize);
			}
			if (LaunchInitializer.HasParameter(new string[]
				{
					"-noupnp"
				}))
			{
				Netplay.UseUPNP = false;
			}
			if (LaunchInitializer.HasParameter(new string[]
				{
					"-experimental"
				}))
			{
				Main.UseExperimentalFeatures = true;
			}
			string world;
			if ((world = LaunchInitializer.TryParameter(new string[]
				{
					"-world"
				})) != null)
			{
				game.SetWorld(world, false);
			}
			else if (SocialAPI.Mode == SocialMode.Steam && (world = LaunchInitializer.TryParameter(new string[]
				{
					"-cloudworld"
				})) != null)
			{
				game.SetWorld(world, true);
			}
			string configPath;
			if ((configPath = LaunchInitializer.TryParameter(new string[]
				{
					"-config"
				})) != null)
			{
				game.LoadDedConfig(configPath);
			}
		}

		private static bool HasParameter(params string[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (Program.LaunchParameters.ContainsKey(keys[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static string TryParameter(params string[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				string text;
				if (Program.LaunchParameters.TryGetValue(keys[i], out text))
				{
					if (text == null)
					{
						text = "";
					}
					return text;
				}
			}
			return null;
		}
	}
}
