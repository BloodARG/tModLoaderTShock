using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ShapeFloodFill : GenShape
	{
		private int _maximumActions;

		public ShapeFloodFill(int maximumActions = 100)
		{
			this._maximumActions = maximumActions;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			Queue<Point> queue = new Queue<Point>();
			HashSet<Point16> hashSet = new HashSet<Point16>();
			queue.Enqueue(origin);
			int num = this._maximumActions;
			while (queue.Count > 0)
			{
				if (num <= 0)
				{
					break;
				}
				Point point = queue.Dequeue();
				if (!hashSet.Contains(new Point16(point.X, point.Y)))
				{
					bool flag = base.UnitApply(action, origin, point.X, point.Y, new object[0]);
					if (flag)
					{
						hashSet.Add(new Point16(point));
						num--;
						if (point.X + 1 < Main.maxTilesX - 1)
						{
							queue.Enqueue(new Point(point.X + 1, point.Y));
						}
						if (point.X - 1 >= 1)
						{
							queue.Enqueue(new Point(point.X - 1, point.Y));
						}
						if (point.Y + 1 < Main.maxTilesY - 1)
						{
							queue.Enqueue(new Point(point.X, point.Y + 1));
						}
						if (point.Y - 1 >= 1)
						{
							queue.Enqueue(new Point(point.X, point.Y - 1));
						}
					}
				}
			}
			while (queue.Count > 0)
			{
				Point item = queue.Dequeue();
				if (!hashSet.Contains(new Point16(item.X, item.Y)))
				{
					queue.Enqueue(item);
					break;
				}
			}
			return queue.Count == 0;
		}
	}
}
