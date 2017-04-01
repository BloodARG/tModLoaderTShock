using System;
using Terraria.UI;

namespace Terraria.GameContent.UI.States
{
	public class UISortableElement : UIElement
	{
		public int OrderIndex;

		public UISortableElement(int index)
		{
			this.OrderIndex = index;
		}

		public override int CompareTo(object obj)
		{
			UISortableElement uISortableElement = obj as UISortableElement;
			if (uISortableElement != null)
			{
				return this.OrderIndex.CompareTo(uISortableElement.OrderIndex);
			}
			return base.CompareTo(obj);
		}
	}
}
