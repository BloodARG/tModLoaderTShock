using System;
using Terraria.DataStructures;

namespace Terraria.Modules
{
	public class AnchorDataModule
	{
		public AnchorData top;
		public AnchorData bottom;
		public AnchorData left;
		public AnchorData right;
		public bool wall;

		public AnchorDataModule(AnchorDataModule copyFrom = null)
		{
			if (copyFrom == null)
			{
				this.top = default(AnchorData);
				this.bottom = default(AnchorData);
				this.left = default(AnchorData);
				this.right = default(AnchorData);
				this.wall = false;
				return;
			}
			this.top = copyFrom.top;
			this.bottom = copyFrom.bottom;
			this.left = copyFrom.left;
			this.right = copyFrom.right;
			this.wall = copyFrom.wall;
		}
	}
}
