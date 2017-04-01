using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI;
using Terraria.ID;

namespace Terraria.Cinematics
{
	public class DD2Film : Film
	{
		private NPC _dryad;
		private NPC _ogre;
		private NPC _portal;
		private List<NPC> _army = new List<NPC>();
		private List<NPC> _critters = new List<NPC>();
		private Vector2 _startPoint;

		public DD2Film()
		{
			base.AppendKeyFrames(new FrameEvent[]
				{
					new FrameEvent(this.CreateDryad),
					new FrameEvent(this.CreateCritters)
				});
			base.AppendSequences(120, new FrameEvent[]
				{
					new FrameEvent(this.DryadStand),
					new FrameEvent(this.DryadLookRight)
				});
			base.AppendSequences(100, new FrameEvent[]
				{
					new FrameEvent(this.DryadLookRight),
					new FrameEvent(this.DryadInteract)
				});
			base.AddKeyFrame(base.AppendPoint - 20, new FrameEvent(this.CreatePortal));
			base.AppendSequences(30, new FrameEvent[]
				{
					new FrameEvent(this.DryadLookLeft),
					new FrameEvent(this.DryadStand)
				});
			base.AppendSequences(40, new FrameEvent[]
				{
					new FrameEvent(this.DryadConfusedEmote),
					new FrameEvent(this.DryadStand),
					new FrameEvent(this.DryadLookLeft)
				});
			base.AppendKeyFrame(new FrameEvent(this.CreateOgre));
			base.AddKeyFrame(base.AppendPoint + 60, new FrameEvent(this.SpawnJavalinThrower));
			base.AddKeyFrame(base.AppendPoint + 120, new FrameEvent(this.SpawnGoblin));
			base.AddKeyFrame(base.AppendPoint + 180, new FrameEvent(this.SpawnGoblin));
			base.AddKeyFrame(base.AppendPoint + 240, new FrameEvent(this.SpawnWitherBeast));
			base.AppendSequences(30, new FrameEvent[]
				{
					new FrameEvent(this.DryadStand),
					new FrameEvent(this.DryadLookLeft)
				});
			base.AppendSequences(30, new FrameEvent[]
				{
					new FrameEvent(this.DryadLookRight),
					new FrameEvent(this.DryadWalk)
				});
			base.AppendSequences(300, new FrameEvent[]
				{
					new FrameEvent(this.DryadAttack),
					new FrameEvent(this.DryadLookLeft)
				});
			base.AppendKeyFrame(new FrameEvent(this.RemoveEnemyDamage));
			base.AppendSequences(60, new FrameEvent[]
				{
					new FrameEvent(this.DryadLookRight),
					new FrameEvent(this.DryadStand),
					new FrameEvent(this.DryadAlertEmote)
				});
			base.AddSequences(base.AppendPoint - 90, 60, new FrameEvent[]
				{
					new FrameEvent(this.OgreLookLeft),
					new FrameEvent(this.OgreStand)
				});
			base.AddKeyFrame(base.AppendPoint - 12, new FrameEvent(this.OgreSwingSound));
			base.AddSequences(base.AppendPoint - 30, 50, new FrameEvent[]
				{
					new FrameEvent(this.DryadPortalKnock),
					new FrameEvent(this.DryadStand)
				});
			base.AppendKeyFrame(new FrameEvent(this.RestoreEnemyDamage));
			base.AppendSequences(40, new FrameEvent[]
				{
					new FrameEvent(this.DryadPortalFade),
					new FrameEvent(this.DryadStand)
				});
			base.AppendSequence(180, new FrameEvent(this.DryadStand));
			base.AddSequence(0, base.AppendPoint, new FrameEvent(this.PerFrameSettings));
		}

		private void PerFrameSettings(FrameEventData evt)
		{
			CombatText.clearAll();
		}

		private void CreateDryad(FrameEventData evt)
		{
			this._dryad = this.PlaceNPCOnGround(20, this._startPoint);
			this._dryad.knockBackResist = 0f;
			this._dryad.immortal = true;
			this._dryad.dontTakeDamage = true;
			this._dryad.takenDamageMultiplier = 0f;
			this._dryad.immune[255] = 100000;
		}

		private void DryadInteract(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				this._dryad.ai[0] = 9f;
				if (evt.IsFirstFrame)
				{
					this._dryad.ai[1] = (float)evt.Duration;
				}
				this._dryad.localAI[0] = 0f;
			}
		}

		private void SpawnWitherBeast(FrameEventData evt)
		{
			int num = NPC.NewNPC((int)this._portal.Center.X, (int)this._portal.Bottom.Y, 568, 0, 0f, 0f, 0f, 0f, 255);
			NPC nPC = Main.npc[num];
			nPC.knockBackResist = 0f;
			nPC.immortal = true;
			nPC.dontTakeDamage = true;
			nPC.takenDamageMultiplier = 0f;
			nPC.immune[255] = 100000;
			nPC.friendly = this._ogre.friendly;
			this._army.Add(nPC);
		}

