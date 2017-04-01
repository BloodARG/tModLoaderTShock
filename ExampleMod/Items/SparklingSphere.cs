using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class SparklingSphere : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Sparkling Sphere";
			item.holdStyle = 4;
			item.width = 40;
			item.height = 40;
			item.toolTip = "Hold to watch magic happen!";
			item.value = 10000;
			item.rare = 2;
		}

		public override void HoldStyle(Player player)
		{
			Vector2 position = GetLightPosition(player);
			if ((position.Y >= player.Center.Y) == (player.gravDir == 1))
			{
				player.itemLocation.X = player.Center.X + 6f * player.direction;
				player.itemLocation.Y = player.position.Y + 21f + 23f * player.gravDir + player.mount.PlayerOffsetHitbox;
			}
			else
			{
				player.itemLocation.X = player.Center.X;
				player.itemLocation.Y = player.position.Y + 21f - 3f * player.gravDir + player.mount.PlayerOffsetHitbox;
			}
			player.itemRotation = 0f;
		}

		public override bool HoldItemFrame(Player player)
		{
			Vector2 position = GetLightPosition(player);
			if ((position.Y >= player.Center.Y) == (player.gravDir == 1))
			{
				player.bodyFrame.Y = player.bodyFrame.Height * 3;
			}
			else
			{
				player.bodyFrame.Y = player.bodyFrame.Height * 2;
			}
			return true;
		}

		public override void HoldItem(Player player)
		{
			Vector2 position = GetLightPosition(player) - new Vector2(20f, 20f);
			if (Main.rand.Next(10) == 0)
			{
				Dust.NewDust(player.position, player.width, player.height, mod.DustType<Dusts.Sparkle>());
			}
			if (Main.rand.Next(3) == 0)
			{
				Dust.NewDust(position, 40, 40, mod.DustType("Sparkle"));
			}
		}

		private Vector2 GetLightPosition(Player player)
		{
			Vector2 position = Main.screenPosition;
			position.X += Main.mouseX;
			position.Y += player.gravDir == 1 ? Main.mouseY : Main.screenHeight - Main.mouseY;
			return position;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ExampleItem", 10);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}