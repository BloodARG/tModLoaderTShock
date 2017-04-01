using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.GameContent
{
	public class PortalHelper
	{
		public const int PORTALS_PER_PERSON = 2;
		private static int[,] FoundPortals;
		private static int[] PortalCooldownForPlayers;
		private static int[] PortalCooldownForNPCs;
		private static readonly Vector2[] EDGES;
		private static readonly Vector2[] SLOPE_EDGES;
		private static readonly Point[] SLOPE_OFFSETS;

		static PortalHelper()
		{
			PortalHelper.FoundPortals = new int[256, 2];
			PortalHelper.PortalCooldownForPlayers = new int[256];
			PortalHelper.PortalCooldownForNPCs = new int[200];
			PortalHelper.EDGES = new Vector2[]
			{
				new Vector2(0f, 1f),
				new Vector2(0f, -1f),
				new Vector2(1f, 0f),
				new Vector2(-1f, 0f)
			};
			PortalHelper.SLOPE_EDGES = new Vector2[]
			{
				new Vector2(1f, -1f),
				new Vector2(-1f, -1f),
				new Vector2(1f, 1f),
				new Vector2(-1f, 1f)
			};
			PortalHelper.SLOPE_OFFSETS = new Point[]
			{
				new Point(1, -1),
				new Point(-1, -1),
				new Point(1, 1),
				new Point(-1, 1)
			};
			for (int i = 0; i < PortalHelper.SLOPE_EDGES.Length; i++)
			{
				PortalHelper.SLOPE_EDGES[i].Normalize();
			}
			for (int j = 0; j < PortalHelper.FoundPortals.GetLength(0); j++)
			{
				PortalHelper.FoundPortals[j, 0] = -1;
				PortalHelper.FoundPortals[j, 1] = -1;
			}
		}

		public static void UpdatePortalPoints()
		{
			for (int i = 0; i < PortalHelper.FoundPortals.GetLength(0); i++)
			{
				PortalHelper.FoundPortals[i, 0] = -1;
				PortalHelper.FoundPortals[i, 1] = -1;
			}
			for (int j = 0; j < PortalHelper.PortalCooldownForPlayers.Length; j++)
			{
				if (PortalHelper.PortalCooldownForPlayers[j] > 0)
				{
					PortalHelper.PortalCooldownForPlayers[j]--;
				}
			}
			for (int k = 0; k < PortalHelper.PortalCooldownForNPCs.Length; k++)
			{
				if (PortalHelper.PortalCooldownForNPCs[k] > 0)
				{
					PortalHelper.PortalCooldownForNPCs[k]--;
				}
			}
			for (int l = 0; l < 1000; l++)
			{
				Projectile projectile = Main.projectile[l];
				if (projectile.active && projectile.type == 602 && projectile.ai[1] >= 0f && projectile.ai[1] <= 1f && projectile.owner >= 0 && projectile.owner <= 255)
				{
					PortalHelper.FoundPortals[projectile.owner, (int)projectile.ai[1]] = l;
				}
			}
		}

		public static void TryGoingThroughPortals(Entity ent)
		{
			float num = 0f;
			Vector2 arg_0C_0 = ent.velocity;
			int width = ent.width;
			int height = ent.height;
			int num2 = 1;
			if (ent is Player)
			{
				num2 = (int)((Player)ent).gravDir;
			}
			for (int i = 0; i < PortalHelper.FoundPortals.GetLength(0); i++)
			{
				if (PortalHelper.FoundPortals[i, 0] != -1 && PortalHelper.FoundPortals[i, 1] != -1 && (!(ent is Player) || (i < PortalHelper.PortalCooldownForPlayers.Length && PortalHelper.PortalCooldownForPlayers[i] <= 0)) && (!(ent is NPC) || (i < PortalHelper.PortalCooldownForNPCs.Length && PortalHelper.PortalCooldownForNPCs[i] <= 0)))
				{
					for (int j = 0; j < 2; j++)
					{
						Projectile projectile = Main.projectile[PortalHelper.FoundPortals[i, j]];
						Vector2 lineStart;
						Vector2 lineEnd;
						PortalHelper.GetPortalEdges(projectile.Center, projectile.ai[0], out lineStart, out lineEnd);
						if (Collision.CheckAABBvLineCollision(ent.position + ent.velocity, ent.Size, lineStart, lineEnd, 2f, ref num))
						{
							Projectile projectile2 = Main.projectile[PortalHelper.FoundPortals[i, 1 - j]];
							float scaleFactor = ent.Hitbox.Distance(projectile.Center);
							int num3;
							int num4;
							Vector2 vector = PortalHelper.GetPortalOutingPoint(ent.Size, projectile2.Center, projectile2.ai[0], out num3, out num4) + Vector2.Normalize(new Vector2((float)num3, (float)num4)) * scaleFactor;
							Vector2 vector2 = Vector2.UnitX * 16f;
							if (!(Collision.TileCollision(vector - vector2, vector2, width, height, true, true, num2) != vector2))
							{
								vector2 = -Vector2.UnitX * 16f;
								if (!(Collision.TileCollision(vector - vector2, vector2, width, height, true, true, num2) != vector2))
								{
									vector2 = Vector2.UnitY * 16f;
									if (!(Collision.TileCollision(vector - vector2, vector2, width, height, true, true, num2) != vector2))
									{
										vector2 = -Vector2.UnitY * 16f;
										if (!(Collision.TileCollision(vector - vector2, vector2, width, height, true, true, num2) != vector2))
										{
											float num5 = 0.1f;
											if (num4 == -num2)
											{
												num5 = 0.1f;
											}
											if (ent.velocity == Vector2.Zero)
											{
												ent.velocity = (projectile.ai[0] - 1.57079637f).ToRotationVector2() * num5;
											}
											if (ent.velocity.Length() < num5)
											{
												ent.velocity.Normalize();
												ent.velocity *= num5;
											}
											Vector2 vector3 = Vector2.Normalize(new Vector2((float)num3, (float)num4));
											if (vector3.HasNaNs() || vector3 == Vector2.Zero)
											{
												vector3 = Vector2.UnitX * (float)ent.direction;
											}
											ent.velocity = vector3 * ent.velocity.Length();
											if ((num4 == -num2 && Math.Sign(ent.velocity.Y) != -num2) || Math.Abs(ent.velocity.Y) < 0.1f)
											{
												ent.velocity.Y = (float)(-(float)num2) * 0.1f;
											}
											int num6 = (int)((float)(projectile2.owner * 2) + projectile2.ai[1]);
											int lastPortalColorIndex = num6 + ((num6 % 2 == 0) ? 1 : -1);
											if (ent is Player)
											{
												Player player = (Player)ent;
												player.lastPortalColorIndex = lastPortalColorIndex;
												player.Teleport(vector, 4, num6);
												if (Main.netMode == 1)
												{
													NetMessage.SendData(96, -1, -1, "", player.whoAmI, vector.X, vector.Y, (float)num6, 0, 0, 0);
													NetMessage.SendData(13, -1, -1, "", player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
												}
												PortalHelper.PortalCooldownForPlayers[i] = 10;
												return;
											}
											if (ent is NPC)
											{
												NPC nPC = (NPC)ent;
												nPC.lastPortalColorIndex = lastPortalColorIndex;
												nPC.Teleport(vector, 4, num6);
												if (Main.netMode == 1)
												{
													NetMessage.SendData(100, -1, -1, "", nPC.whoAmI, vector.X, vector.Y, (float)num6, 0, 0, 0);
													NetMessage.SendData(23, -1, -1, "", nPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
												}
												PortalHelper.PortalCooldownForPlayers[i] = 10;
											}
											return;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static int TryPlacingPortal(Projectile theBolt, Vector2 velocity, Vector2 theCrashVelocity)
		{
			Vector2 vector = velocity / velocity.Length();
			Vector2 vec = PortalHelper.FindCollision(theBolt.position, theBolt.position + velocity + vector * 32f);
			Point position = vec.ToTileCoordinates();
			Tile tile = Main.tile[position.X, position.Y];
			Vector2 vector2 = new Vector2((float)(position.X * 16 + 8), (float)(position.Y * 16 + 8));
			if (!WorldGen.SolidOrSlopedTile(tile))
			{
				return -1;
			}
			int num = (int)tile.slope();
			bool flag = tile.halfBrick();
			for (int i = 0; i < (flag ? 2 : PortalHelper.EDGES.Length); i++)
			{
				float num2 = Vector2.Dot(PortalHelper.EDGES[i], vector);
				Point point;
				if (num2 > 0f && PortalHelper.FindValidLine(position, (int)PortalHelper.EDGES[i].Y, (int)(-(int)PortalHelper.EDGES[i].X), out point))
				{
					vector2 = new Vector2((float)(point.X * 16 + 8), (float)(point.Y * 16 + 8));
					return PortalHelper.AddPortal(vector2 - PortalHelper.EDGES[i] * (flag ? 0f : 8f), (float)Math.Atan2((double)PortalHelper.EDGES[i].Y, (double)PortalHelper.EDGES[i].X) + 1.57079637f, (int)theBolt.ai[0], theBolt.direction);
				}
			}
			if (num != 0)
			{
				Vector2 value = PortalHelper.SLOPE_EDGES[num - 1];
				float num3 = Vector2.Dot(value, -vector);
				Point point2;
				if (num3 > 0f && PortalHelper.FindValidLine(position, -PortalHelper.SLOPE_OFFSETS[num - 1].Y, PortalHelper.SLOPE_OFFSETS[num - 1].X, out point2))
				{
					vector2 = new Vector2((float)(point2.X * 16 + 8), (float)(point2.Y * 16 + 8));
					return PortalHelper.AddPortal(vector2, (float)Math.Atan2((double)value.Y, (double)value.X) - 1.57079637f, (int)theBolt.ai[0], theBolt.direction);
				}
			}
			return -1;
		}

		private static bool FindValidLine(Point position, int xOffset, int yOffset, out Point bestPosition)
		{
			bestPosition = position;
			if (PortalHelper.IsValidLine(position, xOffset, yOffset))
			{
				return true;
			}
			Point point = new Point(position.X - xOffset, position.Y - yOffset);
			if (PortalHelper.IsValidLine(point, xOffset, yOffset))
			{
				bestPosition = point;
				return true;
			}
			Point point2 = new Point(position.X + xOffset, position.Y + yOffset);
			if (PortalHelper.IsValidLine(point2, xOffset, yOffset))
			{
				bestPosition = point2;
				return true;
			}
			return false;
		}

		private static bool IsValidLine(Point position, int xOffset, int yOffset)
		{
			Tile tile = Main.tile[position.X, position.Y];
			Tile tile2 = Main.tile[position.X - xOffset, position.Y - yOffset];
			Tile tile3 = Main.tile[position.X + xOffset, position.Y + yOffset];
			return !PortalHelper.BlockPortals(Main.tile[position.X + yOffset, position.Y - xOffset]) && !PortalHelper.BlockPortals(Main.tile[position.X + yOffset - xOffset, position.Y - xOffset - yOffset]) && !PortalHelper.BlockPortals(Main.tile[position.X + yOffset + xOffset, position.Y - xOffset + yOffset]) && (WorldGen.SolidOrSlopedTile(tile) && WorldGen.SolidOrSlopedTile(tile2) && WorldGen.SolidOrSlopedTile(tile3) && tile2.HasSameSlope(tile) && tile3.HasSameSlope(tile));
		}

		private static bool BlockPortals(Tile t)
		{
			return t.active() && !Main.tileCut[(int)t.type] && !TileID.Sets.BreakableWhenPlacing[(int)t.type] && Main.tileSolid[(int)t.type];
		}

		private static Vector2 FindCollision(Vector2 startPosition, Vector2 stopPosition)
		{
			int lastX = 0;
			int lastY = 0;
			Utils.PlotLine(startPosition.ToTileCoordinates(), stopPosition.ToTileCoordinates(), delegate(int x, int y)
				{
					lastX = x;
					lastY = y;
					return !WorldGen.SolidOrSlopedTile(x, y);
				}, false);
			return new Vector2((float)lastX * 16f, (float)lastY * 16f);
		}

		private static int AddPortal(Vector2 position, float angle, int form, int direction)
		{
			if (!PortalHelper.SupportedTilesAreFine(position, angle))
			{
				return -1;
			}
			PortalHelper.RemoveMyOldPortal(form);
			PortalHelper.RemoveIntersectingPortals(position, angle);
			int num = Projectile.NewProjectile(position.X, position.Y, 0f, 0f, 602, 0, 0f, Main.myPlayer, angle, (float)form);
			Main.projectile[num].direction = direction;
			Main.projectile[num].netUpdate = true;
			return num;
		}

		private static void RemoveMyOldPortal(int form)
		{
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.type == 602 && projectile.owner == Main.myPlayer && projectile.ai[1] == (float)form)
				{
					projectile.Kill();
					return;
				}
			}
		}

		private static void RemoveIntersectingPortals(Vector2 position, float angle)
		{
			Vector2 a;
			Vector2 a2;
			PortalHelper.GetPortalEdges(position, angle, out a, out a2);
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && projectile.type == 602)
				{
					Vector2 b;
					Vector2 b2;
					PortalHelper.GetPortalEdges(projectile.Center, projectile.ai[0], out b, out b2);
					if (Collision.CheckLinevLine(a, a2, b, b2).Length > 0)
					{
						if (projectile.owner != Main.myPlayer && Main.netMode != 2)
						{
							NetMessage.SendData(95, -1, -1, "", i, 0f, 0f, 0f, 0, 0, 0);
						}
						projectile.Kill();
						if (Main.netMode == 2)
						{
							NetMessage.SendData(29, -1, -1, "", projectile.whoAmI, (float)projectile.owner, 0f, 0f, 0, 0, 0);
						}
					}
				}
			}
		}

		public static Color GetPortalColor(int colorIndex)
		{
			return PortalHelper.GetPortalColor(colorIndex / 2, colorIndex % 2);
		}

		public static Color GetPortalColor(int player, int portal)
		{
			Color result = Color.White;
			if (Main.netMode == 0)
			{
				if (portal == 0)
				{
					result = Main.hslToRgb(0.12f, 1f, 0.5f);
				}
				else
				{
					result = Main.hslToRgb(0.52f, 1f, 0.6f);
				}
			}
			else
			{
				float num = 0.08f;
				float num2 = 0.5f;
				result = Main.hslToRgb((num2 + (float)player * (num * 2f) + (float)portal * num) % 1f, 1f, 0.5f);
			}
			result.A = 66;
			return result;
		}

		private static void GetPortalEdges(Vector2 position, float angle, out Vector2 start, out Vector2 end)
		{
			Vector2 value = angle.ToRotationVector2();
			start = position + value * -22f;
			end = position + value * 22f;
		}

		private static Vector2 GetPortalOutingPoint(Vector2 objectSize, Vector2 portalPosition, float portalAngle, out int bonusX, out int bonusY)
		{
			int num = (int)Math.Round((double)(MathHelper.WrapAngle(portalAngle) / 0.7853982f));
			if (num == 2 || num == -2)
			{
				bonusX = ((num == 2) ? -1 : 1);
				bonusY = 0;
				return portalPosition + new Vector2((num == 2) ? (-objectSize.X) : 0f, -objectSize.Y / 2f);
			}
			if (num == 0 || num == 4)
			{
				bonusX = 0;
				bonusY = ((num == 0) ? 1 : -1);
				return portalPosition + new Vector2(-objectSize.X / 2f, (num == 0) ? 0f : (-objectSize.Y));
			}
			if (num == -3 || num == 3)
			{
				bonusX = ((num == -3) ? 1 : -1);
				bonusY = -1;
				return portalPosition + new Vector2((num == -3) ? 0f : (-objectSize.X), -objectSize.Y);
			}
			if (num == 1 || num == -1)
			{
				bonusX = ((num == -1) ? 1 : -1);
				bonusY = 1;
				return portalPosition + new Vector2((num == -1) ? 0f : (-objectSize.X), 0f);
			}
			Main.NewText("Broken portal! (over4s = " + num + ")", 255, 255, 255, false);
			bonusX = 0;
			bonusY = 0;
			return portalPosition;
		}

		public static void SyncPortalsOnPlayerJoin(int plr, int fluff, List<Point> dontInclude, out List<Point> portals, out List<Point> portalCenters)
		{
			portals = new List<Point>();
			portalCenters = new List<Point>();
			for (int i = 0; i < 1000; i++)
			{
				Projectile projectile = Main.projectile[i];
				if (projectile.active && (projectile.type == 602 || projectile.type == 601))
				{
					Vector2 center = projectile.Center;
					int sectionX = Netplay.GetSectionX((int)(center.X / 16f));
					int sectionY = Netplay.GetSectionY((int)(center.Y / 16f));
					for (int j = sectionX - fluff; j < sectionX + fluff + 1; j++)
					{
						for (int k = sectionY - fluff; k < sectionY + fluff + 1; k++)
						{
							if (j >= 0 && j < Main.maxSectionsX && k >= 0 && k < Main.maxSectionsY && !Netplay.Clients[plr].TileSections[j, k] && !dontInclude.Contains(new Point(j, k)))
							{
								portals.Add(new Point(j, k));
								if (!portalCenters.Contains(new Point(sectionX, sectionY)))
								{
									portalCenters.Add(new Point(sectionX, sectionY));
								}
							}
						}
					}
				}
			}
		}

		public static void SyncPortalSections(Vector2 portalPosition, int fluff)
		{
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					RemoteClient.CheckSection(i, portalPosition, fluff);
				}
			}
		}

		public static bool SupportedTilesAreFine(Vector2 portalCenter, float portalAngle)
		{
			Point point = portalCenter.ToTileCoordinates();
			int num = (int)Math.Round((double)(MathHelper.WrapAngle(portalAngle) / 0.7853982f));
			int num2;
			int num3;
			if (num == 2 || num == -2)
			{
				num2 = ((num == 2) ? -1 : 1);
				num3 = 0;
			}
			else if (num == 0 || num == 4)
			{
				num2 = 0;
				num3 = ((num == 0) ? 1 : -1);
			}
			else if (num == -3 || num == 3)
			{
				num2 = ((num == -3) ? 1 : -1);
				num3 = -1;
			}
			else
			{
				if (num != 1 && num != -1)
				{
					Main.NewText(string.Concat(new object[]
							{
								"Broken portal! (over4s = ",
								num,
								" , ",
								portalAngle,
								")"
							}), 255, 255, 255, false);
					return false;
				}
				num2 = ((num == -1) ? 1 : -1);
				num3 = 1;
			}
			if (num2 != 0 && num3 != 0)
			{
				int num4 = 3;
				if (num2 == -1 && num3 == 1)
				{
					num4 = 5;
				}
				if (num2 == 1 && num3 == -1)
				{
					num4 = 2;
				}
				if (num2 == 1 && num3 == 1)
				{
					num4 = 4;
				}
				num4--;
				return PortalHelper.SupportedSlope(point.X, point.Y, num4) && PortalHelper.SupportedSlope(point.X + num2, point.Y - num3, num4) && PortalHelper.SupportedSlope(point.X - num2, point.Y + num3, num4);
			}
			if (num2 != 0)
			{
				if (num2 == 1)
				{
					point.X--;
				}
				return PortalHelper.SupportedNormal(point.X, point.Y) && PortalHelper.SupportedNormal(point.X, point.Y - 1) && PortalHelper.SupportedNormal(point.X, point.Y + 1);
			}
			if (num3 != 0)
			{
				if (num3 == 1)
				{
					point.Y--;
				}
				return (PortalHelper.SupportedNormal(point.X, point.Y) && PortalHelper.SupportedNormal(point.X + 1, point.Y) && PortalHelper.SupportedNormal(point.X - 1, point.Y)) || (PortalHelper.SupportedHalfbrick(point.X, point.Y) && PortalHelper.SupportedHalfbrick(point.X + 1, point.Y) && PortalHelper.SupportedHalfbrick(point.X - 1, point.Y));
			}
			return true;
		}

		private static bool SupportedSlope(int x, int y, int slope)
		{
			Tile tile = Main.tile[x, y];
			return tile != null && tile.nactive() && !Main.tileCut[(int)tile.type] && !TileID.Sets.BreakableWhenPlacing[(int)tile.type] && Main.tileSolid[(int)tile.type] && (int)tile.slope() == slope;
		}

		private static bool SupportedHalfbrick(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile != null && tile.nactive() && !Main.tileCut[(int)tile.type] && !TileID.Sets.BreakableWhenPlacing[(int)tile.type] && Main.tileSolid[(int)tile.type] && tile.halfBrick();
		}

		private static bool SupportedNormal(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile != null && tile.nactive() && !Main.tileCut[(int)tile.type] && !TileID.Sets.BreakableWhenPlacing[(int)tile.type] && Main.tileSolid[(int)tile.type] && !TileID.Sets.NotReallySolid[(int)tile.type] && !tile.halfBrick() && tile.slope() == 0;
		}
	}
}
