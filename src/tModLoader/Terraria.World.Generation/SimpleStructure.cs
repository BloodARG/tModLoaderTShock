using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public class SimpleStructure : GenStructure
	{
		private int[,] _data;
		private int _width;
		private int _height;
		private GenAction[] _actions;
		private bool _xMirror;
		private bool _yMirror;

		public int Width
		{
			get
			{
				return this._width;
			}
		}

		public int Height
		{
			get
			{
				return this._height;
			}
		}

		public SimpleStructure(params string[] data)
		{
			this.ReadData(data);
		}

		public SimpleStructure(string data)
		{
			this.ReadData(data.Split(new char[]
					{
						'\n'
					}));
		}

		private void ReadData(string[] lines)
		{
			this._height = lines.Length;
			this._width = lines[0].Length;
			this._data = new int[this._width, this._height];
			for (int i = 0; i < this._height; i++)
			{
				for (int j = 0; j < this._width; j++)
				{
					int num = (int)lines[i][j];
					if (num >= 48 && num <= 57)
					{
						this._data[j, i] = num - 48;
					}
					else
					{
						this._data[j, i] = -1;
					}
				}
			}
		}

		public SimpleStructure SetActions(params GenAction[] actions)
		{
			this._actions = actions;
			return this;
		}

		public SimpleStructure Mirror(bool horizontalMirror, bool verticalMirror)
		{
			this._xMirror = horizontalMirror;
			this._yMirror = verticalMirror;
			return this;
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			if (!structures.CanPlace(new Rectangle(origin.X, origin.Y, this._width, this._height), 0))
			{
				return false;
			}
			for (int i = 0; i < this._width; i++)
			{
				for (int j = 0; j < this._height; j++)
				{
					int num = this._xMirror ? (-i) : i;
					int num2 = this._yMirror ? (-j) : j;
					if (this._data[i, j] != -1 && !this._actions[this._data[i, j]].Apply(origin, num + origin.X, num2 + origin.Y, new object[0]))
					{
						return false;
					}
				}
			}
			structures.AddStructure(new Rectangle(origin.X, origin.Y, this._width, this._height), 0);
			return true;
		}
	}
}
