using System;
using System.IO;

namespace Terraria.DataStructures
{
	public class CachedBuffer
	{
		public readonly byte[] Data;
		public readonly BinaryWriter Writer;
		public readonly BinaryReader Reader;
		private readonly MemoryStream _memoryStream;
		private bool _isActive = true;

		public int Length
		{
			get
			{
				return this.Data.Length;
			}
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
		}

		public CachedBuffer(byte[] data)
		{
			this.Data = data;
			this._memoryStream = new MemoryStream(data);
			this.Writer = new BinaryWriter(this._memoryStream);
			this.Reader = new BinaryReader(this._memoryStream);
		}

		internal CachedBuffer Activate()
		{
			this._isActive = true;
			this._memoryStream.Position = 0L;
			return this;
		}

		public void Recycle()
		{
			if (this._isActive)
			{
				this._isActive = false;
				BufferPool.Recycle(this);
			}
		}
	}
}
