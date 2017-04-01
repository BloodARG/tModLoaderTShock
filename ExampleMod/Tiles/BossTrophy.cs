using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ExampleMod.Tiles
{
	public class BossTrophy : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(Type);
			dustType = 7;
			disableSmartCursor = true;
			AddMapEntry(new Color(120, 85, 60), "Trophy");
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int item = 0;
			switch (frameX / 54)
			{
				case 0:
					item = mod.ItemType("AbominationTrophy");
					break;
				case 1:
					item = mod.ItemType("PuritySpiritTrophy");
					break;
				case 2:
					item = mod.ItemType("BunnyTrophy");
					break;
				case 3:
					item = mod.ItemType("TreeTrophy");
					break;
			}
			if (item > 0)
			{
				Item.NewItem(i * 16, j * 16, 48, 48, item);
			}
		}
	}
}