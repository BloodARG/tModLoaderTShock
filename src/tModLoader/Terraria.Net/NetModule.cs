using System;
using System.IO;

namespace Terraria.Net
{
	public abstract class NetModule
	{
		protected const int HEADER_SIZE = 5;
		public ushort Id;

		public NetModule()
		{
		}

		public abstract bool Deserialize(BinaryReader reader, int userId);

		protected static NetPacket CreatePacket<T>(int size) where T : NetModule
		{
			ushort id = NetManager.Instance.GetId<T>();
			NetPacket result = new NetPacket(id, size + 5);
			result.Writer.Write((ushort)(size + 5));
			result.Writer.Write((byte)82);
			result.Writer.Write(id);
			return result;
		}
	}
}
