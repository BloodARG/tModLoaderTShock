﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.NRefactory.CSharp;

namespace Terraria.ModLoader.Setup
{
	public class PatchTask : Task
	{
		public readonly string baseDir;
		public readonly string srcDir;
		public readonly string patchDir;
		public readonly ProgramSetting<DateTime> cutoff;
		public readonly CSharpFormattingOptions format;
		private int warnings;
		private int failures;
		private StreamWriter logFile;

		public string FullBaseDir => Path.Combine(Program.baseDir, baseDir);
		public string FullSrcDir => Path.Combine(Program.baseDir, srcDir);
		public string FullPatchDir => Path.Combine(Program.baseDir, patchDir);

		public PatchTask(ITaskInterface taskInterface, string baseDir, string srcDir, string patchDir,
			ProgramSetting<DateTime> cutoff, CSharpFormattingOptions format = null) : base(taskInterface)
		{
			this.baseDir = baseDir;
			this.srcDir = srcDir;
			this.patchDir = patchDir;
			this.format = format;
			this.cutoff = cutoff;
		}

		public override bool StartupWarning()
		{
			return MessageBox.Show(
					"Any changes in /" + srcDir + " that have not been converted to patches will be lost.",
					"Possible loss of data", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
				== DialogResult.OK;
		}

		public override void Run()
		{
			taskInterface.SetStatus("Deleting Old Src");

			if (Directory.Exists(FullSrcDir))
				Directory.Delete(FullSrcDir, true);

			var baseFiles = Directory.EnumerateFiles(FullBaseDir, "*", SearchOption.AllDirectories);
			var patchFiles = Directory.EnumerateFiles(FullPatchDir, "*", SearchOption.AllDirectories);

			var removedFileList = Path.Combine(FullPatchDir, DiffTask.RemovedFileList);
			var removedFiles = File.Exists(removedFileList) ? new HashSet<string>(File.ReadAllLines(removedFileList)) : new HashSet<string>();

			var copyItems = new List<WorkItem>();
			var patchItems = new List<WorkItem>();
			var formatItems = new List<WorkItem>();


			foreach (var file in baseFiles)
			{
				var relPath = RelPath(FullBaseDir, file);
				if (DiffTask.excluded.Any(relPath.StartsWith) || removedFiles.Contains(relPath))
					continue;

				var srcPath = Path.Combine(FullSrcDir, relPath);
				copyItems.Add(new WorkItem("Copying: " + relPath, () => Copy(file, srcPath)));

				if (format != null && file.EndsWith(".cs"))
					formatItems.Add(new WorkItem("Formatting: " + relPath,
						() => FormatTask.Format(srcPath, format, taskInterface.CancellationToken())));
			}

			foreach (var file in patchFiles)
			{
				var relPath = RelPath(FullPatchDir, file);
				if (relPath.EndsWith(".patch"))
					patchItems.Add(new WorkItem("Patching: " + relPath, () => Patch(relPath)));
				else if (relPath != DiffTask.RemovedFileList)
					copyItems.Add(new WorkItem("Copying: " + relPath, () => Copy(file, Path.Combine(FullSrcDir, relPath))));
			}

			taskInterface.SetMaxProgress(copyItems.Count + formatItems.Count + patchItems.Count);
			ExecuteParallel(copyItems, false);
			ExecuteParallel(formatItems, false);

			try
			{
				CreateDirectory(Program.LogDir);
				logFile = new StreamWriter(Path.Combine(Program.LogDir, "patch.log"));
				ExecuteParallel(patchItems, false);
			}
			finally {
				logFile?.Close();
			}

			cutoff.Set(DateTime.Now);
		}

		public override bool Failed()
		{
			return failures > 0;
		}

		public override bool Warnings()
		{
			return warnings > 0;
		}

		public override void FinishedDialog()
		{
			MessageBox.Show(
				$"Patches applied with {failures} failures and {warnings} warnings.\nSee /logs/patch.log for details",
				"Patch Results", MessageBoxButtons.OK, Failed() ? MessageBoxIcon.Error : MessageBoxIcon.Warning);
		}

		private void Patch(string relPath)
		{
			var patchFullName = relPath.Remove(relPath.Length - 6);
			if (!File.Exists(Path.Combine(FullSrcDir, patchFullName)))
			{
				Log("MISSING file " + Path.Combine(srcDir, patchFullName) + "\r\n");
				failures++;
				return;
			}

			var patchText = File.ReadAllText(Path.Combine(FullPatchDir, relPath));
			patchText = PreparePatch(patchText);

			CallPatch(patchText, Path.Combine(srcDir, patchFullName));

			//just a copy of the original if the patch wasn't perfect, delete it, we still have it
			var fileName = Path.GetFileName(patchFullName);
			var fuzzFile = Path.Combine(FullSrcDir, Path.GetDirectoryName(patchFullName),
				fileName.Substring(0, Math.Min(fileName.Length, 13)) + "~");
			if (File.Exists(fuzzFile))
				File.Delete(fuzzFile);
		}

		//generates destination hunk offsets and enforces windows line endings
		private static string PreparePatch(string patchText) {
			var r = new Regex(DiffTask.HunkOffsetRegex);
			var lines = patchText.Split('\n');
			int delta = 0;
			for (int i = 0; i < lines.Length; i++) {
				lines[i] = lines[i].TrimEnd();
				if (lines[i].StartsWith("@@")) {
					var m = r.Match(lines[i]);
					var hunkOffset = int.Parse(m.Groups[1].Value) + delta;
					delta += int.Parse(m.Groups[4].Value) - int.Parse(m.Groups[2].Value);
					lines[i] = m.Result($"@@ -$1,$2 +{hunkOffset},$4 @@");
				}
			}
			return string.Join(Environment.NewLine, lines);
		}

		private void Log(string text)
		{
			lock (logFile)
			{
				logFile.Write(text);
			}
		}

		private void CallPatch(string patchText, string srcFile)
		{
			var output = new StringBuilder();
			var error = new StringBuilder();
			var log = new StringBuilder();
			Program.RunCmd(Program.toolsDir, Path.Combine(Program.toolsDir, "applydiff.exe"),
				$"-u -N -p0 -d {Program.baseDir} {srcFile}",
				s => { output.Append(s); lock(log) log.Append(s); },
				s => { error.Append(s); lock(log) log.Append(s); },
				patchText
			);

			Log(log.ToString());

			if (error.Length > 0)
				throw new Exception(error.ToString());

			foreach (var line in output.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None))
			{
				if (line.StartsWith("Hunk"))
				{
					if (line.Contains("FAILED")) failures++;
					else if (line.Contains("fuzz")) warnings++;
				}
			}
		}
	}
}