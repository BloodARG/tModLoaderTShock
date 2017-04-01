using System;

namespace Terraria.World.Generation
{
	public static class Passes
	{
		public class Clear : GenPass
		{
			public Clear() : base("clear", 1f)
			{
			}

			public override void Apply(GenerationProgress progress)
			{
				for (int i = 0; i < GenBase._worldWidth; i++)
				{
					for (int j = 0; j < GenBase._worldHeight; j++)
					{
						if (GenBase._tiles[i, j] == null)
						{
							GenBase._tiles[i, j] = new Tile();
						}
						else
						{
							GenBase._tiles[i, j].ClearEverything();
						}
					}
				}
			}
		}

		public class ScatterCustom : GenPass
		{
			private GenBase.CustomPerUnitAction _perUnit;

			private int _count;

			public ScatterCustom(string name, float loadWeight, int count, GenBase.CustomPerUnitAction perUnit = null) : base(name, loadWeight)
			{
				this._perUnit = perUnit;
				this._count = count;
			}

			public void SetCustomAction(GenBase.CustomPerUnitAction perUnit)
			{
				this._perUnit = perUnit;
			}

			public override void Apply(GenerationProgress progress)
			{
				int i = this._count;
				while (i > 0)
				{
					if (this._perUnit(GenBase._random.Next(1, GenBase._worldWidth), GenBase._random.Next(1, GenBase._worldHeight), new object[0]))
					{
						i--;
					}
				}
			}
		}
	}
}
