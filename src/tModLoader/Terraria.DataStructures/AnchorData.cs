using System;
using Terraria.Enums;

namespace Terraria.DataStructures
{
	public struct AnchorData
	{
		public AnchorType type;
		public int tileCount;
		public int checkStart;
		public static AnchorData Empty = default(AnchorData);

		public AnchorData(AnchorType type, int count, int start)
		{
			this.type = type;
			this.tileCount = count;
			this.checkStart = start;
		}

		public static bool operator ==(AnchorData data1, AnchorData data2)
		{
			return data1.type == data2.type && data1.tileCount == data2.tileCount && data1.checkStart == data2.checkStart;
		}

		public static bool operator !=(AnchorData data1, AnchorData data2)
		{
			return data1.type != data2.type || data1.tileCount != data2.tileCount || data1.checkStart != data2.checkStart;
		}

		public override bool Equals(object obj)
		{
			return obj is AnchorData && (this.type == ((AnchorData)obj).type && this.tileCount == ((AnchorData)obj).tileCount) && this.checkStart == ((AnchorData)obj).checkStart;
		}

		public override int GetHashCode()
		{
			byte b = (byte)this.checkStart;
			byte b2 = (byte)this.tileCount;
			ushort num = (ushort)this.type;
			return (int)num << 16 | (int)b2 << 8 | (int)b;
		}
	}
}
