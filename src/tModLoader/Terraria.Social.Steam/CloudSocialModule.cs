using Steamworks;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	public class CloudSocialModule : Terraria.Social.Base.CloudSocialModule
	{
		private const uint WRITE_CHUNK_SIZE = 1024u;
		private object ioLock = new object();
		private byte[] writeBuffer = new byte[1024];

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Shutdown()
		{
		}

		public override List<string> GetFiles(string matchPattern)
		{
			List<string> result;
			lock (this.ioLock)
			{
				matchPattern = "^" + matchPattern + "$";
				List<string> list = new List<string>();
				int fileCount = SteamRemoteStorage.GetFileCount();
				Regex regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
				for (int i = 0; i < fileCount; i++)
				{
					int num;
					string fileNameAndSize = SteamRemoteStorage.GetFileNameAndSize(i, out num);
					Match match = regex.Match(fileNameAndSize);
					if (match.Length > 0)
					{
						list.Add(fileNameAndSize);
					}
				}
				result = list;
			}
			return result;
		}

		public override bool Write(string path, byte[] data, int length)
		{
			bool result;
			lock (this.ioLock)
			{
				UGCFileWriteStreamHandle_t writeHandle = SteamRemoteStorage.FileWriteStreamOpen(path);
				uint num = 0u;
				while ((ulong)num < (ulong)((long)length))
				{
					int num2 = (int)Math.Min(1024L, (long)length - (long)((ulong)num));
					Array.Copy(data, (long)((ulong)num), this.writeBuffer, 0L, (long)num2);
					SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, this.writeBuffer, num2);
					num += 1024u;
				}
				result = SteamRemoteStorage.FileWriteStreamClose(writeHandle);
			}
			return result;
		}

		public override int GetFileSize(string path)
		{
			int fileSize;
			lock (this.ioLock)
			{
				fileSize = SteamRemoteStorage.GetFileSize(path);
			}
			return fileSize;
		}

		public override void Read(string path, byte[] buffer, int size)
		{
			lock (this.ioLock)
			{
				SteamRemoteStorage.FileRead(path, buffer, size);
			}
		}

		public override bool HasFile(string path)
		{
			bool result;
			lock (this.ioLock)
			{
				result = SteamRemoteStorage.FileExists(path);
			}
			return result;
		}

		public override bool Delete(string path)
		{
			bool result;
			lock (this.ioLock)
			{
				result = SteamRemoteStorage.FileDelete(path);
			}
			return result;
		}
	}
}
