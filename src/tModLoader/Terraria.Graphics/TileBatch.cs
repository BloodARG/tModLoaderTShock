using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#if !WINDOWS
using System.Collections.Generic;
#endif
namespace Terraria.Graphics
{
	#if WINDOWS
	public class TileBatch

#else
	public class TileBatch : IDisposable
#endif
	{
		#if WINDOWS
		private struct SpriteData
		{
			public Vector4 Source;
			public Vector4 Destination;
			public Vector2 Origin;
			public SpriteEffects Effects;
			public VertexColors Colors;
			public float Rotation;
		}
		private static readonly float[] CORNER_OFFSET_X;
		private static readonly float[] CORNER_OFFSET_Y;
		private GraphicsDevice _graphicsDevice;
		private TileBatch.SpriteData[] _spriteDataQueue = new TileBatch.SpriteData[2048];
		private Texture2D[] _spriteTextures;
		private int _queuedSpriteCount;

#else
		private class TextureComparer : IComparer<TileBatch.BatchDrawInfo>
		{
			public static TileBatch.TextureComparer Instance = new TileBatch.TextureComparer();

			public int Compare(TileBatch.BatchDrawInfo info1, TileBatch.BatchDrawInfo info2)
			{
				return info1.Texture.GetHashCode().CompareTo(info2.Texture.GetHashCode());
			}
		}

		private struct BatchDrawInfo
		{
			public static readonly TileBatch.BatchDrawInfo Empty = new TileBatch.BatchDrawInfo(null, 0, 0);
			public readonly Texture2D Texture;
			public readonly int Index;
			public int Count;

			public BatchDrawInfo(Texture2D texture, int index, int count)
			{
				this.Texture = texture;
				this.Index = index;
				this.Count = count;
			}

			public BatchDrawInfo(Texture2D texture)
			{
				this.Texture = texture;
				this.Index = 0;
				this.Count = 0;
			}
		}

		private class BatchDrawGroup
		{
			public VertexPositionColorTexture[] VertexArray;
			public TileBatch.BatchDrawInfo[] BatchDraws;
			public int BatchDrawCount;
			public int SpriteCount;

			public int VertexCount
			{
				get
				{
					return this.SpriteCount * 4;
				}
			}

			public BatchDrawGroup()
			{
				this.VertexArray = new VertexPositionColorTexture[8192];
				this.BatchDraws = new TileBatch.BatchDrawInfo[2048];
				this.BatchDrawCount = 0;
				this.SpriteCount = 0;
			}

			public void Reset()
			{
				this.BatchDrawCount = 0;
				this.SpriteCount = 0;
			}

			public void AddBatch(TileBatch.BatchDrawInfo batch)
			{
				TileBatch.BatchDrawInfo[] arg_18_0 = this.BatchDraws;
				int batchDrawCount = this.BatchDrawCount;
				this.BatchDrawCount = batchDrawCount + 1;
				arg_18_0[batchDrawCount] = batch;
			}
		}

		private SpriteSortMode _sortMode;
		private const int MAX_SPRITES = 2048;
		private const int MAX_VERTICES = 8192;
		private const int MAX_INDICES = 12288;
		private const int INITIAL_BATCH_DRAW_GROUP_COUNT = 32;
		private short[] _sortedIndexData = new short[12288];
		private DynamicIndexBuffer _sortedIndexBuffer;
		private int _lastBatchDrawGroupIndex;
		private TileBatch.BatchDrawInfo _currentBatchDrawInfo;
		private List<TileBatch.BatchDrawGroup> _batchDrawGroups = new List<TileBatch.BatchDrawGroup>();
		private readonly GraphicsDevice _graphicsDevice;
		#endif
		private SpriteBatch _spriteBatch;
		#if WINDOWS
		private static Vector2 _vector2Zero;
		private static Rectangle? _nullRectangle;
		private DynamicVertexBuffer _vertexBuffer;
		private DynamicIndexBuffer _indexBuffer;
		private short[] _fallbackIndexData;
		private VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[8192];
		private int _vertexBufferPosition;
		public TileBatch(GraphicsDevice graphicsDevice)
		{
			this._graphicsDevice = graphicsDevice;
			this._spriteBatch = new SpriteBatch(graphicsDevice);
			this.Allocate();
        }
		private void Allocate()
		{
			if (this._vertexBuffer == null || this._vertexBuffer.IsDisposed)
			{
				this._vertexBuffer = new DynamicVertexBuffer(this._graphicsDevice, typeof(VertexPositionColorTexture), 8192, BufferUsage.WriteOnly);
				this._vertexBufferPosition = 0;
				this._vertexBuffer.ContentLost += delegate(object sender, EventArgs e)
				{
					this._vertexBufferPosition = 0;
				};
			}
			if (this._indexBuffer == null || this._indexBuffer.IsDisposed)
			{
				if (this._fallbackIndexData == null)
				{
					this._fallbackIndexData = new short[12288];
					for (int i = 0; i < 2048; i++)
					{
						this._fallbackIndexData[i * 6] = (short)(i * 4);
						this._fallbackIndexData[i * 6 + 1] = (short)(i * 4 + 1);
						this._fallbackIndexData[i * 6 + 2] = (short)(i * 4 + 2);
						this._fallbackIndexData[i * 6 + 3] = (short)(i * 4);
						this._fallbackIndexData[i * 6 + 4] = (short)(i * 4 + 2);
						this._fallbackIndexData[i * 6 + 5] = (short)(i * 4 + 3);
					}
				}
				this._indexBuffer = new DynamicIndexBuffer(this._graphicsDevice, typeof(short), 12288, BufferUsage.WriteOnly);
				this._indexBuffer.SetData<short>(this._fallbackIndexData);
				this._indexBuffer.ContentLost += delegate(object sender, EventArgs e)
				{
					this._indexBuffer.SetData<short>(this._fallbackIndexData);
				};
			}
		}
        private void FlushRenderState()
		{
			this.Allocate();
			this._graphicsDevice.SetVertexBuffer(this._vertexBuffer);
			this._graphicsDevice.Indices = this._indexBuffer;
			this._graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		}
		public void Dispose()
		{
			if (this._vertexBuffer != null)
			{
				this._vertexBuffer.Dispose();
			}
			if (this._indexBuffer != null)
			{
				this._indexBuffer.Dispose();
			}
		}
		public void Begin()
		{
			this._spriteBatch.Begin();
			this._spriteBatch.End();
		}
		public void Draw(Texture2D texture, Vector2 position, VertexColors colors)
		{
			Vector4 vector = default(Vector4);
			vector.X = position.X;
			vector.Y = position.Y;
			vector.Z = 1f;
			vector.W = 1f;
			this.InternalDraw(texture, ref vector, true, ref TileBatch._nullRectangle, ref colors, ref TileBatch._vector2Zero, SpriteEffects.None, 0f);
		}
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, VertexColors colors, Vector2 origin, float scale, SpriteEffects effects)
		{
			Vector4 vector = default(Vector4);
			vector.X = position.X;
			vector.Y = position.Y;
			vector.Z = scale;
			vector.W = scale;
			this.InternalDraw(texture, ref vector, true, ref sourceRectangle, ref colors, ref origin, effects, 0f);
		}
		public void Draw(Texture2D texture, Vector4 destination, VertexColors colors)
		{
			this.InternalDraw(texture, ref destination, false, ref TileBatch._nullRectangle, ref colors, ref TileBatch._vector2Zero, SpriteEffects.None, 0f);
		}
		public void Draw(Texture2D texture, Vector2 position, VertexColors colors, Vector2 scale)
		{
			Vector4 vector = default(Vector4);
			vector.X = position.X;
			vector.Y = position.Y;
			vector.Z = scale.X;
			vector.W = scale.Y;
			this.InternalDraw(texture, ref vector, true, ref TileBatch._nullRectangle, ref colors, ref TileBatch._vector2Zero, SpriteEffects.None, 0f);
		}
		public void Draw(Texture2D texture, Vector4 destination, Rectangle? sourceRectangle, VertexColors colors)
		{
			this.InternalDraw(texture, ref destination, false, ref sourceRectangle, ref colors, ref TileBatch._vector2Zero, SpriteEffects.None, 0f);
		}
		public void Draw(Texture2D texture, Vector4 destination, Rectangle? sourceRectangle, VertexColors colors, Vector2 origin, SpriteEffects effects, float rotation)
		{
			this.InternalDraw(texture, ref destination, false, ref sourceRectangle, ref colors, ref origin, effects, rotation);
		}
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, VertexColors colors)
		{
			Vector4 vector = default(Vector4);
			vector.X = (float)destinationRectangle.X;
			vector.Y = (float)destinationRectangle.Y;
			vector.Z = (float)destinationRectangle.Width;
			vector.W = (float)destinationRectangle.Height;
			this.InternalDraw(texture, ref vector, false, ref sourceRectangle, ref colors, ref TileBatch._vector2Zero, SpriteEffects.None, 0f);
		}
		private static short[] CreateIndexData()
		{
			short[] array = new short[12288];
			for (int i = 0; i < 2048; i++)
			{
				array[i * 6] = (short)(i * 4);
				array[i * 6 + 1] = (short)(i * 4 + 1);
				array[i * 6 + 2] = (short)(i * 4 + 2);
				array[i * 6 + 3] = (short)(i * 4);
				array[i * 6 + 4] = (short)(i * 4 + 2);
				array[i * 6 + 5] = (short)(i * 4 + 3);
			}
			return array;
		}
		private unsafe void InternalDraw(Texture2D texture, ref Vector4 destination, bool scaleDestination, ref Rectangle? sourceRectangle, ref VertexColors colors, ref Vector2 origin, SpriteEffects effects, float rotation)
		{
			if (this._queuedSpriteCount >= this._spriteDataQueue.Length)
			{
				Array.Resize<TileBatch.SpriteData>(ref this._spriteDataQueue, this._spriteDataQueue.Length << 1);
			}
			fixed (TileBatch.SpriteData* ptr = &this._spriteDataQueue[this._queuedSpriteCount])
			{
				float num = destination.Z;
				float num2 = destination.W;
				if (sourceRectangle.HasValue)
				{
					Rectangle value = sourceRectangle.Value;
					ptr->Source.X = (float)value.X;
					ptr->Source.Y = (float)value.Y;
					ptr->Source.Z = (float)value.Width;
					ptr->Source.W = (float)value.Height;
					if (scaleDestination)
					{
						num *= (float)value.Width;
						num2 *= (float)value.Height;
					}
				}
				else
				{
					float num3 = (float)texture.Width;
					float num4 = (float)texture.Height;
					ptr->Source.X = 0f;
					ptr->Source.Y = 0f;
					ptr->Source.Z = num3;
					ptr->Source.W = num4;
					if (scaleDestination)
					{
						num *= num3;
						num2 *= num4;
					}
				}
				ptr->Destination.X = destination.X;
				ptr->Destination.Y = destination.Y;
				ptr->Destination.Z = num;
				ptr->Destination.W = num2;
				ptr->Origin.X = origin.X;
				ptr->Origin.Y = origin.Y;
				ptr->Effects = effects;
				ptr->Colors = colors;
				ptr->Rotation = rotation;
			}
			if (this._spriteTextures == null || this._spriteTextures.Length != this._spriteDataQueue.Length)
			{
				Array.Resize<Texture2D>(ref this._spriteTextures, this._spriteDataQueue.Length);
			}
			this._spriteTextures[this._queuedSpriteCount++] = texture;
		}
		public void End()
		{
			if (this._queuedSpriteCount == 0)
			{
				return;
			}
			this.FlushRenderState();
			this.Flush();
		}
		private void Flush()
		{
			Texture2D texture2D = null;
			int num = 0;
			for (int i = 0; i < this._queuedSpriteCount; i++)
			{
				if (this._spriteTextures[i] != texture2D)
				{
					if (i > num)
					{
						this.RenderBatch(texture2D, this._spriteDataQueue, num, i - num);
					}
					num = i;
					texture2D = this._spriteTextures[i];
				}
			}
			this.RenderBatch(texture2D, this._spriteDataQueue, num, this._queuedSpriteCount - num);
			Array.Clear(this._spriteTextures, 0, this._queuedSpriteCount);
			this._queuedSpriteCount = 0;
		}
		private unsafe void RenderBatch(Texture2D texture, TileBatch.SpriteData[] sprites, int offset, int count)
		{
			this._graphicsDevice.Textures[0] = texture;
			float num = 1f / (float)texture.Width;
			float num2 = 1f / (float)texture.Height;
			while (count > 0)
			{
				SetDataOptions options = SetDataOptions.NoOverwrite;
				int num3 = count;
				if (num3 > 2048 - this._vertexBufferPosition)
				{
					num3 = 2048 - this._vertexBufferPosition;
					if (num3 < 256)
					{
						this._vertexBufferPosition = 0;
						options = SetDataOptions.Discard;
						num3 = count;
						if (num3 > 2048)
						{
							num3 = 2048;
						}
					}
				}
				fixed (TileBatch.SpriteData* ptr = &sprites[offset])
				{
					fixed (VertexPositionColorTexture* ptr2 = &this._vertices[0])
					{
						TileBatch.SpriteData* ptr3 = ptr;
						VertexPositionColorTexture* ptr4 = ptr2;
						for (int i = 0; i < num3; i++)
						{
							float num4;
							float num5;
							if (ptr3->Rotation != 0f)
							{
								num4 = (float)Math.Cos((double)ptr3->Rotation);
								num5 = (float)Math.Sin((double)ptr3->Rotation);
							}
							else
							{
								num4 = 1f;
								num5 = 0f;
							}
							float num6 = ptr3->Origin.X / ptr3->Source.Z;
							float num7 = ptr3->Origin.Y / ptr3->Source.W;
							ptr4->Color = ptr3->Colors.TopLeftColor;
							ptr4[1].Color = ptr3->Colors.TopRightColor;
							ptr4[2].Color = ptr3->Colors.BottomRightColor;
							ptr4[3].Color = ptr3->Colors.BottomLeftColor;
							for (int j = 0; j < 4; j++)
							{
								float num8 = TileBatch.CORNER_OFFSET_X[j];
								float num9 = TileBatch.CORNER_OFFSET_Y[j];
								float num10 = (num8 - num6) * ptr3->Destination.Z;
								float num11 = (num9 - num7) * ptr3->Destination.W;
								float x = ptr3->Destination.X + num10 * num4 - num11 * num5;
								float y = ptr3->Destination.Y + num10 * num5 + num11 * num4;
								if ((ptr3->Effects & SpriteEffects.FlipVertically) != SpriteEffects.None)
								{
									num8 = 1f - num8;
								}
								if ((ptr3->Effects & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
								{
									num9 = 1f - num9;
								}
								ptr4->Position.X = x;
								ptr4->Position.Y = y;
								ptr4->Position.Z = 0f;
								ptr4->TextureCoordinate.X = (ptr3->Source.X + num8 * ptr3->Source.Z) * num;
								ptr4->TextureCoordinate.Y = (ptr3->Source.Y + num9 * ptr3->Source.W) * num2;
								ptr4++;
							}
							ptr3++;
						}
					}
				}
				int offsetInBytes = this._vertexBufferPosition * sizeof(VertexPositionColorTexture) * 4;
				this._vertexBuffer.SetData<VertexPositionColorTexture>(offsetInBytes, this._vertices, 0, num3 * 4, sizeof(VertexPositionColorTexture), options);
				int minVertexIndex = this._vertexBufferPosition * 4;
				int numVertices = num3 * 4;
				int startIndex = this._vertexBufferPosition * 6;
				int primitiveCount = num3 * 2;
				this._graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, minVertexIndex, numVertices, startIndex, primitiveCount);
				this._vertexBufferPosition += num3;
				offset += num3;
				count -= num3;
			}
		}
		static TileBatch()
		{
			// Note: this type is marked as 'beforefieldinit'.
			float[] array = new float[4];
			array[1] = 1f;
			array[2] = 1f;
			TileBatch.CORNER_OFFSET_X = array;
			TileBatch.CORNER_OFFSET_Y = new float[]
			{
				0f,
				0f,
				1f,
				1f
			};
        }

#else
		public void Begin()
		{
			this.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState)
		{
			this.Begin(sortMode, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
		{
			this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
		{
			this.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformationMatrix)
		{
			if (sortMode != SpriteSortMode.Deferred && sortMode != SpriteSortMode.Texture)
			{
				throw new NotImplementedException("TileBatch only supports SpriteSortMode.Deferred and SpriteSortMode.Texture.");
			}
			this._sortMode = sortMode;
			this._spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformationMatrix);
			this._spriteBatch.End();
		}

		public void End()
		{
			if (this._sortMode == SpriteSortMode.Deferred)
			{
				this.DrawBatch();
				return;
			}
			if (this._sortMode == SpriteSortMode.Texture)
			{
				this.SortedDrawBatch();
			}
		}

		public void Draw(Texture2D texture, Vector2 position, VertexColors colors)
		{
			this.Draw(texture, position, null, colors, Vector2.Zero, 1f, SpriteEffects.None);
		}

		public void Draw(Texture2D texture, Vector4 destination, VertexColors colors)
		{
			this.InternalDraw(texture, destination, null, colors, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}

		public void Draw(Texture2D texture, Vector4 destination, Rectangle? sourceRectangle, VertexColors colors)
		{
			this.InternalDraw(texture, destination, sourceRectangle, colors, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, VertexColors colors, Vector2 origin, float scale, SpriteEffects effects)
		{
			float z;
			float w;
			if (sourceRectangle.HasValue)
			{
				z = (float)sourceRectangle.Value.Width * scale;
				w = (float)sourceRectangle.Value.Height * scale;
			}
			else
			{
				z = (float)texture.Width * scale;
				w = (float)texture.Height * scale;
			}
			this.InternalDraw(texture, new Vector4(position.X, position.Y, z, w), sourceRectangle, colors, 0f, origin * scale, effects, 0f);
		}

		public void Draw(Texture2D texture, Vector4 destination, Rectangle? sourceRectangle, VertexColors colors, Vector2 origin, SpriteEffects effects, float rotation)
		{
			this.InternalDraw(texture, destination, sourceRectangle, colors, rotation, origin, effects, 0f);
		}

		internal void InternalDraw(Texture2D texture, Vector4 destinationRectangle, Rectangle? sourceRectangle, VertexColors colors, float rotation, Vector2 origin, SpriteEffects effect, float depth)
		{
			Vector4 vector;
			if (sourceRectangle.HasValue)
			{
				vector.X = (float)sourceRectangle.Value.X;
				vector.Y = (float)sourceRectangle.Value.Y;
				vector.Z = (float)sourceRectangle.Value.Width;
				vector.W = (float)sourceRectangle.Value.Height;
			}
			else
			{
				vector.X = 0f;
				vector.Y = 0f;
				vector.Z = (float)texture.Width;
				vector.W = (float)texture.Height;
			}
			Vector2 vector2;
			vector2.X = vector.X / (float)texture.Width;
			vector2.Y = vector.Y / (float)texture.Height;
			Vector2 vector3;
			vector3.X = (vector.X + vector.Z) / (float)texture.Width;
			vector3.Y = (vector.Y + vector.W) / (float)texture.Height;
			if ((effect & SpriteEffects.FlipVertically) != SpriteEffects.None)
			{
				float y = vector3.Y;
				vector3.Y = vector2.Y;
				vector2.Y = y;
			}
			if ((effect & SpriteEffects.FlipHorizontally) != SpriteEffects.None)
			{
				float x = vector3.X;
				vector3.X = vector2.X;
				vector2.X = x;
			}
			this.QueueSprite(destinationRectangle, -origin, colors, vector, vector2, vector3, texture, depth, rotation);
		}

		public TileBatch(GraphicsDevice graphicsDevice)
		{
			this._spriteBatch = new SpriteBatch(graphicsDevice);
			this._graphicsDevice = graphicsDevice;
			for (int i = 0; i < 32; i++)
			{
				this._batchDrawGroups.Add(new TileBatch.BatchDrawGroup());
			}
			this._sortedIndexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 12288, BufferUsage.WriteOnly);
		}

		~TileBatch()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this._sortedIndexBuffer.Dispose();
			this._spriteBatch.Dispose();
		}

		private void SortAndApplyIndexData(TileBatch.BatchDrawGroup batchDrawGroup)
		{
			Array.Sort<TileBatch.BatchDrawInfo>(batchDrawGroup.BatchDraws, 0, batchDrawGroup.BatchDrawCount, TileBatch.TextureComparer.Instance);
			int num = 0;
			for (int i = 0; i < batchDrawGroup.BatchDrawCount; i++)
			{
				TileBatch.BatchDrawInfo batchDrawInfo = batchDrawGroup.BatchDraws[i];
				for (int j = 0; j < batchDrawInfo.Count; j++)
				{
					int num2 = batchDrawInfo.Index * 4 + j * 4;
					this._sortedIndexData[num] = (short)num2;
					this._sortedIndexData[num + 1] = (short)(num2 + 1);
					this._sortedIndexData[num + 2] = (short)(num2 + 2);
					this._sortedIndexData[num + 3] = (short)(num2 + 3);
					this._sortedIndexData[num + 4] = (short)(num2 + 2);
					this._sortedIndexData[num + 5] = (short)(num2 + 1);
					num += 6;
				}
			}
			this._sortedIndexBuffer.SetData<short>(this._sortedIndexData, 0, num);
		}

		private void SortedDrawBatch()
		{
			if (this._lastBatchDrawGroupIndex == 0 && this._batchDrawGroups[this._lastBatchDrawGroupIndex].SpriteCount == 0)
			{
				return;
			}
			this.FlushRemainingBatch();
			VertexBuffer vertexBuffer = this._graphicsDevice.GetVertexBuffers()[0].VertexBuffer;
			this._graphicsDevice.Indices = this._sortedIndexBuffer;
			for (int i = 0; i <= this._lastBatchDrawGroupIndex; i++)
			{
				TileBatch.BatchDrawGroup batchDrawGroup = this._batchDrawGroups[i];
				int vertexCount = batchDrawGroup.VertexCount;
				vertexBuffer.SetData<VertexPositionColorTexture>(this._batchDrawGroups[i].VertexArray, 0, vertexCount);
				this.SortAndApplyIndexData(batchDrawGroup);
				int num = 0;
				for (int j = 0; j < batchDrawGroup.BatchDrawCount; j++)
				{
					TileBatch.BatchDrawInfo batchDrawInfo = batchDrawGroup.BatchDraws[j];
					this._graphicsDevice.Textures[0] = batchDrawInfo.Texture;
					int num2 = batchDrawInfo.Count;
					while (j + 1 < batchDrawGroup.BatchDrawCount && batchDrawInfo.Texture == batchDrawGroup.BatchDraws[j + 1].Texture)
					{
						num2 += batchDrawGroup.BatchDraws[j + 1].Count;
						j++;
					}
					this._graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, num2 * 4, num, num2 * 2);
					num += num2 * 6;
				}
				batchDrawGroup.Reset();
			}
			this._currentBatchDrawInfo = TileBatch.BatchDrawInfo.Empty;
			this._lastBatchDrawGroupIndex = 0;
		}

		private void DrawBatch()
		{
			if (this._lastBatchDrawGroupIndex == 0 && this._batchDrawGroups[this._lastBatchDrawGroupIndex].SpriteCount == 0)
			{
				return;
			}
			this.FlushRemainingBatch();
			VertexBuffer vertexBuffer = this._graphicsDevice.GetVertexBuffers()[0].VertexBuffer;
			for (int i = 0; i <= this._lastBatchDrawGroupIndex; i++)
			{
				TileBatch.BatchDrawGroup batchDrawGroup = this._batchDrawGroups[i];
				int vertexCount = batchDrawGroup.VertexCount;
				vertexBuffer.SetData<VertexPositionColorTexture>(this._batchDrawGroups[i].VertexArray, 0, vertexCount);
				for (int j = 0; j < batchDrawGroup.BatchDrawCount; j++)
				{
					TileBatch.BatchDrawInfo batchDrawInfo = batchDrawGroup.BatchDraws[j];
					this._graphicsDevice.Textures[0] = batchDrawInfo.Texture;
					this._graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, batchDrawInfo.Count * 4, batchDrawInfo.Index * 6, batchDrawInfo.Count * 2);
				}
				batchDrawGroup.Reset();
			}
			this._currentBatchDrawInfo = TileBatch.BatchDrawInfo.Empty;
			this._lastBatchDrawGroupIndex = 0;
		}

		private void QueueSprite(Vector4 destinationRect, Vector2 origin, VertexColors colors, Vector4 sourceRectangle, Vector2 texCoordTL, Vector2 texCoordBR, Texture2D texture, float depth, float rotation)
		{
			this.UpdateCurrentBatchDraw(texture);
			float num = origin.X / sourceRectangle.Z;
			float arg_49_0 = origin.Y / sourceRectangle.W;
			float x = destinationRect.X;
			float y = destinationRect.Y;
			float z = destinationRect.Z;
			float w = destinationRect.W;
			float num2 = num * z;
			float num3 = arg_49_0 * w;
			TileBatch.BatchDrawGroup arg_8C_0 = this._batchDrawGroups[this._lastBatchDrawGroupIndex];
			float num4;
			float num5;
			if (rotation != 0f)
			{
				num4 = (float)Math.Cos((double)rotation);
				num5 = (float)Math.Sin((double)rotation);
			}
			else
			{
				num4 = 1f;
				num5 = 0f;
			}
			int num6 = arg_8C_0.VertexCount;
			arg_8C_0.VertexArray[num6].Position.X = x + num2 * num4 - num3 * num5;
			arg_8C_0.VertexArray[num6].Position.Y = y + num2 * num5 + num3 * num4;
			arg_8C_0.VertexArray[num6].Position.Z = depth;
			arg_8C_0.VertexArray[num6].Color = colors.TopLeftColor;
			arg_8C_0.VertexArray[num6].TextureCoordinate.X = texCoordTL.X;
			arg_8C_0.VertexArray[num6].TextureCoordinate.Y = texCoordTL.Y;
			num6++;
			arg_8C_0.VertexArray[num6].Position.X = x + (num2 + z) * num4 - num3 * num5;
			arg_8C_0.VertexArray[num6].Position.Y = y + (num2 + z) * num5 + num3 * num4;
			arg_8C_0.VertexArray[num6].Position.Z = depth;
			arg_8C_0.VertexArray[num6].Color = colors.TopRightColor;
			arg_8C_0.VertexArray[num6].TextureCoordinate.X = texCoordBR.X;
			arg_8C_0.VertexArray[num6].TextureCoordinate.Y = texCoordTL.Y;
			num6++;
			arg_8C_0.VertexArray[num6].Position.X = x + num2 * num4 - (num3 + w) * num5;
			arg_8C_0.VertexArray[num6].Position.Y = y + num2 * num5 + (num3 + w) * num4;
			arg_8C_0.VertexArray[num6].Position.Z = depth;
			arg_8C_0.VertexArray[num6].Color = colors.BottomLeftColor;
			arg_8C_0.VertexArray[num6].TextureCoordinate.X = texCoordTL.X;
			arg_8C_0.VertexArray[num6].TextureCoordinate.Y = texCoordBR.Y;
			num6++;
			arg_8C_0.VertexArray[num6].Position.X = x + (num2 + z) * num4 - (num3 + w) * num5;
			arg_8C_0.VertexArray[num6].Position.Y = y + (num2 + z) * num5 + (num3 + w) * num4;
			arg_8C_0.VertexArray[num6].Position.Z = depth;
			arg_8C_0.VertexArray[num6].Color = colors.BottomRightColor;
			arg_8C_0.VertexArray[num6].TextureCoordinate.X = texCoordBR.X;
			arg_8C_0.VertexArray[num6].TextureCoordinate.Y = texCoordBR.Y;
			arg_8C_0.SpriteCount++;
		}

		private void UpdateCurrentBatchDraw(Texture2D texture)
		{
			TileBatch.BatchDrawGroup batchDrawGroup = this._batchDrawGroups[this._lastBatchDrawGroupIndex];
			if (batchDrawGroup.SpriteCount >= 2048)
			{
				this._currentBatchDrawInfo.Count = 2048 - this._currentBatchDrawInfo.Index;
				this._batchDrawGroups[this._lastBatchDrawGroupIndex].AddBatch(this._currentBatchDrawInfo);
				this._currentBatchDrawInfo = new TileBatch.BatchDrawInfo(texture);
				this._lastBatchDrawGroupIndex++;
				if (this._lastBatchDrawGroupIndex >= this._batchDrawGroups.Count)
				{
					this._batchDrawGroups.Add(new TileBatch.BatchDrawGroup());
					return;
				}
			}
			else if (texture != this._currentBatchDrawInfo.Texture)
			{
				if (batchDrawGroup.SpriteCount != 0 || this._lastBatchDrawGroupIndex != 0)
				{
					this._currentBatchDrawInfo.Count = batchDrawGroup.SpriteCount - this._currentBatchDrawInfo.Index;
					batchDrawGroup.AddBatch(this._currentBatchDrawInfo);
				}
				this._currentBatchDrawInfo = new TileBatch.BatchDrawInfo(texture, batchDrawGroup.SpriteCount, 0);
			}
		}

		private void FlushRemainingBatch()
		{
			TileBatch.BatchDrawGroup batchDrawGroup = this._batchDrawGroups[this._lastBatchDrawGroupIndex];
			if (this._currentBatchDrawInfo.Index != batchDrawGroup.SpriteCount)
			{
				this._currentBatchDrawInfo.Count = batchDrawGroup.SpriteCount - this._currentBatchDrawInfo.Index;
				batchDrawGroup.AddBatch(this._currentBatchDrawInfo);
			}
		}
		#endif
	}
}
