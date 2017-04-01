using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI;

namespace Terraria
{
	public class MessageBuffer
	{
		public const int readBufferMax = 131070;

		public const int writeBufferMax = 131070;

		public bool broadcast;

		public byte[] readBuffer = new byte[131070];

		public byte[] writeBuffer = new byte[131070];

		public bool writeLocked;

		public int messageLength;

		public int totalData;

		public int whoAmI;

		public int spamCount;

		public int maxSpam;

		public bool checkBytes;

		public MemoryStream readerStream;

		public MemoryStream writerStream;

		public BinaryReader reader;

		public BinaryWriter writer;

		public static event TileChangeReceivedEvent OnTileChangeReceived;

		public void Reset()
		{
			Array.Clear(this.readBuffer, 0, this.readBuffer.Length);
			Array.Clear(this.writeBuffer, 0, this.writeBuffer.Length);
			this.writeLocked = false;
			this.messageLength = 0;
			this.totalData = 0;
			this.spamCount = 0;
			this.broadcast = false;
			this.checkBytes = false;
			this.ResetReader();
			this.ResetWriter();
		}

		public void ResetReader()
		{
			if (this.readerStream != null)
			{
				this.readerStream.Close();
			}
			this.readerStream = new MemoryStream(this.readBuffer);
			this.reader = new BinaryReader(this.readerStream);
		}

		public void ResetWriter()
		{
			if (this.writerStream != null)
			{
				this.writerStream.Close();
			}
			this.writerStream = new MemoryStream(this.writeBuffer);
			this.writer = new BinaryWriter(this.writerStream);
		}

