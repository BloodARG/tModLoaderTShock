using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Projectiles.PuritySpirit
{
	public class PuritySnake : ModProjectile
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = "ExampleMod/Projectiles/ShadowArm";
			return base.Autoload(ref name, ref texture);
		}

		public override void SetDefaults()
		{
			projectile.name = "Void Trail";
			projectile.width = 80;
			projectile.height = 80;
			projectile.hide = true;
			projectile.penetrate = -1;
			projectile.magic = true;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 180;
			cooldownSlot = 1;
		}

		public override void AI()
		{
			NPC source = Main.npc[(int)projectile.ai[0]];
			if (!source.active || source.type != mod.NPCType("PuritySpirit"))
			{
				projectile.Kill();
				return;
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 240f)
			{
				projectile.Kill();
				return;
			}
			if (projectile.localAI[0] % 10 == 0)
			{
				ExamplePlayer modPlayer = Main.LocalPlayer.GetModPlayer<ExamplePlayer>(mod);
				if (modPlayer.heroLives > 0)
				{
					Main.PlaySound(SoundID.Item20);
				}
				else
				{
					Main.PlaySound(SoundID.Item20, projectile.position);
				}
			}
			if (projectile.localAI[0] > 60f)
			{
				IList<int> targets = ((NPCs.PuritySpirit.PuritySpirit)source.modNPC).targets;
				Vector2 offset = Vector2.Zero;
				bool flag = false;
				foreach (int player in targets)
				{
					Vector2 newOffset = Main.player[player].Center - projectile.Center;
					if (!flag || offset.Length() > newOffset.Length())
					{
						offset = newOffset;
						flag = true;
					}
				}
				if (offset != Vector2.Zero)
				{
					offset.Normalize();
				}
				offset *= 7f + 3f * (1 - projectile.ai[1]);
				projectile.velocity = offset;
			}
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				CreateDust(projectile.oldPos[k]);
			}
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			if (target.hurtCooldowns[1] <= 0)
			{
				ExamplePlayer modPlayer = target.GetModPlayer<ExamplePlayer>(mod);
				modPlayer.defenseEffect = 1f;
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				if (projectile.oldPos[k] == Vector2.Zero)
				{
					return null;
				}
				projHitbox.X = (int)projectile.oldPos[k].X;
				projHitbox.Y = (int)projectile.oldPos[k].Y;
				if (projHitbox.Intersects(targetHitbox))
				{
					return true;
				}
			}
			return null;
		}

		public void CreateDust(Vector2 pos)
		{
			if (Main.rand.Next(5) == 0)
			{
				int dust = Dust.NewDust(pos, projectile.width, projectile.height, mod.DustType("Smoke"), 0f, 0f, 0, new Color(0, 180, 0));
				Main.dust[dust].scale = 2f;
				Main.dust[dust].velocity *= 0.5f;
				Main.dust[dust].noLight = true;
			}
		}
	}
}