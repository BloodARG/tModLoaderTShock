using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Chat;

namespace ExampleMod.Items
{
	public class Face : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Face";
			item.width = 20;
			item.height = 20;
			item.toolTip = "How are you feeling today?";
			// See here for help on using Tags: http://terraria.gamepedia.com/Chat#Tags
			item.toolTip2 = "[c/FF0000:Colors ][c/00FF00:are ][c/0000FF:fun ]and so are items: [i:" + item.type + "][i:" + mod.ItemType<CarKey>() + "][i/s123:" + ItemID.Ectoplasm + "]";
			item.value = 100;
			item.rare = 1;
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}

		public override DrawAnimation GetAnimation()
		{
			return new DrawAnimationVertical(30, 4);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Face", "I'm feeling just fine!");
			line.overrideColor = new Color(100, 100, 255);
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.mod == "Terraria" && line2.Name == "ItemName")
				{
					line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
				}
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ExampleItem");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}