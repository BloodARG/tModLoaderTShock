using System;
using System.Collections.Generic;
using Terraria.Social.Base;
using Terraria.Social.Steam;

namespace Terraria.Social
{
	public static class SocialAPI
	{
		private static SocialMode _mode = SocialMode.None;
		public static Terraria.Social.Base.FriendsSocialModule Friends;
		public static Terraria.Social.Base.AchievementsSocialModule Achievements;
		public static Terraria.Social.Base.CloudSocialModule Cloud;
		public static Terraria.Social.Base.NetSocialModule Network;
		public static Terraria.Social.Base.OverlaySocialModule Overlay;
		private static List<ISocialModule> _modules;

		public static SocialMode Mode
		{
			get
			{
				return SocialAPI._mode;
			}
		}

		public static void Initialize(SocialMode? mode = null)
		{
			if (!mode.HasValue)
			{
				mode = new SocialMode?(SocialMode.None);
#if CLIENT
#if DEBUG
                if (!Program.LaunchParameters.ContainsKey("-nosteam"))
#endif
				mode = new SocialMode?(SocialMode.Steam);
#else
				if (Program.LaunchParameters.ContainsKey("-steam"))
				{
					mode = new SocialMode?(SocialMode.Steam);
				}
#endif
			}
			SocialAPI._mode = mode.Value;
			SocialAPI._modules = new List<ISocialModule>();
			SocialMode mode2 = SocialAPI.Mode;
			if (mode2 == SocialMode.Steam)
			{
				SocialAPI.LoadSteam();
			}
			foreach (ISocialModule current in SocialAPI._modules)
			{
				current.Initialize();
			}
		}

		public static void Shutdown()
		{
			SocialAPI._modules.Reverse();
			foreach (ISocialModule current in SocialAPI._modules)
			{
				current.Shutdown();
			}
		}

		private static T LoadModule<T>() where T : ISocialModule, new()
		{
#if MAC
			T t = Activator.CreateInstance<T>();
#else
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
#endif
			SocialAPI._modules.Add(t);
			return t;
		}

		private static T LoadModule<T>(T module) where T : ISocialModule
		{
			SocialAPI._modules.Add(module);
			return module;
		}

		private static void LoadSteam()
		{
			SocialAPI.LoadModule<CoreSocialModule>();
			SocialAPI.Friends = SocialAPI.LoadModule<Terraria.Social.Steam.FriendsSocialModule>();
			SocialAPI.Achievements = SocialAPI.LoadModule<Terraria.Social.Steam.AchievementsSocialModule>();
			SocialAPI.Cloud = SocialAPI.LoadModule<Terraria.Social.Steam.CloudSocialModule>();
			SocialAPI.Overlay = SocialAPI.LoadModule<Terraria.Social.Steam.OverlaySocialModule>();
#if CLIENT
			SocialAPI.Network = SocialAPI.LoadModule<NetClientSocialModule>();
#else
			SocialAPI.Network = SocialAPI.LoadModule<NetServerSocialModule>();
#endif
		}
	}
}
