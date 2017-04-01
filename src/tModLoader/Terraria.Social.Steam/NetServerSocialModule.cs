using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace Terraria.Social.Steam
{
	public class NetServerSocialModule : NetSocialModule
	{
		private ServerMode _mode;
		private Callback<P2PSessionRequest_t> _p2pSessionRequest;
		private bool _acceptingClients;
		private SocketConnectionAccepted _connectionAcceptedCallback;

		public NetServerSocialModule()
			: base(1, 2)
		{
		}

		private void BroadcastConnectedUsers()
		{
			List<ulong> list = new List<ulong>();
			foreach (KeyValuePair<CSteamID, NetSocialModule.ConnectionState> current in this._connectionStateMap)
			{
				if (current.Value == NetSocialModule.ConnectionState.Connected)
				{
					list.Add(current.Key.m_SteamID);
				}
			}
			byte[] array = new byte[list.Count * 8 + 1];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)1);
					foreach (ulong current2 in list)
					{
						binaryWriter.Write(current2);
					}
				}
			}
			this._lobby.SendMessage(array);
		}

		public override void Initialize()
		{
			base.Initialize();
			this._reader.SetReadEvent(new SteamP2PReader.OnReadEvent(this.OnPacketRead));
			this._p2pSessionRequest = Callback<P2PSessionRequest_t>.Create(new Callback<P2PSessionRequest_t>.DispatchDelegate(this.OnP2PSessionRequest));
			if (Program.LaunchParameters.ContainsKey("-lobby"))
			{
				this._mode |= ServerMode.Lobby;
				string a;
				if ((a = Program.LaunchParameters["-lobby"]) != null)
				{
					if (a == "private")
					{
						this._lobby.Create(true, new CallResult<LobbyCreated_t>.APIDispatchDelegate(this.OnLobbyCreated));
						goto IL_E2;
					}
					if (a == "friends")
					{
						this._mode |= ServerMode.FriendsCanJoin;
						this._lobby.Create(false, new CallResult<LobbyCreated_t>.APIDispatchDelegate(this.OnLobbyCreated));
						goto IL_E2;
					}
				}
				Console.WriteLine(Language.GetTextValue("Error.InvalidLobbyFlag", "private", "friends"));
			}
			IL_E2:
			if (Program.LaunchParameters.ContainsKey("-friendsoffriends"))
			{
				this._mode |= ServerMode.FriendsOfFriends;
			}
		}

		public override ulong GetLobbyId()
		{
			return this._lobby.Id.m_SteamID;
		}

		public override void OpenInviteInterface()
		{
		}

		public override void CancelJoin()
		{
		}

		public override bool CanInvite()
		{
			return false;
		}

		public override void LaunchLocalServer(Process process, ServerMode mode)
		{
		}

		public override bool StartListening(SocketConnectionAccepted callback)
		{
			this._acceptingClients = true;
			this._connectionAcceptedCallback = callback;
			return true;
		}

		public override void StopListening()
		{
			this._acceptingClients = false;
		}

		public override void Connect(RemoteAddress address)
		{
		}

		public override void Close(RemoteAddress address)
		{
			CSteamID user = base.RemoteAddressToSteamId(address);
			this.Close(user);
		}

		private void Close(CSteamID user)
		{
			if (!this._connectionStateMap.ContainsKey(user))
			{
				return;
			}
			SteamUser.EndAuthSession(user);
			SteamNetworking.CloseP2PSessionWithUser(user);
			this._connectionStateMap[user] = NetSocialModule.ConnectionState.Inactive;
			this._reader.ClearUser(user);
			this._writer.ClearUser(user);
		}

		private void OnLobbyCreated(LobbyCreated_t result, bool failure)
		{
			if (failure)
			{
				return;
			}
			SteamFriends.SetRichPresence("status", Language.GetTextValue("Social.StatusInGame"));
		}

		private bool OnPacketRead(byte[] data, int length, CSteamID userId)
		{
			if (!this._connectionStateMap.ContainsKey(userId) || this._connectionStateMap[userId] == NetSocialModule.ConnectionState.Inactive)
			{
				P2PSessionRequest_t result;
				result.m_steamIDRemote = userId;
				this.OnP2PSessionRequest(result);
				if (!this._connectionStateMap.ContainsKey(userId) || this._connectionStateMap[userId] == NetSocialModule.ConnectionState.Inactive)
				{
					return false;
				}
			}
			NetSocialModule.ConnectionState connectionState = this._connectionStateMap[userId];
			if (connectionState != NetSocialModule.ConnectionState.Authenticating)
			{
				return connectionState == NetSocialModule.ConnectionState.Connected;
			}
			if (length < 3)
			{
				return false;
			}
			int num = (int)data[1] << 8 | (int)data[0];
			if (num != length)
			{
				return false;
			}
			if (data[2] != 93)
			{
				return false;
			}
			byte[] array = new byte[data.Length - 3];
			Array.Copy(data, 3, array, 0, array.Length);
			switch (SteamUser.BeginAuthSession(array, array.Length, userId))
			{
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultOK:
					this._connectionStateMap[userId] = NetSocialModule.ConnectionState.Connected;
					this.BroadcastConnectedUsers();
					break;
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket:
					this.Close(userId);
					break;
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultDuplicateRequest:
					this.Close(userId);
					break;
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidVersion:
					this.Close(userId);
					break;
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultGameMismatch:
					this.Close(userId);
					break;
				case EBeginAuthSessionResult.k_EBeginAuthSessionResultExpiredTicket:
					this.Close(userId);
					break;
			}
			return false;
		}

		private void OnP2PSessionRequest(P2PSessionRequest_t result)
		{
			CSteamID steamIDRemote = result.m_steamIDRemote;
			if (this._connectionStateMap.ContainsKey(steamIDRemote) && this._connectionStateMap[steamIDRemote] != NetSocialModule.ConnectionState.Inactive)
			{
				SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
				return;
			}
			if (!this._acceptingClients)
			{
				return;
			}
			if (!this._mode.HasFlag(ServerMode.FriendsOfFriends) && SteamFriends.GetFriendRelationship(steamIDRemote) != EFriendRelationship.k_EFriendRelationshipFriend)
			{
				return;
			}
			SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
			P2PSessionState_t p2PSessionState_t;
			while (SteamNetworking.GetP2PSessionState(steamIDRemote, out p2PSessionState_t) && p2PSessionState_t.m_bConnecting == 1)
			{
			}
			if (p2PSessionState_t.m_bConnectionActive == 0)
			{
				this.Close(steamIDRemote);
			}
			this._connectionStateMap[steamIDRemote] = NetSocialModule.ConnectionState.Authenticating;
			this._connectionAcceptedCallback(new SocialSocket(new SteamAddress(steamIDRemote)));
		}
	}
}
