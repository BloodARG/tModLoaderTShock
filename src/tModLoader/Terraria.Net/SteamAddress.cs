using Steamworks;
using System;

namespace Terraria.Net
{
	public class SteamAddress : RemoteAddress
	{
		public readonly CSteamID SteamId;
		private string _friendlyName;

		public SteamAddress(CSteamID steamId)
		{
			this.Type = AddressType.Steam;
			this.SteamId = steamId;
		}

		public override string ToString()
		{
			string str = (this.SteamId.m_SteamID % 2uL).ToString();
			string str2 = ((this.SteamId.m_SteamID - (76561197960265728uL + this.SteamId.m_SteamID % 2uL)) / 2uL).ToString();
			return "STEAM_0:" + str + ":" + str2;
		}

		public override string GetIdentifier()
		{
			return this.ToString();
		}

		public override bool IsLocalHost()
		{
			if (Program.LaunchParameters.ContainsKey("-localsteamid"))
			{
				string arg_33_0 = Program.LaunchParameters["-localsteamid"];
				ulong steamID = this.SteamId.m_SteamID;
				return arg_33_0.Equals(steamID.ToString());
			}
			return false;
		}

		public override string GetFriendlyName()
		{
			if (this._friendlyName == null)
			{
				this._friendlyName = SteamFriends.GetFriendPersonaName(this.SteamId);
			}
			return this._friendlyName;
		}
	}
}
