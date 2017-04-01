using System;
using System.Collections.Generic;

namespace Terraria.Net
{
	public class LegacyNetBufferPool
	{
		private const int SMALL_BUFFER_SIZE = 256;
		private const int MEDIUM_BUFFER_SIZE = 1024;
		private const int LARGE_BUFFER_SIZE = 16384;
		private static object bufferLock = new object();
		private static Queue<byte[]> _smallBufferQueue = new Queue<byte[]>();
		private static Queue<byte[]> _mediumBufferQueue = new Queue<byte[]>();
		private static Queue<byte[]> _largeBufferQueue = new Queue<byte[]>();
		private static int _smallBufferCount = 0;
		private static int _mediumBufferCount = 0;
		private static int _largeBufferCount = 0;
		private static int _customBufferCount = 0;

		public static byte[] RequestBuffer(int size)
		{
			byte[] result;
			lock (LegacyNetBufferPool.bufferLock)
			{
				if (size <= 256)
				{
					if (LegacyNetBufferPool._smallBufferQueue.Count == 0)
					{
						LegacyNetBufferPool._smallBufferCount++;
						result = new byte[256];
					}
					else
					{
						result = LegacyNetBufferPool._smallBufferQueue.Dequeue();
					}
				}
				else if (size <= 1024)
				{
					if (LegacyNetBufferPool._mediumBufferQueue.Count == 0)
					{
						LegacyNetBufferPool._mediumBufferCount++;
						result = new byte[1024];
					}
					else
					{
						result = LegacyNetBufferPool._mediumBufferQueue.Dequeue();
					}
				}
				else if (size <= 16384)
				{
					if (LegacyNetBufferPool._largeBufferQueue.Count == 0)
					{
						LegacyNetBufferPool._largeBufferCount++;
						result = new byte[16384];
					}
					else
					{
						result = LegacyNetBufferPool._largeBufferQueue.Dequeue();
					}
				}
				else
				{
					LegacyNetBufferPool._customBufferCount++;
					result = new byte[size];
				}
			}
			return result;
		}

		public static byte[] RequestBuffer(byte[] data, int offset, int size)
		{
			byte[] array = LegacyNetBufferPool.RequestBuffer(size);
			Buffer.BlockCopy(data, offset, array, 0, size);
			return array;
		}

		public static void ReturnBuffer(byte[] buffer)
		{
			int num = buffer.Length;
			lock (LegacyNetBufferPool.bufferLock)
			{
				if (num <= 256)
				{
					LegacyNetBufferPool._smallBufferQueue.Enqueue(buffer);
				}
				else if (num <= 1024)
				{
					LegacyNetBufferPool._mediumBufferQueue.Enqueue(buffer);
				}
				else if (num <= 16384)
				{
					LegacyNetBufferPool._largeBufferQueue.Enqueue(buffer);
				}
			}
		}

		public static void DisplayBufferSizes()
		{
			lock (LegacyNetBufferPool.bufferLock)
			{
				Main.NewText(string.Concat(new object[]
						{
							"Small Buffers:  ",
							LegacyNetBufferPool._smallBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._smallBufferCount
						}), 255, 255, 255, false);
				Main.NewText(string.Concat(new object[]
						{
							"Medium Buffers: ",
							LegacyNetBufferPool._mediumBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._mediumBufferCount
						}), 255, 255, 255, false);
				Main.NewText(string.Concat(new object[]
						{
							"Large Buffers:  ",
							LegacyNetBufferPool._largeBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._largeBufferCount
						}), 255, 255, 255, false);
				Main.NewText("Custom Buffers: 0 queued of " + LegacyNetBufferPool._customBufferCount, 255, 255, 255, false);
			}
		}

		public static void PrintBufferSizes()
		{
			lock (LegacyNetBufferPool.bufferLock)
			{
				Console.WriteLine(string.Concat(new object[]
						{
							"Small Buffers:  ",
							LegacyNetBufferPool._smallBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._smallBufferCount
						}));
				Console.WriteLine(string.Concat(new object[]
						{
							"Medium Buffers: ",
							LegacyNetBufferPool._mediumBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._mediumBufferCount
						}));
				Console.WriteLine(string.Concat(new object[]
						{
							"Large Buffers:  ",
							LegacyNetBufferPool._largeBufferQueue.Count,
							" queued of ",
							LegacyNetBufferPool._largeBufferCount
						}));
				Console.WriteLine("Custom Buffers: 0 queued of " + LegacyNetBufferPool._customBufferCount);
				Console.WriteLine("");
			}
		}
	}
}
