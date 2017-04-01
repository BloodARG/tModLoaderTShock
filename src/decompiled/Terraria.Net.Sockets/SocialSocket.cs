using System;
using System.Threading;
using Terraria.Social;

namespace Terraria.Net.Sockets
{
	public class SocialSocket : ISocket
	{
		private delegate void InternalReadCallback(byte[] data, int offset, int size, SocketReceiveCallback callback, object state);

		private RemoteAddress _remoteAddress;

		public SocialSocket()
		{
		}

		public SocialSocket(RemoteAddress remoteAddress)
		{
			this._remoteAddress = remoteAddress;
		}

		void ISocket.Close()
		{
			if (this._remoteAddress == null)
			{
				return;
			}
			SocialAPI.Network.Close(this._remoteAddress);
			this._remoteAddress = null;
		}

		bool ISocket.IsConnected()
		{
			return SocialAPI.Network.IsConnected(this._remoteAddress);
		}

		void ISocket.Connect(RemoteAddress address)
		{
			this._remoteAddress = address;
			SocialAPI.Network.Connect(address);
		}

		void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
		{
			SocialAPI.Network.Send(this._remoteAddress, data, size);
			callback.BeginInvoke(state, null, null);
		}

		private void ReadCallback(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
		{
			int size2;
			while ((size2 = SocialAPI.Network.Receive(this._remoteAddress, data, offset, size)) == 0)
			{
				Thread.Sleep(1);
			}
			callback(state, size2);
		}

		void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
		{
			SocialSocket.InternalReadCallback internalReadCallback = new SocialSocket.InternalReadCallback(this.ReadCallback);
			internalReadCallback.BeginInvoke(data, offset, size, callback, state, null, null);
		}

		void ISocket.SendQueuedPackets()
		{
		}

		bool ISocket.IsDataAvailable()
		{
			return SocialAPI.Network.IsDataAvailable(this._remoteAddress);
		}

		RemoteAddress ISocket.GetRemoteAddress()
		{
			return this._remoteAddress;
		}

		bool ISocket.StartListening(SocketConnectionAccepted callback)
		{
			return SocialAPI.Network.StartListening(callback);
		}

		void ISocket.StopListening()
		{
			SocialAPI.Network.StopListening();
		}
	}
}
