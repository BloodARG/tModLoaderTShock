using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using TerrariaApi.Server;

namespace Terraria
{
	public class Animation
	{
		private static List<Animation> _animations;
		private static Dictionary<Point16, Animation> _temporaryAnimations;
		private static List<Point16> _awaitingRemoval;
		private static List<Animation> _awaitingAddition;
		private bool _temporary;
		private Point16 _coordinates;
		private ushort _tileType;
		private int _frame;
		private int _frameMax;
		private int _frameCounter;
		private int _frameCounterMax;
		private int[] _frameData;

		public static void Initialize()
		{
			Animation._animations = new List<Animation>();
			Animation._temporaryAnimations = new Dictionary<Point16, Animation>();
			Animation._awaitingRemoval = new List<Point16>();
			Animation._awaitingAddition = new List<Animation>();
		}

		private void SetDefaults(int type)
		{
            //if (ServerApi.Hooks.InvokeItemSetDefaultsString(ref Itemame, this))
            //{
            //    return;
            //}
            //this.name = "";
            //bool flag = false;
            //if (ItemName != "")
            //{
            //    for (int i = 0; i < 3770; i++)
            //    {
            //        if (Main.itemName[i] == ItemName)
            //        {
            //            this.SetDefaults(i, false);
            //            this.checkMat();
            //            return;
            //        }
            //    }
            //    this.name = "";
                this._tileType = 0;
			this._frame = 0;
			this._frameMax = 0;
			this._frameCounter = 0;
			this._frameCounterMax = 0;
			this._temporary = false;
			switch (type)
			{
				case 0:
					this._frameMax = 5;
					this._frameCounterMax = 12;
					this._frameData = new int[this._frameMax];
					for (int i = 0; i < this._frameMax; i++)
					{
						this._frameData[i] = i + 1;
					}
					return;
				case 1:
					this._frameMax = 5;
					this._frameCounterMax = 12;
					this._frameData = new int[this._frameMax];
					for (int j = 0; j < this._frameMax; j++)
					{
						this._frameData[j] = 5 - j;
					}
					return;
				case 2:
					this._frameCounterMax = 6;
					this._frameData = new int[]
					{
						1,
						2,
						2,
						2,
						1
					};
					this._frameMax = this._frameData.Length;
					return;
				default:
					return;
			}
		}

		public static void NewTemporaryAnimation(int type, ushort tileType, int x, int y)
		{
			Point16 coordinates = new Point16(x, y);
			if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
			{
				return;
			}
			Animation animation = new Animation();
			animation.SetDefaults(type);
			animation._tileType = tileType;
			animation._coordinates = coordinates;
			animation._temporary = true;
			Animation._awaitingAddition.Add(animation);
			if (Main.netMode == 2)
			{
				NetMessage.SendTemporaryAnimation(-1, type, (int)tileType, x, y);
			}
		}

		private static void RemoveTemporaryAnimation(short x, short y)
		{
			Point16 point = new Point16(x, y);
			if (Animation._temporaryAnimations.ContainsKey(point))
			{
				Animation._awaitingRemoval.Add(point);
			}
		}

		public static void UpdateAll()
		{
			for (int i = 0; i < Animation._animations.Count; i++)
			{
				Animation._animations[i].Update();
			}
			if (Animation._awaitingAddition.Count > 0)
			{
				for (int j = 0; j < Animation._awaitingAddition.Count; j++)
				{
					Animation animation = Animation._awaitingAddition[j];
					Animation._temporaryAnimations[animation._coordinates] = animation;
				}
				Animation._awaitingAddition.Clear();
			}
			foreach (KeyValuePair<Point16, Animation> current in Animation._temporaryAnimations)
			{
				current.Value.Update();
			}
			if (Animation._awaitingRemoval.Count > 0)
			{
				for (int k = 0; k < Animation._awaitingRemoval.Count; k++)
				{
					Animation._temporaryAnimations.Remove(Animation._awaitingRemoval[k]);
				}
				Animation._awaitingRemoval.Clear();
			}
		}

		public void Update()
		{
			if (this._temporary)
			{
				Tile tile = Main.tile[(int)this._coordinates.X, (int)this._coordinates.Y];
				if (tile != null && tile.type != this._tileType)
				{
					Animation.RemoveTemporaryAnimation(this._coordinates.X, this._coordinates.Y);
					return;
				}
			}
			this._frameCounter++;
			if (this._frameCounter >= this._frameCounterMax)
			{
				this._frameCounter = 0;
				this._frame++;
				if (this._frame >= this._frameMax)
				{
					this._frame = 0;
					if (this._temporary)
					{
						Animation.RemoveTemporaryAnimation(this._coordinates.X, this._coordinates.Y);
					}
				}
			}
		}

		public static bool GetTemporaryFrame(int x, int y, out int frameData)
		{
			Point16 key = new Point16(x, y);
			Animation animation;
			if (!Animation._temporaryAnimations.TryGetValue(key, out animation))
			{
				frameData = 0;
				return false;
			}
			frameData = animation._frameData[animation._frame];
			return true;
		}
	}
}
