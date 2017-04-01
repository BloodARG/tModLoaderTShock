using Ionic.Zlib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Net.Sockets;
using Terraria.Social;

namespace Terraria
{
	public class NetMessage
	{
		public static MessageBuffer[] buffer = new MessageBuffer[257];

		private static PlayerDeathReason _currentPlayerDeathReason;

		public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
		{
			if (Main.netMode == 0)
			{
				return;
			}
			int num = 256;
			if (Main.netMode == 2 && remoteClient >= 0)
			{
				num = remoteClient;
			}
			lock (NetMessage.buffer[num])
			{
				BinaryWriter writer = NetMessage.buffer[num].writer;
				if (writer == null)
				{
					NetMessage.buffer[num].ResetWriter();
					writer = NetMessage.buffer[num].writer;
				}
				writer.BaseStream.Position = 0L;
				long position = writer.BaseStream.Position;
				writer.BaseStream.Position += 2L;
				writer.Write((byte)msgType);
				switch (msgType)
				{
				case 1:
					writer.Write("Terraria" + 188);
					break;
				case 2:
					writer.Write(text);
					if (Main.dedServ)
					{
						Console.WriteLine(Language.GetTextValue("CLI.ClientWasBooted", Netplay.Clients[num].Socket.GetRemoteAddress().ToString(), text));
					}
					break;
				case 3:
					writer.Write((byte)remoteClient);
					break;
				case 4:
				{
					Player player = Main.player[number];
					writer.Write((byte)number);
					writer.Write((byte)player.skinVariant);
					writer.Write((byte)player.hair);
					writer.Write(text);
					writer.Write(player.hairDye);
					BitsByte bb = 0;
					for (int i = 0; i < 8; i++)
					{
						bb[i] = player.hideVisual[i];
					}
					writer.Write(bb);
					bb = 0;
					for (int j = 0; j < 2; j++)
					{
						bb[j] = player.hideVisual[j + 8];
					}
					writer.Write(bb);
					writer.Write(player.hideMisc);
					writer.WriteRGB(player.hairColor);
					writer.WriteRGB(player.skinColor);
					writer.WriteRGB(player.eyeColor);
					writer.WriteRGB(player.shirtColor);
					writer.WriteRGB(player.underShirtColor);
					writer.WriteRGB(player.pantsColor);
					writer.WriteRGB(player.shoeColor);
					BitsByte bb2 = 0;
					if (player.difficulty == 1)
					{
						bb2[0] = true;
					}
					else if (player.difficulty == 2)
					{
						bb2[1] = true;
					}
					bb2[2] = player.extraAccessory;
					writer.Write(bb2);
					break;
				}
				case 5:
				{
					writer.Write((byte)number);
					writer.Write((byte)number2);
					Player player2 = Main.player[number];
					Item item;
					if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length + player2.bank.item.Length + player2.bank2.item.Length + 1))
					{
						item = player2.bank3.item[(int)number2 - 58 - (player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length + player2.bank.item.Length + player2.bank2.item.Length + 1) - 1];
					}
					else if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length + player2.bank.item.Length + player2.bank2.item.Length))
					{
						item = player2.trashItem;
					}
					else if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length + player2.bank.item.Length))
					{
						item = player2.bank2.item[(int)number2 - 58 - (player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length + player2.bank.item.Length) - 1];
					}
					else if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length))
					{
						item = player2.bank.item[(int)number2 - 58 - (player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length) - 1];
					}
					else if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length))
					{
						item = player2.miscDyes[(int)number2 - 58 - (player2.armor.Length + player2.dye.Length + player2.miscEquips.Length) - 1];
					}
					else if (number2 > (float)(58 + player2.armor.Length + player2.dye.Length))
					{
						item = player2.miscEquips[(int)number2 - 58 - (player2.armor.Length + player2.dye.Length) - 1];
					}
					else if (number2 > (float)(58 + player2.armor.Length))
					{
						item = player2.dye[(int)number2 - 58 - player2.armor.Length - 1];
					}
					else if (number2 > 58f)
					{
						item = player2.armor[(int)number2 - 58 - 1];
					}
					else
					{
						item = player2.inventory[(int)number2];
					}
					if (item.name == "" || item.stack == 0 || item.type == 0)
					{
						item.SetDefaults(0, false);
					}
					int num2 = item.stack;
					int netID = item.netID;
					if (num2 < 0)
					{
						num2 = 0;
					}
					writer.Write((short)num2);
					writer.Write((byte)number3);
					writer.Write((short)netID);
					break;
				}
				case 7:
				{
					writer.Write((int)Main.time);
					BitsByte bb3 = 0;
					bb3[0] = Main.dayTime;
					bb3[1] = Main.bloodMoon;
					bb3[2] = Main.eclipse;
					writer.Write(bb3);
					writer.Write((byte)Main.moonPhase);
					writer.Write((short)Main.maxTilesX);
					writer.Write((short)Main.maxTilesY);
					writer.Write((short)Main.spawnTileX);
					writer.Write((short)Main.spawnTileY);
					writer.Write((short)Main.worldSurface);
					writer.Write((short)Main.rockLayer);
					writer.Write(Main.worldID);
					writer.Write(Main.worldName);
					writer.Write(Main.ActiveWorldFileData.UniqueId.ToString());
					writer.Write((byte)Main.moonType);
					writer.Write((byte)WorldGen.treeBG);
					writer.Write((byte)WorldGen.corruptBG);
					writer.Write((byte)WorldGen.jungleBG);
					writer.Write((byte)WorldGen.snowBG);
					writer.Write((byte)WorldGen.hallowBG);
					writer.Write((byte)WorldGen.crimsonBG);
					writer.Write((byte)WorldGen.desertBG);
					writer.Write((byte)WorldGen.oceanBG);
					writer.Write((byte)Main.iceBackStyle);
					writer.Write((byte)Main.jungleBackStyle);
					writer.Write((byte)Main.hellBackStyle);
					writer.Write(Main.windSpeedSet);
					writer.Write((byte)Main.numClouds);
					for (int k = 0; k < 3; k++)
					{
						writer.Write(Main.treeX[k]);
					}
					for (int l = 0; l < 4; l++)
					{
						writer.Write((byte)Main.treeStyle[l]);
					}
					for (int m = 0; m < 3; m++)
					{
						writer.Write(Main.caveBackX[m]);
					}
					for (int n = 0; n < 4; n++)
					{
						writer.Write((byte)Main.caveBackStyle[n]);
					}
					if (!Main.raining)
					{
						Main.maxRaining = 0f;
					}
					writer.Write(Main.maxRaining);
					BitsByte bb4 = 0;
					bb4[0] = WorldGen.shadowOrbSmashed;
					bb4[1] = NPC.downedBoss1;
					bb4[2] = NPC.downedBoss2;
					bb4[3] = NPC.downedBoss3;
					bb4[4] = Main.hardMode;
					bb4[5] = NPC.downedClown;
					bb4[7] = NPC.downedPlantBoss;
					writer.Write(bb4);
					BitsByte bb5 = 0;
					bb5[0] = NPC.downedMechBoss1;
					bb5[1] = NPC.downedMechBoss2;
					bb5[2] = NPC.downedMechBoss3;
					bb5[3] = NPC.downedMechBossAny;
					bb5[4] = (Main.cloudBGActive >= 1f);
					bb5[5] = WorldGen.crimson;
					bb5[6] = Main.pumpkinMoon;
					bb5[7] = Main.snowMoon;
					writer.Write(bb5);
					BitsByte bb6 = 0;
					bb6[0] = Main.expertMode;
					bb6[1] = Main.fastForwardTime;
					bb6[2] = Main.slimeRain;
					bb6[3] = NPC.downedSlimeKing;
					bb6[4] = NPC.downedQueenBee;
					bb6[5] = NPC.downedFishron;
					bb6[6] = NPC.downedMartians;
					bb6[7] = NPC.downedAncientCultist;
					writer.Write(bb6);
					BitsByte bb7 = 0;
					bb7[0] = NPC.downedMoonlord;
					bb7[1] = NPC.downedHalloweenKing;
					bb7[2] = NPC.downedHalloweenTree;
					bb7[3] = NPC.downedChristmasIceQueen;
					bb7[4] = NPC.downedChristmasSantank;
					bb7[5] = NPC.downedChristmasTree;
					bb7[6] = NPC.downedGolemBoss;
					bb7[7] = BirthdayParty.PartyIsUp;
					writer.Write(bb7);
					BitsByte bb8 = 0;
					bb8[0] = NPC.downedPirates;
					bb8[1] = NPC.downedFrost;
					bb8[2] = NPC.downedGoblins;
					bb8[3] = Sandstorm.Happening;
					bb8[4] = DD2Event.Ongoing;
					bb8[5] = DD2Event.DownedInvasionT1;
					bb8[6] = DD2Event.DownedInvasionT2;
					bb8[7] = DD2Event.DownedInvasionT3;
					writer.Write(bb8);
					writer.Write((sbyte)Main.invasionType);
					if (SocialAPI.Network != null)
					{
						writer.Write(SocialAPI.Network.GetLobbyId());
					}
					else
					{
						writer.Write(0uL);
					}
					writer.Write(Sandstorm.IntendedSeverity);
					break;
				}
				case 8:
					writer.Write(number);
					writer.Write((int)number2);
					break;
				case 9:
					writer.Write(number);
					writer.Write(text);
					break;
				case 10:
				{
					int num3 = NetMessage.CompressTileBlock(number, (int)number2, (short)number3, (short)number4, NetMessage.buffer[num].writeBuffer, (int)writer.BaseStream.Position);
					writer.BaseStream.Position += (long)num3;
					break;
				}
				case 11:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					break;
				case 12:
					writer.Write((byte)number);
					writer.Write((short)Main.player[number].SpawnX);
					writer.Write((short)Main.player[number].SpawnY);
					break;
				case 13:
				{
					Player player3 = Main.player[number];
					writer.Write((byte)number);
					BitsByte bb9 = 0;
					bb9[0] = player3.controlUp;
					bb9[1] = player3.controlDown;
					bb9[2] = player3.controlLeft;
					bb9[3] = player3.controlRight;
					bb9[4] = player3.controlJump;
					bb9[5] = player3.controlUseItem;
					bb9[6] = (player3.direction == 1);
					writer.Write(bb9);
					BitsByte bb10 = 0;
					bb10[0] = player3.pulley;
					bb10[1] = (player3.pulley && player3.pulleyDir == 2);
					bb10[2] = (player3.velocity != Vector2.Zero);
					bb10[3] = player3.vortexStealthActive;
					bb10[4] = (player3.gravDir == 1f);
					bb10[5] = player3.shieldRaised;
					writer.Write(bb10);
					writer.Write((byte)player3.selectedItem);
					writer.WriteVector2(player3.position);
					if (bb10[2])
					{
						writer.WriteVector2(player3.velocity);
					}
					break;
				}
				case 14:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					break;
				case 16:
					writer.Write((byte)number);
					writer.Write((short)Main.player[number].statLife);
					writer.Write((short)Main.player[number].statLifeMax);
					break;
				case 17:
					writer.Write((byte)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					writer.Write((byte)number5);
					break;
				case 18:
					writer.Write(Main.dayTime ? 1 : 0);
					writer.Write((int)Main.time);
					writer.Write(Main.sunModY);
					writer.Write(Main.moonModY);
					break;
				case 19:
					writer.Write((byte)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((number4 == 1f) ? 1 : 0);
					break;
				case 20:
				{
					int num4 = number;
					int num5 = (int)number2;
					int num6 = (int)number3;
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (num5 < num4)
					{
						num5 = num4;
					}
					if (num5 >= Main.maxTilesX + num4)
					{
						num5 = Main.maxTilesX - num4 - 1;
					}
					if (num6 < num4)
					{
						num6 = num4;
					}
					if (num6 >= Main.maxTilesY + num4)
					{
						num6 = Main.maxTilesY - num4 - 1;
					}
					if (number5 == 0)
					{
						writer.Write((ushort)(num4 & 32767));
					}
					else
					{
						writer.Write((ushort)((num4 & 32767) | 32768));
						writer.Write((byte)number5);
					}
					writer.Write((short)num5);
					writer.Write((short)num6);
					for (int num7 = num5; num7 < num5 + num4; num7++)
					{
						for (int num8 = num6; num8 < num6 + num4; num8++)
						{
							BitsByte bb11 = 0;
							BitsByte bb12 = 0;
							byte b = 0;
							byte b2 = 0;
							Tile tile = Main.tile[num7, num8];
							bb11[0] = tile.active();
							bb11[2] = (tile.wall > 0);
							bb11[3] = (tile.liquid > 0 && Main.netMode == 2);
							bb11[4] = tile.wire();
							bb11[5] = tile.halfBrick();
							bb11[6] = tile.actuator();
							bb11[7] = tile.inActive();
							bb12[0] = tile.wire2();
							bb12[1] = tile.wire3();
							if (tile.active() && tile.color() > 0)
							{
								bb12[2] = true;
								b = tile.color();
							}
							if (tile.wall > 0 && tile.wallColor() > 0)
							{
								bb12[3] = true;
								b2 = tile.wallColor();
							}
							bb12 += (byte)(tile.slope() << 4);
							bb12[7] = tile.wire4();
							writer.Write(bb11);
							writer.Write(bb12);
							if (b > 0)
							{
								writer.Write(b);
							}
							if (b2 > 0)
							{
								writer.Write(b2);
							}
							if (tile.active())
							{
								writer.Write(tile.type);
								if (Main.tileFrameImportant[(int)tile.type])
								{
									writer.Write(tile.frameX);
									writer.Write(tile.frameY);
								}
							}
							if (tile.wall > 0)
							{
								writer.Write(tile.wall);
							}
							if (tile.liquid > 0 && Main.netMode == 2)
							{
								writer.Write(tile.liquid);
								writer.Write(tile.liquidType());
							}
						}
					}
					break;
				}
				case 21:
				case 90:
				{
					Item item2 = Main.item[number];
					writer.Write((short)number);
					writer.WriteVector2(item2.position);
					writer.WriteVector2(item2.velocity);
					writer.Write((short)item2.stack);
					writer.Write(item2.prefix);
					writer.Write((byte)number2);
					short value = 0;
					if (item2.active && item2.stack > 0)
					{
						value = (short)item2.netID;
					}
					writer.Write(value);
					break;
				}
				case 22:
					writer.Write((short)number);
					writer.Write((byte)Main.item[number].owner);
					break;
				case 23:
				{
					NPC nPC = Main.npc[number];
					writer.Write((short)number);
					writer.WriteVector2(nPC.position);
					writer.WriteVector2(nPC.velocity);
					writer.Write((ushort)nPC.target);
					int num9 = nPC.life;
					if (!nPC.active)
					{
						num9 = 0;
					}
					if (!nPC.active || nPC.life <= 0)
					{
						nPC.netSkip = 0;
					}
					if (nPC.name == null)
					{
						nPC.name = "";
					}
					short value2 = (short)nPC.netID;
					bool[] array = new bool[4];
					BitsByte bb13 = 0;
					bb13[0] = (nPC.direction > 0);
					bb13[1] = (nPC.directionY > 0);
					bb13[2] = (array[0] = (nPC.ai[0] != 0f));
					bb13[3] = (array[1] = (nPC.ai[1] != 0f));
					bb13[4] = (array[2] = (nPC.ai[2] != 0f));
					bb13[5] = (array[3] = (nPC.ai[3] != 0f));
					bb13[6] = (nPC.spriteDirection > 0);
					bb13[7] = (num9 == nPC.lifeMax);
					writer.Write(bb13);
					for (int num10 = 0; num10 < NPC.maxAI; num10++)
					{
						if (array[num10])
						{
							writer.Write(nPC.ai[num10]);
						}
					}
					writer.Write(value2);
					if (!bb13[7])
					{
						byte b3 = Main.npcLifeBytes[nPC.netID];
						writer.Write(b3);
						if (b3 == 2)
						{
							writer.Write((short)num9);
						}
						else if (b3 == 4)
						{
							writer.Write(num9);
						}
						else
						{
							writer.Write((sbyte)num9);
						}
					}
					if (nPC.type >= 0 && nPC.type < 580 && Main.npcCatchable[nPC.type])
					{
						writer.Write((byte)nPC.releaseOwner);
					}
					break;
				}
				case 24:
					writer.Write((short)number);
					writer.Write((byte)number2);
					break;
				case 25:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					writer.Write((byte)number3);
					writer.Write((byte)number4);
					writer.Write(text);
					break;
				case 26:
				{
					writer.Write((byte)number);
					writer.Write((byte)(number2 + 1f));
					writer.Write((short)number3);
					writer.Write(text);
					BitsByte bb14 = 0;
					bb14[0] = (number4 == 1f);
					bb14[1] = (number5 == 1);
					bb14[2] = (number6 == 0);
					bb14[3] = (number6 == 1);
					bb14[4] = (number6 == 2);
					writer.Write(bb14);
					break;
				}
				case 27:
				{
					Projectile projectile = Main.projectile[number];
					writer.Write((short)projectile.identity);
					writer.WriteVector2(projectile.position);
					writer.WriteVector2(projectile.velocity);
					writer.Write(projectile.knockBack);
					writer.Write((short)projectile.damage);
					writer.Write((byte)projectile.owner);
					writer.Write((short)projectile.type);
					BitsByte bb15 = 0;
					for (int num11 = 0; num11 < Projectile.maxAI; num11++)
					{
						if (projectile.ai[num11] != 0f)
						{
							bb15[num11] = true;
						}
					}
					if (projectile.type > 0 && projectile.type < 714 && ProjectileID.Sets.NeedsUUID[projectile.type])
					{
						bb15[Projectile.maxAI] = true;
					}
					writer.Write(bb15);
					for (int num12 = 0; num12 < Projectile.maxAI; num12++)
					{
						if (bb15[num12])
						{
							writer.Write(projectile.ai[num12]);
						}
					}
					if (bb15[Projectile.maxAI])
					{
						writer.Write((short)projectile.projUUID);
					}
					break;
				}
				case 28:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write(number3);
					writer.Write((byte)(number4 + 1f));
					writer.Write((byte)number5);
					break;
				case 29:
					writer.Write((short)number);
					writer.Write((byte)number2);
					break;
				case 30:
					writer.Write((byte)number);
					writer.Write(Main.player[number].hostile);
					break;
				case 31:
					writer.Write((short)number);
					writer.Write((short)number2);
					break;
				case 32:
				{
					Item item3 = Main.chest[number].item[(int)((byte)number2)];
					writer.Write((short)number);
					writer.Write((byte)number2);
					short value3 = (short)item3.netID;
					if (item3.name == null)
					{
						value3 = 0;
					}
					writer.Write((short)item3.stack);
					writer.Write(item3.prefix);
					writer.Write(value3);
					break;
				}
				case 33:
				{
					int num13 = 0;
					int num14 = 0;
					int num15 = 0;
					string text2 = null;
					if (number > -1)
					{
						num13 = Main.chest[number].x;
						num14 = Main.chest[number].y;
					}
					if (number2 == 1f)
					{
						num15 = (int)((byte)text.Length);
						if (num15 == 0 || num15 > 20)
						{
							num15 = 255;
						}
						else
						{
							text2 = text;
						}
					}
					writer.Write((short)number);
					writer.Write((short)num13);
					writer.Write((short)num14);
					writer.Write((byte)num15);
					if (text2 != null)
					{
						writer.Write(text2);
					}
					break;
				}
				case 34:
					writer.Write((byte)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					if (Main.netMode == 2)
					{
						Netplay.GetSectionX((int)number2);
						Netplay.GetSectionY((int)number3);
						writer.Write((short)number5);
					}
					else
					{
						writer.Write(0);
					}
					break;
				case 35:
				case 66:
					writer.Write((byte)number);
					writer.Write((short)number2);
					break;
				case 36:
				{
					Player player4 = Main.player[number];
					writer.Write((byte)number);
					writer.Write(player4.zone1);
					writer.Write(player4.zone2);
					writer.Write(player4.zone3);
					writer.Write(player4.zone4);
					break;
				}
				case 38:
					writer.Write(text);
					break;
				case 39:
					writer.Write((short)number);
					break;
				case 40:
					writer.Write((byte)number);
					writer.Write((short)Main.player[number].talkNPC);
					break;
				case 41:
					writer.Write((byte)number);
					writer.Write(Main.player[number].itemRotation);
					writer.Write((short)Main.player[number].itemAnimation);
					break;
				case 42:
					writer.Write((byte)number);
					writer.Write((short)Main.player[number].statMana);
					writer.Write((short)Main.player[number].statManaMax);
					break;
				case 43:
					writer.Write((byte)number);
					writer.Write((short)number2);
					break;
				case 44:
					writer.Write((byte)number);
					writer.Write((byte)(number2 + 1f));
					writer.Write((short)number3);
					writer.Write((byte)number4);
					writer.Write(text);
					break;
				case 45:
					writer.Write((byte)number);
					writer.Write((byte)Main.player[number].team);
					break;
				case 46:
					writer.Write((short)number);
					writer.Write((short)number2);
					break;
				case 47:
					writer.Write((short)number);
					writer.Write((short)Main.sign[number].x);
					writer.Write((short)Main.sign[number].y);
					writer.Write(Main.sign[number].text);
					writer.Write((byte)number2);
					break;
				case 48:
				{
					Tile tile2 = Main.tile[number, (int)number2];
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write(tile2.liquid);
					writer.Write(tile2.liquidType());
					break;
				}
				case 50:
					writer.Write((byte)number);
					for (int num16 = 0; num16 < 22; num16++)
					{
						writer.Write((byte)Main.player[number].buffType[num16]);
					}
					break;
				case 51:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					break;
				case 52:
					writer.Write((byte)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					break;
				case 53:
					writer.Write((short)number);
					writer.Write((byte)number2);
					writer.Write((short)number3);
					break;
				case 54:
					writer.Write((short)number);
					for (int num17 = 0; num17 < 5; num17++)
					{
						writer.Write((byte)Main.npc[number].buffType[num17]);
						writer.Write((short)Main.npc[number].buffTime[num17]);
					}
					break;
				case 55:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					writer.Write((int)number3);
					break;
				case 56:
				{
					string value4 = null;
					if (Main.netMode == 2)
					{
						value4 = Main.npc[number].displayName;
					}
					else if (Main.netMode == 1)
					{
						value4 = text;
					}
					writer.Write((short)number);
					writer.Write(value4);
					break;
				}
				case 57:
					writer.Write(WorldGen.tGood);
					writer.Write(WorldGen.tEvil);
					writer.Write(WorldGen.tBlood);
					break;
				case 58:
					writer.Write((byte)number);
					writer.Write(number2);
					break;
				case 59:
					writer.Write((short)number);
					writer.Write((short)number2);
					break;
				case 60:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((byte)number4);
					break;
				case 61:
					writer.Write((short)number);
					writer.Write((short)number2);
					break;
				case 62:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					break;
				case 63:
				case 64:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((byte)number3);
					break;
				case 65:
				{
					BitsByte bb16 = 0;
					bb16[0] = ((number & 1) == 1);
					bb16[1] = ((number & 2) == 2);
					bb16[2] = ((number5 & 1) == 1);
					bb16[3] = ((number5 & 2) == 2);
					writer.Write(bb16);
					writer.Write((short)number2);
					writer.Write(number3);
					writer.Write(number4);
					break;
				}
				case 68:
					writer.Write(Main.clientUUID);
					break;
				case 69:
					Netplay.GetSectionX((int)number2);
					Netplay.GetSectionY((int)number3);
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write(text);
					break;
				case 70:
					writer.Write((short)number);
					writer.Write((byte)number2);
					break;
				case 71:
					writer.Write(number);
					writer.Write((int)number2);
					writer.Write((short)number3);
					writer.Write((byte)number4);
					break;
				case 72:
					for (int num18 = 0; num18 < 40; num18++)
					{
						writer.Write((short)Main.travelShop[num18]);
					}
					break;
				case 74:
				{
					writer.Write((byte)Main.anglerQuest);
					bool value5 = Main.anglerWhoFinishedToday.Contains(text);
					writer.Write(value5);
					break;
				}
				case 76:
					writer.Write((byte)number);
					writer.Write(Main.player[number].anglerQuestsFinished);
					break;
				case 77:
					if (Main.netMode != 2)
					{
						return;
					}
					writer.Write((short)number);
					writer.Write((ushort)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					break;
				case 78:
					writer.Write(number);
					writer.Write((int)number2);
					writer.Write((sbyte)number3);
					writer.Write((sbyte)number4);
					break;
				case 79:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					writer.Write((byte)number5);
					writer.Write((sbyte)number6);
					writer.Write(number7 == 1);
					break;
				case 80:
					writer.Write((byte)number);
					writer.Write((short)number2);
					break;
				case 81:
					writer.Write(number2);
					writer.Write(number3);
					writer.WriteRGB(new Color
					{
						PackedValue = (uint)number
					});
					writer.Write(text);
					break;
				case 83:
				{
					int num19 = number;
					if (num19 < 0 && num19 >= 267)
					{
						num19 = 1;
					}
					int value6 = NPC.killCount[num19];
					writer.Write((short)num19);
					writer.Write(value6);
					break;
				}
				case 84:
				{
					byte b4 = (byte)number;
					float stealth = Main.player[(int)b4].stealth;
					writer.Write(b4);
					writer.Write(stealth);
					break;
				}
				case 85:
				{
					byte value7 = (byte)number;
					writer.Write(value7);
					break;
				}
				case 86:
				{
					writer.Write(number);
					bool flag2 = TileEntity.ByID.ContainsKey(number);
					writer.Write(flag2);
					if (flag2)
					{
						TileEntity.Write(writer, TileEntity.ByID[number], true);
					}
					break;
				}
				case 87:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((byte)number3);
					break;
				case 88:
				{
					BitsByte bb17 = (byte)number2;
					BitsByte bb18 = (byte)number3;
					writer.Write((short)number);
					writer.Write(bb17);
					Item item4 = Main.item[number];
					if (bb17[0])
					{
						writer.Write(item4.color.PackedValue);
					}
					if (bb17[1])
					{
						writer.Write((ushort)item4.damage);
					}
					if (bb17[2])
					{
						writer.Write(item4.knockBack);
					}
					if (bb17[3])
					{
						writer.Write((ushort)item4.useAnimation);
					}
					if (bb17[4])
					{
						writer.Write((ushort)item4.useTime);
					}
					if (bb17[5])
					{
						writer.Write((short)item4.shoot);
					}
					if (bb17[6])
					{
						writer.Write(item4.shootSpeed);
					}
					if (bb17[7])
					{
						writer.Write(bb18);
						if (bb18[0])
						{
							writer.Write((ushort)item4.width);
						}
						if (bb18[1])
						{
							writer.Write((ushort)item4.height);
						}
						if (bb18[2])
						{
							writer.Write(item4.scale);
						}
						if (bb18[3])
						{
							writer.Write((short)item4.ammo);
						}
						if (bb18[4])
						{
							writer.Write((short)item4.useAmmo);
						}
						if (bb18[5])
						{
							writer.Write(item4.notAmmo);
						}
					}
					break;
				}
				case 89:
				{
					writer.Write((short)number);
					writer.Write((short)number2);
					Item item5 = Main.player[(int)number4].inventory[(int)number3];
					writer.Write((short)item5.netID);
					writer.Write(item5.prefix);
					writer.Write((short)item5.stack);
					break;
				}
				case 91:
					writer.Write(number);
					writer.Write((byte)number2);
					if (number2 != 255f)
					{
						writer.Write((ushort)number3);
						writer.Write((byte)number4);
						writer.Write((byte)number5);
						if (number5 < 0)
						{
							writer.Write((short)number6);
						}
					}
					break;
				case 92:
					writer.Write((short)number);
					writer.Write(number2);
					writer.Write(number3);
					writer.Write(number4);
					break;
				case 95:
					writer.Write((ushort)number);
					break;
				case 96:
				{
					writer.Write((byte)number);
					Player player5 = Main.player[number];
					writer.Write((short)number4);
					writer.Write(number2);
					writer.Write(number3);
					writer.WriteVector2(player5.velocity);
					break;
				}
				case 97:
					writer.Write((short)number);
					break;
				case 98:
					writer.Write((short)number);
					break;
				case 99:
					writer.Write((byte)number);
					writer.WriteVector2(Main.player[number].MinionRestTargetPoint);
					break;
				case 100:
				{
					writer.Write((ushort)number);
					NPC nPC2 = Main.npc[number];
					writer.Write((short)number4);
					writer.Write(number2);
					writer.Write(number3);
					writer.WriteVector2(nPC2.velocity);
					break;
				}
				case 101:
					writer.Write((ushort)NPC.ShieldStrengthTowerSolar);
					writer.Write((ushort)NPC.ShieldStrengthTowerVortex);
					writer.Write((ushort)NPC.ShieldStrengthTowerNebula);
					writer.Write((ushort)NPC.ShieldStrengthTowerStardust);
					break;
				case 102:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					writer.Write(number3);
					writer.Write(number4);
					break;
				case 103:
					writer.Write(NPC.MoonLordCountdown);
					break;
				case 104:
					writer.Write((byte)number);
					writer.Write((short)number2);
					writer.Write(((short)number3 < 0) ? 0f : number3);
					writer.Write((byte)number4);
					writer.Write(number5);
					writer.Write((byte)number6);
					break;
				case 105:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write(number3 == 1f);
					break;
				case 106:
				{
					HalfVector2 halfVector = new HalfVector2((float)number, number2);
					writer.Write(halfVector.PackedValue);
					break;
				}
				case 107:
					writer.Write((byte)number);
					writer.Write((byte)number2);
					writer.Write((byte)number3);
					writer.Write((byte)number4);
					writer.Write(text);
					writer.Write((short)number5);
					break;
				case 108:
					writer.Write((short)number);
					writer.Write(number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					writer.Write((short)number5);
					writer.Write((short)number6);
					writer.Write((byte)number7);
					break;
				case 109:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((short)number4);
					writer.Write((byte)number5);
					break;
				case 110:
					writer.Write((short)number);
					writer.Write((short)number2);
					writer.Write((byte)number3);
					break;
				case 112:
					writer.Write((byte)number);
					writer.Write((short)number2);
					writer.Write((short)number3);
					writer.Write((byte)number4);
					writer.Write((short)number5);
					break;
				case 113:
					writer.Write((short)number);
					writer.Write((short)number2);
					break;
				case 115:
					writer.Write((byte)number);
					writer.Write((short)Main.player[number].MinionAttackTargetNPC);
					break;
				case 116:
					writer.Write(number);
					break;
				case 117:
					writer.Write((byte)number);
					NetMessage._currentPlayerDeathReason.WriteSelfTo(writer);
					writer.Write((short)number2);
					writer.Write((byte)(number3 + 1f));
					writer.Write((byte)number4);
					writer.Write((sbyte)number5);
					break;
				case 118:
					writer.Write((byte)number);
					NetMessage._currentPlayerDeathReason.WriteSelfTo(writer);
					writer.Write((short)number2);
					writer.Write((byte)(number3 + 1f));
					writer.Write((byte)number4);
					break;
				}
				int num20 = (int)writer.BaseStream.Position;
				writer.BaseStream.Position = position;
				writer.Write((short)num20);
				writer.BaseStream.Position = (long)num20;
				if (Main.netMode == 1)
				{
					if (!Netplay.Connection.Socket.IsConnected())
					{
						goto IL_30DF;
					}
					try
					{
						NetMessage.buffer[num].spamCount++;
						Main.txMsg++;
						Main.txData += num20;
						Main.txMsgType[msgType]++;
						Main.txDataType[msgType] += num20;
						Netplay.Connection.Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Connection.ClientWriteCallBack), null);
						goto IL_30DF;
					}
					catch
					{
						goto IL_30DF;
					}
				}
				if (remoteClient == -1)
				{
					if (msgType == 34 || msgType == 69)
					{
						for (int num21 = 0; num21 < 256; num21++)
						{
							if (num21 != ignoreClient && NetMessage.buffer[num21].broadcast && Netplay.Clients[num21].IsConnected())
							{
								try
								{
									NetMessage.buffer[num21].spamCount++;
									Main.txMsg++;
									Main.txData += num20;
									Main.txMsgType[msgType]++;
									Main.txDataType[msgType] += num20;
									Netplay.Clients[num21].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num21].ServerWriteCallBack), null);
								}
								catch
								{
								}
							}
						}
					}
					else if (msgType == 20)
					{
						for (int num22 = 0; num22 < 256; num22++)
						{
							if (num22 != ignoreClient && NetMessage.buffer[num22].broadcast && Netplay.Clients[num22].IsConnected() && Netplay.Clients[num22].SectionRange(number, (int)number2, (int)number3))
							{
								try
								{
									NetMessage.buffer[num22].spamCount++;
									Main.txMsg++;
									Main.txData += num20;
									Main.txMsgType[msgType]++;
									Main.txDataType[msgType] += num20;
									Netplay.Clients[num22].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num22].ServerWriteCallBack), null);
								}
								catch
								{
								}
							}
						}
					}
					else if (msgType == 23)
					{
						NPC nPC3 = Main.npc[number];
						for (int num23 = 0; num23 < 256; num23++)
						{
							if (num23 != ignoreClient && NetMessage.buffer[num23].broadcast && Netplay.Clients[num23].IsConnected())
							{
								bool flag3 = false;
								if (nPC3.boss || nPC3.netAlways || nPC3.townNPC || !nPC3.active)
								{
									flag3 = true;
								}
								else if (nPC3.netSkip <= 0)
								{
									Rectangle rect = Main.player[num23].getRect();
									Rectangle rect2 = nPC3.getRect();
									rect2.X -= 2500;
									rect2.Y -= 2500;
									rect2.Width += 5000;
									rect2.Height += 5000;
									if (rect.Intersects(rect2))
									{
										flag3 = true;
									}
								}
								else
								{
									flag3 = true;
								}
								if (flag3)
								{
									try
									{
										NetMessage.buffer[num23].spamCount++;
										Main.txMsg++;
										Main.txData += num20;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num20;
										Netplay.Clients[num23].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num23].ServerWriteCallBack), null);
									}
									catch
									{
									}
								}
							}
						}
						nPC3.netSkip++;
						if (nPC3.netSkip > 4)
						{
							nPC3.netSkip = 0;
						}
					}
					else if (msgType == 28)
					{
						NPC nPC4 = Main.npc[number];
						for (int num24 = 0; num24 < 256; num24++)
						{
							if (num24 != ignoreClient && NetMessage.buffer[num24].broadcast && Netplay.Clients[num24].IsConnected())
							{
								bool flag4 = false;
								if (nPC4.life <= 0)
								{
									flag4 = true;
								}
								else
								{
									Rectangle rect3 = Main.player[num24].getRect();
									Rectangle rect4 = nPC4.getRect();
									rect4.X -= 3000;
									rect4.Y -= 3000;
									rect4.Width += 6000;
									rect4.Height += 6000;
									if (rect3.Intersects(rect4))
									{
										flag4 = true;
									}
								}
								if (flag4)
								{
									try
									{
										NetMessage.buffer[num24].spamCount++;
										Main.txMsg++;
										Main.txData += num20;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num20;
										Netplay.Clients[num24].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num24].ServerWriteCallBack), null);
									}
									catch
									{
									}
								}
							}
						}
					}
					else if (msgType == 13)
					{
						for (int num25 = 0; num25 < 256; num25++)
						{
							if (num25 != ignoreClient && NetMessage.buffer[num25].broadcast && Netplay.Clients[num25].IsConnected())
							{
								try
								{
									NetMessage.buffer[num25].spamCount++;
									Main.txMsg++;
									Main.txData += num20;
									Main.txMsgType[msgType]++;
									Main.txDataType[msgType] += num20;
									Netplay.Clients[num25].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num25].ServerWriteCallBack), null);
								}
								catch
								{
								}
							}
						}
						Main.player[number].netSkip++;
						if (Main.player[number].netSkip > 2)
						{
							Main.player[number].netSkip = 0;
						}
					}
					else if (msgType == 27)
					{
						Projectile projectile2 = Main.projectile[number];
						for (int num26 = 0; num26 < 256; num26++)
						{
							if (num26 != ignoreClient && NetMessage.buffer[num26].broadcast && Netplay.Clients[num26].IsConnected())
							{
								bool flag5 = false;
								if (projectile2.type == 12 || Main.projPet[projectile2.type] || projectile2.aiStyle == 11 || projectile2.netImportant)
								{
									flag5 = true;
								}
								else
								{
									Rectangle rect5 = Main.player[num26].getRect();
									Rectangle rect6 = projectile2.getRect();
									rect6.X -= 5000;
									rect6.Y -= 5000;
									rect6.Width += 10000;
									rect6.Height += 10000;
									if (rect5.Intersects(rect6))
									{
										flag5 = true;
									}
								}
								if (flag5)
								{
									try
									{
										NetMessage.buffer[num26].spamCount++;
										Main.txMsg++;
										Main.txData += num20;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num20;
										Netplay.Clients[num26].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num26].ServerWriteCallBack), null);
									}
									catch
									{
									}
								}
							}
						}
					}
					else
					{
						for (int num27 = 0; num27 < 256; num27++)
						{
							if (num27 != ignoreClient && (NetMessage.buffer[num27].broadcast || (Netplay.Clients[num27].State >= 3 && msgType == 10)) && Netplay.Clients[num27].IsConnected())
							{
								try
								{
									NetMessage.buffer[num27].spamCount++;
									Main.txMsg++;
									Main.txData += num20;
									Main.txMsgType[msgType]++;
									Main.txDataType[msgType] += num20;
									Netplay.Clients[num27].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[num27].ServerWriteCallBack), null);
								}
								catch
								{
								}
							}
						}
					}
				}
				else if (Netplay.Clients[remoteClient].IsConnected())
				{
					try
					{
						NetMessage.buffer[remoteClient].spamCount++;
						Main.txMsg++;
						Main.txData += num20;
						Main.txMsgType[msgType]++;
						Main.txDataType[msgType] += num20;
						Netplay.Clients[remoteClient].Socket.AsyncSend(NetMessage.buffer[num].writeBuffer, 0, num20, new SocketSendCallback(Netplay.Clients[remoteClient].ServerWriteCallBack), null);
					}
					catch
					{
					}
				}
				IL_30DF:
				if (Main.verboseNetplay)
				{
					for (int num28 = 0; num28 < num20; num28++)
					{
					}
					for (int num29 = 0; num29 < num20; num29++)
					{
						byte arg_310B_0 = NetMessage.buffer[num].writeBuffer[num29];
					}
				}
				NetMessage.buffer[num].writeLocked = false;
				if (msgType == 19 && Main.netMode == 1)
				{
					NetMessage.SendTileSquare(num, (int)number2, (int)number3, 5, TileChangeType.None);
				}
				if (msgType == 2 && Main.netMode == 2)
				{
					Netplay.Clients[num].PendingTermination = true;
				}
			}
		}

		public static int CompressTileBlock(int xStart, int yStart, short width, short height, byte[] buffer, int bufferStart)
		{
			int result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(xStart);
					binaryWriter.Write(yStart);
					binaryWriter.Write(width);
					binaryWriter.Write(height);
					NetMessage.CompressTileBlock_Inner(binaryWriter, xStart, yStart, (int)width, (int)height);
					int num = buffer.Length;
					if ((long)bufferStart + memoryStream.Length > (long)num)
					{
						result = (int)((long)(num - bufferStart) + memoryStream.Length);
					}
					else
					{
						memoryStream.Position = 0L;
						MemoryStream memoryStream2 = new MemoryStream();
						using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress, true))
						{
							memoryStream.CopyTo(deflateStream);
							deflateStream.Flush();
							deflateStream.Close();
							deflateStream.Dispose();
						}
						if (memoryStream.Length <= memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							buffer[bufferStart] = 0;
							bufferStart++;
							memoryStream.Read(buffer, bufferStart, (int)memoryStream.Length);
							result = (int)memoryStream.Length + 1;
						}
						else
						{
							memoryStream2.Position = 0L;
							buffer[bufferStart] = 1;
							bufferStart++;
							memoryStream2.Read(buffer, bufferStart, (int)memoryStream2.Length);
							result = (int)memoryStream2.Length + 1;
						}
					}
				}
			}
			return result;
		}

		public static void CompressTileBlock_Inner(BinaryWriter writer, int xStart, int yStart, int width, int height)
		{
			short[] array = new short[1000];
			short[] array2 = new short[1000];
			short[] array3 = new short[1000];
			short num = 0;
			short num2 = 0;
			short num3 = 0;
			short num4 = 0;
			int num5 = 0;
			int num6 = 0;
			byte b = 0;
			byte[] array4 = new byte[13];
			Tile tile = null;
			for (int i = yStart; i < yStart + height; i++)
			{
				for (int j = xStart; j < xStart + width; j++)
				{
					Tile tile2 = Main.tile[j, i];
					if (tile2.isTheSameAs(tile))
					{
						num4 += 1;
					}
					else
					{
						if (tile != null)
						{
							if (num4 > 0)
							{
								array4[num5] = (byte)(num4 & 255);
								num5++;
								if (num4 > 255)
								{
									b |= 128;
									array4[num5] = (byte)(((int)num4 & 65280) >> 8);
									num5++;
								}
								else
								{
									b |= 64;
								}
							}
							array4[num6] = b;
							writer.Write(array4, num6, num5 - num6);
							num4 = 0;
						}
						num5 = 3;
						byte b3;
						byte b2 = b = (b3 = 0);
						if (tile2.active())
						{
							b |= 2;
							array4[num5] = (byte)tile2.type;
							num5++;
							if (tile2.type > 255)
							{
								array4[num5] = (byte)(tile2.type >> 8);
								num5++;
								b |= 32;
							}
							if (tile2.type == 21 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
							{
								short num7 = (short)Chest.FindChest(j, i);
								if (num7 != -1)
								{
									array[(int)num] = num7;
									num += 1;
								}
							}
							if (tile2.type == 88 && tile2.frameX % 54 == 0 && tile2.frameY % 36 == 0)
							{
								short num8 = (short)Chest.FindChest(j, i);
								if (num8 != -1)
								{
									array[(int)num] = num8;
									num += 1;
								}
							}
							if (tile2.type == 85 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
							{
								short num9 = (short)Sign.ReadSign(j, i, true);
								if (num9 != -1)
								{
									short[] arg_1FB_0 = array2;
									short expr_1F3 = num2;
									num2 = expr_1F3 + 1;
									arg_1FB_0[(int)expr_1F3] = num9;
								}
							}
							if (tile2.type == 55 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
							{
								short num10 = (short)Sign.ReadSign(j, i, true);
								if (num10 != -1)
								{
									short[] arg_23C_0 = array2;
									short expr_234 = num2;
									num2 = expr_234 + 1;
									arg_23C_0[(int)expr_234] = num10;
								}
							}
							if (tile2.type == 425 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
							{
								short num11 = (short)Sign.ReadSign(j, i, true);
								if (num11 != -1)
								{
									short[] arg_280_0 = array2;
									short expr_278 = num2;
									num2 = expr_278 + 1;
									arg_280_0[(int)expr_278] = num11;
								}
							}
							if (tile2.type == 378 && tile2.frameX % 36 == 0 && tile2.frameY == 0)
							{
								int num12 = TETrainingDummy.Find(j, i);
								if (num12 != -1)
								{
									short[] arg_2C0_0 = array3;
									short expr_2B7 = num3;
									num3 = expr_2B7 + 1;
									arg_2C0_0[(int)expr_2B7] = (short)num12;
								}
							}
							if (tile2.type == 395 && tile2.frameX % 36 == 0 && tile2.frameY == 0)
							{
								int num13 = TEItemFrame.Find(j, i);
								if (num13 != -1)
								{
									short[] arg_300_0 = array3;
									short expr_2F7 = num3;
									num3 = expr_2F7 + 1;
									arg_300_0[(int)expr_2F7] = (short)num13;
								}
							}
							if (Main.tileFrameImportant[(int)tile2.type])
							{
								array4[num5] = (byte)(tile2.frameX & 255);
								num5++;
								array4[num5] = (byte)(((int)tile2.frameX & 65280) >> 8);
								num5++;
								array4[num5] = (byte)(tile2.frameY & 255);
								num5++;
								array4[num5] = (byte)(((int)tile2.frameY & 65280) >> 8);
								num5++;
							}
							if (tile2.color() != 0)
							{
								b3 |= 8;
								array4[num5] = tile2.color();
								num5++;
							}
						}
						if (tile2.wall != 0)
						{
							b |= 4;
							array4[num5] = tile2.wall;
							num5++;
							if (tile2.wallColor() != 0)
							{
								b3 |= 16;
								array4[num5] = tile2.wallColor();
								num5++;
							}
						}
						if (tile2.liquid != 0)
						{
							if (tile2.lava())
							{
								b |= 16;
							}
							else if (tile2.honey())
							{
								b |= 24;
							}
							else
							{
								b |= 8;
							}
							array4[num5] = tile2.liquid;
							num5++;
						}
						if (tile2.wire())
						{
							b2 |= 2;
						}
						if (tile2.wire2())
						{
							b2 |= 4;
						}
						if (tile2.wire3())
						{
							b2 |= 8;
						}
						int num14;
						if (tile2.halfBrick())
						{
							num14 = 16;
						}
						else if (tile2.slope() != 0)
						{
							num14 = (int)(tile2.slope() + 1) << 4;
						}
						else
						{
							num14 = 0;
						}
						b2 |= (byte)num14;
						if (tile2.actuator())
						{
							b3 |= 2;
						}
						if (tile2.inActive())
						{
							b3 |= 4;
						}
						if (tile2.wire4())
						{
							b3 |= 32;
						}
						num6 = 2;
						if (b3 != 0)
						{
							b2 |= 1;
							array4[num6] = b3;
							num6--;
						}
						if (b2 != 0)
						{
							b |= 1;
							array4[num6] = b2;
							num6--;
						}
						tile = tile2;
					}
				}
			}
			if (num4 > 0)
			{
				array4[num5] = (byte)(num4 & 255);
				num5++;
				if (num4 > 255)
				{
					b |= 128;
					array4[num5] = (byte)(((int)num4 & 65280) >> 8);
					num5++;
				}
				else
				{
					b |= 64;
				}
			}
			array4[num6] = b;
			writer.Write(array4, num6, num5 - num6);
			writer.Write(num);
			for (int k = 0; k < (int)num; k++)
			{
				Chest chest = Main.chest[(int)array[k]];
				writer.Write(array[k]);
				writer.Write((short)chest.x);
				writer.Write((short)chest.y);
				writer.Write(chest.name);
			}
			writer.Write(num2);
			for (int l = 0; l < (int)num2; l++)
			{
				Sign sign = Main.sign[(int)array2[l]];
				writer.Write(array2[l]);
				writer.Write((short)sign.x);
				writer.Write((short)sign.y);
				writer.Write(sign.text);
			}
			writer.Write(num3);
			for (int m = 0; m < (int)num3; m++)
			{
				TileEntity.Write(writer, TileEntity.ByID[(int)array3[m]], false);
			}
		}

		public static void DecompressTileBlock(byte[] buffer, int bufferStart, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(buffer, bufferStart, bufferLength);
				memoryStream.Position = 0L;
				bool flag = memoryStream.ReadByte() != 0;
				MemoryStream memoryStream3;
				if (flag)
				{
					MemoryStream memoryStream2 = new MemoryStream();
					using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true))
					{
						deflateStream.CopyTo(memoryStream2);
						deflateStream.Close();
					}
					memoryStream3 = memoryStream2;
					memoryStream3.Position = 0L;
				}
				else
				{
					memoryStream3 = memoryStream;
					memoryStream3.Position = 1L;
				}
				using (BinaryReader binaryReader = new BinaryReader(memoryStream3))
				{
					int xStart = binaryReader.ReadInt32();
					int yStart = binaryReader.ReadInt32();
					short width = binaryReader.ReadInt16();
					short height = binaryReader.ReadInt16();
					NetMessage.DecompressTileBlock_Inner(binaryReader, xStart, yStart, (int)width, (int)height);
				}
			}
		}

		public static void DecompressTileBlock_Inner(BinaryReader reader, int xStart, int yStart, int width, int height)
		{
			Tile tile = null;
			int num = 0;
			for (int i = yStart; i < yStart + height; i++)
			{
				for (int j = xStart; j < xStart + width; j++)
				{
					if (num != 0)
					{
						num--;
						if (Main.tile[j, i] == null)
						{
							Main.tile[j, i] = new Tile(tile);
						}
						else
						{
							Main.tile[j, i].CopyFrom(tile);
						}
					}
					else
					{
						byte b2;
						byte b = b2 = 0;
						tile = Main.tile[j, i];
						if (tile == null)
						{
							tile = new Tile();
							Main.tile[j, i] = tile;
						}
						else
						{
							tile.ClearEverything();
						}
						byte b3 = reader.ReadByte();
						if ((b3 & 1) == 1)
						{
							b2 = reader.ReadByte();
							if ((b2 & 1) == 1)
							{
								b = reader.ReadByte();
							}
						}
						bool flag = tile.active();
						byte b4;
						if ((b3 & 2) == 2)
						{
							tile.active(true);
							ushort type = tile.type;
							int num2;
							if ((b3 & 32) == 32)
							{
								b4 = reader.ReadByte();
								num2 = (int)reader.ReadByte();
								num2 = (num2 << 8 | (int)b4);
							}
							else
							{
								num2 = (int)reader.ReadByte();
							}
							tile.type = (ushort)num2;
							if (Main.tileFrameImportant[num2])
							{
								tile.frameX = reader.ReadInt16();
								tile.frameY = reader.ReadInt16();
							}
							else if (!flag || tile.type != type)
							{
								tile.frameX = -1;
								tile.frameY = -1;
							}
							if ((b & 8) == 8)
							{
								tile.color(reader.ReadByte());
							}
						}
						if ((b3 & 4) == 4)
						{
							tile.wall = reader.ReadByte();
							if ((b & 16) == 16)
							{
								tile.wallColor(reader.ReadByte());
							}
						}
						b4 = (byte)((b3 & 24) >> 3);
						if (b4 != 0)
						{
							tile.liquid = reader.ReadByte();
							if (b4 > 1)
							{
								if (b4 == 2)
								{
									tile.lava(true);
								}
								else
								{
									tile.honey(true);
								}
							}
						}
						if (b2 > 1)
						{
							if ((b2 & 2) == 2)
							{
								tile.wire(true);
							}
							if ((b2 & 4) == 4)
							{
								tile.wire2(true);
							}
							if ((b2 & 8) == 8)
							{
								tile.wire3(true);
							}
							b4 = (byte)((b2 & 112) >> 4);
							if (b4 != 0 && Main.tileSolid[(int)tile.type])
							{
								if (b4 == 1)
								{
									tile.halfBrick(true);
								}
								else
								{
									tile.slope(b4 - 1);
								}
							}
						}
						if (b > 0)
						{
							if ((b & 2) == 2)
							{
								tile.actuator(true);
							}
							if ((b & 4) == 4)
							{
								tile.inActive(true);
							}
							if ((b & 32) == 32)
							{
								tile.wire4(true);
							}
						}
						b4 = (byte)((b3 & 192) >> 6);
						if (b4 == 0)
						{
							num = 0;
						}
						else if (b4 == 1)
						{
							num = (int)reader.ReadByte();
						}
						else
						{
							num = (int)reader.ReadInt16();
						}
					}
				}
			}
			short num3 = reader.ReadInt16();
			for (int k = 0; k < (int)num3; k++)
			{
				short num4 = reader.ReadInt16();
				short x = reader.ReadInt16();
				short y = reader.ReadInt16();
				string name = reader.ReadString();
				if (num4 >= 0 && num4 < 1000)
				{
					if (Main.chest[(int)num4] == null)
					{
						Main.chest[(int)num4] = new Chest(false);
					}
					Main.chest[(int)num4].name = name;
					Main.chest[(int)num4].x = (int)x;
					Main.chest[(int)num4].y = (int)y;
				}
			}
			num3 = reader.ReadInt16();
			for (int l = 0; l < (int)num3; l++)
			{
				short num5 = reader.ReadInt16();
				short x2 = reader.ReadInt16();
				short y2 = reader.ReadInt16();
				string text = reader.ReadString();
				if (num5 >= 0 && num5 < 1000)
				{
					if (Main.sign[(int)num5] == null)
					{
						Main.sign[(int)num5] = new Sign();
					}
					Main.sign[(int)num5].text = text;
					Main.sign[(int)num5].x = (int)x2;
					Main.sign[(int)num5].y = (int)y2;
				}
			}
			num3 = reader.ReadInt16();
			for (int m = 0; m < (int)num3; m++)
			{
				TileEntity tileEntity = TileEntity.Read(reader, false);
				TileEntity.ByID[tileEntity.ID] = tileEntity;
				TileEntity.ByPosition[tileEntity.Position] = tileEntity;
			}
		}

		public static void RecieveBytes(byte[] bytes, int streamLength, int i = 256)
		{
			lock (NetMessage.buffer[i])
			{
				try
				{
					Buffer.BlockCopy(bytes, 0, NetMessage.buffer[i].readBuffer, NetMessage.buffer[i].totalData, streamLength);
					NetMessage.buffer[i].totalData += streamLength;
					NetMessage.buffer[i].checkBytes = true;
				}
				catch
				{
					if (Main.netMode == 1)
					{
						Main.menuMode = 15;
						Main.statusText = Language.GetTextValue("Error.BadHeaderBufferOverflow");
						Netplay.disconnect = true;
					}
					else
					{
						Netplay.Clients[i].PendingTermination = true;
					}
				}
			}
		}

		public static void CheckBytes(int bufferIndex = 256)
		{
			lock (NetMessage.buffer[bufferIndex])
			{
				int num = 0;
				int i = NetMessage.buffer[bufferIndex].totalData;
				try
				{
					while (i >= 2)
					{
						int num2 = (int)BitConverter.ToUInt16(NetMessage.buffer[bufferIndex].readBuffer, num);
						if (i < num2)
						{
							break;
						}
						long position = NetMessage.buffer[bufferIndex].reader.BaseStream.Position;
						int num3;
						NetMessage.buffer[bufferIndex].GetData(num + 2, num2 - 2, out num3);
						NetMessage.buffer[bufferIndex].reader.BaseStream.Position = position + (long)num2;
						i -= num2;
						num += num2;
					}
				}
				catch
				{
#if SERVER
					if (num < NetMessage.buffer.Length - 100)
					{
						Console.WriteLine("Error on message " + NetMessage.buffer[num + 2]);
					}
#endif
					i = 0;
					num = 0;
				}
				if (i != NetMessage.buffer[bufferIndex].totalData)
				{
					for (int j = 0; j < i; j++)
					{
						NetMessage.buffer[bufferIndex].readBuffer[j] = NetMessage.buffer[bufferIndex].readBuffer[j + num];
					}
					NetMessage.buffer[bufferIndex].totalData = i;
				}
				NetMessage.buffer[bufferIndex].checkBytes = false;
			}
		}

		public static void BootPlayer(int plr, string msg)
		{
			NetMessage.SendData(2, plr, -1, msg, 0, 0f, 0f, 0f, 0, 0, 0);
		}

		public static void SendObjectPlacment(int whoAmi, int x, int y, int type, int style, int alternative, int random, int direction)
		{
			int remoteClient;
			int ignoreClient;
			if (Main.netMode == 2)
			{
				remoteClient = -1;
				ignoreClient = whoAmi;
			}
			else
			{
				remoteClient = whoAmi;
				ignoreClient = -1;
			}
			NetMessage.SendData(79, remoteClient, ignoreClient, "", x, (float)y, (float)type, (float)style, alternative, random, direction);
		}

		public static void SendTemporaryAnimation(int whoAmi, int animationType, int tileType, int xCoord, int yCoord)
		{
			NetMessage.SendData(77, whoAmi, -1, "", animationType, (float)tileType, (float)xCoord, (float)yCoord, 0, 0, 0);
		}

		public static void SendPlayerHurt(int playerTargetIndex, PlayerDeathReason reason, int damage, int direction, bool critical, bool pvp, int hitContext, int remoteClient = -1, int ignoreClient = -1)
		{
			NetMessage._currentPlayerDeathReason = reason;
			BitsByte bb = 0;
			bb[0] = critical;
			bb[1] = pvp;
			NetMessage.SendData(117, remoteClient, ignoreClient, "", playerTargetIndex, (float)damage, (float)direction, (float)bb, hitContext, 0, 0);
		}

		public static void SendPlayerDeath(int playerTargetIndex, PlayerDeathReason reason, int damage, int direction, bool pvp, int remoteClient = -1, int ignoreClient = -1)
		{
			NetMessage._currentPlayerDeathReason = reason;
			BitsByte bb = 0;
			bb[0] = pvp;
			NetMessage.SendData(118, remoteClient, ignoreClient, "", playerTargetIndex, (float)damage, (float)direction, (float)bb, 0, 0, 0);
		}

		public static void SendTileRange(int whoAmi, int tileX, int tileY, int xSize, int ySize, TileChangeType changeType = TileChangeType.None)
		{
			int number;
			if (xSize < ySize)
			{
				number = ySize;
			}
			else
			{
				number = xSize;
			}
			NetMessage.SendData(20, whoAmi, -1, "", number, (float)tileX, (float)tileY, 0f, (int)changeType, 0, 0);
		}

		public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size, TileChangeType changeType = TileChangeType.None)
		{
			int num = (size - 1) / 2;
			NetMessage.SendData(20, whoAmi, -1, "", size, (float)(tileX - num), (float)(tileY - num), 0f, (int)changeType, 0, 0);
		}

		public static void SendTravelShop(int remoteClient)
		{
			if (Main.netMode == 2)
			{
				NetMessage.SendData(72, remoteClient, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		public static void SendAnglerQuest(int remoteClient)
		{
			if (Main.netMode != 2)
			{
				return;
			}
			if (remoteClient == -1)
			{
				for (int i = 0; i < 255; i++)
				{
					if (Netplay.Clients[i].State == 10)
					{
						NetMessage.SendData(74, i, -1, Main.player[i].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				return;
			}
			if (Netplay.Clients[remoteClient].State == 10)
			{
				NetMessage.SendData(74, remoteClient, -1, Main.player[remoteClient].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		public static void SendSection(int whoAmi, int sectionX, int sectionY, bool skipSent = false)
		{
			if (Main.netMode != 2)
			{
				return;
			}
			try
			{
				if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY)
				{
					if (!skipSent || !Netplay.Clients[whoAmi].TileSections[sectionX, sectionY])
					{
						Netplay.Clients[whoAmi].TileSections[sectionX, sectionY] = true;
						int number = sectionX * 200;
						int num = sectionY * 150;
						int num2 = 150;
						for (int i = num; i < num + 150; i += num2)
						{
							NetMessage.SendData(10, whoAmi, -1, "", number, (float)i, 200f, (float)num2, 0, 0, 0);
						}
						for (int j = 0; j < 200; j++)
						{
							if (Main.npc[j].active && Main.npc[j].townNPC)
							{
								int sectionX2 = Netplay.GetSectionX((int)(Main.npc[j].position.X / 16f));
								int sectionY2 = Netplay.GetSectionY((int)(Main.npc[j].position.Y / 16f));
								if (sectionX2 == sectionX && sectionY2 == sectionY)
								{
									NetMessage.SendData(23, whoAmi, -1, "", j, 0f, 0f, 0f, 0, 0, 0);
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		public static void greetPlayer(int plr)
		{
			if (Main.motd == "")
			{
				NetMessage.SendData(25, plr, -1, Lang.mp[18] + " " + Main.worldName + "!", 255, 255f, 240f, 20f, 0, 0, 0);
			}
			else
			{
				NetMessage.SendData(25, plr, -1, Main.motd, 255, 255f, 240f, 20f, 0, 0, 0);
			}
			string text = "";
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					if (text == "")
					{
						text += Main.player[i].name;
					}
					else
					{
						text = text + ", " + Main.player[i].name;
					}
				}
			}
			NetMessage.SendData(25, plr, -1, Language.GetTextValue("Game.JoinGreeting", text), 255, 255f, 240f, 20f, 0, 0, 0);
		}

		public static void sendWater(int x, int y)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendData(48, -1, -1, "", x, (float)y, 0f, 0f, 0, 0, 0);
				return;
			}
			for (int i = 0; i < 256; i++)
			{
				if ((NetMessage.buffer[i].broadcast || Netplay.Clients[i].State >= 3) && Netplay.Clients[i].IsConnected())
				{
					int num = x / 200;
					int num2 = y / 150;
					if (Netplay.Clients[i].TileSections[num, num2])
					{
						NetMessage.SendData(48, i, -1, "", x, (float)y, 0f, 0f, 0, 0, 0);
					}
				}
			}
		}

		public static void SyncDisconnectedPlayer(int plr)
		{
			NetMessage.SyncOnePlayer(plr, -1, plr);
			NetMessage.EnsureLocalPlayerIsPresent();
		}

		public static void SyncConnectedPlayer(int plr)
		{
			NetMessage.SyncOnePlayer(plr, -1, plr);
			for (int i = 0; i < 255; i++)
			{
				if (plr != i && Main.player[i].active)
				{
					NetMessage.SyncOnePlayer(i, plr, -1);
				}
			}
			NetMessage.SendNPCHousesAndTravelShop(plr);
			NetMessage.SendAnglerQuest(plr);
			NetMessage.EnsureLocalPlayerIsPresent();
		}

		private static void SendNPCHousesAndTravelShop(int plr)
		{
			bool flag = false;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && Main.npc[i].townNPC && NPC.TypeToHeadIndex(Main.npc[i].type) != -1)
				{
					if (!flag && Main.npc[i].type == 368)
					{
						flag = true;
					}
					int num = 0;
					if (Main.npc[i].homeless)
					{
						num = 1;
					}
					NetMessage.SendData(60, plr, -1, "", i, (float)Main.npc[i].homeTileX, (float)Main.npc[i].homeTileY, (float)num, 0, 0, 0);
				}
			}
			if (flag)
			{
				NetMessage.SendTravelShop(plr);
			}
		}

		private static void EnsureLocalPlayerIsPresent()
		{
			if (!Main.autoShutdown)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < 255; i++)
			{
				if (Netplay.Clients[i].State == 10 && Netplay.Clients[i].Socket.GetRemoteAddress().IsLocalHost())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Console.WriteLine(Language.GetTextValue("Net.ServerAutoShutdown"));
				WorldFile.saveWorld();
				Netplay.disconnect = true;
			}
		}

		private static void SyncOnePlayer(int plr, int toWho, int fromWho)
		{
			int num = 0;
			if (Main.player[plr].active)
			{
				num = 1;
			}
			if (Netplay.Clients[plr].State == 10)
			{
				NetMessage.SendData(14, toWho, fromWho, "", plr, (float)num, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(4, toWho, fromWho, Main.player[plr].name, plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(13, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(16, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(30, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(45, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(42, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(50, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				for (int i = 0; i < 59; i++)
				{
					NetMessage.SendData(5, toWho, fromWho, Main.player[plr].inventory[i].name, plr, (float)i, (float)Main.player[plr].inventory[i].prefix, 0f, 0, 0, 0);
				}
				for (int j = 0; j < Main.player[plr].armor.Length; j++)
				{
					NetMessage.SendData(5, toWho, fromWho, Main.player[plr].armor[j].name, plr, (float)(59 + j), (float)Main.player[plr].armor[j].prefix, 0f, 0, 0, 0);
				}
				for (int k = 0; k < Main.player[plr].dye.Length; k++)
				{
					NetMessage.SendData(5, toWho, fromWho, Main.player[plr].dye[k].name, plr, (float)(58 + Main.player[plr].armor.Length + 1 + k), (float)Main.player[plr].dye[k].prefix, 0f, 0, 0, 0);
				}
				for (int l = 0; l < Main.player[plr].miscEquips.Length; l++)
				{
					NetMessage.SendData(5, toWho, fromWho, "", plr, (float)(58 + Main.player[plr].armor.Length + Main.player[plr].dye.Length + 1 + l), (float)Main.player[plr].miscEquips[l].prefix, 0f, 0, 0, 0);
				}
				for (int m = 0; m < Main.player[plr].miscDyes.Length; m++)
				{
					NetMessage.SendData(5, toWho, fromWho, "", plr, (float)(58 + Main.player[plr].armor.Length + Main.player[plr].dye.Length + Main.player[plr].miscEquips.Length + 1 + m), (float)Main.player[plr].miscDyes[m].prefix, 0f, 0, 0, 0);
				}
				if (!Netplay.Clients[plr].IsAnnouncementCompleted)
				{
					Netplay.Clients[plr].IsAnnouncementCompleted = true;
					NetMessage.SendData(25, -1, plr, Main.player[plr].name + " " + Lang.mp[19], 255, 255f, 240f, 20f, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine(Main.player[plr].name + " " + Lang.mp[19]);
						return;
					}
				}
			}
			else
			{
				num = 0;
				NetMessage.SendData(14, -1, plr, "", plr, (float)num, 0f, 0f, 0, 0, 0);
				if (Netplay.Clients[plr].IsAnnouncementCompleted)
				{
					Netplay.Clients[plr].IsAnnouncementCompleted = false;
					NetMessage.SendData(25, -1, plr, Netplay.Clients[plr].Name + " " + Lang.mp[20], 255, 255f, 240f, 20f, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine(Netplay.Clients[plr].Name + " " + Lang.mp[20]);
					}
					Netplay.Clients[plr].Name = "Anonymous";
				}
			}
		}
	}
}