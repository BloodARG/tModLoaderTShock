using Steamworks;
using System;
using System.Diagnostics;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace Terraria.Social.Steam
{
	public class NetClientSocialModule : NetSocialModule
	{
		private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
		private Callback<P2PSessionRequest_t> _p2pSessionRequest;
		private Callback<P2PSessionConnectFail_t> _p2pSessionConnectfail;
		private HAuthTicket _authTicket = HAuthTicket.Invalid;
		private byte[] _authData = new byte[1021];
		private uint _authDataLength;
		private bool _hasLocalHost;

		public NetClientSocialModule()
			: base(2, 1)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this._gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequest));
			this._p2pSessionRequest = Callback<P2PSessionRequest_t>.Create(new Callback<P2PSessionRequest_t>.DispatchDelegate(this.OnP2PSessionRequest));
			this._p2pSessionConnectfail = Callback<P2PSessionConnectFail_t>.Create(new Callback<P2PSessionConnectFail_t>.DispatchDelegate(this.OnSessionConnectFail));
			Main.OnEngineLoad += new Action(this.CheckParameters);
		}

		private void CheckParameters()
		{
			ulong ulSteamID;
			if (Program.LaunchParameters.ContainsKey("+connect_lobby") && ulong.TryParse(Program.LaunchParameters["+connect_lobby"], out ulSteamID))
			{
				CSteamID lobbySteamId = new CSteamID(ulSteamID);
				if (lobbySteamId.IsValid())
				{
					Main.OpenPlayerSelect(delegate(PlayerFileData playerData)
						{
							Main.ServerSideCharacter = false;
							playerData.SetAsActive();
							Main.menuMode = 882;
							Main.statusText = Language.GetTextValue("Social.Joining");
							this._lobby.Join(lobbySteamId, new CallResult<LobbyEnter_t>.APIDispatchDelegate(this.OnLobbyEntered));
						});
				}
			}
		}

		public override void LaunchLocalServer(Process process, ServerMode mode)
		{
			if (this._lobby.State != LobbyState.Inactive)
			{
				this._lobby.Leave();
			}
			ProcessStartInfo expr_1E = process.StartInfo;
			expr_1E.Arguments = expr_1E.Arguments + " -steam -localsteamid " + SteamUser.GetSteamID().m_SteamID;
			if (mode.HasFlag(ServerMode.Lobby))
			{
				this._hasLocalHost = true;
				if (mode.HasFlag(ServerMode.FriendsCanJoin))
				{
					ProcessStartInfo expr_78 = process.StartInfo;
					expr_78.Arguments += " -lobby friends";
				}
				else
				{
					ProcessStartInfo expr_95 = process.StartInfo;
					expr_95.Arguments += " -lobby private";
				}
				if (mode.HasFlag(ServerMode.FriendsOfFriends))
				{
					ProcessStartInfo expr_C3 = process.StartInfo;
					expr_C3.Arguments += " -friendsoffriends";
				}
			}
			SteamFriends.SetRichPresence("status", Language.GetTextValue("Social.StatusInGame"));
			Netplay.OnDisconnect += new Action(this.OnDisconnect);
			process.Start();
		}

		public override ulong GetLobbyId()
		{
			return 0uL;
		}

		public override bool StartListening(SocketConnectionAccepted callback)
		{
			return false;
		}

		public override void StopListening()
		{
		}

		public override void Close(RemoteAddress address)
		{
			SteamFriends.ClearRichPresence();
			CSteamID user = base.RemoteAddressToSteamId(address);
			this.Close(user);
		}

		public override bool CanInvite()
		{
			return (this._hasLocalHost || this._lobby.State == LobbyState.Active || Main.LobbyId != 0uL) && Main.netMode != 0;
		}

		public override void OpenInviteInterface()
		{
			this._lobby.OpenInviteOverlay();
		}

		private void Close(CSteamID user)
		{
			if (!this._connectionStateMap.ContainsKey(user))
			{
				return;
			}
			SteamNetworking.CloseP2PSessionWithUser(user);
			this.ClearAuthTicket();
			this._connectionStateMap[user] = NetSocialModule.ConnectionState.Inactive;
			this._lobby.Leave();
			this._reader.ClearUser(user);
			this._writer.ClearUser(user);
		}

		public override void Connect(RemoteAddress address)
		{
		}

		public override void CancelJoin()
		{
			if (this._lobby.State != LobbyState.Inactive)
			{
				this._lobby.Leave();
			}
		}

		private void OnLobbyJoinRequest(GameLobbyJoinRequested_t result)
		{
			if (this._lobby.State != LobbyState.Inactive)
			{
				this._lobby.Leave();
			}
			string friendName = SteamFriends.GetFriendPersonaName(result.m_steamIDFriend);
			Main.OpenPlayerSelect(delegate(PlayerFileData playerData)
				{
					Main.ServerSideCharacter = false;
					playerData.SetAsActive();
					Main.menuMode = 882;
					Main.statusText = Language.GetTextValue("Social.JoiningFriend", friendName);
					this._lobby.Join(result.m_steamIDLobby, new CallResult<LobbyEnter_t>.APIDispatchDelegate(this.OnLobbyEntered));
				});
		}

		private void OnLobbyEntered(LobbyEnter_t result, bool failure)
		{
			SteamNetworking.AllowP2PPacketRelay(true);
			this.SendAuthTicket(this._lobby.Owner);
			int num = 0;
			P2PSessionState_t p2PSessionState_t;
			while (SteamNetworking.GetP2PSessionState(this._lobby.Owner, out p2PSessionState_t) && p2PSessionState_t.m_bConnectionActive != 1)
			{
				switch (p2PSessionState_t.m_eP2PSessionError)
				{
					case 1:
						this.ClearAuthTicket();
						return;
					case 2:
						this.ClearAuthTicket();
						return;
					case 3:
						this.ClearAuthTicket();
						return;
					case 4:
						if (++num > 5)
						{
							this.ClearAuthTicket();
							return;
						}
						SteamNetworking.CloseP2PSessionWithUser(this._lobby.Owner);
						this.SendAuthTicket(this._lobby.Owner);
						break;
					case 5:
						this.ClearAuthTicket();
						return;
				}
			}
			this._connectionStateMap[this._lobby.Owner] = NetSocialModule.ConnectionState.Connected;
			SteamFriends.SetPlayedWith(this._lobby.Owner);
			SteamFriends.SetRichPresence("status", Language.GetTextValue("Social.StatusInGame"));
			Main.clrInput();
			Netplay.ServerPassword = "";
			Main.GetInputText("");
			Main.autoPass = false;
			Main.netMode = 1;
			Netplay.OnConnectedToSocialServer(new SocialSocket(new SteamAddress(this._lobby.Owner)));
		}

		private void SendAuthTicket(CSteamID address)
		{
			if (this._authTicket == HAuthTicket.Invalid)
			{
				this._authTicket = SteamUser.GetAuthSessionTicket(this._authData, this._authData.Length, out this._authDataLength);
			}
			int num = (int)(this._authDataLength + 3u);
			byte[] array = new byte[num];
			array[0] = (byte)(num & 255);
			array[1] = (byte)(num >> 8 & 255);
			array[2] = 93;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this._authDataLength))
			{
				array[num2 + 3] = this._authData[num2];
				num2++;
			}
			SteamNetworking.SendP2PPacket(address, array, (uint)num, EP2PSend.k_EP2PSendReliable, 1);
		}

		private void ClearAuthTicket()
		{
			if (this._authTicket != HAuthTicket.Invalid)
			{
				SteamUser.CancelAuthTicket(this._authTicket);
			}
			this._authTicket = HAuthTicket.Invalid;
			for (int i = 0; i < this._authData.Length; i++)
			{
				this._authData[i] = 0;
			}
			this._authDataLength = 0u;
		}

		private void OnDisconnect()
		{
			SteamFriends.ClearRichPresence();
			this._hasLocalHost = false;
			Netplay.OnDisconnect -= new Action(this.OnDisconnect);
		}

		private void OnSessionConnectFail(P2PSessionConnectFail_t result)
		{
			this.Close(result.m_steamIDRemote);
		}

		private void OnP2PSessionRequest(P2PSessionRequest_t result)
		{
			CSteamID steamIDRemote = result.m_steamIDRemote;
			if (this._connectionStateMap.ContainsKey(steamIDRemote) && this._connectionStateMap[steamIDRemote] != NetSocialModule.ConnectionState.Inactive)
			{
				SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
			}
		}
	}
}
