using System;

namespace Terraria.Modules
{
	public class AnchorTypesModule
	{
		public int[] tileValid;
		public int[] tileInvalid;
		public int[] tileAlternates;
		public int[] wallValid;

		public AnchorTypesModule(AnchorTypesModule copyFrom = null)
		{
			if (copyFrom == null)
			{
				this.tileValid = null;
				this.tileInvalid = null;
				this.tileAlternates = null;
				this.wallValid = null;
				return;
			}
			if (copyFrom.tileValid == null)
			{
				this.tileValid = null;
			}
			else
			{
				this.tileValid = new int[copyFrom.tileValid.Length];
				Array.Copy(copyFrom.tileValid, this.tileValid, this.tileValid.Length);
			}
			if (copyFrom.tileInvalid == null)
			{
				this.tileInvalid = null;
			}
			else
			{
				this.tileInvalid = new int[copyFrom.tileInvalid.Length];
				Array.Copy(copyFrom.tileInvalid, this.tileInvalid, this.tileInvalid.Length);
			}
			if (copyFrom.tileAlternates == null)
			{
				this.tileAlternates = null;
			}
			else
			{
				this.tileAlternates = new int[copyFrom.tileAlternates.Length];
				Array.Copy(copyFrom.tileAlternates, this.tileAlternates, this.tileAlternates.Length);
			}
			if (copyFrom.wallValid == null)
			{
				this.wallValid = null;
				return;
			}
			this.wallValid = new int[copyFrom.wallValid.Length];
			Array.Copy(copyFrom.wallValid, this.wallValid, this.wallValid.Length);
		}
	}
}
