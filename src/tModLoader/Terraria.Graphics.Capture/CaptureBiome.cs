using System;

namespace Terraria.Graphics.Capture
{
	public class CaptureBiome
	{
		public enum TileColorStyle
		{
			Normal,
			Jungle,
			Crimson,
			Corrupt,
			Mushroom
		}

		public static CaptureBiome[] Biomes;
		public readonly int WaterStyle;
		public readonly int BackgroundIndex;
		public readonly int BackgroundIndex2;
		public readonly CaptureBiome.TileColorStyle TileColor;

		public CaptureBiome(int backgroundIndex, int backgroundIndex2, int waterStyle, CaptureBiome.TileColorStyle tileColorStyle = CaptureBiome.TileColorStyle.Normal)
		{
			this.BackgroundIndex = backgroundIndex;
			this.BackgroundIndex2 = backgroundIndex2;
			this.WaterStyle = waterStyle;
			this.TileColor = tileColorStyle;
		}

		static CaptureBiome()
		{
			// Note: this type is marked as 'beforefieldinit'.
			CaptureBiome[] array = new CaptureBiome[12];
			array[0] = new CaptureBiome(0, 0, 0, CaptureBiome.TileColorStyle.Normal);
			array[2] = new CaptureBiome(1, 2, 2, CaptureBiome.TileColorStyle.Corrupt);
			array[3] = new CaptureBiome(3, 0, 3, CaptureBiome.TileColorStyle.Jungle);
			array[4] = new CaptureBiome(6, 2, 4, CaptureBiome.TileColorStyle.Normal);
			array[5] = new CaptureBiome(7, 4, 5, CaptureBiome.TileColorStyle.Normal);
			array[6] = new CaptureBiome(2, 1, 6, CaptureBiome.TileColorStyle.Normal);
			array[7] = new CaptureBiome(9, 6, 7, CaptureBiome.TileColorStyle.Mushroom);
			array[8] = new CaptureBiome(0, 0, 8, CaptureBiome.TileColorStyle.Normal);
			array[10] = new CaptureBiome(8, 5, 10, CaptureBiome.TileColorStyle.Crimson);
			CaptureBiome.Biomes = array;
		}
	}
}
