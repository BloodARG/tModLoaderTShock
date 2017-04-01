﻿using System.Linq;

namespace Terraria.ModLoader.Setup
{
	public class CompositeTask : Task
	{
		private Task[] tasks;
		private Task failed;

		public CompositeTask(ITaskInterface taskInterface, params Task[] tasks) : base(taskInterface) {
			this.tasks = tasks;
		}

		public override bool ConfigurationDialog() {
			return tasks.All(task => task.ConfigurationDialog());
		}

		public override bool Failed() {
			return failed != null;
		}

		public override void FinishedDialog() {
			if (failed != null)
				failed.FinishedDialog();
			else
				foreach (var task in tasks)
					task.FinishedDialog();
		}

		public override void Run() {
			foreach (var task in tasks) {
				task.Run();
				if (task.Failed()) {
					failed = task;
					return;
				}
			}
		}
	}
}
