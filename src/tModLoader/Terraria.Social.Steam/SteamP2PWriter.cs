using Steamworks;
using System;
using System.Collections.Generic;

namespace Terraria.Social.Steam
{
	public class SteamP2PWriter
	{
		public class WriteInformation
		{
			public byte[] Data;
			public int Size;

			public WriteInformation()
			{
				this.Data = new byte[1024];
				this.Size = 0;
			}

			public WriteInformation(byte[] data)
			{
				this.Data = data;
				this.Size = 0;
			}
		}

		private const int BUFFER_SIZE = 1024;
		private Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>> _pendingSendData = new Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>();
		private Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>> _pendingSendDataSwap = new Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>();
		private Queue<byte[]> _bufferPool = new Queue<byte[]>();
		private int _channel;
		private object _lock = new object();

		public SteamP2PWriter(int channel)
		{
			this._channel = channel;
		}

		public void QueueSend(CSteamID user, byte[] data, int length)
		{
			lock (this._lock)
			{
				Queue<SteamP2PWriter.WriteInformation> queue;
				if (this._pendingSendData.ContainsKey(user))
				{
					queue = this._pendingSendData[user];
				}
				else
				{
					queue = (this._pendingSendData[user] = new Queue<SteamP2PWriter.WriteInformation>());
				}
				int i = length;
				int num = 0;
				while (i > 0)
				{
					SteamP2PWriter.WriteInformation writeInformation;
					if (queue.Count == 0 || 1024 - queue.Peek().Size == 0)
					{
						if (this._bufferPool.Count > 0)
						{
							writeInformation = new SteamP2PWriter.WriteInformation(this._bufferPool.Dequeue());
						}
						else
						{
							writeInformation = new SteamP2PWriter.WriteInformation();
						}
						queue.Enqueue(writeInformation);
					}
					else
					{
						writeInformation = queue.Peek();
					}
					int num2 = Math.Min(i, 1024 - writeInformation.Size);
					Array.Copy(data, num, writeInformation.Data, writeInformation.Size, num2);
					writeInformation.Size += num2;
					i -= num2;
					num += num2;
				}
			}
		}

		public void ClearUser(CSteamID user)
		{
			lock (this._lock)
			{
				if (this._pendingSendData.ContainsKey(user))
				{
					Queue<SteamP2PWriter.WriteInformation> queue = this._pendingSendData[user];
					while (queue.Count > 0)
					{
						this._bufferPool.Enqueue(queue.Dequeue().Data);
					}
				}
				if (this._pendingSendDataSwap.ContainsKey(user))
				{
					Queue<SteamP2PWriter.WriteInformation> queue2 = this._pendingSendDataSwap[user];
					while (queue2.Count > 0)
					{
						this._bufferPool.Enqueue(queue2.Dequeue().Data);
					}
				}
			}
		}

		public void SendAll()
		{
			lock (this._lock)
			{
				Utils.Swap<Dictionary<CSteamID, Queue<SteamP2PWriter.WriteInformation>>>(ref this._pendingSendData, ref this._pendingSendDataSwap);
			}
			foreach (KeyValuePair<CSteamID, Queue<SteamP2PWriter.WriteInformation>> current in this._pendingSendDataSwap)
			{
				Queue<SteamP2PWriter.WriteInformation> value = current.Value;
				while (value.Count > 0)
				{
					SteamP2PWriter.WriteInformation writeInformation = value.Dequeue();
					bool flag2 = SteamNetworking.SendP2PPacket(current.Key, writeInformation.Data, (uint)writeInformation.Size, EP2PSend.k_EP2PSendReliable, this._channel);
					this._bufferPool.Enqueue(writeInformation.Data);
				}
			}
		}
	}
}
