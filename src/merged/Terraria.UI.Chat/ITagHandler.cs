using Microsoft.Xna.Framework;
using System;

namespace Terraria.UI.Chat
{
	public interface ITagHandler
	{
		TextSnippet Parse(string text, Color baseColor = default(Color), string options = null);
	}
}
