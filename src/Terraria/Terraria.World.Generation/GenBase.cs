using System;
using Terraria.Utilities;

namespace Terraria.World.Generation
{
	public class GenBase
	{
		public delegate bool CustomPerUnitAction(int x, int y, params object[] args);

		protected static UnifiedRandom _random
		{
			get
			{
				return WorldGen.genRand;
			}
		}

		protected static Tile[,] _tiles
		{
			get
			{
				return Main.tile;
			}
		}

		protected static int _worldWidth
		{
			get
			{
				return Main.maxTilesX;
			}
		}

		protected static int _worldHeight
		{
			get
			{
				return Main.maxTilesY;
			}
		}
	}
}