		private void SpawnJavalinThrower(FrameEventData evt)
		{
			int num = NPC.NewNPC((int)this._portal.Center.X, (int)this._portal.Bottom.Y, 561, 0, 0f, 0f, 0f, 0f, 255);
			NPC nPC = Main.npc[num];
			nPC.knockBackResist = 0f;
			nPC.immortal = true;
			nPC.dontTakeDamage = true;
			nPC.takenDamageMultiplier = 0f;
			nPC.immune[255] = 100000;
			nPC.friendly = this._ogre.friendly;
			this._army.Add(nPC);
		}

		private void SpawnGoblin(FrameEventData evt)
		{
			int num = NPC.NewNPC((int)this._portal.Center.X, (int)this._portal.Bottom.Y, 552, 0, 0f, 0f, 0f, 0f, 255);
			NPC nPC = Main.npc[num];
			nPC.knockBackResist = 0f;
			nPC.immortal = true;
			nPC.dontTakeDamage = true;
			nPC.takenDamageMultiplier = 0f;
			nPC.immune[255] = 100000;
			nPC.friendly = this._ogre.friendly;
			this._army.Add(nPC);
		}

		private void CreateCritters(FrameEventData evt)
		{
			for (int i = 0; i < 5; i++)
			{
				float num = (float)i / 5f;
				NPC nPC = this.PlaceNPCOnGround((int)Utils.SelectRandom<short>(Main.rand, new short[]
						{
							46,
							46,
							299,
							538
						}), this._startPoint + new Vector2((num - 0.25f) * 400f + Main.rand.NextFloat() * 50f - 25f, 0f));
				nPC.ai[0] = 0f;
				nPC.ai[1] = 600f;
				this._critters.Add(nPC);
			}
			if (this._dryad == null)
			{
				return;
			}
			for (int j = 0; j < 10; j++)
			{
				float arg_CB_0 = (float)j / 10f;
				int num2 = NPC.NewNPC((int)this._dryad.position.X + Main.rand.Next(-1000, 800), (int)this._dryad.position.Y - Main.rand.Next(-50, 300), 356, 0, 0f, 0f, 0f, 0f, 255);
				NPC nPC2 = Main.npc[num2];
				nPC2.ai[0] = Main.rand.NextFloat() * 4f - 2f;
				nPC2.ai[1] = Main.rand.NextFloat() * 4f - 2f;
				nPC2.velocity.X = Main.rand.NextFloat() * 4f - 2f;
				this._critters.Add(nPC2);
			}
		}

		private void OgreSwingSound(FrameEventData evt)
		{
			Main.PlaySound(SoundID.DD2_OgreAttack, this._ogre.Center);
		}

