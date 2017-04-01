using System;

namespace Terraria.Map
{
	public struct MapTile
	{
		public ushort Type;
		public byte Light;
		private byte _extraData;

		public bool IsChanged
		{
			get
			{
				return (this._extraData & 128) == 128;
			}
			set
			{
				if (value)
				{
					this._extraData |= 128;
					return;
				}
				this._extraData &= 127;
			}
		}

		public byte Color
		{
			get
			{
				return (byte)(this._extraData & 127);
			}
			set
			{
				this._extraData = (byte)((this._extraData & 128) | (value & 127));
			}
		}

		private MapTile(ushort type, byte light, byte extraData)
		{
			this.Type = type;
			this.Light = light;
			this._extraData = extraData;
		}

		public bool Equals(ref MapTile other)
		{
			return this.Light == other.Light && this.Type == other.Type && this.Color == other.Color;
		}

		public bool EqualsWithoutLight(ref MapTile other)
		{
			return this.Type == other.Type && this.Color == other.Color;
		}

		public void Clear()
		{
			this.Type = 0;
			this.Light = 0;
			this._extraData = 0;
		}

		public MapTile WithLight(byte light)
		{
			return new MapTile(this.Type, light, (byte)(this._extraData | 128));
		}

		public static MapTile Create(ushort type, byte light, byte color)
		{
			return new MapTile(type, light, (byte)(color | 128));
		}
	}
}
