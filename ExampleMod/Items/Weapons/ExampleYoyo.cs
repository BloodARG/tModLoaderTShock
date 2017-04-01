﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items.Weapons
{
	public class ExampleYoyo : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Example Yoyo";
			item.useStyle = 5;
			item.width = 24;
			item.height = 24;
			item.noUseGraphic = true;
			item.UseSound = SoundID.Item1;
			item.melee = true;
			item.channel = true;
			item.noMelee = true;
			item.shoot = mod.ProjectileType<Projectiles.ExampleYoyo>();
			item.useAnimation = 25;
			item.useTime = 25;
			item.shootSpeed = 16f;

			item.knockBack = 2.5f;
			item.damage = 9;
			item.value = Item.sellPrice(0, 0, 1, 0);
			item.rare = 0;

			// These are all related to gamepad controls and don't seem to affect anything else
			ItemID.Sets.Yoyo[item.type] = true;
			ItemID.Sets.GamepadExtraRange[item.type] = 15;
			ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ExampleItem", 10);
			recipe.AddIngredient(ItemID.WoodYoyo);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
