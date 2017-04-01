using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Tile_Entities;
using Terraria.ModLoader;

namespace Terraria.DataStructures
{
	public abstract class TileEntity
	{
		public const int MaxEntitiesPerChunk = 1000;
		public static Dictionary<int, TileEntity> ByID = new Dictionary<int, TileEntity>();
		public static Dictionary<Point16, TileEntity> ByPosition = new Dictionary<Point16, TileEntity>();
		public static int TileEntitiesNextID = 0;
		public int ID;
		public Point16 Position;
		public byte type;

		public static event Action _UpdateStart;
		public static event Action _UpdateEnd;
		public static event Action<int, int, int> _NetPlaceEntity;

		public static int AssignNewID()
		{
			return TileEntity.TileEntitiesNextID++;
		}

		public static void Clear()
		{
			TileEntity.ByID.Clear();
			TileEntity.ByPosition.Clear();
			TileEntity.TileEntitiesNextID = 0;
		}

		public static void UpdateStart()
		{
			if (TileEntity._UpdateStart != null)
			{
				TileEntity._UpdateStart();
			}
		}

		public static void UpdateEnd()
		{
			if (TileEntity._UpdateEnd != null)
			{
				TileEntity._UpdateEnd();
			}
		}

		public static void InitializeAll()
		{
			TETrainingDummy.Initialize();
			TEItemFrame.Initialize();
			TELogicSensor.Initialize();
			ModTileEntity.Initialize();
		}

		public static void PlaceEntityNet(int x, int y, int type)
		{
			if (!WorldGen.InWorld(x, y, 0))
			{
				return;
			}
			if (TileEntity.ByPosition.ContainsKey(new Point16(x, y)))
			{
				return;
			}
			if (TileEntity._NetPlaceEntity != null)
			{
				TileEntity._NetPlaceEntity(x, y, type);
			}
		}

		public virtual void Update()
		{
		}

		public static void Write(BinaryWriter writer, TileEntity ent, bool networkSend = false)
		{
			writer.Write(ent.type);
			ent.WriteInner(writer, networkSend);
		}

		public static TileEntity Read(BinaryReader reader, bool networkSend = false)
		{
			TileEntity tileEntity = null;
			byte b = reader.ReadByte();
			switch (b)
			{
				case 0:
					tileEntity = new TETrainingDummy();
					break;
				case 1:
					tileEntity = new TEItemFrame();
					break;
				case 2:
					tileEntity = new TELogicSensor();
					break;
				default:
					tileEntity = ModTileEntity.ConstructFromType(b);
					break;
			}
			tileEntity.type = b;
			tileEntity.ReadInner(reader, networkSend);
			return tileEntity;
		}

		private void WriteInner(BinaryWriter writer, bool networkSend)
		{
			if (!networkSend)
			{
				writer.Write(this.ID);
			}
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			this.WriteExtraData(writer, networkSend);
		}

		private void ReadInner(BinaryReader reader, bool networkSend)
		{
			if (!networkSend)
			{
				this.ID = reader.ReadInt32();
			}
			this.Position = new Point16(reader.ReadInt16(), reader.ReadInt16());
			this.ReadExtraData(reader, networkSend);
		}

		public virtual void WriteExtraData(BinaryWriter writer, bool networkSend)
		{
		}

		public virtual void ReadExtraData(BinaryReader reader, bool networkSend)
		{
		}
	}
}
