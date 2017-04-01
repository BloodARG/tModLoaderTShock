using System;

namespace Terraria.World.Generation
{
	public abstract class MicroBiome : GenStructure
	{
		public MicroBiome()
		{
		}

		public virtual void Reset()
		{
		}

		public static void ResetAll()
		{
			foreach (MicroBiome current in BiomeCollection.Biomes)
			{
				current.Reset();
			}
		}
	}
}
