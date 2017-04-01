using System;

namespace Terraria.UI
{
	public struct StyleDimension
	{
		public static StyleDimension Fill = new StyleDimension(0f, 1f);
		public static StyleDimension Empty = new StyleDimension(0f, 0f);
		public float Pixels;
		public float Precent;

		public StyleDimension(float pixels, float precent)
		{
			this.Pixels = pixels;
			this.Precent = precent;
		}

		public void Set(float pixels, float precent)
		{
			this.Pixels = pixels;
			this.Precent = precent;
		}

		public float GetValue(float containerSize)
		{
			return this.Pixels + this.Precent * containerSize;
		}
	}
}
