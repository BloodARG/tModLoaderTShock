using System;
using System.Collections.Generic;

namespace Terraria.UI.Gamepad
{
	public class UILinkPage
	{
		public int ID;
		public int PageOnLeft = -1;
		public int PageOnRight = -1;
		public int DefaultPoint;
		public int CurrentPoint;
		public Dictionary<int, UILinkPoint> LinkMap = new Dictionary<int, UILinkPoint>();

		public event Action<int, int> ReachEndEvent;
		public event Action TravelEvent;
		public event Action LeaveEvent;
		public event Action EnterEvent;
		public event Action UpdateEvent;
		public event Func<bool> IsValidEvent;
		public event Func<bool> CanEnterEvent;
		public event Func<string> OnSpecialInteracts;

		public UILinkPage()
		{
		}

		public UILinkPage(int id)
		{
			this.ID = id;
		}

		public void Update()
		{
			if (this.UpdateEvent != null)
			{
				this.UpdateEvent();
			}
		}

		public void Leave()
		{
			if (this.LeaveEvent != null)
			{
				this.LeaveEvent();
			}
		}

		public void Enter()
		{
			if (this.EnterEvent != null)
			{
				this.EnterEvent();
			}
		}

		public bool IsValid()
		{
			return this.IsValidEvent == null || this.IsValidEvent();
		}

		public bool CanEnter()
		{
			return this.CanEnterEvent == null || this.CanEnterEvent();
		}

		public void TravelUp()
		{
			this.Travel(this.LinkMap[this.CurrentPoint].Up);
		}

		public void TravelDown()
		{
			this.Travel(this.LinkMap[this.CurrentPoint].Down);
		}

		public void TravelLeft()
		{
			this.Travel(this.LinkMap[this.CurrentPoint].Left);
		}

		public void TravelRight()
		{
			this.Travel(this.LinkMap[this.CurrentPoint].Right);
		}

		public void SwapPageLeft()
		{
			UILinkPointNavigator.ChangePage(this.PageOnLeft);
		}

		public void SwapPageRight()
		{
			UILinkPointNavigator.ChangePage(this.PageOnRight);
		}

		private void Travel(int next)
		{
			if (next < 0)
			{
				if (this.ReachEndEvent != null)
				{
					this.ReachEndEvent(this.CurrentPoint, next);
					if (this.TravelEvent != null)
					{
						this.TravelEvent();
						return;
					}
				}
			}
			else
			{
				UILinkPointNavigator.ChangePoint(next);
				if (this.TravelEvent != null)
				{
					this.TravelEvent();
				}
			}
		}

		public string SpecialInteractions()
		{
			if (this.OnSpecialInteracts != null)
			{
				return this.OnSpecialInteracts();
			}
			return string.Empty;
		}
	}
}
