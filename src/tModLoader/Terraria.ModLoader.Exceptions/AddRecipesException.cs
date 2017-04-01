﻿using System;

namespace Terraria.ModLoader.Exceptions
{
	public class AddRecipesException : Exception
	{
		public readonly string modName;

		public AddRecipesException(Mod mod, string message, Exception inner)
			: base(message, inner)
		{
			this.modName = mod.Name;
		}
	}
}
