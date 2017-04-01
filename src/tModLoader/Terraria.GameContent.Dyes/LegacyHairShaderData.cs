using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Dyes
{
	public class LegacyHairShaderData : HairShaderData
	{
		public delegate Color ColorProcessingMethod(Player player, Color color, ref bool lighting);

		private LegacyHairShaderData.ColorProcessingMethod _colorProcessor;

		public LegacyHairShaderData()
			: base(null, null)
		{
			this._shaderDisabled = true;
		}

		public override Color GetColor(Player player, Color lightColor)
		{
			bool flag = true;
			Color result = this._colorProcessor(player, player.hairColor, ref flag);
			if (flag)
			{
				return new Color(result.ToVector4() * lightColor.ToVector4());
			}
			return result;
		}

		public LegacyHairShaderData UseLegacyMethod(LegacyHairShaderData.ColorProcessingMethod colorProcessor)
		{
			this._colorProcessor = colorProcessor;
			return this;
		}
	}
}
