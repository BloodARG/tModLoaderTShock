using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items.Weapons
{
	public class ExampleSword : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Example Sword";		//The name of your weapon
			item.damage = 50;			//The damage of your weapon
			item.melee = true;			//Is your weapon a melee weapon?
			item.width = 40;			//Weapon's texture's width
			item.height = 40;			//Weapon's texture's height
			item.toolTip = "This is a modded sword.";	//The text showed below your weapon's name
			item.useTime = 20;			//The time span of using the weapon. Remember in terraria, 60 frames is a second.
			item.useAnimation = 20;			//The time span of the using animation of the weapon, suggest set it the same as useTime.
			item.useStyle = 1;			//The use style of weapon, 1 for swinging, 2 for drinking, 3 act like shortsword, 4 for use like life crystal, 5 for use staffs or guns
			item.knockBack = 6;			//The force of knockback of the weapon. Maxium is 20
			item.value = 10000;			//The value of the weapon
			item.rare = 2;				//The rarity of the weapon, from -1 to 13
			item.UseSound = SoundID.Item1;		//The sound when the weapon is using
			item.autoReuse = true;			//Whether the weapon can use automaticly by pressing mousebutton
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ExampleItem", 10);
			recipe.AddTile(null, "ExampleWorkbench");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(3) == 0)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, mod.DustType("Sparkle"));
				//Emit dusts when swing the sword
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 60);		//Add Onfire buff to the NPC for 1 second
		}
	}
}
