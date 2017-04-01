using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items.Placeable
{
	public class ExampleChest : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Example Chest";
			item.width = 26;
			item.height = 22;
			item.maxStack = 99;
			AddTooltip("This is a modded chest.");
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 500;
			item.createTile = mod.TileType("ExampleChest");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Chest);
			recipe.AddIngredient(null, "ExampleBlock", 10);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}