using System;

namespace Terraria.World.Generation
{
	public static class Conditions
	{
		public class IsTile : GenCondition
		{
			private ushort[] _types;

			public IsTile(params ushort[] types)
			{
				this._types = types;
			}

			protected override bool CheckValidity(int x, int y)
			{
				if (GenBase._tiles[x, y].active())
				{
					for (int i = 0; i < this._types.Length; i++)
					{
						if (GenBase._tiles[x, y].type == this._types[i])
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public class Continue : GenCondition
		{
			protected override bool CheckValidity(int x, int y)
			{
				return false;
			}
		}

		public class IsSolid : GenCondition
		{
			protected override bool CheckValidity(int x, int y)
			{
				return GenBase._tiles[x, y].active() && Main.tileSolid[(int)GenBase._tiles[x, y].type];
			}
		}

		public class HasLava : GenCondition
		{
			protected override bool CheckValidity(int x, int y)
			{
				return GenBase._tiles[x, y].liquid > 0 && GenBase._tiles[x, y].liquidType() == 1;
			}
		}
	}
}
