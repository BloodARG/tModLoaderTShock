using Microsoft.Xna.Framework;
using System;

namespace Terraria
{
	public class CombatText
	{
		public static readonly Color DamagedFriendly = new Color(255, 80, 90, 255);
		public static readonly Color DamagedFriendlyCrit = new Color(255, 100, 30, 255);
		public static readonly Color DamagedHostile = new Color(255, 160, 80, 255);
		public static readonly Color DamagedHostileCrit = new Color(255, 100, 30, 255);
		public static readonly Color OthersDamagedHostile = CombatText.DamagedHostile * 0.4f;
		public static readonly Color OthersDamagedHostileCrit = CombatText.DamagedHostileCrit * 0.4f;
		public static readonly Color HealLife = new Color(100, 255, 100, 255);
		public static readonly Color HealMana = new Color(100, 100, 255, 255);
		public static readonly Color LifeRegen = new Color(255, 60, 70, 255);
		public static readonly Color LifeRegenNegative = new Color(255, 140, 40, 255);
		public Vector2 position;
		public Vector2 velocity;
		public float alpha;
		public int alphaDir = 1;
		public string text;
		public float scale = 1f;
		public float rotation;
		public Color color;
		public bool active;
		public int lifeTime;
		public bool crit;
		public bool dot;

		public static int NewText(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false)
		{
			if (Main.netMode == 2)
			{
				return 100;
			}
			for (int i = 0; i < 100; i++)
			{
				if (!Main.combatText[i].active)
				{
					int num = 0;
					if (dramatic)
					{
						num = 1;
					}
					Vector2 vector = Main.fontCombatText[num].MeasureString(text);
					Main.combatText[i].alpha = 1f;
					Main.combatText[i].alphaDir = -1;
					Main.combatText[i].active = true;
					Main.combatText[i].scale = 0f;
					Main.combatText[i].rotation = 0f;
					Main.combatText[i].position.X = (float)location.X + (float)location.Width * 0.5f - vector.X * 0.5f;
					Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.25f - vector.Y * 0.5f;
					CombatText expr_FD_cp_0 = Main.combatText[i];
					expr_FD_cp_0.position.X = expr_FD_cp_0.position.X + (float)Main.rand.Next(-(int)((double)location.Width * 0.5), (int)((double)location.Width * 0.5) + 1);
					CombatText expr_149_cp_0 = Main.combatText[i];
					expr_149_cp_0.position.Y = expr_149_cp_0.position.Y + (float)Main.rand.Next(-(int)((double)location.Height * 0.5), (int)((double)location.Height * 0.5) + 1);
					Main.combatText[i].color = color;
					Main.combatText[i].text = text;
					Main.combatText[i].velocity.Y = -7f;
					if (Main.player[Main.myPlayer].gravDir == -1f)
					{
						CombatText expr_1DC_cp_0 = Main.combatText[i];
						expr_1DC_cp_0.velocity.Y = expr_1DC_cp_0.velocity.Y * -1f;
						Main.combatText[i].position.Y = (float)location.Y + (float)location.Height * 0.75f + vector.Y * 0.5f;
					}
					Main.combatText[i].lifeTime = 60;
					Main.combatText[i].crit = dramatic;
					Main.combatText[i].dot = dot;
					if (dramatic)
					{
						Main.combatText[i].text = text;
						Main.combatText[i].lifeTime *= 2;
						CombatText expr_27F_cp_0 = Main.combatText[i];
						expr_27F_cp_0.velocity.Y = expr_27F_cp_0.velocity.Y * 2f;
						Main.combatText[i].velocity.X = (float)Main.rand.Next(-25, 26) * 0.05f;
						Main.combatText[i].rotation = (float)(Main.combatText[i].lifeTime / 2) * 0.002f;
						if (Main.combatText[i].velocity.X < 0f)
						{
							Main.combatText[i].rotation *= -1f;
						}
					}
					if (dot)
					{
						Main.combatText[i].velocity.Y = -4f;
						Main.combatText[i].lifeTime = 40;
					}
					return i;
				}
			}
			return 100;
		}

		public static void clearAll()
		{
			for (int i = 0; i < 100; i++)
			{
				Main.combatText[i].active = false;
			}
		}

		public void Update()
		{
			if (this.active)
			{
				this.alpha += (float)this.alphaDir * 0.05f;
				if ((double)this.alpha <= 0.6)
				{
					this.alphaDir = 1;
				}
				if (this.alpha >= 1f)
				{
					this.alpha = 1f;
					this.alphaDir = -1;
				}
				if (this.dot)
				{
					this.velocity.Y = this.velocity.Y + 0.15f;
				}
				else
				{
					this.velocity.Y = this.velocity.Y * 0.92f;
					if (this.crit)
					{
						this.velocity.Y = this.velocity.Y * 0.92f;
					}
				}
				this.velocity.X = this.velocity.X * 0.93f;
				this.position += this.velocity;
				this.lifeTime--;
				if (this.lifeTime <= 0)
				{
					this.scale -= 0.1f;
					if ((double)this.scale < 0.1)
					{
						this.active = false;
					}
					this.lifeTime = 0;
					if (this.crit)
					{
						this.alphaDir = -1;
						this.scale += 0.07f;
						return;
					}
				}
				else
				{
					if (this.crit)
					{
						if (this.velocity.X < 0f)
						{
							this.rotation += 0.001f;
						}
						else
						{
							this.rotation -= 0.001f;
						}
					}
					if (this.dot)
					{
						this.scale += 0.5f;
						if ((double)this.scale > 0.8)
						{
							this.scale = 0.8f;
							return;
						}
					}
					else
					{
						if (this.scale < 1f)
						{
							this.scale += 0.1f;
						}
						if (this.scale > 1f)
						{
							this.scale = 1f;
						}
					}
				}
			}
		}

		public static void UpdateCombatText()
		{
			for (int i = 0; i < 100; i++)
			{
				if (Main.combatText[i].active)
				{
					Main.combatText[i].Update();
				}
			}
		}
	}
}
