using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace Terraria.World.Generation
{
	public class ShapeData
	{
		private HashSet<Point16> _points;

		public int Count
		{
			get
			{
				return this._points.Count;
			}
		}

		public ShapeData()
		{
			this._points = new HashSet<Point16>();
		}

		public ShapeData(ShapeData original)
		{
			this._points = new HashSet<Point16>(original._points);
		}

		public void Add(int x, int y)
		{
			this._points.Add(new Point16(x, y));
		}

		public void Remove(int x, int y)
		{
			this._points.Remove(new Point16(x, y));
		}

		public HashSet<Point16> GetData()
		{
			return this._points;
		}

		public void Clear()
		{
			this._points.Clear();
		}

		public bool Contains(int x, int y)
		{
			return this._points.Contains(new Point16(x, y));
		}

		public void Add(ShapeData shapeData, Point localOrigin, Point remoteOrigin)
		{
			foreach (Point16 current in shapeData.GetData())
			{
				this.Add(remoteOrigin.X - localOrigin.X + (int)current.X, remoteOrigin.Y - localOrigin.Y + (int)current.Y);
			}
		}

		public void Subtract(ShapeData shapeData, Point localOrigin, Point remoteOrigin)
		{
			foreach (Point16 current in shapeData.GetData())
			{
				this.Remove(remoteOrigin.X - localOrigin.X + (int)current.X, remoteOrigin.Y - localOrigin.Y + (int)current.Y);
			}
		}

		public static Rectangle GetBounds(Point origin, params ShapeData[] shapes)
		{
			int num = (int)shapes[0]._points.First<Point16>().X;
			int num2 = num;
			int num3 = (int)shapes[0]._points.First<Point16>().Y;
			int num4 = num3;
			for (int i = 0; i < shapes.Length; i++)
			{
				foreach (Point16 current in shapes[i]._points)
				{
					num = Math.Max(num, (int)current.X);
					num2 = Math.Min(num2, (int)current.X);
					num3 = Math.Max(num3, (int)current.Y);
					num4 = Math.Min(num4, (int)current.Y);
				}
			}
			return new Rectangle(num2 + origin.X, num4 + origin.Y, num - num2, num3 - num4);
		}
	}
}
