using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public abstract class GenStructure : GenBase
	{
		public abstract bool Place(Point origin, StructureMap structures);
	}
}
