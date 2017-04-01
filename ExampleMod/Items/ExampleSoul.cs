﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Items
{
	public class ExampleSoul : ModItem
	{
		// TODO -- Velocity Y smaller, post NewItem?
		public override void SetDefaults()
		{
			item.name = "Soul of Exampleness";
			Item refItem = new Item();
			refItem.SetDefaults(ItemID.SoulofSight);
			item.width = refItem.width;
			item.height = refItem.height;
			item.maxStack = 999;
			item.toolTip = "'The essence of example creatures'";
			item.value = 1000;
			item.rare = 3;
			ItemID.Sets.AnimatesAsSoul[item.type] = true;
			ItemID.Sets.ItemIconPulse[item.type] = true;
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}

		public override DrawAnimation GetAnimation()
		{
			// ticksperframe, frameCount
			return new DrawAnimationVertical(5, 4);
		}

		// The following 2 methods are purely to show off these 2 hooks. Don't use them in your own code.
		public override void GrabRange(Player player, ref int grabRange)
		{
			grabRange *= 3;
		}

		public override bool GrabStyle(Player player)
		{
			Vector2 vectorItemToPlayer = player.Center - item.Center;
			Vector2 movement = -vectorItemToPlayer.SafeNormalize(default(Vector2)) * 0.1f;
			item.velocity = item.velocity + movement;
			item.velocity = Collision.TileCollision(item.position, item.velocity, item.width, item.height);
			return true;
		}

		public override void PostUpdate()
		{
			Lighting.AddLight(item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
		}
	}

	public class SoulGlobalNPC : GlobalNPC
	{
		public override void NPCLoot(NPC npc)
		{
			if (Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<ExamplePlayer>(mod).ZoneExample)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExampleSoul"), 1);
			}
		}
	}
}