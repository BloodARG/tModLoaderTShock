using Microsoft.Xna.Framework;
using System;

namespace Terraria.UI.Gamepad
{
	public class UILinkPoint
	{
		public int ID;

		public bool Enabled;

		public Vector2 Position;

		public int Left;

		public int Right;

		public int Up;

		public int Down;

		public event Func<string> OnSpecialInteracts;

		public int Page
		{
			get;
			private set;
		}

		public UILinkPoint(int id, bool enabled, int left, int right, int up, int down)
		{
			this.ID = id;
			this.Enabled = enabled;
			this.Left = left;
			this.Right = right;
			this.Up = up;
			this.Down = down;
		}

		public void SetPage(int page)
		{
			this.Page = page;
		}

		public void Unlink()
		{
			this.Left = -3;
			this.Right = -4;
			this.Up = -1;
			this.Down = -2;
		}

		public string SpecialInteractions()
		{
			if (this.OnSpecialInteracts != null)
			{
				return this.OnSpecialInteracts();
			}
			return string.Empty;
		}
	}
}
