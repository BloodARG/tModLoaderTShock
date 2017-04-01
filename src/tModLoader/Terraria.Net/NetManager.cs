using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Terraria.Net
{
	public class NetManager
	{
		private class PacketTypeStorage<T> where T : NetModule
		{
			public static T Module;
		}

		public static NetManager Instance = new NetManager();
		private Dictionary<ushort, NetModule> _modules = new Dictionary<ushort, NetModule>();
		private ushort ModuleCount;
		private static long _trafficTotal = 0L;
		private static Stopwatch _trafficTimer = NetManager.CreateStopwatch();

		private static Stopwatch CreateStopwatch()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public void Register<T>() where T : NetModule, new()
		{
			T t = Activator.CreateInstance<T>();
			t.Id = this.ModuleCount;
			NetManager.PacketTypeStorage<T>.Module = t;
			this._modules[this.ModuleCount] = t;
			this.ModuleCount += 1;
		}

		public NetModule GetModule<T>() where T : NetModule
		{
			return NetManager.PacketTypeStorage<T>.Module;
		}

		public ushort GetId<T>() where T : NetModule
		{
			return NetManager.PacketTypeStorage<T>.Module.Id;
		}

		public void Read(BinaryReader reader, int userId)
		{
			ushort key = reader.ReadUInt16();
			if (this._modules.ContainsKey(key))
			{
				this._modules[key].Deserialize(reader, userId);
			}
		}

		public void Broadcast(NetPacket packet, int ignoreClient = -1)
		{
			for (int i = 0; i < 256; i++)
			{
				if (i != ignoreClient && Netplay.Clients[i].IsConnected())
				{
					NetManager.SendData(Netplay.Clients[i].Socket, packet);
				}
			}
		}

		public void SendToServer(NetPacket packet)
		{
			NetManager.SendData(Netplay.Connection.Socket, packet);
		}

		public static void SendData(ISocket socket, NetPacket packet)
		{
			try
			{
				socket.AsyncSend(packet.Buffer.Data, 0, packet.Length, new SocketSendCallback(NetManager.SendCallback), packet);
			}
			catch
			{
				Console.WriteLine(Language.GetTextValue("Error.ExceptionNormal", Language.GetTextValue("Error.DataSentAfterConnectionLost")));
			}
		}

		public static void SendCallback(object state)
		{
			((NetPacket)state).Recycle();
		}

		private static void UpdateStats(int length)
		{
			NetManager._trafficTotal += (long)length;
			double totalSeconds = NetManager._trafficTimer.Elapsed.TotalSeconds;
			if (totalSeconds > 5.0)
			{
				double num = (double)NetManager._trafficTotal;
				double d = num / totalSeconds;
				double num2 = Math.Floor(d) / 1000.0;
				Console.WriteLine("NetManager :: Sending at " + num2 + " kbps.");
				NetManager._trafficTimer.Restart();
				NetManager._trafficTotal = 0L;
			}
		}
	}
}