		private void DryadPortalKnock(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				if (evt.Frame == 20)
				{
					NPC expr_21_cp_0 = this._dryad;
					expr_21_cp_0.velocity.Y = expr_21_cp_0.velocity.Y - 7f;
					NPC expr_3D_cp_0 = this._dryad;
					expr_3D_cp_0.velocity.X = expr_3D_cp_0.velocity.X - 8f;
					Main.PlaySound(3, (int)this._dryad.Center.X, (int)this._dryad.Center.Y, 1, 1f, 0f);
				}
				if (evt.Frame >= 20)
				{
					this._dryad.ai[0] = 1f;
					this._dryad.ai[1] = (float)evt.Remaining;
					this._dryad.rotation += 0.05f;
				}
			}
			if (this._ogre != null)
			{
				if (evt.Frame > 40)
				{
					this._ogre.target = Main.myPlayer;
					this._ogre.direction = 1;
					return;
				}
				this._ogre.direction = -1;
				this._ogre.ai[1] = 0f;
				this._ogre.ai[0] = Math.Min(40f, this._ogre.ai[0]);
				this._ogre.target = 300 + this._dryad.whoAmI;
			}
		}

		private void RemoveEnemyDamage(FrameEventData evt)
		{
			this._ogre.friendly = true;
			foreach (NPC current in this._army)
			{
				current.friendly = true;
			}
		}

		private void RestoreEnemyDamage(FrameEventData evt)
		{
			this._ogre.friendly = false;
			foreach (NPC current in this._army)
			{
				current.friendly = false;
			}
		}

		private void DryadPortalFade(FrameEventData evt)
		{
			if (this._dryad != null && this._portal != null)
			{
				if (evt.IsFirstFrame)
				{
					Main.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, this._dryad.Center);
				}
				float num = (float)(evt.Frame - 7) / (float)(evt.Duration - 7);
				num = Math.Max(0f, num);
				this._dryad.color = new Color(Vector3.Lerp(Vector3.One, new Vector3(0.5f, 0f, 0.8f), num));
				this._dryad.Opacity = 1f - num;
				this._dryad.rotation += 0.05f * (num * 4f + 1f);
				this._dryad.scale = 1f - num;
				if (this._dryad.position.X < this._portal.Right.X)
				{
					NPC expr_FC_cp_0 = this._dryad;
					expr_FC_cp_0.velocity.X = expr_FC_cp_0.velocity.X * 0.95f;
					NPC expr_118_cp_0 = this._dryad;
					expr_118_cp_0.velocity.Y = expr_118_cp_0.velocity.Y * 0.55f;
				}
				int num2 = (int)(6f * num);
				float num3 = this._dryad.Size.Length() / 2f;
				num3 /= 20f;
				for (int i = 0; i < num2; i++)
				{
					if (Main.rand.Next(5) == 0)
					{
						Dust dust = Dust.NewDustDirect(this._dryad.position, this._dryad.width, this._dryad.height, 27, this._dryad.velocity.X * 1f, 0f, 100, default(Color), 1f);
						dust.scale = 0.55f;
						dust.fadeIn = 0.7f;
						dust.velocity *= 0.1f * num3;
						dust.velocity += this._dryad.velocity;
					}
				}
			}
		}

		private void CreatePortal(FrameEventData evt)
		{
			this._portal = this.PlaceNPCOnGround(549, this._startPoint + new Vector2(-240f, 0f));
			this._portal.immortal = true;
		}

		private void DryadStand(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				this._dryad.ai[0] = 0f;
				this._dryad.ai[1] = (float)evt.Remaining;
			}
		}

		private void DryadLookRight(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				this._dryad.direction = 1;
				this._dryad.spriteDirection = 1;
			}
		}

		private void DryadLookLeft(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				this._dryad.direction = -1;
				this._dryad.spriteDirection = -1;
			}
		}

		private void DryadWalk(FrameEventData evt)
		{
			this._dryad.ai[0] = 1f;
			this._dryad.ai[1] = 2f;
		}

		private void DryadConfusedEmote(FrameEventData evt)
		{
			if (this._dryad != null && evt.IsFirstFrame)
			{
				EmoteBubble.NewBubble(87, new WorldUIAnchor(this._dryad), evt.Duration);
			}
		}

		private void DryadAlertEmote(FrameEventData evt)
		{
			if (this._dryad != null && evt.IsFirstFrame)
			{
				EmoteBubble.NewBubble(3, new WorldUIAnchor(this._dryad), evt.Duration);
			}
		}

		private void CreateOgre(FrameEventData evt)
		{
			int num = NPC.NewNPC((int)this._portal.Center.X, (int)this._portal.Bottom.Y, 576, 0, 0f, 0f, 0f, 0f, 255);
			this._ogre = Main.npc[num];
			this._ogre.knockBackResist = 0f;
			this._ogre.immortal = true;
			this._ogre.dontTakeDamage = true;
			this._ogre.takenDamageMultiplier = 0f;
			this._ogre.immune[255] = 100000;
		}

		private void OgreStand(FrameEventData evt)
		{
			if (this._ogre != null)
			{
				this._ogre.ai[0] = 0f;
				this._ogre.ai[1] = 0f;
				this._ogre.velocity = Vector2.Zero;
			}
		}

		private void DryadAttack(FrameEventData evt)
		{
			if (this._dryad != null)
			{
				this._dryad.ai[0] = 14f;
				this._dryad.ai[1] = (float)evt.Remaining;
				this._dryad.dryadWard = false;
			}
		}

		private void OgreLookRight(FrameEventData evt)
		{
			if (this._ogre != null)
			{
				this._ogre.direction = 1;
				this._ogre.spriteDirection = 1;
			}
		}

		private void OgreLookLeft(FrameEventData evt)
		{
			if (this._ogre != null)
			{
				this._ogre.direction = -1;
				this._ogre.spriteDirection = -1;
			}
		}

		public override void OnBegin()
		{
			Main.NewText("DD2Film: Begin", 255, 255, 255, false);
			Main.dayTime = true;
			Main.time = 27000.0;
			this._startPoint = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY - 32f);
			base.OnBegin();
		}

		private NPC PlaceNPCOnGround(int type, Vector2 position)
		{
			int num = (int)position.X;
			int num2 = (int)position.Y;
			int i = num / 16;
			int num3 = num2 / 16;
			while (!WorldGen.SolidTile(i, num3))
			{
				num3++;
			}
			num2 = num3 * 16;
			int start = 100;
			if (type == 20)
			{
				start = 1;
			}
			else if (type == 576)
			{
				start = 50;
			}
			int num4 = NPC.NewNPC(num, num2, type, start, 0f, 0f, 0f, 0f, 255);
			return Main.npc[num4];
		}

		public override void OnEnd()
		{
			if (this._dryad != null)
			{
				this._dryad.active = false;
			}
			if (this._portal != null)
			{
				this._portal.active = false;
			}
			if (this._ogre != null)
			{
				this._ogre.active = false;
			}
			foreach (NPC current in this._critters)
			{
				current.active = false;
			}
			foreach (NPC current2 in this._army)
			{
				current2.active = false;
			}
			Main.NewText("DD2Film: End", 255, 255, 255, false);
			base.OnEnd();
		}
	}
}
