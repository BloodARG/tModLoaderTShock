using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;

namespace Terraria
{
	public static class DelegateMethods
	{
		public static class Minecart
		{
			public static Vector2 rotationOrigin;
			public static float rotation;

			public static void Sparks(Vector2 dustPosition)
			{
				dustPosition += new Vector2((float)((Main.rand.Next(2) == 0) ? 13 : -13), 0f).RotatedBy((double)DelegateMethods.Minecart.rotation, default(Vector2));
				int num = Dust.NewDust(dustPosition, 1, 1, 213, (float)Main.rand.Next(-2, 3), (float)Main.rand.Next(-2, 3), 0, default(Color), 1f);
				Main.dust[num].noGravity = true;
				Main.dust[num].fadeIn = Main.dust[num].scale + 1f + 0.01f * (float)Main.rand.Next(0, 51);
				Main.dust[num].noGravity = true;
				Main.dust[num].velocity *= (float)Main.rand.Next(15, 51) * 0.01f;
				Dust expr_F8_cp_0 = Main.dust[num];
				expr_F8_cp_0.velocity.X = expr_F8_cp_0.velocity.X * ((float)Main.rand.Next(25, 101) * 0.01f);
				Dust expr_125_cp_0 = Main.dust[num];
				expr_125_cp_0.velocity.Y = expr_125_cp_0.velocity.Y - (float)Main.rand.Next(15, 31) * 0.1f;
				Dust expr_152_cp_0 = Main.dust[num];
				expr_152_cp_0.position.Y = expr_152_cp_0.position.Y - 4f;
				if (Main.rand.Next(3) != 0)
				{
					Main.dust[num].noGravity = false;
					return;
				}
				Main.dust[num].scale *= 0.6f;
			}

			public static void SparksMech(Vector2 dustPosition)
			{
				dustPosition += new Vector2((float)((Main.rand.Next(2) == 0) ? 13 : -13), 0f).RotatedBy((double)DelegateMethods.Minecart.rotation, default(Vector2));
				int num = Dust.NewDust(dustPosition, 1, 1, 260, (float)Main.rand.Next(-2, 3), (float)Main.rand.Next(-2, 3), 0, default(Color), 1f);
				Main.dust[num].noGravity = true;
				Main.dust[num].fadeIn = Main.dust[num].scale + 0.5f + 0.01f * (float)Main.rand.Next(0, 51);
				Main.dust[num].noGravity = true;
				Main.dust[num].velocity *= (float)Main.rand.Next(15, 51) * 0.01f;
				Dust expr_F8_cp_0 = Main.dust[num];
				expr_F8_cp_0.velocity.X = expr_F8_cp_0.velocity.X * ((float)Main.rand.Next(25, 101) * 0.01f);
				Dust expr_125_cp_0 = Main.dust[num];
				expr_125_cp_0.velocity.Y = expr_125_cp_0.velocity.Y - (float)Main.rand.Next(15, 31) * 0.1f;
				Dust expr_152_cp_0 = Main.dust[num];
				expr_152_cp_0.position.Y = expr_152_cp_0.position.Y - 4f;
				if (Main.rand.Next(3) != 0)
				{
					Main.dust[num].noGravity = false;
					return;
				}
				Main.dust[num].scale *= 0.6f;
			}
		}

		public static Vector3 v3_1 = Vector3.Zero;
		public static float f_1 = 0f;
		public static Color c_1 = Color.Transparent;
		public static int i_1 = 0;
		public static TileCuttingContext tilecut_0 = TileCuttingContext.Unknown;

		public static bool TestDust(int x, int y)
		{
			if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
			{
				return false;
			}
			int num = Dust.NewDust(new Vector2((float)x, (float)y) * 16f + new Vector2(8f), 0, 0, 6, 0f, 0f, 0, default(Color), 1f);
			Main.dust[num].noGravity = true;
			Main.dust[num].noLight = true;
			return true;
		}

		public static bool CastLight(int x, int y)
		{
			if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
			{
				return false;
			}
			if (Main.tile[x, y] == null)
			{
				return false;
			}
			Lighting.AddLight(x, y, DelegateMethods.v3_1.X, DelegateMethods.v3_1.Y, DelegateMethods.v3_1.Z);
			return true;
		}

