using Steamworks;
using System;
using System.Threading;
using System.Windows.Forms;
using Terraria.Localization;

namespace Terraria.Social.Steam
{
	public class CoreSocialModule : ISocialModule
	{
		public const int SteamAppId = 105600;
		private static CoreSocialModule _instance;
		private bool IsSteamValid;
		private object _steamTickLock = new object();
		private object _steamCallbackLock = new object();
		private Callback<GameOverlayActivated_t> _onOverlayActivated;

		public static event Action OnTick;

		public void Initialize()
		{
			CoreSocialModule._instance = this;
#if CLIENT
			if (SteamAPI.RestartAppIfNecessary(new AppId_t(105600u)))
			{
				Environment.Exit(1);
				return;
			}
#endif
			if (!SteamAPI.Init())
			{
				MessageBox.Show(Language.GetTextValue("Error.LaunchFromSteam"), Language.GetTextValue("Error.Error"));
				Environment.Exit(1);
			}
			this.IsSteamValid = true;
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SteamCallbackLoop), null);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.SteamTickLoop), null);
			Main.OnTick += new Action(this.PulseSteamTick);
			Main.OnTick += new Action(this.PulseSteamCallback);
#if MAC
			this._onOverlayActivated = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnOverlayActivated));
#endif
		}

		public void PulseSteamTick()
		{
			if (Monitor.TryEnter(this._steamTickLock))
			{
				Monitor.Pulse(this._steamTickLock);
				Monitor.Exit(this._steamTickLock);
			}
		}

		public void PulseSteamCallback()
		{
			if (Monitor.TryEnter(this._steamCallbackLock))
			{
				Monitor.Pulse(this._steamCallbackLock);
				Monitor.Exit(this._steamCallbackLock);
			}
		}

		public static void Pulse()
		{
			CoreSocialModule._instance.PulseSteamTick();
			CoreSocialModule._instance.PulseSteamCallback();
		}

		private void SteamTickLoop(object context)
		{
			Monitor.Enter(this._steamTickLock);
			while (this.IsSteamValid)
			{
				if (CoreSocialModule.OnTick != null)
				{
					CoreSocialModule.OnTick();
				}
				Monitor.Wait(this._steamTickLock);
			}
			Monitor.Exit(this._steamTickLock);
		}

		private void SteamCallbackLoop(object context)
		{
			Monitor.Enter(this._steamCallbackLock);
			while (this.IsSteamValid)
			{
				SteamAPI.RunCallbacks();
				Monitor.Wait(this._steamCallbackLock);
			}
			Monitor.Exit(this._steamCallbackLock);
			SteamAPI.Shutdown();
		}

		public void Shutdown()
		{
#if WINDOWS
			Application.ApplicationExit += delegate(object obj, EventArgs evt)
#else
			this.IsSteamValid = false;
			AppDomain.CurrentDomain.ProcessExit += delegate(object obj, EventArgs evt)
#endif
            {
				this.IsSteamValid = false;
			};
		}

		public void OnOverlayActivated(GameOverlayActivated_t result)
		{
			Main.instance.IsMouseVisible = (result.m_bActive == 1);
		}
	}
}
