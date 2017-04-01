using System;
using System.Collections.Generic;

namespace Terraria.DataStructures
{
	public class MethodSequenceListItem
	{
		public string Name;
		public MethodSequenceListItem Parent;
		public Func<bool> Method;
		public bool Skip;

		public MethodSequenceListItem(string name, Func<bool> method, MethodSequenceListItem parent = null)
		{
			this.Name = name;
			this.Method = method;
			this.Parent = parent;
		}

		public bool ShouldAct(List<MethodSequenceListItem> sequence)
		{
			return !this.Skip && sequence.Contains(this) && (this.Parent == null || this.Parent.ShouldAct(sequence));
		}

		public bool Act()
		{
			return this.Method();
		}

		public static void ExecuteSequence(List<MethodSequenceListItem> sequence)
		{
			foreach (MethodSequenceListItem current in sequence)
			{
				if (current.ShouldAct(sequence))
				{
					bool flag = !current.Act();
					if (flag)
					{
						break;
					}
				}
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
				{
					"name: ",
					this.Name,
					" skip: ",
					this.Skip,
					" parent: ",
					this.Parent
				});
		}
	}
}
