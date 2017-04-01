using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;

namespace Terraria.World.Generation
{
	public static class ModShapes
	{
		public class All : GenModShape
		{
			public All(ShapeData data)
				: base(data)
			{
			}

			public override bool Perform(Point origin, GenAction action)
			{
				foreach (Point16 current in this._data.GetData())
				{
					if (!base.UnitApply(action, origin, (int)current.X + origin.X, (int)current.Y + origin.Y, new object[0]) && this._quitOnFail)
					{
						return false;
					}
				}
				return true;
			}
		}

		public class OuterOutline : GenModShape
		{
			private static readonly int[] POINT_OFFSETS = new int[]
			{
				1,
				0,
				-1,
				0,
				0,
				1,
				0,
				-1,
				1,
				1,
				1,
				-1,
				-1,
				1,
				-1,
				-1
			};
			private bool _useDiagonals;
			private bool _useInterior;

			public OuterOutline(ShapeData data, bool useDiagonals = true, bool useInterior = false)
				: base(data)
			{
				this._useDiagonals = useDiagonals;
				this._useInterior = useInterior;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = this._useDiagonals ? 16 : 8;
				foreach (Point16 current in this._data.GetData())
				{
					if (this._useInterior && !base.UnitApply(action, origin, (int)current.X + origin.X, (int)current.Y + origin.Y, new object[0]) && this._quitOnFail)
					{
						bool result = false;
						return result;
					}
					for (int i = 0; i < num; i += 2)
					{
						if (!this._data.Contains((int)current.X + ModShapes.OuterOutline.POINT_OFFSETS[i], (int)current.Y + ModShapes.OuterOutline.POINT_OFFSETS[i + 1]) && !base.UnitApply(action, origin, origin.X + (int)current.X + ModShapes.OuterOutline.POINT_OFFSETS[i], origin.Y + (int)current.Y + ModShapes.OuterOutline.POINT_OFFSETS[i + 1], new object[0]) && this._quitOnFail)
						{
							bool result = false;
							return result;
						}
					}
				}
				return true;
			}
		}

		public class InnerOutline : GenModShape
		{
			private static readonly int[] POINT_OFFSETS = new int[]
			{
				1,
				0,
				-1,
				0,
				0,
				1,
				0,
				-1,
				1,
				1,
				1,
				-1,
				-1,
				1,
				-1,
				-1
			};
			private bool _useDiagonals;

			public InnerOutline(ShapeData data, bool useDiagonals = true)
				: base(data)
			{
				this._useDiagonals = useDiagonals;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = this._useDiagonals ? 16 : 8;
				foreach (Point16 current in this._data.GetData())
				{
					bool flag = false;
					for (int i = 0; i < num; i += 2)
					{
						if (!this._data.Contains((int)current.X + ModShapes.InnerOutline.POINT_OFFSETS[i], (int)current.Y + ModShapes.InnerOutline.POINT_OFFSETS[i + 1]))
						{
							flag = true;
							break;
						}
					}
					if (flag && !base.UnitApply(action, origin, (int)current.X + origin.X, (int)current.Y + origin.Y, new object[0]) && this._quitOnFail)
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
