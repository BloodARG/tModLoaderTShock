using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public static class Modifiers
	{
		public class ShapeScale : GenAction
		{
			private int _scale;

			public ShapeScale(int scale)
			{
				this._scale = scale;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				bool flag = false;
				for (int i = 0; i < this._scale; i++)
				{
					for (int j = 0; j < this._scale; j++)
					{
						flag |= !base.UnitApply(origin, (x - origin.X << 1) + i + origin.X, (y - origin.Y << 1) + j + origin.Y, new object[0]);
					}
				}
				return !flag;
			}
		}

		public class Expand : GenAction
		{
			private int _xExpansion;

			private int _yExpansion;

			public Expand(int expansion)
			{
				this._xExpansion = expansion;
				this._yExpansion = expansion;
			}

			public Expand(int xExpansion, int yExpansion)
			{
				this._xExpansion = xExpansion;
				this._yExpansion = yExpansion;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				bool flag = false;
				for (int i = -this._xExpansion; i <= this._xExpansion; i++)
				{
					for (int j = -this._yExpansion; j <= this._yExpansion; j++)
					{
						flag |= !base.UnitApply(origin, x + i, y + j, args);
					}
				}
				return !flag;
			}
		}

		public class RadialDither : GenAction
		{
			private float _innerRadius;

			private float _outerRadius;

			public RadialDither(float innerRadius, float outerRadius)
			{
				this._innerRadius = innerRadius;
				this._outerRadius = outerRadius;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				Vector2 value = new Vector2((float)origin.X, (float)origin.Y);
				Vector2 value2 = new Vector2((float)x, (float)y);
				float num = Vector2.Distance(value2, value);
				float num2 = Math.Max(0f, Math.Min(1f, (num - this._innerRadius) / (this._outerRadius - this._innerRadius)));
				if (GenBase._random.NextDouble() > (double)num2)
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class Blotches : GenAction
		{
			private int _minX;

			private int _minY;

			private int _maxX;

			private int _maxY;

			private double _chance;

			public Blotches(int scale = 2, double chance = 0.3)
			{
				this._minX = scale;
				this._minY = scale;
				this._maxX = scale;
				this._maxY = scale;
				this._chance = chance;
			}

			public Blotches(int xScale, int yScale, double chance = 0.3)
			{
				this._minX = xScale;
				this._maxX = xScale;
				this._minY = yScale;
				this._maxY = yScale;
				this._chance = chance;
			}

			public Blotches(int leftScale, int upScale, int rightScale, int downScale, double chance = 0.3)
			{
				this._minX = leftScale;
				this._maxX = rightScale;
				this._minY = upScale;
				this._maxY = downScale;
				this._chance = chance;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				GenBase._random.NextDouble();
				if (GenBase._random.NextDouble() < this._chance)
				{
					bool flag = false;
					int num = GenBase._random.Next(1 - this._minX, 1);
					int num2 = GenBase._random.Next(0, this._maxX);
					int num3 = GenBase._random.Next(1 - this._minY, 1);
					int num4 = GenBase._random.Next(0, this._maxY);
					for (int i = num; i <= num2; i++)
					{
						for (int j = num3; j <= num4; j++)
						{
							flag |= !base.UnitApply(origin, x + i, y + j, args);
						}
					}
					return !flag;
				}
				return base.UnitApply(origin, x, y, args);
			}
		}

		public class Conditions : GenAction
		{
			private GenCondition[] _conditions;

			public Conditions(params GenCondition[] conditions)
			{
				this._conditions = conditions;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				bool flag = true;
				for (int i = 0; i < this._conditions.Length; i++)
				{
					flag &= this._conditions[i].IsValid(x, y);
				}
				if (flag)
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class OnlyWalls : GenAction
		{
			private byte[] _types;

			public OnlyWalls(params byte[] types)
			{
				this._types = types;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				for (int i = 0; i < this._types.Length; i++)
				{
					if (GenBase._tiles[x, y].wall == this._types[i])
					{
						return base.UnitApply(origin, x, y, args);
					}
				}
				return base.Fail();
			}
		}

		public class OnlyTiles : GenAction
		{
			private ushort[] _types;

			public OnlyTiles(params ushort[] types)
			{
				this._types = types;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (!GenBase._tiles[x, y].active())
				{
					return base.Fail();
				}
				for (int i = 0; i < this._types.Length; i++)
				{
					if (GenBase._tiles[x, y].type == this._types[i])
					{
						return base.UnitApply(origin, x, y, args);
					}
				}
				return base.Fail();
			}
		}

		public class IsTouching : GenAction
		{
			private static readonly int[] DIRECTIONS = new int[]
			{
				0,
				-1,
				1,
				0,
				-1,
				0,
				0,
				1,
				-1,
				-1,
				1,
				-1,
				-1,
				1,
				1,
				1
			};

			private bool _useDiagonals;

			private ushort[] _tileIds;

			public IsTouching(bool useDiagonals, params ushort[] tileIds)
			{
				this._useDiagonals = useDiagonals;
				this._tileIds = tileIds;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				int num = this._useDiagonals ? 16 : 8;
				for (int i = 0; i < num; i += 2)
				{
					Tile tile = GenBase._tiles[x + Modifiers.IsTouching.DIRECTIONS[i], y + Modifiers.IsTouching.DIRECTIONS[i + 1]];
					if (tile.active())
					{
						for (int j = 0; j < this._tileIds.Length; j++)
						{
							if (tile.type == this._tileIds[j])
							{
								return base.UnitApply(origin, x, y, args);
							}
						}
					}
				}
				return base.Fail();
			}
		}

		public class NotTouching : GenAction
		{
			private static readonly int[] DIRECTIONS = new int[]
			{
				0,
				-1,
				1,
				0,
				-1,
				0,
				0,
				1,
				-1,
				-1,
				1,
				-1,
				-1,
				1,
				1,
				1
			};

			private bool _useDiagonals;

			private ushort[] _tileIds;

			public NotTouching(bool useDiagonals, params ushort[] tileIds)
			{
				this._useDiagonals = useDiagonals;
				this._tileIds = tileIds;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				int num = this._useDiagonals ? 16 : 8;
				for (int i = 0; i < num; i += 2)
				{
					Tile tile = GenBase._tiles[x + Modifiers.NotTouching.DIRECTIONS[i], y + Modifiers.NotTouching.DIRECTIONS[i + 1]];
					if (tile.active())
					{
						for (int j = 0; j < this._tileIds.Length; j++)
						{
							if (tile.type == this._tileIds[j])
							{
								return base.Fail();
							}
						}
					}
				}
				return base.UnitApply(origin, x, y, args);
			}
		}

		public class IsTouchingAir : GenAction
		{
			private static readonly int[] DIRECTIONS = new int[]
			{
				0,
				-1,
				1,
				0,
				-1,
				0,
				0,
				1,
				-1,
				-1,
				1,
				-1,
				-1,
				1,
				1,
				1
			};

			private bool _useDiagonals;

			public IsTouchingAir(bool useDiagonals = false)
			{
				this._useDiagonals = useDiagonals;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				int num = this._useDiagonals ? 16 : 8;
				for (int i = 0; i < num; i += 2)
				{
					if (!GenBase._tiles[x + Modifiers.IsTouchingAir.DIRECTIONS[i], y + Modifiers.IsTouchingAir.DIRECTIONS[i + 1]].active())
					{
						return base.UnitApply(origin, x, y, args);
					}
				}
				return base.Fail();
			}
		}

		public class SkipTiles : GenAction
		{
			private ushort[] _types;

			public SkipTiles(params ushort[] types)
			{
				this._types = types;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (!GenBase._tiles[x, y].active())
				{
					return base.UnitApply(origin, x, y, args);
				}
				for (int i = 0; i < this._types.Length; i++)
				{
					if (GenBase._tiles[x, y].type == this._types[i])
					{
						return base.Fail();
					}
				}
				return base.UnitApply(origin, x, y, args);
			}
		}

		public class HasLiquid : GenAction
		{
			private int _liquidType;

			private int _liquidLevel;

			public HasLiquid(int liquidLevel = -1, int liquidType = -1)
			{
				this._liquidLevel = liquidLevel;
				this._liquidType = liquidType;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				Tile tile = GenBase._tiles[x, y];
				if ((this._liquidType == -1 || this._liquidType == (int)tile.liquidType()) && ((this._liquidLevel == -1 && tile.liquid != 0) || this._liquidLevel == (int)tile.liquid))
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class SkipWalls : GenAction
		{
			private byte[] _types;

			public SkipWalls(params byte[] types)
			{
				this._types = types;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				for (int i = 0; i < this._types.Length; i++)
				{
					if (GenBase._tiles[x, y].wall == this._types[i])
					{
						return base.Fail();
					}
				}
				return base.UnitApply(origin, x, y, args);
			}
		}

		public class IsEmpty : GenAction
		{
			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (!GenBase._tiles[x, y].active())
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class IsSolid : GenAction
		{
			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (GenBase._tiles[x, y].active() && WorldGen.SolidTile(x, y))
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class IsNotSolid : GenAction
		{
			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (!GenBase._tiles[x, y].active() || !WorldGen.SolidTile(x, y))
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class RectangleMask : GenAction
		{
			private int _xMin;

			private int _yMin;

			private int _xMax;

			private int _yMax;

			public RectangleMask(int xMin, int xMax, int yMin, int yMax)
			{
				this._xMin = xMin;
				this._yMin = yMin;
				this._xMax = xMax;
				this._yMax = yMax;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (x >= this._xMin + origin.X && x <= this._xMax + origin.X && y >= this._yMin + origin.Y && y <= this._yMax + origin.Y)
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class Offset : GenAction
		{
			private int _xOffset;

			private int _yOffset;

			public Offset(int x, int y)
			{
				this._xOffset = x;
				this._yOffset = y;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				return base.UnitApply(origin, x + this._xOffset, y + this._yOffset, args);
			}
		}

		public class Dither : GenAction
		{
			private double _failureChance;

			public Dither(double failureChance = 0.5)
			{
				this._failureChance = failureChance;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (GenBase._random.NextDouble() >= this._failureChance)
				{
					return base.UnitApply(origin, x, y, args);
				}
				return base.Fail();
			}
		}

		public class Flip : GenAction
		{
			private bool _flipX;

			private bool _flipY;

			public Flip(bool flipX, bool flipY)
			{
				this._flipX = flipX;
				this._flipY = flipY;
			}

			public override bool Apply(Point origin, int x, int y, params object[] args)
			{
				if (this._flipX)
				{
					x = origin.X * 2 - x;
				}
				if (this._flipY)
				{
					y = origin.Y * 2 - y;
				}
				return base.UnitApply(origin, x, y, args);
			}
		}
	}
}