		public static bool CastLightOpen(int x, int y)
		{
			if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
			{
				return false;
			}
			if (Main.tile[x, y] == null)
			{
				return false;
			}
			if (!Main.tile[x, y].active() || Main.tile[x, y].inActive() || Main.tileSolidTop[(int)Main.tile[x, y].type] || !Main.tileSolid[(int)Main.tile[x, y].type])
			{
				Lighting.AddLight(x, y, DelegateMethods.v3_1.X, DelegateMethods.v3_1.Y, DelegateMethods.v3_1.Z);
			}
			return true;
		}

		public static bool NotDoorStand(int x, int y)
		{
			return Main.tile[x, y] == null || !Main.tile[x, y].active() || Main.tile[x, y].type != 11 || (Main.tile[x, y].frameX >= 18 && Main.tile[x, y].frameX < 54);
		}

		public static bool CutTiles(int x, int y)
		{
			if (!WorldGen.InWorld(x, y, 1))
			{
				return false;
			}
			if (Main.tile[x, y] == null)
			{
				return false;
			}
			if (!Main.tileCut[(int)Main.tile[x, y].type])
			{
				return true;
			}
			if (WorldGen.CanCutTile(x, y, DelegateMethods.tilecut_0))
			{
				WorldGen.KillTile(x, y, false, false, false);
				if (Main.netMode != 0)
				{
					NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y, 0f, 0, 0, 0);
				}
			}
			return true;
		}

		public static bool SearchAvoidedByNPCs(int x, int y)
		{
			return WorldGen.InWorld(x, y, 1) && Main.tile[x, y] != null && (!Main.tile[x, y].active() || !TileID.Sets.AvoidedByNPCs[(int)Main.tile[x, y].type]);
		}

		public static void RainbowLaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
		{
			color = DelegateMethods.c_1;
			if (stage == 0)
			{
				distCovered = 33f;
				frame = new Rectangle(0, 0, 26, 22);
				origin = frame.Size() / 2f;
				return;
			}
			if (stage == 1)
			{
				frame = new Rectangle(0, 25, 26, 28);
				distCovered = (float)frame.Height;
				origin = new Vector2((float)(frame.Width / 2), 0f);
				return;
			}
			if (stage == 2)
			{
				distCovered = 22f;
				frame = new Rectangle(0, 56, 26, 22);
				origin = new Vector2((float)(frame.Width / 2), 1f);
				return;
			}
			distCovered = 9999f;
			frame = Rectangle.Empty;
			origin = Vector2.Zero;
			color = Color.Transparent;
		}

		public static void TurretLaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
		{
			color = DelegateMethods.c_1;
			if (stage == 0)
			{
				distCovered = 32f;
				frame = new Rectangle(0, 0, 22, 20);
				origin = frame.Size() / 2f;
				return;
			}
			if (stage == 1)
			{
				DelegateMethods.i_1++;
				int num = DelegateMethods.i_1 % 5;
				frame = new Rectangle(0, 22 * (num + 1), 22, 20);
				distCovered = (float)(frame.Height - 1);
				origin = new Vector2((float)(frame.Width / 2), 0f);
				return;
			}
			if (stage == 2)
			{
				frame = new Rectangle(0, 154, 22, 30);
				distCovered = (float)frame.Height;
				origin = new Vector2((float)(frame.Width / 2), 1f);
				return;
			}
			distCovered = 9999f;
			frame = Rectangle.Empty;
			origin = Vector2.Zero;
			color = Color.Transparent;
		}

		public static void LightningLaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
		{
			color = DelegateMethods.c_1 * DelegateMethods.f_1;
			if (stage == 0)
			{
				distCovered = 0f;
				frame = new Rectangle(0, 0, 21, 8);
				origin = frame.Size() / 2f;
				return;
			}
			if (stage == 1)
			{
				frame = new Rectangle(0, 8, 21, 6);
				distCovered = (float)frame.Height;
				origin = new Vector2((float)(frame.Width / 2), 0f);
				return;
			}
			if (stage == 2)
			{
				distCovered = 8f;
				frame = new Rectangle(0, 14, 21, 8);
				origin = new Vector2((float)(frame.Width / 2), 2f);
				return;
			}
			distCovered = 9999f;
			frame = Rectangle.Empty;
			origin = Vector2.Zero;
			color = Color.Transparent;
		}

		public static int CompareYReverse(Point a, Point b)
		{
			return b.Y.CompareTo(a.Y);
		}

		public static int CompareDrawSorterByYScale(DrawData a, DrawData b)
		{
			return a.scale.Y.CompareTo(b.scale.Y);
		}
	}
}
