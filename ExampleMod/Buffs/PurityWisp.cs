using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Buffs
{
	public class PurityWisp : ModBuff
	{
		public override void SetDefaults()
		{
			Main.buffName[Type] = "Purity Wisp";
			Main.buffTip[Type] = "The purity wisp will fight for you";
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			ExamplePlayer modPlayer = player.GetModPlayer<ExamplePlayer>(mod);
			if (player.ownedProjectileCounts[mod.ProjectileType("PurityWisp")] > 0)
			{
				modPlayer.purityMinion = true;
			}
			if (!modPlayer.purityMinion)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
			else
			{
				player.buffTime[buffIndex] = 18000;
			}
		}
	}
}