		public void GetData(int start, int length, out int messageType)
		{
			if (this.whoAmI < 256)
			{
				Netplay.Clients[this.whoAmI].TimeOutTimer = 0;
			}
			else
			{
				Netplay.Connection.TimeOutTimer = 0;
			}
			int num = start + 1;
			byte b = this.readBuffer[start];
			messageType = (int)b;
			if (b >= 119)
			{
				return;
			}
			Main.rxMsg++;
			Main.rxData += length;
			Main.rxMsgType[(int)b]++;
			Main.rxDataType[(int)b] += length;
			if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
			{
				Netplay.Connection.StatusCount++;
			}
			if (Main.verboseNetplay)
			{
				for (int i = start; i < start + length; i++)
				{
				}
				for (int j = start; j < start + length; j++)
				{
					byte arg_D6_0 = this.readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && Netplay.Clients[this.whoAmI].State == -1)
			{
				NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1], 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			if (Main.netMode == 2 && Netplay.Clients[this.whoAmI].State < 10 && b > 12 && b != 93 && b != 16 && b != 42 && b != 50 && b != 38 && b != 68)
			{
				NetMessage.BootPlayer(this.whoAmI, Lang.mp[2]);
			}
			if (this.reader == null)
			{
				this.ResetReader();
			}
			this.reader.BaseStream.Position = (long)num;
			switch (b)
			{
			case 1:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[this.whoAmI].Socket.GetRemoteAddress()))
				{
					NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[3], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (Netplay.Clients[this.whoAmI].State != 0)
				{
					return;
				}
				string a = this.reader.ReadString();
				if (!(a == "Terraria" + 188))
				{
					NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[4], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (string.IsNullOrEmpty(Netplay.ServerPassword))
				{
					Netplay.Clients[this.whoAmI].State = 1;
					NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Netplay.Clients[this.whoAmI].State = -1;
				NetMessage.SendData(37, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 2:
				if (Main.netMode != 1)
				{
					return;
				}
				Netplay.disconnect = true;
				Main.statusText = this.reader.ReadString();
				return;
			case 3:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				if (Netplay.Connection.State == 1)
				{
					Netplay.Connection.State = 2;
				}
				int num2 = (int)this.reader.ReadByte();
				if (num2 != Main.myPlayer)
				{
					Main.player[num2] = Main.ActivePlayerFileData.Player;
					Main.player[Main.myPlayer] = new Player();
				}
				Main.player[num2].whoAmI = num2;
				Main.myPlayer = num2;
				Player player = Main.player[num2];
				NetMessage.SendData(4, -1, -1, player.name, num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(68, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(16, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(42, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(50, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				for (int k = 0; k < 59; k++)
				{
					NetMessage.SendData(5, -1, -1, player.inventory[k].name, num2, (float)k, (float)player.inventory[k].prefix, 0f, 0, 0, 0);
				}
				for (int l = 0; l < player.armor.Length; l++)
				{
					NetMessage.SendData(5, -1, -1, player.armor[l].name, num2, (float)(59 + l), (float)player.armor[l].prefix, 0f, 0, 0, 0);
				}
				for (int m = 0; m < player.dye.Length; m++)
				{
					NetMessage.SendData(5, -1, -1, player.dye[m].name, num2, (float)(58 + player.armor.Length + 1 + m), (float)player.dye[m].prefix, 0f, 0, 0, 0);
				}
				for (int n = 0; n < player.miscEquips.Length; n++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + 1 + n), (float)player.miscEquips[n].prefix, 0f, 0, 0, 0);
				}
				for (int num3 = 0; num3 < player.miscDyes.Length; num3++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + 1 + num3), (float)player.miscDyes[num3].prefix, 0f, 0, 0, 0);
				}
				for (int num4 = 0; num4 < player.bank.item.Length; num4++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + 1 + num4), (float)player.bank.item[num4].prefix, 0f, 0, 0, 0);
				}
				for (int num5 = 0; num5 < player.bank2.item.Length; num5++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + 1 + num5), (float)player.bank2.item[num5].prefix, 0f, 0, 0, 0);
				}
				NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + player.bank2.item.Length + 1), (float)player.trashItem.prefix, 0f, 0, 0, 0);
				for (int num6 = 0; num6 < player.bank3.item.Length; num6++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + player.bank2.item.Length + 2 + num6), (float)player.bank3.item[num6].prefix, 0f, 0, 0, 0);
				}
				NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				if (Netplay.Connection.State == 2)
				{
					Netplay.Connection.State = 3;
					return;
				}
				return;
			}
			case 4:
			{
				int num7 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num7 = this.whoAmI;
				}
				if (num7 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				Player player2 = Main.player[num7];
				player2.whoAmI = num7;
				player2.skinVariant = (int)this.reader.ReadByte();
				player2.skinVariant = (int)MathHelper.Clamp((float)player2.skinVariant, 0f, 9f);
				player2.hair = (int)this.reader.ReadByte();
				if (player2.hair >= 134)
				{
					player2.hair = 0;
				}
				player2.name = this.reader.ReadString().Trim().Trim();
				player2.hairDye = this.reader.ReadByte();
				BitsByte bitsByte = this.reader.ReadByte();
				for (int num8 = 0; num8 < 8; num8++)
				{
					player2.hideVisual[num8] = bitsByte[num8];
				}
				bitsByte = this.reader.ReadByte();
				for (int num9 = 0; num9 < 2; num9++)
				{
					player2.hideVisual[num9 + 8] = bitsByte[num9];
				}
				player2.hideMisc = this.reader.ReadByte();
				player2.hairColor = this.reader.ReadRGB();
				player2.skinColor = this.reader.ReadRGB();
				player2.eyeColor = this.reader.ReadRGB();
				player2.shirtColor = this.reader.ReadRGB();
				player2.underShirtColor = this.reader.ReadRGB();
				player2.pantsColor = this.reader.ReadRGB();
				player2.shoeColor = this.reader.ReadRGB();
				BitsByte bitsByte2 = this.reader.ReadByte();
				player2.difficulty = 0;
				if (bitsByte2[0])
				{
					Player expr_BED = player2;
					expr_BED.difficulty += 1;
				}
				if (bitsByte2[1])
				{
					Player expr_C07 = player2;
					expr_C07.difficulty += 2;
				}
				if (player2.difficulty > 2)
				{
					player2.difficulty = 2;
				}
				player2.extraAccessory = bitsByte2[2];
				if (Main.netMode != 2)
				{
					return;
				}
				bool flag = false;
				if (Netplay.Clients[this.whoAmI].State < 10)
				{
					for (int num10 = 0; num10 < 255; num10++)
					{
						if (num10 != num7 && player2.name == Main.player[num10].name && Netplay.Clients[num10].IsActive)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					NetMessage.SendData(2, this.whoAmI, -1, player2.name + " " + Lang.mp[5], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (player2.name.Length > Player.nameLen)
				{
					NetMessage.SendData(2, this.whoAmI, -1, Language.GetTextValue("Net.NameTooLong"), 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (player2.name == "")
				{
					NetMessage.SendData(2, this.whoAmI, -1, Language.GetTextValue("Net.EmptyName"), 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Netplay.Clients[this.whoAmI].Name = player2.name;
				Netplay.Clients[this.whoAmI].Name = player2.name;
				NetMessage.SendData(4, -1, this.whoAmI, player2.name, num7, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 5:
			{
				int num11 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num11 = this.whoAmI;
				}
				if (num11 == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[num11].IsStackingItems())
				{
					return;
				}
				Player player3 = Main.player[num11];
				lock (player3)
				{
					int num12 = (int)this.reader.ReadByte();
					int stack = (int)this.reader.ReadInt16();
					int num13 = (int)this.reader.ReadByte();
					int type = (int)this.reader.ReadInt16();
					Item[] array = null;
					int num14 = 0;
					bool flag3 = false;
					if (num12 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length + 1)
					{
						num14 = num12 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length + 1) - 1;
						array = player3.bank3.item;
					}
					else if (num12 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length)
					{
						flag3 = true;
					}
					else if (num12 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length)
					{
						num14 = num12 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length) - 1;
						array = player3.bank2.item;
					}
					else if (num12 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
					{
						num14 = num12 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length) - 1;
						array = player3.bank.item;
					}
					else if (num12 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length)
					{
						num14 = num12 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length) - 1;
						array = player3.miscDyes;
					}
					else if (num12 > 58 + player3.armor.Length + player3.dye.Length)
					{
						num14 = num12 - 58 - (player3.armor.Length + player3.dye.Length) - 1;
						array = player3.miscEquips;
					}
					else if (num12 > 58 + player3.armor.Length)
					{
						num14 = num12 - 58 - player3.armor.Length - 1;
						array = player3.dye;
					}
					else if (num12 > 58)
					{
						num14 = num12 - 58 - 1;
						array = player3.armor;
					}
					else
					{
						num14 = num12;
						array = player3.inventory;
					}
					if (flag3)
					{
						player3.trashItem = new Item();
						player3.trashItem.netDefaults(type);
						player3.trashItem.stack = stack;
						player3.trashItem.Prefix(num13);
					}
					else if (num12 <= 58)
					{
						int type2 = array[num14].type;
						int stack2 = array[num14].stack;
						array[num14] = new Item();
						array[num14].netDefaults(type);
						array[num14].stack = stack;
						array[num14].Prefix(num13);
						if (num11 == Main.myPlayer && num14 == 58)
						{
							Main.mouseItem = array[num14].Clone();
						}
						if (num11 == Main.myPlayer && Main.netMode == 1)
						{
							Main.player[num11].inventoryChestStack[num12] = false;
							if (array[num14].stack != stack2 || array[num14].type != type2)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7, -1, -1, 1, 1f, 0f);
							}
						}
					}
					else
					{
						array[num14] = new Item();
						array[num14].netDefaults(type);
						array[num14].stack = stack;
						array[num14].Prefix(num13);
					}
					if (Main.netMode == 2 && num11 == this.whoAmI && num12 <= 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
					{
						NetMessage.SendData(5, -1, this.whoAmI, "", num11, (float)num12, (float)num13, 0f, 0, 0, 0);
					}
					return;
				}
				break;
			}
			case 6:
				break;
			case 7:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				Main.time = (double)this.reader.ReadInt32();
				BitsByte bitsByte3 = this.reader.ReadByte();
				Main.dayTime = bitsByte3[0];
				Main.bloodMoon = bitsByte3[1];
				Main.eclipse = bitsByte3[2];
				Main.moonPhase = (int)this.reader.ReadByte();
				Main.maxTilesX = (int)this.reader.ReadInt16();
				Main.maxTilesY = (int)this.reader.ReadInt16();
				Main.spawnTileX = (int)this.reader.ReadInt16();
				Main.spawnTileY = (int)this.reader.ReadInt16();
				Main.worldSurface = (double)this.reader.ReadInt16();
				Main.rockLayer = (double)this.reader.ReadInt16();
				Main.worldID = this.reader.ReadInt32();
				Main.worldName = this.reader.ReadString();
				Main.ActiveWorldFileData.UniqueId = new Guid(this.reader.ReadString());
				Main.ActiveWorldFileData.WorldGeneratorVersion = 807453851649uL;
				Main.moonType = (int)this.reader.ReadByte();
				WorldGen.setBG(0, (int)this.reader.ReadByte());
				WorldGen.setBG(1, (int)this.reader.ReadByte());
				WorldGen.setBG(2, (int)this.reader.ReadByte());
				WorldGen.setBG(3, (int)this.reader.ReadByte());
				WorldGen.setBG(4, (int)this.reader.ReadByte());
				WorldGen.setBG(5, (int)this.reader.ReadByte());
				WorldGen.setBG(6, (int)this.reader.ReadByte());
				WorldGen.setBG(7, (int)this.reader.ReadByte());
				Main.iceBackStyle = (int)this.reader.ReadByte();
				Main.jungleBackStyle = (int)this.reader.ReadByte();
				Main.hellBackStyle = (int)this.reader.ReadByte();
				Main.windSpeedSet = this.reader.ReadSingle();
				Main.numClouds = (int)this.reader.ReadByte();
				for (int num15 = 0; num15 < 3; num15++)
				{
					Main.treeX[num15] = this.reader.ReadInt32();
				}
				for (int num16 = 0; num16 < 4; num16++)
				{
					Main.treeStyle[num16] = (int)this.reader.ReadByte();
				}
				for (int num17 = 0; num17 < 3; num17++)
				{
					Main.caveBackX[num17] = this.reader.ReadInt32();
				}
				for (int num18 = 0; num18 < 4; num18++)
				{
					Main.caveBackStyle[num18] = (int)this.reader.ReadByte();
				}
				Main.maxRaining = this.reader.ReadSingle();
				Main.raining = (Main.maxRaining > 0f);
				BitsByte bitsByte4 = this.reader.ReadByte();
				WorldGen.shadowOrbSmashed = bitsByte4[0];
				NPC.downedBoss1 = bitsByte4[1];
				NPC.downedBoss2 = bitsByte4[2];
				NPC.downedBoss3 = bitsByte4[3];
				Main.hardMode = bitsByte4[4];
				NPC.downedClown = bitsByte4[5];
				Main.ServerSideCharacter = bitsByte4[6];
				NPC.downedPlantBoss = bitsByte4[7];
				BitsByte bitsByte5 = this.reader.ReadByte();
				NPC.downedMechBoss1 = bitsByte5[0];
				NPC.downedMechBoss2 = bitsByte5[1];
				NPC.downedMechBoss3 = bitsByte5[2];
				NPC.downedMechBossAny = bitsByte5[3];
				Main.cloudBGActive = (float)(bitsByte5[4] ? 1 : 0);
				WorldGen.crimson = bitsByte5[5];
				Main.pumpkinMoon = bitsByte5[6];
				Main.snowMoon = bitsByte5[7];
				BitsByte bitsByte6 = this.reader.ReadByte();
				Main.expertMode = bitsByte6[0];
				Main.fastForwardTime = bitsByte6[1];
				Main.UpdateSundial();
				bool flag4 = bitsByte6[2];
				NPC.downedSlimeKing = bitsByte6[3];
				NPC.downedQueenBee = bitsByte6[4];
				NPC.downedFishron = bitsByte6[5];
				NPC.downedMartians = bitsByte6[6];
				NPC.downedAncientCultist = bitsByte6[7];
				BitsByte bitsByte7 = this.reader.ReadByte();
				NPC.downedMoonlord = bitsByte7[0];
				NPC.downedHalloweenKing = bitsByte7[1];
				NPC.downedHalloweenTree = bitsByte7[2];
				NPC.downedChristmasIceQueen = bitsByte7[3];
				NPC.downedChristmasSantank = bitsByte7[4];
				NPC.downedChristmasTree = bitsByte7[5];
				NPC.downedGolemBoss = bitsByte7[6];
				BirthdayParty.ManualParty = bitsByte7[7];
				BitsByte bitsByte8 = this.reader.ReadByte();
				NPC.downedPirates = bitsByte8[0];
				NPC.downedFrost = bitsByte8[1];
				NPC.downedGoblins = bitsByte8[2];
				Sandstorm.Happening = bitsByte8[3];
				DD2Event.Ongoing = bitsByte8[4];
				DD2Event.DownedInvasionT1 = bitsByte8[5];
				DD2Event.DownedInvasionT2 = bitsByte8[6];
				DD2Event.DownedInvasionT3 = bitsByte8[7];
				if (flag4)
				{
					Main.StartSlimeRain(true);
				}
				else
				{
					Main.StopSlimeRain(true);
				}
				Main.invasionType = (int)this.reader.ReadSByte();
				Main.LobbyId = this.reader.ReadUInt64();
				Sandstorm.IntendedSeverity = this.reader.ReadSingle();
				if (Netplay.Connection.State == 3)
				{
					Netplay.Connection.State = 4;
					return;
				}
				return;
			}
			case 8:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int num19 = this.reader.ReadInt32();
				int num20 = this.reader.ReadInt32();
				bool flag5 = true;
				if (num19 == -1 || num20 == -1)
				{
					flag5 = false;
				}
				else if (num19 < 10 || num19 > Main.maxTilesX - 10)
				{
					flag5 = false;
				}
				else if (num20 < 10 || num20 > Main.maxTilesY - 10)
				{
					flag5 = false;
				}
				int num21 = Netplay.GetSectionX(Main.spawnTileX) - 2;
				int num22 = Netplay.GetSectionY(Main.spawnTileY) - 1;
				int num23 = num21 + 5;
				int num24 = num22 + 3;
				if (num21 < 0)
				{
					num21 = 0;
				}
				if (num23 >= Main.maxSectionsX)
				{
					num23 = Main.maxSectionsX - 1;
				}
				if (num22 < 0)
				{
					num22 = 0;
				}
				if (num24 >= Main.maxSectionsY)
				{
					num24 = Main.maxSectionsY - 1;
				}
				int num25 = (num23 - num21) * (num24 - num22);
				List<Point> list = new List<Point>();
				for (int num26 = num21; num26 < num23; num26++)
				{
					for (int num27 = num22; num27 < num24; num27++)
					{
						list.Add(new Point(num26, num27));
					}
				}
				int num28 = -1;
				int num29 = -1;
				if (flag5)
				{
					num19 = Netplay.GetSectionX(num19) - 2;
					num20 = Netplay.GetSectionY(num20) - 1;
					num28 = num19 + 5;
					num29 = num20 + 3;
					if (num19 < 0)
					{
						num19 = 0;
					}
					if (num28 >= Main.maxSectionsX)
					{
						num28 = Main.maxSectionsX - 1;
					}
					if (num20 < 0)
					{
						num20 = 0;
					}
					if (num29 >= Main.maxSectionsY)
					{
						num29 = Main.maxSectionsY - 1;
					}
					for (int num30 = num19; num30 < num28; num30++)
					{
						for (int num31 = num20; num31 < num29; num31++)
						{
							if (num30 < num21 || num30 >= num23 || num31 < num22 || num31 >= num24)
							{
								list.Add(new Point(num30, num31));
								num25++;
							}
						}
					}
				}
				int num32 = 1;
				List<Point> list2;
				List<Point> list3;
				PortalHelper.SyncPortalsOnPlayerJoin(this.whoAmI, 1, list, out list2, out list3);
				num25 += list2.Count;
				if (Netplay.Clients[this.whoAmI].State == 2)
				{
					Netplay.Clients[this.whoAmI].State = 3;
				}
				NetMessage.SendData(9, this.whoAmI, -1, Lang.inter[44], num25, 0f, 0f, 0f, 0, 0, 0);
				Netplay.Clients[this.whoAmI].StatusText2 = Language.GetTextValue("Net.IsReceivingTileData");
				Netplay.Clients[this.whoAmI].StatusMax += num25;
				for (int num33 = num21; num33 < num23; num33++)
				{
					for (int num34 = num22; num34 < num24; num34++)
					{
						NetMessage.SendSection(this.whoAmI, num33, num34, false);
					}
				}
				NetMessage.SendData(11, this.whoAmI, -1, "", num21, (float)num22, (float)(num23 - 1), (float)(num24 - 1), 0, 0, 0);
				if (flag5)
				{
					for (int num35 = num19; num35 < num28; num35++)
					{
						for (int num36 = num20; num36 < num29; num36++)
						{
							NetMessage.SendSection(this.whoAmI, num35, num36, true);
						}
					}
					NetMessage.SendData(11, this.whoAmI, -1, "", num19, (float)num20, (float)(num28 - 1), (float)(num29 - 1), 0, 0, 0);
				}
				for (int num37 = 0; num37 < list2.Count; num37++)
				{
					NetMessage.SendSection(this.whoAmI, list2[num37].X, list2[num37].Y, true);
				}
				for (int num38 = 0; num38 < list3.Count; num38++)
				{
					NetMessage.SendData(11, this.whoAmI, -1, "", list3[num38].X - num32, (float)(list3[num38].Y - num32), (float)(list3[num38].X + num32 + 1), (float)(list3[num38].Y + num32 + 1), 0, 0, 0);
				}
				for (int num39 = 0; num39 < 400; num39++)
				{
					if (Main.item[num39].active)
					{
						NetMessage.SendData(21, this.whoAmI, -1, "", num39, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(22, this.whoAmI, -1, "", num39, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num40 = 0; num40 < 200; num40++)
				{
					if (Main.npc[num40].active)
					{
						NetMessage.SendData(23, this.whoAmI, -1, "", num40, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num41 = 0; num41 < 1000; num41++)
				{
					if (Main.projectile[num41].active && (Main.projPet[Main.projectile[num41].type] || Main.projectile[num41].netImportant))
					{
						NetMessage.SendData(27, this.whoAmI, -1, "", num41, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num42 = 0; num42 < 267; num42++)
				{
					NetMessage.SendData(83, this.whoAmI, -1, "", num42, 0f, 0f, 0f, 0, 0, 0);
				}
				NetMessage.SendData(49, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(57, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(103, -1, -1, "", NPC.MoonLordCountdown, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(101, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 9:
				if (Main.netMode != 1)
				{
					return;
				}
				Netplay.Connection.StatusMax += this.reader.ReadInt32();
				Netplay.Connection.StatusText = this.reader.ReadString();
				return;
			case 10:
				if (Main.netMode != 1)
				{
					return;
				}
				NetMessage.DecompressTileBlock(this.readBuffer, num, length);
				return;
			case 11:
				if (Main.netMode != 1)
				{
					return;
				}
				WorldGen.SectionTileFrame((int)this.reader.ReadInt16(), (int)this.reader.ReadInt16(), (int)this.reader.ReadInt16(), (int)this.reader.ReadInt16());
				return;
			case 12:
			{
				int num43 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num43 = this.whoAmI;
				}
				Player player4 = Main.player[num43];
				player4.SpawnX = (int)this.reader.ReadInt16();
				player4.SpawnY = (int)this.reader.ReadInt16();
				player4.Spawn();
				if (num43 == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
				}
				if (Main.netMode != 2 || Netplay.Clients[this.whoAmI].State < 3)
				{
					return;
				}
				if (Netplay.Clients[this.whoAmI].State == 3)
				{
					Netplay.Clients[this.whoAmI].State = 10;
					NetMessage.greetPlayer(this.whoAmI);
					NetMessage.buffer[this.whoAmI].broadcast = true;
					NetMessage.SyncConnectedPlayer(this.whoAmI);
					NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(74, this.whoAmI, -1, Main.player[this.whoAmI].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 13:
			{
				int num44 = (int)this.reader.ReadByte();
				if (num44 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num44 = this.whoAmI;
				}
				Player player5 = Main.player[num44];
				BitsByte bitsByte9 = this.reader.ReadByte();
				player5.controlUp = bitsByte9[0];
				player5.controlDown = bitsByte9[1];
				player5.controlLeft = bitsByte9[2];
				player5.controlRight = bitsByte9[3];
				player5.controlJump = bitsByte9[4];
				player5.controlUseItem = bitsByte9[5];
				player5.direction = (bitsByte9[6] ? 1 : -1);
				BitsByte bitsByte10 = this.reader.ReadByte();
				if (bitsByte10[0])
				{
					player5.pulley = true;
					player5.pulleyDir = (bitsByte10[1] ? (byte)2 : (byte)1);
				}
				else
				{
					player5.pulley = false;
				}
				player5.selectedItem = (int)this.reader.ReadByte();
				player5.position = this.reader.ReadVector2();
				if (bitsByte10[2])
				{
					player5.velocity = this.reader.ReadVector2();
				}
				else
				{
					player5.velocity = Vector2.Zero;
				}
				player5.vortexStealthActive = bitsByte10[3];
				player5.gravDir = (float)(bitsByte10[4] ? 1 : -1);
				if (Main.netMode == 2 && Netplay.Clients[this.whoAmI].State == 10)
				{
					NetMessage.SendData(13, -1, this.whoAmI, "", num44, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 14:
			{
				int num45 = (int)this.reader.ReadByte();
				int num46 = (int)this.reader.ReadByte();
				if (Main.netMode != 1)
				{
					return;
				}
				bool active = Main.player[num45].active;
				if (num46 == 1)
				{
					if (!Main.player[num45].active)
					{
						Main.player[num45] = new Player();
					}
					Main.player[num45].active = true;
				}
				else
				{
					Main.player[num45].active = false;
				}
				if (active == Main.player[num45].active)
				{
					return;
				}
				if (Main.player[num45].active)
				{
					Player.Hooks.PlayerConnect(num45);
					return;
				}
				Player.Hooks.PlayerDisconnect(num45);
				return;
			}
			case 15:
			case 67:
			case 93:
			case 94:
				return;
			case 16:
			{
				int num47 = (int)this.reader.ReadByte();
				if (num47 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num47 = this.whoAmI;
				}
				Player player6 = Main.player[num47];
				player6.statLife = (int)this.reader.ReadInt16();
				player6.statLifeMax = (int)this.reader.ReadInt16();
				if (player6.statLifeMax < 100)
				{
					player6.statLifeMax = 100;
				}
				player6.dead = (player6.statLife <= 0);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(16, -1, this.whoAmI, "", num47, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 17:
			{
				byte b2 = this.reader.ReadByte();
				int num48 = (int)this.reader.ReadInt16();
				int num49 = (int)this.reader.ReadInt16();
				short num50 = this.reader.ReadInt16();
				int num51 = (int)this.reader.ReadByte();
				bool flag6 = num50 == 1;
				if (!WorldGen.InWorld(num48, num49, 3))
				{
					return;
				}
				if (Main.tile[num48, num49] == null)
				{
					Main.tile[num48, num49] = new Tile();
				}
				if (Main.netMode == 2)
				{
					if (!flag6)
					{
						if (b2 == 0 || b2 == 2 || b2 == 4)
						{
							Netplay.Clients[this.whoAmI].SpamDeleteBlock += 1f;
						}
						if (b2 == 1 || b2 == 3)
						{
							Netplay.Clients[this.whoAmI].SpamAddBlock += 1f;
						}
					}
					if (!Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(num48), Netplay.GetSectionY(num49)])
					{
						flag6 = true;
					}
				}
				if (b2 == 0)
				{
					WorldGen.KillTile(num48, num49, flag6, false, false);
				}
				if (b2 == 1)
				{
					WorldGen.PlaceTile(num48, num49, (int)num50, false, true, -1, num51);
				}
				if (b2 == 2)
				{
					WorldGen.KillWall(num48, num49, flag6);
				}
				if (b2 == 3)
				{
					WorldGen.PlaceWall(num48, num49, (int)num50, false);
				}
				if (b2 == 4)
				{
					WorldGen.KillTile(num48, num49, flag6, false, true);
				}
				if (b2 == 5)
				{
					WorldGen.PlaceWire(num48, num49);
				}
				if (b2 == 6)
				{
					WorldGen.KillWire(num48, num49);
				}
				if (b2 == 7)
				{
					WorldGen.PoundTile(num48, num49);
				}
				if (b2 == 8)
				{
					WorldGen.PlaceActuator(num48, num49);
				}
				if (b2 == 9)
				{
					WorldGen.KillActuator(num48, num49);
				}
				if (b2 == 10)
				{
					WorldGen.PlaceWire2(num48, num49);
				}
				if (b2 == 11)
				{
					WorldGen.KillWire2(num48, num49);
				}
				if (b2 == 12)
				{
					WorldGen.PlaceWire3(num48, num49);
				}
				if (b2 == 13)
				{
					WorldGen.KillWire3(num48, num49);
				}
				if (b2 == 14)
				{
					WorldGen.SlopeTile(num48, num49, (int)num50);
				}
				if (b2 == 15)
				{
					Minecart.FrameTrack(num48, num49, true, false);
				}
				if (b2 == 16)
				{
					WorldGen.PlaceWire4(num48, num49);
				}
				if (b2 == 17)
				{
					WorldGen.KillWire4(num48, num49);
				}
				if (b2 == 18)
				{
					Wiring.SetCurrentUser(this.whoAmI);
					Wiring.PokeLogicGate(num48, num49);
					Wiring.SetCurrentUser(-1);
					return;
				}
				if (b2 == 19)
				{
					Wiring.SetCurrentUser(this.whoAmI);
					Wiring.Actuate(num48, num49);
					Wiring.SetCurrentUser(-1);
					return;
				}
				if (Main.netMode != 2)
				{
					return;
				}
				NetMessage.SendData(17, -1, this.whoAmI, "", (int)b2, (float)num48, (float)num49, (float)num50, num51, 0, 0);
				if (b2 == 1 && num50 == 53)
				{
					NetMessage.SendTileSquare(-1, num48, num49, 1, TileChangeType.None);
					return;
				}
				return;
			}
			case 18:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.dayTime = (this.reader.ReadByte() == 1);
				Main.time = (double)this.reader.ReadInt32();
				Main.sunModY = this.reader.ReadInt16();
				Main.moonModY = this.reader.ReadInt16();
				return;
			case 19:
			{
				byte b3 = this.reader.ReadByte();
				int num52 = (int)this.reader.ReadInt16();
				int num53 = (int)this.reader.ReadInt16();
				if (!WorldGen.InWorld(num52, num53, 3))
				{
					return;
				}
				int num54 = (this.reader.ReadByte() == 0) ? -1 : 1;
				if (b3 == 0)
				{
					WorldGen.OpenDoor(num52, num53, num54);
				}
				else if (b3 == 1)
				{
					WorldGen.CloseDoor(num52, num53, true);
				}
				else if (b3 == 2)
				{
					WorldGen.ShiftTrapdoor(num52, num53, num54 == 1, 1);
				}
				else if (b3 == 3)
				{
					WorldGen.ShiftTrapdoor(num52, num53, num54 == 1, 0);
				}
				else if (b3 == 4)
				{
					WorldGen.ShiftTallGate(num52, num53, false);
				}
				else if (b3 == 5)
				{
					WorldGen.ShiftTallGate(num52, num53, true);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(19, -1, this.whoAmI, "", (int)b3, (float)num52, (float)num53, (float)((num54 == 1) ? 1 : 0), 0, 0, 0);
					return;
				}
				return;
			}
			case 20:
			{
				ushort num55 = this.reader.ReadUInt16();
				short num56 = (short)(num55 & 32767);
				bool flag7 = (num55 & 32768) != 0;
				byte b4 = 0;
				if (flag7)
				{
					b4 = this.reader.ReadByte();
				}
				int num57 = (int)this.reader.ReadInt16();
				int num58 = (int)this.reader.ReadInt16();
				if (!WorldGen.InWorld(num57, num58, 3))
				{
					return;
				}
				TileChangeType type3 = TileChangeType.None;
				if (Enum.IsDefined(typeof(TileChangeType), b4))
				{
					type3 = (TileChangeType)b4;
				}
				if (MessageBuffer.OnTileChangeReceived != null)
				{
					MessageBuffer.OnTileChangeReceived(num57, num58, (int)num56, type3);
				}
				BitsByte bitsByte11 = 0;
				BitsByte bitsByte12 = 0;
				for (int num59 = num57; num59 < num57 + (int)num56; num59++)
				{
					for (int num60 = num58; num60 < num58 + (int)num56; num60++)
					{
						if (Main.tile[num59, num60] == null)
						{
							Main.tile[num59, num60] = new Tile();
						}
						Tile tile = Main.tile[num59, num60];
						bool flag8 = tile.active();
						bitsByte11 = this.reader.ReadByte();
						bitsByte12 = this.reader.ReadByte();
						tile.active(bitsByte11[0]);
						tile.wall = (bitsByte11[2] ? (byte)1 : (byte)0);
						bool flag9 = bitsByte11[3];
						if (Main.netMode != 2)
						{
							tile.liquid = (flag9 ? (byte)1 : (byte)0);
						}
						tile.wire(bitsByte11[4]);
						tile.halfBrick(bitsByte11[5]);
						tile.actuator(bitsByte11[6]);
						tile.inActive(bitsByte11[7]);
						tile.wire2(bitsByte12[0]);
						tile.wire3(bitsByte12[1]);
						if (bitsByte12[2])
						{
							tile.color(this.reader.ReadByte());
						}
						if (bitsByte12[3])
						{
							tile.wallColor(this.reader.ReadByte());
						}
						if (tile.active())
						{
							int type4 = (int)tile.type;
							tile.type = this.reader.ReadUInt16();
							if (Main.tileFrameImportant[(int)tile.type])
							{
								tile.frameX = this.reader.ReadInt16();
								tile.frameY = this.reader.ReadInt16();
							}
							else if (!flag8 || (int)tile.type != type4)
							{
								tile.frameX = -1;
								tile.frameY = -1;
							}
							byte b5 = 0;
							if (bitsByte12[4])
							{
								b5 += 1;
							}
							if (bitsByte12[5])
							{
								b5 += 2;
							}
							if (bitsByte12[6])
							{
								b5 += 4;
							}
							tile.slope(b5);
						}
						tile.wire4(bitsByte12[7]);
						if (tile.wall > 0)
						{
							tile.wall = this.reader.ReadByte();
						}
						if (flag9)
						{
							tile.liquid = this.reader.ReadByte();
							tile.liquidType((int)this.reader.ReadByte());
						}
					}
				}
				WorldGen.RangeFrame(num57, num58, num57 + (int)num56, num58 + (int)num56);
				if (Main.netMode == 2)
				{
					NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num56, (float)num57, (float)num58, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 21:
			case 90:
			{
				int num61 = (int)this.reader.ReadInt16();
				Vector2 position = this.reader.ReadVector2();
				Vector2 velocity = this.reader.ReadVector2();
				int stack3 = (int)this.reader.ReadInt16();
				int pre = (int)this.reader.ReadByte();
				int num62 = (int)this.reader.ReadByte();
				int num63 = (int)this.reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num63 == 0)
					{
						Main.item[num61].active = false;
						return;
					}
					int num64 = num61;
					Item item = Main.item[num64];
					bool newAndShiny = (item.newAndShiny || item.netID != num63) && ItemSlot.Options.HighlightNewItems && (num63 < 0 || num63 >= 3884 || !ItemID.Sets.NeverShiny[num63]);
					item.netDefaults(num63);
					item.newAndShiny = newAndShiny;
					item.Prefix(pre);
					item.stack = stack3;
					item.position = position;
					item.velocity = velocity;
					item.active = true;
					if (b == 90)
					{
						item.instanced = true;
						item.owner = Main.myPlayer;
						item.keepTime = 600;
					}
					item.wet = Collision.WetCollision(item.position, item.width, item.height);
					return;
				}
				else
				{
					if (Main.itemLockoutTime[num61] > 0)
					{
						return;
					}
					if (num63 == 0)
					{
						if (num61 < 400)
						{
							Main.item[num61].active = false;
							NetMessage.SendData(21, -1, -1, "", num61, 0f, 0f, 0f, 0, 0, 0);
							return;
						}
						return;
					}
					else
					{
						bool flag10 = false;
						if (num61 == 400)
						{
							flag10 = true;
						}
						if (flag10)
						{
							Item item2 = new Item();
							item2.netDefaults(num63);
							num61 = Item.NewItem((int)position.X, (int)position.Y, item2.width, item2.height, item2.type, stack3, true, 0, false, false);
						}
						Item item3 = Main.item[num61];
						item3.netDefaults(num63);
						item3.Prefix(pre);
						item3.stack = stack3;
						item3.position = position;
						item3.velocity = velocity;
						item3.active = true;
						item3.owner = Main.myPlayer;
						if (flag10)
						{
							NetMessage.SendData(21, -1, -1, "", num61, 0f, 0f, 0f, 0, 0, 0);
							if (num62 == 0)
							{
								Main.item[num61].ownIgnore = this.whoAmI;
								Main.item[num61].ownTime = 100;
							}
							Main.item[num61].FindOwner(num61);
							return;
						}
						NetMessage.SendData(21, -1, this.whoAmI, "", num61, 0f, 0f, 0f, 0, 0, 0);
						return;
					}
				}
				break;
			}
			case 22:
			{
				int num65 = (int)this.reader.ReadInt16();
				int num66 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && Main.item[num65].owner != this.whoAmI)
				{
					return;
				}
				Main.item[num65].owner = num66;
				if (num66 == Main.myPlayer)
				{
					Main.item[num65].keepTime = 15;
				}
				else
				{
					Main.item[num65].keepTime = 0;
				}
				if (Main.netMode == 2)
				{
					Main.item[num65].owner = 255;
					Main.item[num65].keepTime = 15;
					NetMessage.SendData(22, -1, -1, "", num65, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 23:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num67 = (int)this.reader.ReadInt16();
				Vector2 vector = this.reader.ReadVector2();
				Vector2 velocity2 = this.reader.ReadVector2();
				int target = (int)this.reader.ReadUInt16();
				BitsByte bitsByte13 = this.reader.ReadByte();
				float[] array2 = new float[NPC.maxAI];
				for (int num68 = 0; num68 < NPC.maxAI; num68++)
				{
					if (bitsByte13[num68 + 2])
					{
						array2[num68] = this.reader.ReadSingle();
					}
					else
					{
						array2[num68] = 0f;
					}
				}
				int num69 = (int)this.reader.ReadInt16();
				int num70 = 0;
				if (!bitsByte13[7])
				{
					byte b6 = this.reader.ReadByte();
					if (b6 == 2)
					{
						num70 = (int)this.reader.ReadInt16();
					}
					else if (b6 == 4)
					{
						num70 = this.reader.ReadInt32();
					}
					else
					{
						num70 = (int)this.reader.ReadSByte();
					}
				}
				int num71 = -1;
				NPC nPC = Main.npc[num67];
				if (!nPC.active || nPC.netID != num69)
				{
					if (nPC.active)
					{
						num71 = nPC.type;
					}
					nPC.active = true;
					nPC.netDefaults(num69);
				}
				if (Vector2.DistanceSquared(nPC.position, vector) < 6400f)
				{
					nPC.visualOffset = nPC.position - vector;
				}
				nPC.position = vector;
				nPC.velocity = velocity2;
				nPC.target = target;
				nPC.direction = (bitsByte13[0] ? 1 : -1);
				nPC.directionY = (bitsByte13[1] ? 1 : -1);
				nPC.spriteDirection = (bitsByte13[6] ? 1 : -1);
				if (bitsByte13[7])
				{
					num70 = (nPC.life = nPC.lifeMax);
				}
				else
				{
					nPC.life = num70;
				}
				if (num70 <= 0)
				{
					nPC.active = false;
				}
				for (int num72 = 0; num72 < NPC.maxAI; num72++)
				{
					nPC.ai[num72] = array2[num72];
				}
				if (num71 > -1 && num71 != nPC.type)
				{
					nPC.TransformVisuals(num71, nPC.type);
				}
				if (num69 == 262)
				{
					NPC.plantBoss = num67;
				}
				if (num69 == 245)
				{
					NPC.golemBoss = num67;
				}
				if (nPC.type >= 0 && nPC.type < 580 && Main.npcCatchable[nPC.type])
				{
					nPC.releaseOwner = (short)this.reader.ReadByte();
					return;
				}
				return;
			}
			case 24:
			{
				int num73 = (int)this.reader.ReadInt16();
				int num74 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num74 = this.whoAmI;
				}
				Player player7 = Main.player[num74];
				Main.npc[num73].StrikeNPC(player7.inventory[player7.selectedItem].damage, player7.inventory[player7.selectedItem].knockBack, player7.direction, false, false, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(24, -1, this.whoAmI, "", num73, (float)num74, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(23, -1, -1, "", num73, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 25:
			{
				int num75 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num75 = this.whoAmI;
				}
				Color c = this.reader.ReadRGB();
				if (Main.netMode == 2)
				{
					c = new Color(255, 255, 255);
				}
				string text = this.reader.ReadString();
				if (Main.netMode == 1)
				{
					string text2 = text;
					if (num75 < 255)
					{
						text2 = NameTagHandler.GenerateTag(Main.player[num75].name) + " " + text;
						Main.player[num75].chatOverhead.NewMessage(text, Main.chatLength / 2);
					}
					Main.NewTextMultiline(text2, false, c, -1);
					return;
				}
				if (Main.netMode != 2)
				{
					return;
				}
				string text3 = text.ToLower();
				if (text3 == Lang.mp[6] || text3 == Lang.mp[21])
				{
					string text4 = "";
					for (int num76 = 0; num76 < 255; num76++)
					{
						if (Main.player[num76].active)
						{
							if (text4 == "")
							{
								text4 = Main.player[num76].name;
							}
							else
							{
								text4 = text4 + ", " + Main.player[num76].name;
							}
						}
					}
					NetMessage.SendData(25, this.whoAmI, -1, Lang.mp[7] + " " + text4 + ".", 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				if (text3.StartsWith("/me "))
				{
					NetMessage.SendData(25, -1, -1, "*" + Main.player[this.whoAmI].name + " " + text.Substring(4), 255, 200f, 100f, 0f, 0, 0, 0);
					return;
				}
				if (text3 == Lang.mp[8])
				{
					NetMessage.SendData(25, -1, -1, string.Concat(new object[]
					{
						"*",
						Main.player[this.whoAmI].name,
						" ",
						Lang.mp[9],
						" ",
						Main.rand.Next(1, 101)
					}), 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				if (text3.StartsWith("/p "))
				{
					int team = Main.player[this.whoAmI].team;
					c = Main.teamColor[team];
					if (team != 0)
					{
						for (int num77 = 0; num77 < 255; num77++)
						{
							if (Main.player[num77].team == team)
							{
								NetMessage.SendData(25, num77, -1, text.Substring(3), num75, (float)c.R, (float)c.G, (float)c.B, 0, 0, 0);
							}
						}
						return;
					}
					NetMessage.SendData(25, this.whoAmI, -1, Lang.mp[10], 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				else
				{
					if (Main.player[this.whoAmI].difficulty == 2)
					{
						c = Main.hcColor;
					}
					else if (Main.player[this.whoAmI].difficulty == 1)
					{
						c = Main.mcColor;
					}
					NetMessage.SendData(25, -1, -1, text, num75, (float)c.R, (float)c.G, (float)c.B, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine("<" + Main.player[this.whoAmI].name + "> " + text);
						return;
					}
					return;
				}
				break;
			}
			case 26:
			{
				int num78 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && this.whoAmI != num78 && (!Main.player[num78].hostile || !Main.player[this.whoAmI].hostile))
				{
					return;
				}
				int num79 = (int)(this.reader.ReadByte() - 1);
				int num80 = (int)this.reader.ReadInt16();
				string text5 = this.reader.ReadString();
				BitsByte bitsByte14 = this.reader.ReadByte();
				bool flag11 = bitsByte14[0];
				bool flag12 = bitsByte14[1];
				int num81 = bitsByte14[2] ? 0 : -1;
				if (bitsByte14[3])
				{
					num81 = 1;
				}
				if (bitsByte14[4])
				{
					num81 = 2;
				}
				Main.player[num78].HurtOld(num80, num79, flag11, true, text5, flag12, num81);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(26, -1, this.whoAmI, text5, num78, (float)num79, (float)num80, (float)(flag11 ? 1 : 0), flag12 ? 1 : 0, num81, 0);
					return;
				}
				return;
			}
			case 27:
			{
				int num82 = (int)this.reader.ReadInt16();
				Vector2 position2 = this.reader.ReadVector2();
				Vector2 velocity3 = this.reader.ReadVector2();
				float knockBack = this.reader.ReadSingle();
				int damage = (int)this.reader.ReadInt16();
				int num83 = (int)this.reader.ReadByte();
				int num84 = (int)this.reader.ReadInt16();
				BitsByte bitsByte15 = this.reader.ReadByte();
				float[] array3 = new float[Projectile.maxAI];
				for (int num85 = 0; num85 < Projectile.maxAI; num85++)
				{
					if (bitsByte15[num85])
					{
						array3[num85] = this.reader.ReadSingle();
					}
					else
					{
						array3[num85] = 0f;
					}
				}
				int num86 = (int)(bitsByte15[Projectile.maxAI] ? this.reader.ReadInt16() : -1);
				if (num86 >= 1000)
				{
					num86 = -1;
				}
				if (Main.netMode == 2)
				{
					num83 = this.whoAmI;
					if (Main.projHostile[num84])
					{
						return;
					}
				}
				int num87 = 1000;
				for (int num88 = 0; num88 < 1000; num88++)
				{
					if (Main.projectile[num88].owner == num83 && Main.projectile[num88].identity == num82 && Main.projectile[num88].active)
					{
						num87 = num88;
						break;
					}
				}
				if (num87 == 1000)
				{
					for (int num89 = 0; num89 < 1000; num89++)
					{
						if (!Main.projectile[num89].active)
						{
							num87 = num89;
							break;
						}
					}
				}
				Projectile projectile = Main.projectile[num87];
				if (!projectile.active || projectile.type != num84)
				{
					projectile.SetDefaults(num84);
					if (Main.netMode == 2)
					{
						Netplay.Clients[this.whoAmI].SpamProjectile += 1f;
					}
				}
				projectile.identity = num82;
				projectile.position = position2;
				projectile.velocity = velocity3;
				projectile.type = num84;
				projectile.damage = damage;
				projectile.knockBack = knockBack;
				projectile.owner = num83;
				for (int num90 = 0; num90 < Projectile.maxAI; num90++)
				{
					projectile.ai[num90] = array3[num90];
				}
				if (num86 >= 0)
				{
					projectile.projUUID = num86;
					Main.projectileIdentity[num83, num86] = num87;
				}
				projectile.ProjectileFixDesperation();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(27, -1, this.whoAmI, "", num87, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 28:
			{
				int num91 = (int)this.reader.ReadInt16();
				int num92 = (int)this.reader.ReadInt16();
				float num93 = this.reader.ReadSingle();
				int num94 = (int)(this.reader.ReadByte() - 1);
				byte b7 = this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					if (num92 < 0)
					{
						num92 = 0;
					}
					Main.npc[num91].PlayerInteraction(this.whoAmI);
				}
				if (num92 >= 0)
				{
					Main.npc[num91].StrikeNPC(num92, num93, num94, b7 == 1, false, true);
				}
				else
				{
					Main.npc[num91].life = 0;
					Main.npc[num91].HitEffect(0, 10.0);
					Main.npc[num91].active = false;
				}
				if (Main.netMode != 2)
				{
					return;
				}
				NetMessage.SendData(28, -1, this.whoAmI, "", num91, (float)num92, num93, (float)num94, (int)b7, 0, 0);
				if (Main.npc[num91].life <= 0)
				{
					NetMessage.SendData(23, -1, -1, "", num91, 0f, 0f, 0f, 0, 0, 0);
				}
				else
				{
					Main.npc[num91].netUpdate = true;
				}
				if (Main.npc[num91].realLife < 0)
				{
					return;
				}
				if (Main.npc[Main.npc[num91].realLife].life <= 0)
				{
					NetMessage.SendData(23, -1, -1, "", Main.npc[num91].realLife, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Main.npc[Main.npc[num91].realLife].netUpdate = true;
				return;
			}
			case 29:
			{
				int num95 = (int)this.reader.ReadInt16();
				int num96 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num96 = this.whoAmI;
				}
				for (int num97 = 0; num97 < 1000; num97++)
				{
					if (Main.projectile[num97].owner == num96 && Main.projectile[num97].identity == num95 && Main.projectile[num97].active)
					{
						Main.projectile[num97].Kill();
						break;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(29, -1, this.whoAmI, "", num95, (float)num96, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 30:
			{
				int num98 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num98 = this.whoAmI;
				}
				bool flag13 = this.reader.ReadBoolean();
				Main.player[num98].hostile = flag13;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(30, -1, this.whoAmI, "", num98, 0f, 0f, 0f, 0, 0, 0);
					string str = " " + Lang.mp[flag13 ? 11 : 12];
					Color color = Main.teamColor[Main.player[num98].team];
					NetMessage.SendData(25, -1, -1, Main.player[num98].name + str, 255, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
					return;
				}
				return;
			}
			case 31:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x = (int)this.reader.ReadInt16();
				int y = (int)this.reader.ReadInt16();
				int num99 = Chest.FindChest(x, y);
				if (num99 > -1 && Chest.UsingChest(num99) == -1)
				{
					for (int num100 = 0; num100 < 40; num100++)
					{
						NetMessage.SendData(32, this.whoAmI, -1, "", num99, (float)num100, 0f, 0f, 0, 0, 0);
					}
					NetMessage.SendData(33, this.whoAmI, -1, "", num99, 0f, 0f, 0f, 0, 0, 0);
					Main.player[this.whoAmI].chest = num99;
					if (Main.myPlayer == this.whoAmI)
					{
						Main.recBigList = false;
					}
					NetMessage.SendData(80, -1, this.whoAmI, "", this.whoAmI, (float)num99, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 32:
			{
				int num101 = (int)this.reader.ReadInt16();
				int num102 = (int)this.reader.ReadByte();
				int stack4 = (int)this.reader.ReadInt16();
				int pre2 = (int)this.reader.ReadByte();
				int type5 = (int)this.reader.ReadInt16();
				if (Main.chest[num101] == null)
				{
					Main.chest[num101] = new Chest(false);
				}
				if (Main.chest[num101].item[num102] == null)
				{
					Main.chest[num101].item[num102] = new Item();
				}
				Main.chest[num101].item[num102].netDefaults(type5);
				Main.chest[num101].item[num102].Prefix(pre2);
				Main.chest[num101].item[num102].stack = stack4;
				Recipe.FindRecipes();
				return;
			}
			case 33:
			{
				int num103 = (int)this.reader.ReadInt16();
				int num104 = (int)this.reader.ReadInt16();
				int num105 = (int)this.reader.ReadInt16();
				int num106 = (int)this.reader.ReadByte();
				string text6 = string.Empty;
				if (num106 != 0)
				{
					if (num106 <= 20)
					{
						text6 = this.reader.ReadString();
					}
					else if (num106 != 255)
					{
						num106 = 0;
					}
				}
				if (Main.netMode != 1)
				{
					if (num106 != 0)
					{
						int chest = Main.player[this.whoAmI].chest;
						Chest chest2 = Main.chest[chest];
						chest2.name = text6;
						NetMessage.SendData(69, -1, this.whoAmI, text6, chest, (float)chest2.x, (float)chest2.y, 0f, 0, 0, 0);
					}
					Main.player[this.whoAmI].chest = num103;
					Recipe.FindRecipes();
					NetMessage.SendData(80, -1, this.whoAmI, "", this.whoAmI, (float)num103, 0f, 0f, 0, 0, 0);
					return;
				}
				Player player8 = Main.player[Main.myPlayer];
				if (player8.chest == -1)
				{
					Main.playerInventory = true;
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
				}
				else if (player8.chest != num103 && num103 != -1)
				{
					Main.playerInventory = true;
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					Main.recBigList = false;
				}
				else if (player8.chest != -1 && num103 == -1)
				{
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.recBigList = false;
				}
				player8.chest = num103;
				player8.chestX = num104;
				player8.chestY = num105;
				Recipe.FindRecipes();
				if (Main.tile[num104, num105].frameX >= 36 && Main.tile[num104, num105].frameX < 72)
				{
					AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
					return;
				}
				return;
			}
			case 34:
			{
				byte b8 = this.reader.ReadByte();
				int num107 = (int)this.reader.ReadInt16();
				int num108 = (int)this.reader.ReadInt16();
				int num109 = (int)this.reader.ReadInt16();
				int num110 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num110 = 0;
				}
				if (Main.netMode == 2)
				{
					if (b8 == 0)
					{
						int num111 = WorldGen.PlaceChest(num107, num108, 21, false, num109);
						if (num111 == -1)
						{
							NetMessage.SendData(34, this.whoAmI, -1, "", (int)b8, (float)num107, (float)num108, (float)num109, num111, 0, 0);
							Item.NewItem(num107 * 16, num108 * 16, 32, 32, Chest.chestItemSpawn[num109], 1, true, 0, false, false);
							return;
						}
						NetMessage.SendData(34, -1, -1, "", (int)b8, (float)num107, (float)num108, (float)num109, num111, 0, 0);
						return;
					}
					else if (b8 == 2)
					{
						int num112 = WorldGen.PlaceChest(num107, num108, 88, false, num109);
						if (num112 == -1)
						{
							NetMessage.SendData(34, this.whoAmI, -1, "", (int)b8, (float)num107, (float)num108, (float)num109, num112, 0, 0);
							Item.NewItem(num107 * 16, num108 * 16, 32, 32, Chest.dresserItemSpawn[num109], 1, true, 0, false, false);
							return;
						}
						NetMessage.SendData(34, -1, -1, "", (int)b8, (float)num107, (float)num108, (float)num109, num112, 0, 0);
						return;
					}
					else
					{
						Tile tile2 = Main.tile[num107, num108];
						if (tile2.type == 21 && b8 == 1)
						{
							if (tile2.frameX % 36 != 0)
							{
								num107--;
							}
							if (tile2.frameY % 36 != 0)
							{
								num108--;
							}
							int number = Chest.FindChest(num107, num108);
							WorldGen.KillTile(num107, num108, false, false, false);
							if (!tile2.active())
							{
								NetMessage.SendData(34, -1, -1, "", (int)b8, (float)num107, (float)num108, 0f, number, 0, 0);
								return;
							}
							return;
						}
						else
						{
							if (tile2.type != 88 || b8 != 3)
							{
								return;
							}
							num107 -= (int)(tile2.frameX % 54 / 18);
							if (tile2.frameY % 36 != 0)
							{
								num108--;
							}
							int number2 = Chest.FindChest(num107, num108);
							WorldGen.KillTile(num107, num108, false, false, false);
							if (!tile2.active())
							{
								NetMessage.SendData(34, -1, -1, "", (int)b8, (float)num107, (float)num108, 0f, number2, 0, 0);
								return;
							}
							return;
						}
					}
				}
				else if (b8 == 0)
				{
					if (num110 == -1)
					{
						WorldGen.KillTile(num107, num108, false, false, false);
						return;
					}
					WorldGen.PlaceChestDirect(num107, num108, 21, num109, num110);
					return;
				}
				else
				{
					if (b8 != 2)
					{
						Chest.DestroyChestDirect(num107, num108, num110);
						WorldGen.KillTile(num107, num108, false, false, false);
						return;
					}
					if (num110 == -1)
					{
						WorldGen.KillTile(num107, num108, false, false, false);
						return;
					}
					WorldGen.PlaceDresserDirect(num107, num108, 88, num109, num110);
					return;
				}
				break;
			}
			case 35:
			{
				int num113 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num113 = this.whoAmI;
				}
				int num114 = (int)this.reader.ReadInt16();
				if (num113 != Main.myPlayer || Main.ServerSideCharacter)
				{
					Main.player[num113].HealEffect(num114, true);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(35, -1, this.whoAmI, "", num113, (float)num114, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 36:
			{
				int num115 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num115 = this.whoAmI;
				}
				Player player9 = Main.player[num115];
				player9.zone1 = this.reader.ReadByte();
				player9.zone2 = this.reader.ReadByte();
				player9.zone3 = this.reader.ReadByte();
				player9.zone4 = this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(36, -1, this.whoAmI, "", num115, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 37:
				if (Main.netMode != 1)
				{
					return;
				}
				if (Main.autoPass)
				{
					NetMessage.SendData(38, -1, -1, Netplay.ServerPassword, 0, 0f, 0f, 0f, 0, 0, 0);
					Main.autoPass = false;
					return;
				}
				Netplay.ServerPassword = "";
				Main.menuMode = 31;
				return;
			case 38:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				string a2 = this.reader.ReadString();
				if (a2 == Netplay.ServerPassword)
				{
					Netplay.Clients[this.whoAmI].State = 1;
					NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1], 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 39:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num116 = (int)this.reader.ReadInt16();
				Main.item[num116].owner = 255;
				NetMessage.SendData(22, -1, -1, "", num116, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 40:
			{
				int num117 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num117 = this.whoAmI;
				}
				int talkNPC = (int)this.reader.ReadInt16();
				Main.player[num117].talkNPC = talkNPC;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(40, -1, this.whoAmI, "", num117, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 41:
			{
				int num118 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num118 = this.whoAmI;
				}
				Player player10 = Main.player[num118];
				float itemRotation = this.reader.ReadSingle();
				int itemAnimation = (int)this.reader.ReadInt16();
				player10.itemRotation = itemRotation;
				player10.itemAnimation = itemAnimation;
				player10.channel = player10.inventory[player10.selectedItem].channel;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(41, -1, this.whoAmI, "", num118, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 42:
			{
				int num119 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num119 = this.whoAmI;
				}
				else if (Main.myPlayer == num119 && !Main.ServerSideCharacter)
				{
					return;
				}
				int statMana = (int)this.reader.ReadInt16();
				int statManaMax = (int)this.reader.ReadInt16();
				Main.player[num119].statMana = statMana;
				Main.player[num119].statManaMax = statManaMax;
				return;
			}
			case 43:
			{
				int num120 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num120 = this.whoAmI;
				}
				int num121 = (int)this.reader.ReadInt16();
				if (num120 != Main.myPlayer)
				{
					Main.player[num120].ManaEffect(num121);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(43, -1, this.whoAmI, "", num120, (float)num121, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 44:
			{
				int num122 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num122 = this.whoAmI;
				}
				int num123 = (int)(this.reader.ReadByte() - 1);
				int num124 = (int)this.reader.ReadInt16();
				byte b9 = this.reader.ReadByte();
				string text7 = this.reader.ReadString();
				Main.player[num122].KillMeOld((double)num124, num123, b9 == 1, text7);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(44, -1, this.whoAmI, text7, num122, (float)num123, (float)num124, (float)b9, 0, 0, 0);
					return;
				}
				return;
			}
			case 45:
			{
				int num125 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num125 = this.whoAmI;
				}
				int num126 = (int)this.reader.ReadByte();
				Player player11 = Main.player[num125];
				int team2 = player11.team;
				player11.team = num126;
				Color color2 = Main.teamColor[num126];
				if (Main.netMode == 2)
				{
					NetMessage.SendData(45, -1, this.whoAmI, "", num125, 0f, 0f, 0f, 0, 0, 0);
					string str2 = " " + Lang.mp[13 + num126];
					if (num126 == 5)
					{
						str2 = " " + Lang.mp[22];
					}
					for (int num127 = 0; num127 < 255; num127++)
					{
						if (num127 == this.whoAmI || (team2 > 0 && Main.player[num127].team == team2) || (num126 > 0 && Main.player[num127].team == num126))
						{
							NetMessage.SendData(25, num127, -1, player11.name + str2, 255, (float)color2.R, (float)color2.G, (float)color2.B, 0, 0, 0);
						}
					}
					return;
				}
				return;
			}
			case 46:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int i2 = (int)this.reader.ReadInt16();
				int j2 = (int)this.reader.ReadInt16();
				int num128 = Sign.ReadSign(i2, j2, true);
				if (num128 >= 0)
				{
					NetMessage.SendData(47, this.whoAmI, -1, "", num128, (float)this.whoAmI, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 47:
			{
				int num129 = (int)this.reader.ReadInt16();
				int x2 = (int)this.reader.ReadInt16();
				int y2 = (int)this.reader.ReadInt16();
				string text8 = this.reader.ReadString();
				string a3 = null;
				if (Main.sign[num129] != null)
				{
					a3 = Main.sign[num129].text;
				}
				Main.sign[num129] = new Sign();
				Main.sign[num129].x = x2;
				Main.sign[num129].y = y2;
				Sign.TextSign(num129, text8);
				int num130 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && a3 != text8)
				{
					num130 = this.whoAmI;
					NetMessage.SendData(47, -1, this.whoAmI, "", num129, (float)num130, 0f, 0f, 0, 0, 0);
				}
				if (Main.netMode == 1 && num130 == Main.myPlayer && Main.sign[num129] != null)
				{
					Main.playerInventory = false;
					Main.player[Main.myPlayer].talkNPC = -1;
					Main.npcChatCornerItem = 0;
					Main.editSign = false;
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
					Main.player[Main.myPlayer].sign = num129;
					Main.npcChatText = Main.sign[num129].text;
					return;
				}
				return;
			}
			case 48:
			{
				int num131 = (int)this.reader.ReadInt16();
				int num132 = (int)this.reader.ReadInt16();
				byte liquid = this.reader.ReadByte();
				byte liquidType = this.reader.ReadByte();
				if (Main.netMode == 2 && Netplay.spamCheck)
				{
					int num133 = this.whoAmI;
					int num134 = (int)(Main.player[num133].position.X + (float)(Main.player[num133].width / 2));
					int num135 = (int)(Main.player[num133].position.Y + (float)(Main.player[num133].height / 2));
					int num136 = 10;
					int num137 = num134 - num136;
					int num138 = num134 + num136;
					int num139 = num135 - num136;
					int num140 = num135 + num136;
					if (num131 < num137 || num131 > num138 || num132 < num139 || num132 > num140)
					{
						NetMessage.BootPlayer(this.whoAmI, Language.GetTextValue("Net.CheatingLiquidSpam"));
						return;
					}
				}
				if (Main.tile[num131, num132] == null)
				{
					Main.tile[num131, num132] = new Tile();
				}
				lock (Main.tile[num131, num132])
				{
					Main.tile[num131, num132].liquid = liquid;
					Main.tile[num131, num132].liquidType((int)liquidType);
					if (Main.netMode == 2)
					{
						WorldGen.SquareTileFrame(num131, num132, true);
					}
					return;
				}
				goto IL_4C6D;
			}
			case 49:
				goto IL_4C6D;
			case 50:
			{
				int num141 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num141 = this.whoAmI;
				}
				else if (num141 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				Player player12 = Main.player[num141];
				for (int num142 = 0; num142 < 22; num142++)
				{
					player12.buffType[num142] = (int)this.reader.ReadByte();
					if (player12.buffType[num142] > 0)
					{
						player12.buffTime[num142] = 60;
					}
					else
					{
						player12.buffTime[num142] = 0;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(50, -1, this.whoAmI, "", num141, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 51:
			{
				byte b10 = this.reader.ReadByte();
				byte b11 = this.reader.ReadByte();
				if (b11 == 1)
				{
					NPC.SpawnSkeletron();
					return;
				}
				if (b11 == 2)
				{
					if (Main.netMode == 2)
					{
						NetMessage.SendData(51, -1, this.whoAmI, "", (int)b10, (float)b11, 0f, 0f, 0, 0, 0);
						return;
					}
					Main.PlaySound(SoundID.Item1, (int)Main.player[(int)b10].position.X, (int)Main.player[(int)b10].position.Y);
					return;
				}
				else if (b11 == 3)
				{
					if (Main.netMode == 2)
					{
						Main.Sundialing();
						return;
					}
					return;
				}
				else
				{
					if (b11 == 4)
					{
						Main.npc[(int)b10].BigMimicSpawnSmoke();
						return;
					}
					return;
				}
				break;
			}
			case 52:
			{
				int num143 = (int)this.reader.ReadByte();
				int num144 = (int)this.reader.ReadInt16();
				int num145 = (int)this.reader.ReadInt16();
				if (num143 == 1)
				{
					Chest.Unlock(num144, num145);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(52, -1, this.whoAmI, "", 0, (float)num143, (float)num144, (float)num145, 0, 0, 0);
						NetMessage.SendTileSquare(-1, num144, num145, 2, TileChangeType.None);
					}
				}
				if (num143 != 2)
				{
					return;
				}
				WorldGen.UnlockDoor(num144, num145);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(52, -1, this.whoAmI, "", 0, (float)num143, (float)num144, (float)num145, 0, 0, 0);
					NetMessage.SendTileSquare(-1, num144, num145, 2, TileChangeType.None);
					return;
				}
				return;
			}
			case 53:
			{
				int num146 = (int)this.reader.ReadInt16();
				int type6 = (int)this.reader.ReadByte();
				int time = (int)this.reader.ReadInt16();
				Main.npc[num146].AddBuff(type6, time, true);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(54, -1, -1, "", num146, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 54:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num147 = (int)this.reader.ReadInt16();
				NPC nPC2 = Main.npc[num147];
				for (int num148 = 0; num148 < 5; num148++)
				{
					nPC2.buffType[num148] = (int)this.reader.ReadByte();
					nPC2.buffTime[num148] = (int)this.reader.ReadInt16();
				}
				return;
			}
			case 55:
			{
				int num149 = (int)this.reader.ReadByte();
				int num150 = (int)this.reader.ReadByte();
				int num151 = this.reader.ReadInt32();
				if (Main.netMode == 2 && num149 != this.whoAmI && !Main.pvpBuff[num150])
				{
					return;
				}
				if (Main.netMode == 1 && num149 == Main.myPlayer)
				{
					Main.player[num149].AddBuff(num150, num151, true);
					return;
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(55, num149, -1, "", num149, (float)num150, (float)num151, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 56:
			{
				int num152 = (int)this.reader.ReadInt16();
				if (num152 < 0 || num152 >= 200)
				{
					return;
				}
				string displayName = this.reader.ReadString();
				if (Main.netMode == 1)
				{
					Main.npc[num152].displayName = displayName;
					return;
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(56, this.whoAmI, -1, Main.npc[num152].displayName, num152, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 57:
				if (Main.netMode != 1)
				{
					return;
				}
				WorldGen.tGood = this.reader.ReadByte();
				WorldGen.tEvil = this.reader.ReadByte();
				WorldGen.tBlood = this.reader.ReadByte();
				return;
			case 58:
			{
				int num153 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num153 = this.whoAmI;
				}
				float num154 = this.reader.ReadSingle();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(58, -1, this.whoAmI, "", this.whoAmI, num154, 0f, 0f, 0, 0, 0);
					return;
				}
				Player player13 = Main.player[num153];
				Main.harpNote = num154;
				LegacySoundStyle type7 = SoundID.Item26;
				if (player13.inventory[player13.selectedItem].type == 507)
				{
					type7 = SoundID.Item35;
				}
				Main.PlaySound(type7, player13.position);
				return;
			}
			case 59:
			{
				int num155 = (int)this.reader.ReadInt16();
				int num156 = (int)this.reader.ReadInt16();
				Wiring.SetCurrentUser(this.whoAmI);
				Wiring.HitSwitch(num155, num156);
				Wiring.SetCurrentUser(-1);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(59, -1, this.whoAmI, "", num155, (float)num156, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 60:
			{
				int num157 = (int)this.reader.ReadInt16();
				int num158 = (int)this.reader.ReadInt16();
				int num159 = (int)this.reader.ReadInt16();
				byte b12 = this.reader.ReadByte();
				if (num157 >= 200)
				{
					NetMessage.BootPlayer(this.whoAmI, Language.GetTextValue("Net.CheatingInvalid"));
					return;
				}
				if (Main.netMode == 1)
				{
					Main.npc[num157].homeless = (b12 == 1);
					Main.npc[num157].homeTileX = num158;
					Main.npc[num157].homeTileY = num159;
					return;
				}
				if (b12 == 0)
				{
					WorldGen.kickOut(num157);
					return;
				}
				WorldGen.moveRoom(num158, num159, num157);
				return;
			}
			case 61:
			{
				int plr = (int)this.reader.ReadInt16();
				int num160 = (int)this.reader.ReadInt16();
				if (Main.netMode != 2)
				{
					return;
				}
				if (num160 >= 0 && num160 < 580 && NPCID.Sets.MPAllowedEnemies[num160])
				{
					bool flag15 = !NPC.AnyNPCs(num160);
					if (flag15)
					{
						NPC.SpawnOnPlayer(plr, num160);
						return;
					}
					return;
				}
				else if (num160 == -4)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[31], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.startPumpkinMoon();
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 2f, 1f, 0, 0, 0);
						return;
					}
					return;
				}
				else if (num160 == -5)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[34], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.startSnowMoon();
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 1f, 1f, 0, 0, 0);
						return;
					}
					return;
				}
				else if (num160 == -6)
				{
					if (Main.dayTime && !Main.eclipse)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[20], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.eclipse = true;
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						return;
					}
					return;
				}
				else
				{
					if (num160 == -7)
					{
						NetMessage.SendData(25, -1, -1, "martian moon toggled", 255, 50f, 255f, 130f, 0, 0, 0);
						Main.invasionDelay = 0;
						Main.StartInvasion(4);
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, (float)(Main.invasionType + 3), 0f, 0, 0, 0);
						return;
					}
					if (num160 == -8)
					{
						if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
						{
							WorldGen.StartImpendingDoom();
							NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
							return;
						}
						return;
					}
					else
					{
						if (num160 < 0)
						{
							int num161 = 1;
							if (num160 > -5)
							{
								num161 = -num160;
							}
							if (num161 > 0 && Main.invasionType == 0)
							{
								Main.invasionDelay = 0;
								Main.StartInvasion(num161);
							}
							NetMessage.SendData(78, -1, -1, "", 0, 1f, (float)(Main.invasionType + 3), 0f, 0, 0, 0);
							return;
						}
						return;
					}
				}
				break;
			}
			case 62:
			{
				int num162 = (int)this.reader.ReadByte();
				int num163 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num162 = this.whoAmI;
				}
				if (num163 == 1)
				{
					Main.player[num162].NinjaDodge();
				}
				if (num163 == 2)
				{
					Main.player[num162].ShadowDodge();
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(62, -1, this.whoAmI, "", num162, (float)num163, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 63:
			{
				int num164 = (int)this.reader.ReadInt16();
				int num165 = (int)this.reader.ReadInt16();
				byte b13 = this.reader.ReadByte();
				WorldGen.paintTile(num164, num165, b13, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(63, -1, this.whoAmI, "", num164, (float)num165, (float)b13, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 64:
			{
				int num166 = (int)this.reader.ReadInt16();
				int num167 = (int)this.reader.ReadInt16();
				byte b14 = this.reader.ReadByte();
				WorldGen.paintWall(num166, num167, b14, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(64, -1, this.whoAmI, "", num166, (float)num167, (float)b14, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 65:
			{
				BitsByte bitsByte16 = this.reader.ReadByte();
				int num168 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num168 = this.whoAmI;
				}
				Vector2 vector2 = this.reader.ReadVector2();
				int num169 = 0;
				int num170 = 0;
				if (bitsByte16[0])
				{
					num169++;
				}
				if (bitsByte16[1])
				{
					num169 += 2;
				}
				if (bitsByte16[2])
				{
					num170++;
				}
				if (bitsByte16[3])
				{
					num170 += 2;
				}
				if (num169 == 0)
				{
					Main.player[num168].Teleport(vector2, num170, 0);
				}
				else if (num169 == 1)
				{
					Main.npc[num168].Teleport(vector2, num170, 0);
				}
				else if (num169 == 2)
				{
					Main.player[num168].Teleport(vector2, num170, 0);
					if (Main.netMode == 2)
					{
						RemoteClient.CheckSection(this.whoAmI, vector2, 1);
						NetMessage.SendData(65, -1, -1, "", 0, (float)num168, vector2.X, vector2.Y, num170, 0, 0);
						int num171 = -1;
						float num172 = 9999f;
						for (int num173 = 0; num173 < 255; num173++)
						{
							if (Main.player[num173].active && num173 != this.whoAmI)
							{
								Vector2 vector3 = Main.player[num173].position - Main.player[this.whoAmI].position;
								if (vector3.Length() < num172)
								{
									num172 = vector3.Length();
									num171 = num173;
								}
							}
						}
						if (num171 >= 0)
						{
							NetMessage.SendData(25, -1, -1, Language.GetTextValue("Game.HasTeleportedTo", Main.player[this.whoAmI].name, Main.player[num171].name), 255, 250f, 250f, 0f, 0, 0, 0);
						}
					}
				}
				if (Main.netMode == 2 && num169 == 0)
				{
					NetMessage.SendData(65, -1, this.whoAmI, "", 0, (float)num168, vector2.X, vector2.Y, num170, 0, 0);
					return;
				}
				return;
			}
			case 66:
			{
				int num174 = (int)this.reader.ReadByte();
				int num175 = (int)this.reader.ReadInt16();
				if (num175 <= 0)
				{
					return;
				}
				Player player14 = Main.player[num174];
				player14.statLife += num175;
				if (player14.statLife > player14.statLifeMax2)
				{
					player14.statLife = player14.statLifeMax2;
				}
				player14.HealEffect(num175, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(66, -1, this.whoAmI, "", num174, (float)num175, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 68:
				this.reader.ReadString();
				return;
			case 69:
			{
				int num176 = (int)this.reader.ReadInt16();
				int num177 = (int)this.reader.ReadInt16();
				int num178 = (int)this.reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num176 < 0 || num176 >= 1000)
					{
						return;
					}
					Chest chest3 = Main.chest[num176];
					if (chest3 == null)
					{
						chest3 = new Chest(false);
						chest3.x = num177;
						chest3.y = num178;
						Main.chest[num176] = chest3;
					}
					else if (chest3.x != num177 || chest3.y != num178)
					{
						return;
					}
					chest3.name = this.reader.ReadString();
					return;
				}
				else
				{
					if (num176 < -1 || num176 >= 1000)
					{
						return;
					}
					if (num176 == -1)
					{
						num176 = Chest.FindChest(num177, num178);
						if (num176 == -1)
						{
							return;
						}
					}
					Chest chest4 = Main.chest[num176];
					if (chest4.x != num177 || chest4.y != num178)
					{
						return;
					}
					NetMessage.SendData(69, this.whoAmI, -1, chest4.name, num176, (float)num177, (float)num178, 0f, 0, 0, 0);
					return;
				}
				break;
			}
			case 70:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int num179 = (int)this.reader.ReadInt16();
				int who = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					who = this.whoAmI;
				}
				if (num179 < 200 && num179 >= 0)
				{
					NPC.CatchNPC(num179, who);
					return;
				}
				return;
			}
			case 71:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x3 = this.reader.ReadInt32();
				int y3 = this.reader.ReadInt32();
				int type8 = (int)this.reader.ReadInt16();
				byte style = this.reader.ReadByte();
				NPC.ReleaseNPC(x3, y3, type8, (int)style, this.whoAmI);
				return;
			}
			case 72:
				if (Main.netMode != 1)
				{
					return;
				}
				for (int num180 = 0; num180 < 40; num180++)
				{
					Main.travelShop[num180] = (int)this.reader.ReadInt16();
				}
				return;
			case 73:
				Main.player[this.whoAmI].TeleportationPotion();
				return;
			case 74:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.anglerQuest = (int)this.reader.ReadByte();
				Main.anglerQuestFinished = this.reader.ReadBoolean();
				return;
			case 75:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				string name = Main.player[this.whoAmI].name;
				if (!Main.anglerWhoFinishedToday.Contains(name))
				{
					Main.anglerWhoFinishedToday.Add(name);
					return;
				}
				return;
			}
			case 76:
			{
				int num181 = (int)this.reader.ReadByte();
				if (num181 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num181 = this.whoAmI;
				}
				Player player15 = Main.player[num181];
				player15.anglerQuestsFinished = this.reader.ReadInt32();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(76, -1, this.whoAmI, "", num181, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 77:
			{
				short type9 = this.reader.ReadInt16();
				ushort tileType = this.reader.ReadUInt16();
				short x4 = this.reader.ReadInt16();
				short y4 = this.reader.ReadInt16();
				Animation.NewTemporaryAnimation((int)type9, tileType, (int)x4, (int)y4);
				return;
			}
			case 78:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.ReportInvasionProgress(this.reader.ReadInt32(), this.reader.ReadInt32(), (int)this.reader.ReadSByte(), (int)this.reader.ReadSByte());
				return;
			case 79:
			{
				int x5 = (int)this.reader.ReadInt16();
				int y5 = (int)this.reader.ReadInt16();
				short type10 = this.reader.ReadInt16();
				int style2 = (int)this.reader.ReadInt16();
				int num182 = (int)this.reader.ReadByte();
				int random = (int)this.reader.ReadSByte();
				int direction;
				if (this.reader.ReadBoolean())
				{
					direction = 1;
				}
				else
				{
					direction = -1;
				}
				if (Main.netMode == 2)
				{
					Netplay.Clients[this.whoAmI].SpamAddBlock += 1f;
					if (!WorldGen.InWorld(x5, y5, 10) || !Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(x5), Netplay.GetSectionY(y5)])
					{
						return;
					}
				}
				WorldGen.PlaceObject(x5, y5, (int)type10, false, style2, num182, random, direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendObjectPlacment(this.whoAmI, x5, y5, (int)type10, style2, num182, random, direction);
					return;
				}
				return;
			}
			case 80:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num183 = (int)this.reader.ReadByte();
				int num184 = (int)this.reader.ReadInt16();
				if (num184 >= -3 && num184 < 1000)
				{
					Main.player[num183].chest = num184;
					Recipe.FindRecipes();
					return;
				}
				return;
			}
			case 81:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int x6 = (int)this.reader.ReadSingle();
				int y6 = (int)this.reader.ReadSingle();
				Color color3 = this.reader.ReadRGB();
				string text9 = this.reader.ReadString();
				CombatText.NewText(new Rectangle(x6, y6, 0, 0), color3, text9, false, false);
				return;
			}
			case 82:
				NetManager.Instance.Read(this.reader, this.whoAmI);
				return;
			case 83:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num185 = (int)this.reader.ReadInt16();
				int num186 = this.reader.ReadInt32();
				if (num185 >= 0 && num185 < 267)
				{
					NPC.killCount[num185] = num186;
					return;
				}
				return;
			}
			case 84:
			{
				int num187 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num187 = this.whoAmI;
				}
				float stealth = this.reader.ReadSingle();
				Main.player[num187].stealth = stealth;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(84, -1, this.whoAmI, "", num187, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 85:
			{
				int num188 = this.whoAmI;
				byte b15 = this.reader.ReadByte();
				if (Main.netMode == 2 && num188 < 255 && b15 < 58)
				{
					Chest.ServerPlaceItem(this.whoAmI, (int)b15);
					return;
				}
				return;
			}
			case 86:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num189 = this.reader.ReadInt32();
				bool flag16 = !this.reader.ReadBoolean();
				if (!flag16)
				{
					TileEntity tileEntity = TileEntity.Read(this.reader, true);
					tileEntity.ID = num189;
					TileEntity.ByID[tileEntity.ID] = tileEntity;
					TileEntity.ByPosition[tileEntity.Position] = tileEntity;
					return;
				}
				TileEntity tileEntity2;
				if (TileEntity.ByID.TryGetValue(num189, out tileEntity2) && (tileEntity2 is TETrainingDummy || tileEntity2 is TEItemFrame || tileEntity2 is TELogicSensor))
				{
					TileEntity.ByID.Remove(num189);
					TileEntity.ByPosition.Remove(tileEntity2.Position);
					return;
				}
				return;
			}
			case 87:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x7 = (int)this.reader.ReadInt16();
				int y7 = (int)this.reader.ReadInt16();
				int type11 = (int)this.reader.ReadByte();
				if (!WorldGen.InWorld(x7, y7, 0))
				{
					return;
				}
				if (TileEntity.ByPosition.ContainsKey(new Point16(x7, y7)))
				{
					return;
				}
				TileEntity.PlaceEntityNet(x7, y7, type11);
				return;
			}
			case 88:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num190 = (int)this.reader.ReadInt16();
				if (num190 < 0 || num190 > 400)
				{
					return;
				}
				Item item4 = Main.item[num190];
				BitsByte bitsByte17 = this.reader.ReadByte();
				if (bitsByte17[0])
				{
					item4.color.PackedValue = this.reader.ReadUInt32();
				}
				if (bitsByte17[1])
				{
					item4.damage = (int)this.reader.ReadUInt16();
				}
				if (bitsByte17[2])
				{
					item4.knockBack = this.reader.ReadSingle();
				}
				if (bitsByte17[3])
				{
					item4.useAnimation = (int)this.reader.ReadUInt16();
				}
				if (bitsByte17[4])
				{
					item4.useTime = (int)this.reader.ReadUInt16();
				}
				if (bitsByte17[5])
				{
					item4.shoot = (int)this.reader.ReadInt16();
				}
				if (bitsByte17[6])
				{
					item4.shootSpeed = this.reader.ReadSingle();
				}
				if (!bitsByte17[7])
				{
					return;
				}
				bitsByte17 = this.reader.ReadByte();
				if (bitsByte17[0])
				{
					item4.width = (int)this.reader.ReadInt16();
				}
				if (bitsByte17[1])
				{
					item4.height = (int)this.reader.ReadInt16();
				}
				if (bitsByte17[2])
				{
					item4.scale = this.reader.ReadSingle();
				}
				if (bitsByte17[3])
				{
					item4.ammo = (int)this.reader.ReadInt16();
				}
				if (bitsByte17[4])
				{
					item4.useAmmo = (int)this.reader.ReadInt16();
				}
				if (bitsByte17[5])
				{
					item4.notAmmo = this.reader.ReadBoolean();
					return;
				}
				return;
			}
			case 89:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x8 = (int)this.reader.ReadInt16();
				int y8 = (int)this.reader.ReadInt16();
				int netid = (int)this.reader.ReadInt16();
				int prefix = (int)this.reader.ReadByte();
				int stack5 = (int)this.reader.ReadInt16();
				TEItemFrame.TryPlacing(x8, y8, netid, prefix, stack5);
				return;
			}
			case 91:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num191 = this.reader.ReadInt32();
				int num192 = (int)this.reader.ReadByte();
				if (num192 != 255)
				{
					int meta = (int)this.reader.ReadUInt16();
					int num193 = (int)this.reader.ReadByte();
					int num194 = (int)this.reader.ReadByte();
					int metadata = 0;
					if (num194 < 0)
					{
						metadata = (int)this.reader.ReadInt16();
					}
					WorldUIAnchor worldUIAnchor = EmoteBubble.DeserializeNetAnchor(num192, meta);
					lock (EmoteBubble.byID)
					{
						if (!EmoteBubble.byID.ContainsKey(num191))
						{
							EmoteBubble.byID[num191] = new EmoteBubble(num194, worldUIAnchor, num193);
						}
						else
						{
							EmoteBubble.byID[num191].lifeTime = num193;
							EmoteBubble.byID[num191].lifeTimeStart = num193;
							EmoteBubble.byID[num191].emote = num194;
							EmoteBubble.byID[num191].anchor = worldUIAnchor;
						}
						EmoteBubble.byID[num191].ID = num191;
						EmoteBubble.byID[num191].metadata = metadata;
						return;
					}
					goto IL_67C4;
				}
				if (EmoteBubble.byID.ContainsKey(num191))
				{
					EmoteBubble.byID.Remove(num191);
					return;
				}
				return;
			}
			case 92:
				goto IL_67C4;
			case 95:
			{
				ushort num195 = this.reader.ReadUInt16();
				if (Main.netMode != 2)
				{
					return;
				}
				if (num195 < 0 || num195 >= 1000)
				{
					return;
				}
				Projectile projectile2 = Main.projectile[(int)num195];
				if (projectile2.type == 602)
				{
					projectile2.Kill();
					NetMessage.SendData(29, -1, -1, "", projectile2.whoAmI, (float)projectile2.owner, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 96:
			{
				int num196 = (int)this.reader.ReadByte();
				Player player16 = Main.player[num196];
				int num197 = (int)this.reader.ReadInt16();
				Vector2 newPos = this.reader.ReadVector2();
				Vector2 velocity4 = this.reader.ReadVector2();
				int lastPortalColorIndex = num197 + ((num197 % 2 == 0) ? 1 : -1);
				player16.lastPortalColorIndex = lastPortalColorIndex;
				player16.Teleport(newPos, 4, num197);
				player16.velocity = velocity4;
				return;
			}
			case 97:
				if (Main.netMode != 1)
				{
					return;
				}
				AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], (int)this.reader.ReadInt16());
				return;
			case 98:
				if (Main.netMode != 1)
				{
					return;
				}
				AchievementsHelper.NotifyProgressionEvent((int)this.reader.ReadInt16());
				return;
			case 99:
			{
				int num198 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num198 = this.whoAmI;
				}
				Player player17 = Main.player[num198];
				player17.MinionRestTargetPoint = this.reader.ReadVector2();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(99, -1, this.whoAmI, "", num198, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 100:
			{
				int num199 = (int)this.reader.ReadUInt16();
				NPC nPC3 = Main.npc[num199];
				int num200 = (int)this.reader.ReadInt16();
				Vector2 newPos2 = this.reader.ReadVector2();
				Vector2 velocity5 = this.reader.ReadVector2();
				int lastPortalColorIndex2 = num200 + ((num200 % 2 == 0) ? 1 : -1);
				nPC3.lastPortalColorIndex = lastPortalColorIndex2;
				nPC3.Teleport(newPos2, 4, num200);
				nPC3.velocity = velocity5;
				return;
			}
			case 101:
				if (Main.netMode == 2)
				{
					return;
				}
				NPC.ShieldStrengthTowerSolar = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerVortex = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerNebula = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerStardust = (int)this.reader.ReadUInt16();
				if (NPC.ShieldStrengthTowerSolar < 0)
				{
					NPC.ShieldStrengthTowerSolar = 0;
				}
				if (NPC.ShieldStrengthTowerVortex < 0)
				{
					NPC.ShieldStrengthTowerVortex = 0;
				}
				if (NPC.ShieldStrengthTowerNebula < 0)
				{
					NPC.ShieldStrengthTowerNebula = 0;
				}
				if (NPC.ShieldStrengthTowerStardust < 0)
				{
					NPC.ShieldStrengthTowerStardust = 0;
				}
				if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
					return;
				}
				return;
			case 102:
			{
				int num201 = (int)this.reader.ReadByte();
				byte b16 = this.reader.ReadByte();
				Vector2 other = this.reader.ReadVector2();
				if (Main.netMode == 2)
				{
					num201 = this.whoAmI;
					NetMessage.SendData(102, -1, -1, "", num201, (float)b16, other.X, other.Y, 0, 0, 0);
					return;
				}
				Player player18 = Main.player[num201];
				for (int num202 = 0; num202 < 255; num202++)
				{
					Player player19 = Main.player[num202];
					if (player19.active && !player19.dead && (player18.team == 0 || player18.team == player19.team) && player19.Distance(other) < 700f)
					{
						Vector2 value = player18.Center - player19.Center;
						Vector2 vector4 = Vector2.Normalize(value);
						if (!vector4.HasNaNs())
						{
							int type12 = 90;
							float num203 = 0f;
							float num204 = 0.209439516f;
							Vector2 spinningpoint = new Vector2(0f, -8f);
							Vector2 value2 = new Vector2(-3f);
							float num205 = 0f;
							float num206 = 0.005f;
							byte b17 = b16;
							if (b17 != 173)
							{
								if (b17 != 176)
								{
									if (b17 == 179)
									{
										type12 = 86;
									}
								}
								else
								{
									type12 = 88;
								}
							}
							else
							{
								type12 = 90;
							}
							int num207 = 0;
							while ((float)num207 < value.Length() / 6f)
							{
								Vector2 position3 = player19.Center + 6f * (float)num207 * vector4 + spinningpoint.RotatedBy((double)num203, default(Vector2)) + value2;
								num203 += num204;
								int num208 = Dust.NewDust(position3, 6, 6, type12, 0f, 0f, 100, default(Color), 1.5f);
								Main.dust[num208].noGravity = true;
								Main.dust[num208].velocity = Vector2.Zero;
								num205 = (Main.dust[num208].fadeIn = num205 + num206);
								Main.dust[num208].velocity += vector4 * 1.5f;
								num207++;
							}
						}
						player19.NebulaLevelup((int)b16);
					}
				}
				return;
			}
			case 103:
				if (Main.netMode == 1)
				{
					NPC.MoonLordCountdown = this.reader.ReadInt32();
					return;
				}
				return;
			case 104:
			{
				if (Main.netMode != 1 || Main.npcShop <= 0)
				{
					return;
				}
				Item[] item5 = Main.instance.shop[Main.npcShop].item;
				int num209 = (int)this.reader.ReadByte();
				int type13 = (int)this.reader.ReadInt16();
				int stack6 = (int)this.reader.ReadInt16();
				int pre3 = (int)this.reader.ReadByte();
				int value3 = this.reader.ReadInt32();
				BitsByte bitsByte18 = this.reader.ReadByte();
				if (num209 < item5.Length)
				{
					item5[num209] = new Item();
					item5[num209].netDefaults(type13);
					item5[num209].stack = stack6;
					item5[num209].Prefix(pre3);
					item5[num209].value = value3;
					item5[num209].buyOnce = bitsByte18[0];
					return;
				}
				return;
			}
			case 105:
			{
				if (Main.netMode == 1)
				{
					return;
				}
				int i3 = (int)this.reader.ReadInt16();
				int j3 = (int)this.reader.ReadInt16();
				bool on = this.reader.ReadBoolean();
				WorldGen.ToggleGemLock(i3, j3, on);
				return;
			}
			case 106:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				HalfVector2 halfVector = default(HalfVector2);
				halfVector.PackedValue = this.reader.ReadUInt32();
				Utils.PoofOfSmoke(halfVector.ToVector2());
				return;
			}
			case 107:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				this.reader.ReadByte();
				Color c2 = this.reader.ReadRGB();
				string text10 = this.reader.ReadString();
				int widthLimit = (int)this.reader.ReadInt16();
				Main.NewTextMultiline(text10, false, c2, widthLimit);
				return;
			}
			case 108:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int damage2 = (int)this.reader.ReadInt16();
				float knockBack2 = this.reader.ReadSingle();
				int x9 = (int)this.reader.ReadInt16();
				int y9 = (int)this.reader.ReadInt16();
				int angle = (int)this.reader.ReadInt16();
				int ammo = (int)this.reader.ReadInt16();
				int num210 = (int)this.reader.ReadByte();
				if (num210 != Main.myPlayer)
				{
					return;
				}
				WorldGen.ShootFromCannon(x9, y9, angle, ammo, damage2, knockBack2, num210);
				return;
			}
			case 109:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x10 = (int)this.reader.ReadInt16();
				int y10 = (int)this.reader.ReadInt16();
				int x11 = (int)this.reader.ReadInt16();
				int y11 = (int)this.reader.ReadInt16();
				WiresUI.Settings.MultiToolMode toolMode = (WiresUI.Settings.MultiToolMode)this.reader.ReadByte();
				int num211 = this.whoAmI;
				WiresUI.Settings.MultiToolMode toolMode2 = WiresUI.Settings.ToolMode;
				WiresUI.Settings.ToolMode = toolMode;
				Wiring.MassWireOperation(new Point(x10, y10), new Point(x11, y11), Main.player[num211]);
				WiresUI.Settings.ToolMode = toolMode2;
				return;
			}
			case 110:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int type14 = (int)this.reader.ReadInt16();
				int num212 = (int)this.reader.ReadInt16();
				int num213 = (int)this.reader.ReadByte();
				if (num213 != Main.myPlayer)
				{
					return;
				}
				Player player20 = Main.player[num213];
				for (int num214 = 0; num214 < num212; num214++)
				{
					player20.ConsumeItem(type14, false);
				}
				player20.wireOperationsCooldown = 0;
				return;
			}
			case 111:
				if (Main.netMode != 2)
				{
					return;
				}
				BirthdayParty.ToggleManualParty();
				return;
			case 112:
			{
				int num215 = (int)this.reader.ReadByte();
				int num216 = (int)this.reader.ReadInt16();
				int num217 = (int)this.reader.ReadInt16();
				int num218 = (int)this.reader.ReadByte();
				int num219 = (int)this.reader.ReadInt16();
				if (num215 != 1)
				{
					return;
				}
				if (Main.netMode == 1)
				{
					WorldGen.TreeGrowFX(num216, num217, num218, num219);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData((int)b, -1, -1, "", num215, (float)num216, (float)num217, (float)num218, num219, 0, 0);
					return;
				}
				return;
			}
			case 113:
			{
				int x12 = (int)this.reader.ReadInt16();
				int y12 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2 && !Main.snowMoon && !Main.pumpkinMoon)
				{
					if (DD2Event.WouldFailSpawningHere(x12, y12))
					{
						DD2Event.FailureMessage(this.whoAmI);
					}
					DD2Event.SummonCrystal(x12, y12);
					return;
				}
				return;
			}
			case 114:
				if (Main.netMode != 1)
				{
					return;
				}
				DD2Event.WipeEntities();
				return;
			case 115:
			{
				int num220 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num220 = this.whoAmI;
				}
				Player player21 = Main.player[num220];
				player21.MinionAttackTargetNPC = (int)this.reader.ReadInt16();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(115, -1, this.whoAmI, "", num220, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 116:
				if (Main.netMode != 1)
				{
					return;
				}
				DD2Event.TimeLeftBetweenWaves = this.reader.ReadInt32();
				return;
			case 117:
			{
				int num221 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && this.whoAmI != num221 && (!Main.player[num221].hostile || !Main.player[this.whoAmI].hostile))
				{
					return;
				}
				PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(this.reader);
				int damage3 = (int)this.reader.ReadInt16();
				int num222 = (int)(this.reader.ReadByte() - 1);
				BitsByte bitsByte19 = this.reader.ReadByte();
				bool flag18 = bitsByte19[0];
				bool pvp = bitsByte19[1];
				int num223 = (int)this.reader.ReadSByte();
				Main.player[num221].Hurt(playerDeathReason, damage3, num222, pvp, true, flag18, num223);
				if (Main.netMode == 2)
				{
					NetMessage.SendPlayerHurt(num221, playerDeathReason, damage3, num222, flag18, pvp, num223, -1, this.whoAmI);
					return;
				}
				return;
			}
			case 118:
			{
				int num224 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num224 = this.whoAmI;
				}
				PlayerDeathReason playerDeathReason2 = PlayerDeathReason.FromReader(this.reader);
				int num225 = (int)this.reader.ReadInt16();
				int num226 = (int)(this.reader.ReadByte() - 1);
				bool pvp2 = ((BitsByte)this.reader.ReadByte())[0];
				Main.player[num224].KillMe(playerDeathReason2, (double)num225, num226, pvp2);
				if (Main.netMode == 2)
				{
					NetMessage.SendPlayerDeath(num224, playerDeathReason2, num225, num226, pvp2, -1, this.whoAmI);
					return;
				}
				return;
			}
			default:
				return;
			}
			if (Main.netMode != 2)
			{
				return;
			}
			if (Netplay.Clients[this.whoAmI].State == 1)
			{
				Netplay.Clients[this.whoAmI].State = 2;
			}
			NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
			Main.SyncAnInvasion(this.whoAmI);
			return;
			IL_4C6D:
			if (Netplay.Connection.State == 6)
			{
				Netplay.Connection.State = 10;
				Main.ActivePlayerFileData.StartPlayTimer();
				Player.Hooks.EnterWorld(Main.myPlayer);
				Main.player[Main.myPlayer].Spawn();
				return;
			}
			return;
			IL_67C4:
			int num227 = (int)this.reader.ReadInt16();
			float num228 = this.reader.ReadSingle();
			float num229 = this.reader.ReadSingle();
			float num230 = this.reader.ReadSingle();
			if (num227 < 0 || num227 > 200)
			{
				return;
			}
			if (Main.netMode == 1)
			{
				Main.npc[num227].moneyPing(new Vector2(num229, num230));
				Main.npc[num227].extraValue = num228;
				return;
			}
			Main.npc[num227].extraValue += num228;
			NetMessage.SendData(92, -1, -1, "", num227, Main.npc[num227].extraValue, num229, num230, 0, 0, 0);
			return;
		}
	}
}
