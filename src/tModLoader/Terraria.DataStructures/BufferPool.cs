using System;
using System.Collections.Generic;

namespace Terraria.DataStructures
{
	public static class BufferPool
	{
		private const int SMALL_BUFFER_SIZE = 32;
		private const int MEDIUM_BUFFER_SIZE = 256;
		private const int LARGE_BUFFER_SIZE = 16384;
		private static object bufferLock = new object();
		private static Queue<CachedBuffer> SmallBufferQueue = new Queue<CachedBuffer>();
		private static Queue<CachedBuffer> MediumBufferQueue = new Queue<CachedBuffer>();
		private static Queue<CachedBuffer> LargeBufferQueue = new Queue<CachedBuffer>();

		public static CachedBuffer Request(int size)
		{
			CachedBuffer result;
			lock (BufferPool.bufferLock)
			{
				if (size <= 32)
				{
					if (BufferPool.SmallBufferQueue.Count == 0)
					{
						result = new CachedBuffer(new byte[32]);
					}
					else
					{
						result = BufferPool.SmallBufferQueue.Dequeue().Activate();
					}
				}
				else if (size <= 256)
				{
					if (BufferPool.MediumBufferQueue.Count == 0)
					{
						result = new CachedBuffer(new byte[256]);
					}
					else
					{
						result = BufferPool.MediumBufferQueue.Dequeue().Activate();
					}
				}
				else if (size <= 16384)
				{
					if (BufferPool.LargeBufferQueue.Count == 0)
					{
						result = new CachedBuffer(new byte[16384]);
					}
					else
					{
						result = BufferPool.LargeBufferQueue.Dequeue().Activate();
					}
				}
				else
				{
					result = new CachedBuffer(new byte[size]);
				}
			}
			return result;
		}

		public static CachedBuffer Request(byte[] data, int offset, int size)
		{
			CachedBuffer cachedBuffer = BufferPool.Request(size);
			Buffer.BlockCopy(data, offset, cachedBuffer.Data, 0, size);
			return cachedBuffer;
		}

		public static void Recycle(CachedBuffer buffer)
		{
			int length = buffer.Length;
			lock (BufferPool.bufferLock)
			{
				if (length <= 32)
				{
					BufferPool.SmallBufferQueue.Enqueue(buffer);
				}
				else if (length <= 256)
				{
					BufferPool.MediumBufferQueue.Enqueue(buffer);
				}
				else if (length <= 16384)
				{
					BufferPool.LargeBufferQueue.Enqueue(buffer);
				}
			}
		}

		public static void PrintBufferSizes()
		{
			lock (BufferPool.bufferLock)
			{
				Console.WriteLine("SmallBufferQueue.Count: " + BufferPool.SmallBufferQueue.Count);
				Console.WriteLine("MediumBufferQueue.Count: " + BufferPool.MediumBufferQueue.Count);
				Console.WriteLine("LargeBufferQueue.Count: " + BufferPool.LargeBufferQueue.Count);
				Console.WriteLine("");
			}
		}
	}
}
