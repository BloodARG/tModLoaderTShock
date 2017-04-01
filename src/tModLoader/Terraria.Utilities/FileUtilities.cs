using System;
using System.IO;
using System.Text.RegularExpressions;
using Terraria.Social;

namespace Terraria.Utilities
{
	public static class FileUtilities
	{
		private static Regex FileNameRegex = new Regex("^(?<path>.*[\\\\\\/])?(?:$|(?<fileName>.+?)(?:(?<extension>\\.[^.]*$)|$))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static bool Exists(string path, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				return SocialAPI.Cloud.HasFile(path);
			}
			return File.Exists(path);
		}

		public static void Delete(string path, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				SocialAPI.Cloud.Delete(path);
				return;
			}
#if WINDOWS
			FileOperationAPIWrapper.MoveToRecycleBin(path);
#else
			File.Delete(path);
#endif
		}

		public static string GetFullPath(string path, bool cloud)
		{
			if (!cloud)
			{
				return Path.GetFullPath(path);
			}
			return path;
		}

		public static void Copy(string source, string destination, bool cloud, bool overwrite = true)
		{
			if (!cloud)
			{
				File.Copy(source, destination, overwrite);
				return;
			}
			if (SocialAPI.Cloud == null || (!overwrite && SocialAPI.Cloud.HasFile(destination)))
			{
				return;
			}
			SocialAPI.Cloud.Write(destination, SocialAPI.Cloud.Read(source));
		}

		public static void Move(string source, string destination, bool cloud, bool overwrite = true)
		{
			FileUtilities.Copy(source, destination, cloud, overwrite);
			FileUtilities.Delete(source, cloud);
		}

		public static int GetFileSize(string path, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				return SocialAPI.Cloud.GetFileSize(path);
			}
			return (int)new FileInfo(path).Length;
		}

		public static void Read(string path, byte[] buffer, int length, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				SocialAPI.Cloud.Read(path, buffer, length);
				return;
			}
			using (FileStream fileStream = File.OpenRead(path))
			{
				fileStream.Read(buffer, 0, length);
			}
		}

		public static byte[] ReadAllBytes(string path, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				return SocialAPI.Cloud.Read(path);
			}
			return File.ReadAllBytes(path);
		}

		public static void WriteAllBytes(string path, byte[] data, bool cloud)
		{
			FileUtilities.Write(path, data, data.Length, cloud);
		}

		public static void Write(string path, byte[] data, int length, bool cloud)
		{
			if (cloud && SocialAPI.Cloud != null)
			{
				SocialAPI.Cloud.Write(path, data, length);
				return;
			}
			string parentFolderPath = FileUtilities.GetParentFolderPath(path, true);
			if (parentFolderPath != "")
			{
				Directory.CreateDirectory(parentFolderPath);
			}
			using (FileStream fileStream = File.Open(path, FileMode.Create))
			{
				while (fileStream.Position < (long)length)
				{
					fileStream.Write(data, (int)fileStream.Position, Math.Min(length - (int)fileStream.Position, 2048));
				}
			}
		}

		public static bool MoveToCloud(string localPath, string cloudPath)
		{
			if (SocialAPI.Cloud == null)
			{
				return false;
			}
			FileUtilities.WriteAllBytes(cloudPath, FileUtilities.ReadAllBytes(localPath, false), true);
			FileUtilities.Delete(localPath, false);
			return true;
		}

		public static bool MoveToLocal(string cloudPath, string localPath)
		{
			if (SocialAPI.Cloud == null)
			{
				return false;
			}
			FileUtilities.WriteAllBytes(localPath, FileUtilities.ReadAllBytes(cloudPath, true), false);
			FileUtilities.Delete(cloudPath, true);
			return true;
		}

		public static string GetFileName(string path, bool includeExtension = true)
		{
			Match match = FileUtilities.FileNameRegex.Match(path);
			if (match == null || match.Groups["fileName"] == null)
			{
				return "";
			}
			includeExtension &= (match.Groups["extension"] != null);
			return match.Groups["fileName"].Value + (includeExtension ? match.Groups["extension"].Value : "");
		}

		public static string GetParentFolderPath(string path, bool includeExtension = true)
		{
			Match match = FileUtilities.FileNameRegex.Match(path);
			if (match == null || match.Groups["path"] == null)
			{
				return "";
			}
			return match.Groups["path"].Value;
		}

		public static void CopyFolder(string sourcePath, string destinationPath)
		{
			Directory.CreateDirectory(destinationPath);
			string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
			for (int i = 0; i < directories.Length; i++)
			{
				string text = directories[i];
				Directory.CreateDirectory(text.Replace(sourcePath, destinationPath));
			}
			string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
			for (int j = 0; j < files.Length; j++)
			{
				string text2 = files[j];
				File.Copy(text2, text2.Replace(sourcePath, destinationPath), true);
			}
		}
	}
}
