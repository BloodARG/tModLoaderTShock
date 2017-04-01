﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Terraria.ModLoader.Properties;
using static Terraria.ModLoader.Setup.Program;

namespace Terraria.ModLoader.Setup
{
	public partial class MainForm : Form, ITaskInterface
	{
		private CancellationTokenSource cancelSource;

		private bool closeOnCancel;
		private IDictionary<Button, Func<Task>> taskButtons = new Dictionary<Button, Func<Task>>();

		public MainForm()
		{
			InitializeComponent();

			taskButtons[buttonDecompile] = () => new DecompileTask(this, "src/decompiled");
			taskButtons[buttonDiffMerged] = () => new DiffTask(this, "src/decompiled", "src/merged", "patches/merged", new ProgramSetting<DateTime>("MergedDiffCutoff"));
			taskButtons[buttonPatchMerged] = () => new PatchTask(this, "src/decompiled", "src/merged", "patches/merged", new ProgramSetting<DateTime>("MergedDiffCutoff"));
			taskButtons[buttonDiffTerraria] = () => new DiffTask(this, "src/merged", "src/Terraria", "patches/Terraria", new ProgramSetting<DateTime>("TerrariaDiffCutoff"));
			taskButtons[buttonPatchTerraria] = () => new PatchTask(this, "src/merged", "src/Terraria", "patches/Terraria", new ProgramSetting<DateTime>("TerrariaDiffCutoff"));
			taskButtons[buttonDiffModLoader] = () => new DiffTask(this, "src/Terraria", "src/tModLoader", "patches/tModLoader", new ProgramSetting<DateTime>("tModLoaderDiffCutoff"), FormatTask.tModLoaderFormat);
			taskButtons[buttonPatchModLoader] = () => new PatchTask(this, "src/Terraria", "src/tModLoader", "patches/tModLoader", new ProgramSetting<DateTime>("tModLoaderDiffCutoff"), FormatTask.tModLoaderFormat);
			taskButtons[buttonSetupDebugging] = () => new SetupDebugTask(this);

			taskButtons[buttonRegenSource] = () =>
				new RegenSourceTask(this, new[] { buttonPatchMerged, buttonPatchTerraria, buttonPatchModLoader, buttonSetupDebugging }
					.Select(b => taskButtons[b]()).ToArray());

			taskButtons[buttonSetup] = () =>
				new SetupTask(this, new[] { buttonDecompile, buttonPatchMerged, buttonPatchTerraria, buttonPatchModLoader, buttonSetupDebugging }
					.Select(b => taskButtons[b]()).ToArray());

			menuItemWarnings.Checked = Settings.Default.SuppressWarnings;
			menuItemSingleDecompileThread.Checked = Settings.Default.SingleDecompileThread;

			Closing += (sender, args) =>
			{
				if (buttonCancel.Enabled)
				{
					cancelSource.Cancel();
					args.Cancel = true;
					closeOnCancel = true;
				}
			};
		}

		public void SetMaxProgress(int max)
		{
			Invoke(new Action(() =>
			{
				progressBar.Maximum = max;
			}));
		}

		public void SetStatus(string status)
		{
			Invoke(new Action(() =>
			{
				labelStatus.Text = status;
			}));
		}

		public void SetProgress(int progress)
		{
			Invoke(new Action(() =>
			{
				progressBar.Value = progress;
			}));
		}

		public CancellationToken CancellationToken()
		{
			return cancelSource.Token;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			cancelSource.Cancel();
		}

		private void menuItemTerraria_Click(object sender, EventArgs e)
		{
			SelectTerrariaDialog();
		}

		private void menuItemWarnings_Click(object sender, EventArgs e)
		{
			Settings.Default.SuppressWarnings = menuItemWarnings.Checked;
			Settings.Default.Save();
		}

		private void menuItemSingleDecompileThread_Click(object sender, EventArgs e)
		{
			Settings.Default.SingleDecompileThread = menuItemSingleDecompileThread.Checked;
			Settings.Default.Save();
		}

		private void menuItemResetTimeStampOptmizations_Click(object sender, EventArgs e)
		{
			Settings.Default.MergedDiffCutoff = new DateTime(2015, 1, 1);
			Settings.Default.TerrariaDiffCutoff = new DateTime(2015, 1, 1);
			Settings.Default.tModLoaderDiffCutoff = new DateTime(2015, 1, 1);
			Settings.Default.Save();
		}

		private void menuItemDecompileServer_Click(object sender, EventArgs e) {
			RunTask(new DecompileTask(this, "src/decompiled_server", true));
		}

		private void menuItemFormatCode_Click(object sender, EventArgs e) {
			RunTask(new FormatTask(this, FormatTask.tModLoaderFormat));
		}

		private void buttonTask_Click(object sender, EventArgs e)
		{
			RunTask(taskButtons[(Button)sender]());
		}

		private void RunTask(Task task)
		{
			cancelSource = new CancellationTokenSource();
			foreach (var b in taskButtons.Keys) b.Enabled = false;
			buttonCancel.Enabled = true;

			new Thread(() => RunTaskThread(task)).Start();
		}

		private void RunTaskThread(Task task)
		{
			var errorLogFile = Path.Combine(Program.LogDir, "error.log");
			try
			{
				if (File.Exists(errorLogFile))
					File.Delete(errorLogFile);

				if (!task.ConfigurationDialog())
					return;

				if (!Settings.Default.SuppressWarnings && !task.StartupWarning())
					return;

				try
				{
					task.Run();

					if (cancelSource.IsCancellationRequested)
						throw new OperationCanceledException();
				}
				catch (OperationCanceledException e)
				{
					Invoke(new Action(() =>
					{
						labelStatus.Text = "Cancelled";
						if (e.Message != new OperationCanceledException().Message)
							labelStatus.Text += ": " + e.Message;
					}));
					return;
				}

				if ((task.Failed() || task.Warnings() && !Settings.Default.SuppressWarnings))
					task.FinishedDialog();

				Invoke(new Action(() =>
				{
					labelStatus.Text = task.Failed() ? "Failed" : "Done";
				}));
			}
			catch (Exception e)
			{
				var status = "";
				Invoke(new Action(() =>
				{
					status = labelStatus.Text;
					labelStatus.Text = "Error: " + e.Message.Trim();
				}));

				Task.CreateDirectory(Program.LogDir);
				File.WriteAllText(errorLogFile, status + "\r\n" + e);
			}
			finally
			{
				Invoke(new Action(() =>
				{
					foreach (var b in taskButtons.Keys) b.Enabled = true;
					buttonCancel.Enabled = false;
					progressBar.Value = 0;
					if (closeOnCancel) Close();
				}));
			}
		}
	}
}
