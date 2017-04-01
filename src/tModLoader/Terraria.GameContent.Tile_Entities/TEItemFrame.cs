using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria.GameContent.Tile_Entities
{
	public class TEItemFrame : TileEntity
	{
		public Item item;

		public static void Initialize()
		{
			TileEntity._NetPlaceEntity += new Action<int, int, int>(TEItemFrame.NetPlaceEntity);
		}

		public static void NetPlaceEntity(int x, int y, int type)
		{
			if (type != 1)
			{
				return;
			}
			if (!TEItemFrame.ValidTile(x, y))
			{
				return;
			}
			int number = TEItemFrame.Place(x, y);
			NetMessage.SendData(86, -1, -1, "", number, (float)x, (float)y, 0f, 0, 0, 0);
		}

		public TEItemFrame()
		{
			this.item = new Item();
		}

		public static int Place(int x, int y)
		{
			TEItemFrame tEItemFrame = new TEItemFrame();
			tEItemFrame.Position = new Point16(x, y);
			tEItemFrame.ID = TileEntity.AssignNewID();
			tEItemFrame.type = 1;
			TileEntity.ByID[tEItemFrame.ID] = tEItemFrame;
			TileEntity.ByPosition[tEItemFrame.Position] = tEItemFrame;
			return tEItemFrame.ID;
		}

		public static int Hook_AfterPlacement(int x, int y, int type = 395, int style = 0, int direction = 1)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendTileSquare(Main.myPlayer, x, y, 2, TileChangeType.None);
				NetMessage.SendData(87, -1, -1, "", x, (float)y, 1f, 0f, 0, 0, 0);
				return -1;
			}
			return TEItemFrame.Place(x, y);
		}

		public static void Kill(int x, int y)
		{
			TileEntity tileEntity;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out tileEntity) && tileEntity.type == 1)
			{
				TileEntity.ByID.Remove(tileEntity.ID);
				TileEntity.ByPosition.Remove(new Point16(x, y));
			}
		}

		public static int Find(int x, int y)
		{
			TileEntity tileEntity;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out tileEntity) && tileEntity.type == 1)
			{
				return tileEntity.ID;
			}
			return -1;
		}

		public static bool ValidTile(int x, int y)
		{
			return Main.tile[x, y].active() && Main.tile[x, y].type == 395 && Main.tile[x, y].frameY == 0 && Main.tile[x, y].frameX % 36 == 0;
		}

		public override void WriteExtraData(BinaryWriter writer, bool networkSend)
		{
			writer.Write((short)(this.item.netID >= ItemID.Count ? 0 : this.item.netID));
			writer.Write(this.item.prefix);
			writer.Write((short)this.item.stack);
		}

		public override void ReadExtraData(BinaryReader reader, bool networkSend)
		{
			this.item = new Item();
			this.item.netDefaults((int)reader.ReadInt16());
			this.item.Prefix((int)reader.ReadByte());
			this.item.stack = (int)reader.ReadInt16();
		}

		public override string ToString()
		{
			return string.Concat(new object[]
				{
					this.Position.X,
					"x  ",
					this.Position.Y,
					"y item: ",
					this.item.ToString()
				});
		}

		public void DropItem()
		{
			if (Main.netMode != 1)
			{
				Item.NewItem((int)(this.Position.X * 16), (int)(this.Position.Y * 16), 32, 32, this.item.netID, 1, false, (int)this.item.prefix, false, false);
			}
			this.item = new Item();
		}

		public static void TryPlacing(int x, int y, int netid, int prefix, int stack)
		{
			int num = TEItemFrame.Find(x, y);
			if (num == -1)
			{
				int num2 = Item.NewItem(x * 16, y * 16, 32, 32, 1, 1, false, 0, false, false);
				Main.item[num2].netDefaults(netid);
				Main.item[num2].Prefix(prefix);
				Main.item[num2].stack = stack;
				NetMessage.SendData(21, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			TEItemFrame tEItemFrame = (TEItemFrame)TileEntity.ByID[num];
			if (tEItemFrame.item.stack > 0)
			{
				tEItemFrame.DropItem();
			}
			tEItemFrame.item = new Item();
			tEItemFrame.item.netDefaults(netid);
			tEItemFrame.item.Prefix(prefix);
			tEItemFrame.item.stack = stack;
			NetMessage.SendData(86, -1, -1, "", tEItemFrame.ID, (float)x, (float)y, 0f, 0, 0, 0);
		}
	}
}
