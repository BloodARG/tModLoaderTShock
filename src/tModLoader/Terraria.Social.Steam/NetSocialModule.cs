using Steamworks;
using System;
using System.Collections.Concurrent;
using System.IO;
using Terraria.Net;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	public abstract class NetSocialModule : Terraria.Social.Base.NetSocialModule
	{
		public enum ConnectionState
		{
			Inactive,
			Authenticating,
			Connected
		}

		protected delegate void AsyncHandshake(CSteamID client);

		protected const int ServerReadChannel = 1;
		protected const int ClientReadChannel = 2;
		protected const int LobbyMessageJoin = 1;
		protected const ushort GamePort = 27005;
		protected const ushort SteamPort = 27006;
		protected const ushort QueryPort = 27007;
		protected static readonly byte[] _handshake = new byte[]
		{
			10,
			0,
			93,
			114,
			101,
			108,
			111,
			103,
			105,
			99
		};
		protected SteamP2PReader _reader;
		protected SteamP2PWriter _writer;
		protected Lobby _lobby = new Lobby();
		protected ConcurrentDictionary<CSteamID, NetSocialModule.ConnectionState> _connectionStateMap = new ConcurrentDictionary<CSteamID, NetSocialModule.ConnectionState>();
		protected object _steamLock = new object();
		private Callback<LobbyChatMsg_t> _lobbyChatMessage;

		protected NetSocialModule(int readChannel, int writeChannel)
		{
			this._reader = new SteamP2PReader(readChannel);
			this._writer = new SteamP2PWriter(writeChannel);
		}

		public override void Initialize()
		{
			CoreSocialModule.OnTick += new Action(this._reader.ReadTick);
			CoreSocialModule.OnTick += new Action(this._writer.SendAll);
			this._lobbyChatMessage = Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChatMessage));
		}

		public override void Shutdown()
		{
			this._lobby.Leave();
		}

		public override bool IsConnected(RemoteAddress address)
		{
			if (address == null)
			{
				return false;
			}
			CSteamID cSteamID = this.RemoteAddressToSteamId(address);
			if (!this._connectionStateMap.ContainsKey(cSteamID) || this._connectionStateMap[cSteamID] != NetSocialModule.ConnectionState.Connected)
			{
				return false;
			}
			if (this.GetSessionState(cSteamID).m_bConnectionActive != 1)
			{
				this.Close(address);
				return false;
			}
			return true;
		}

		protected virtual void OnLobbyChatMessage(LobbyChatMsg_t result)
		{
			if (result.m_ulSteamIDLobby != this._lobby.Id.m_SteamID)
			{
				return;
			}
			if (result.m_eChatEntryType != 1)
			{
				return;
			}
			if (result.m_ulSteamIDUser != this._lobby.Owner.m_SteamID)
			{
				return;
			}
			byte[] message = this._lobby.GetMessage((int)result.m_iChatID);
			if (message.Length == 0)
			{
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(message))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					byte b = binaryReader.ReadByte();
					byte b2 = b;
					if (b2 == 1)
					{
						while ((long)message.Length - memoryStream.Position >= 8L)
						{
							CSteamID cSteamID = new CSteamID(binaryReader.ReadUInt64());
							if (cSteamID != SteamUser.GetSteamID())
							{
								this._lobby.SetPlayedWith(cSteamID);
							}
						}
					}
				}
			}
		}

		protected P2PSessionState_t GetSessionState(CSteamID userId)
		{
			P2PSessionState_t result;
			SteamNetworking.GetP2PSessionState(userId, out result);
			return result;
		}

		protected CSteamID RemoteAddressToSteamId(RemoteAddress address)
		{
			return ((SteamAddress)address).SteamId;
		}

		public override bool Send(RemoteAddress address, byte[] data, int length)
		{
			CSteamID user = this.RemoteAddressToSteamId(address);
			this._writer.QueueSend(user, data, length);
			return true;
		}

		public override int Receive(RemoteAddress address, byte[] data, int offset, int length)
		{
			if (address == null)
			{
				return 0;
			}
			CSteamID user = this.RemoteAddressToSteamId(address);
			return this._reader.Receive(user, data, offset, length);
		}

		public override bool IsDataAvailable(RemoteAddress address)
		{
			CSteamID id = this.RemoteAddressToSteamId(address);
			return this._reader.IsDataAvailable(id);
		}
	}
}
