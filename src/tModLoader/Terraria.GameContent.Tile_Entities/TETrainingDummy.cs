using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria.GameContent.Tile_Entities
{
	public class TETrainingDummy : TileEntity
	{
		private static Dictionary<int, Rectangle> playerBox = new Dictionary<int, Rectangle>();
		private static bool playerBoxFilled = false;
		public int npc;

		public static void Initialize()
		{
			TileEntity._UpdateStart += new Action(TETrainingDummy.ClearBoxes);
			TileEntity._NetPlaceEntity += new Action<int, int, int>(TETrainingDummy.NetPlaceEntity);
		}

		public static void NetPlaceEntity(int x, int y, int type)
		{
			if (type != 0)
			{
				return;
			}
			if (!TETrainingDummy.ValidTile(x, y))
			{
				return;
			}
			TETrainingDummy.Place(x, y);
		}

		public static void ClearBoxes()
		{
			TETrainingDummy.playerBox.Clear();
			TETrainingDummy.playerBoxFilled = false;
		}

		public override void Update()
		{
			Rectangle value = new Rectangle(0, 0, 32, 48);
			value.Inflate(1600, 1600);
			int x = value.X;
			int y = value.Y;
			if (this.npc != -1)
			{
				if (!Main.npc[this.npc].active || Main.npc[this.npc].type != 488 || Main.npc[this.npc].ai[0] != (float)this.Position.X || Main.npc[this.npc].ai[1] != (float)this.Position.Y)
				{
					this.Deactivate();
					return;
				}
			}
			else
			{
				TETrainingDummy.FillPlayerHitboxes();
				value.X = (int)(this.Position.X * 16) + x;
				value.Y = (int)(this.Position.Y * 16) + y;
				bool flag = false;
				foreach (KeyValuePair<int, Rectangle> current in TETrainingDummy.playerBox)
				{
					if (current.Value.Intersects(value))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.Activate();
				}
			}
		}

		private static void FillPlayerHitboxes()
		{
			if (!TETrainingDummy.playerBoxFilled)
			{
				for (int i = 0; i < 255; i++)
				{
					if (Main.player[i].active)
					{
						TETrainingDummy.playerBox[i] = Main.player[i].getRect();
					}
				}
				TETrainingDummy.playerBoxFilled = true;
			}
		}

		public static bool ValidTile(int x, int y)
		{
			return Main.tile[x, y].active() && Main.tile[x, y].type == 378 && Main.tile[x, y].frameY == 0 && Main.tile[x, y].frameX % 36 == 0;
		}

		public TETrainingDummy()
		{
			this.npc = -1;
		}

		public static int Place(int x, int y)
		{
			TETrainingDummy tETrainingDummy = new TETrainingDummy();
			tETrainingDummy.Position = new Point16(x, y);
			tETrainingDummy.ID = TileEntity.AssignNewID();
			tETrainingDummy.type = 0;
			TileEntity.ByID[tETrainingDummy.ID] = tETrainingDummy;
			TileEntity.ByPosition[tETrainingDummy.Position] = tETrainingDummy;
			return tETrainingDummy.ID;
		}

		public static int Hook_AfterPlacement(int x, int y, int type = 378, int style = 0, int direction = 1)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendTileSquare(Main.myPlayer, x - 1, y - 1, 3, TileChangeType.None);
				NetMessage.SendData(87, -1, -1, "", x - 1, (float)(y - 2), 0f, 0f, 0, 0, 0);
				return -1;
			}
			return TETrainingDummy.Place(x - 1, y - 2);
		}

		public static void Kill(int x, int y)
		{
			TileEntity tileEntity;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out tileEntity) && tileEntity.type == 0)
			{
				TileEntity.ByID.Remove(tileEntity.ID);
				TileEntity.ByPosition.Remove(new Point16(x, y));
			}
		}

		public static int Find(int x, int y)
		{
			TileEntity tileEntity;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out tileEntity) && tileEntity.type == 0)
			{
				return tileEntity.ID;
			}
			return -1;
		}

		public override void WriteExtraData(BinaryWriter writer, bool networkSend)
		{
			writer.Write((short)this.npc);
		}

		public override void ReadExtraData(BinaryReader reader, bool networkSend)
		{
			this.npc = (int)reader.ReadInt16();
		}

		public void Activate()
		{
			int num = NPC.NewNPC((int)(this.Position.X * 16 + 16), (int)(this.Position.Y * 16 + 48), 488, 100, 0f, 0f, 0f, 0f, 255);
			Main.npc[num].ai[0] = (float)this.Position.X;
			Main.npc[num].ai[1] = (float)this.Position.Y;
			Main.npc[num].netUpdate = true;
			this.npc = num;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", this.ID, (float)this.Position.X, (float)this.Position.Y, 0f, 0, 0, 0);
			}
		}

		public void Deactivate()
		{
			if (this.npc != -1)
			{
				Main.npc[this.npc].active = false;
			}
			this.npc = -1;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", this.ID, (float)this.Position.X, (float)this.Position.Y, 0f, 0, 0, 0);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
				{
					this.Position.X,
					"x  ",
					this.Position.Y,
					"y npc: ",
					this.npc
				});
		}
	}
}
