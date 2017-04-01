using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Items.Abomination
{
	public class AbominationBag : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Treasure Bag";
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.toolTip = "Right click to open";
			item.rare = 9;
			item.expert = true;
			bossBagNPC = mod.NPCType("Abomination");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.TryGettingDevArmor();
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("AbominationMask"));
			}
			player.QuickSpawnItem(mod.ItemType("MoltenDrill"));
			player.QuickSpawnItem(mod.ItemType("ElementResidue"));
			player.QuickSpawnItem(mod.ItemType("PurityTotem"));
			player.QuickSpawnItem(mod.ItemType("SixColorShield"));
		}
	}
}