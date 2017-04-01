using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.DataStructures
{
	public class DrawAnimationVertical : DrawAnimation
	{
		public DrawAnimationVertical(int ticksperframe, int frameCount)
		{
			this.Frame = 0;
			this.FrameCounter = 0;
			this.FrameCount = frameCount;
			this.TicksPerFrame = ticksperframe;
		}

		public override void Update()
		{
			if (++this.FrameCounter >= this.TicksPerFrame)
			{
				this.FrameCounter = 0;
				if (++this.Frame >= this.FrameCount)
				{
					this.Frame = 0;
				}
			}
		}

		public override Rectangle GetFrame(Texture2D texture)
		{
			return texture.Frame(1, this.FrameCount, 0, this.Frame);
		}
	}
}
