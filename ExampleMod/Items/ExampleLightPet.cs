using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class ExampleLightPet : ModItem
	{
		public override void SetDefaults()
		{
			item.damage = 0;
			item.useStyle = 1;
			item.name = "Annoying Light";
			item.shoot = mod.ProjectileType("ExampleLightPet");
			item.width = 16;
			item.height = 30;
			item.UseSound = SoundID.Item2;
			item.useAnimation = 20;
			item.useTime = 20;
			item.rare = 8;
			item.noMelee = true;
			item.toolTip = "Summons an annoying light";
			item.value = Item.sellPrice(0, 5, 50, 0);
			item.buffType = mod.BuffType("ExampleLightPet");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Firefly, 10);
			recipe.AddTile(TileID.Furnaces);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void UseStyle(Player player)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			{
				player.AddBuff(item.buffType, 3600, true);
			}
		}
	}
}