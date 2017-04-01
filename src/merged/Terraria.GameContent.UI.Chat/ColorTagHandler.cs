using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Chat
{
	public class ColorTagHandler : ITagHandler
	{
		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			TextSnippet textSnippet = new TextSnippet(text);
			int num;
			if (!int.TryParse(options, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out num))
			{
				return textSnippet;
			}
			textSnippet.Color = new Color(num >> 16 & 255, num >> 8 & 255, num & 255);
			return textSnippet;
		}
	}
}
