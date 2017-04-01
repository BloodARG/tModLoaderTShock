using Steamworks;
using System;
using System.Collections.Generic;

namespace Terraria.Social.Steam
{
	public class SteamP2PReader
	{
		public class ReadResult
		{
			public byte[] Data;
			public uint Size;
			public uint Offset;

			public ReadResult(byte[] data, uint size)
			{
				this.Data = data;
				this.Size = size;
				this.Offset = 0u;
			}
		}

		public delegate bool OnReadEvent(byte[] data, int size, CSteamID user);

		private const int BUFFER_SIZE = 4096;
		public object SteamLock = new object();
		private Dictionary<CSteamID, Queue<SteamP2PReader.ReadResult>> _pendingReadBuffers = new Dictionary<CSteamID, Queue<SteamP2PReader.ReadResult>>();
		private Queue<CSteamID> _deletionQueue = new Queue<CSteamID>();
		private Queue<byte[]> _bufferPool = new Queue<byte[]>();
		private int _channel;
		private SteamP2PReader.OnReadEvent _readEvent;

		public SteamP2PReader(int channel)
		{
			this._channel = channel;
		}

		public void ClearUser(CSteamID id)
		{
			lock (this._pendingReadBuffers)
			{
				this._deletionQueue.Enqueue(id);
			}
		}

		public bool IsDataAvailable(CSteamID id)
		{
			bool result;
			lock (this._pendingReadBuffers)
			{
				if (!this._pendingReadBuffers.ContainsKey(id))
				{
					result = false;
				}
				else
				{
					Queue<SteamP2PReader.ReadResult> queue = this._pendingReadBuffers[id];
					if (queue.Count == 0 || queue.Peek().Size == 0u)
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		public void SetReadEvent(SteamP2PReader.OnReadEvent method)
		{
			this._readEvent = method;
		}

		private bool IsPacketAvailable(out uint size)
		{
			bool result;
			lock (this.SteamLock)
			{
				result = SteamNetworking.IsP2PPacketAvailable(out size, this._channel);
			}
			return result;
		}

		public void ReadTick()
		{
			lock (this._pendingReadBuffers)
			{
				while (this._deletionQueue.Count > 0)
				{
					this._pendingReadBuffers.Remove(this._deletionQueue.Dequeue());
				}
				uint val;
				while (this.IsPacketAvailable(out val))
				{
					byte[] array;
					if (this._bufferPool.Count == 0)
					{
						array = new byte[Math.Max(val, 4096u)];
					}
					else
					{
						array = this._bufferPool.Dequeue();
					}
					uint size;
					CSteamID cSteamID;
					bool flag3;
					lock (this.SteamLock)
					{
						flag3 = SteamNetworking.ReadP2PPacket(array, (uint)array.Length, out size, out cSteamID, this._channel);
					}
					if (flag3)
					{
						if (this._readEvent == null || this._readEvent(array, (int)size, cSteamID))
						{
							if (!this._pendingReadBuffers.ContainsKey(cSteamID))
							{
								this._pendingReadBuffers[cSteamID] = new Queue<SteamP2PReader.ReadResult>();
							}
							this._pendingReadBuffers[cSteamID].Enqueue(new SteamP2PReader.ReadResult(array, size));
						}
						else
						{
							this._bufferPool.Enqueue(array);
						}
					}
				}
			}
		}

		public int Receive(CSteamID user, byte[] buffer, int bufferOffset, int bufferSize)
		{
			uint num = 0u;
			lock (this._pendingReadBuffers)
			{
				if (!this._pendingReadBuffers.ContainsKey(user))
				{
					int result = 0;
					return result;
				}
				Queue<SteamP2PReader.ReadResult> queue = this._pendingReadBuffers[user];
				while (queue.Count > 0)
				{
					SteamP2PReader.ReadResult readResult = queue.Peek();
					uint num2 = Math.Min((uint)(bufferSize - (int)num), readResult.Size - readResult.Offset);
					if (num2 == 0u)
					{
						int result = (int)num;
						return result;
					}
					Array.Copy(readResult.Data, (long)((ulong)readResult.Offset), buffer, (long)bufferOffset + (long)((ulong)num), (long)((ulong)num2));
					if (num2 == readResult.Size - readResult.Offset)
					{
						this._bufferPool.Enqueue(queue.Dequeue().Data);
					}
					else
					{
						readResult.Offset += num2;
					}
					num += num2;
				}
			}
			return (int)num;
		}
	}
}
