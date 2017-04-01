using Terraria.ModLoader;

namespace ExampleMod.Items.Placeable
{
	public class ExampleMusicBox : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Music Box (Example)";
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.createTile = mod.TileType("ExampleMusicBox");
			item.width = 24;
			item.height = 24;
			item.rare = 4;
			item.value = 100000;
			item.accessory = true;
		}
	}
}
