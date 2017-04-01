using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#if WINDOWS
using System.Runtime.InteropServices;
#endif
using System.Threading;
using Terraria.Localization;

#if !WINDOWS
using Terraria.Utilities;
#endif
namespace Terraria.Graphics.Capture
{
	internal class CaptureCamera
	{
		private class CaptureChunk
		{
			public readonly Microsoft.Xna.Framework.Rectangle Area;
			public readonly Microsoft.Xna.Framework.Rectangle ScaledArea;

			public CaptureChunk(Microsoft.Xna.Framework.Rectangle area, Microsoft.Xna.Framework.Rectangle scaledArea)
			{
				this.Area = area;
				this.ScaledArea = scaledArea;
			}
		}

		public const int CHUNK_SIZE = 128;
		public const int FRAMEBUFFER_PIXEL_SIZE = 2048;
		public const int INNER_CHUNK_SIZE = 126;
		public const int MAX_IMAGE_SIZE = 4096;
		public const string CAPTURE_DIRECTORY = "Captures";
		private static bool CameraExists;
		private RenderTarget2D _frameBuffer;
		private RenderTarget2D _scaledFrameBuffer;
		private GraphicsDevice _graphics;
		private readonly object _captureLock = new object();
		private bool _isDisposed;
		private CaptureSettings _activeSettings;
		private Queue<CaptureCamera.CaptureChunk> _renderQueue = new Queue<CaptureCamera.CaptureChunk>();
		private SpriteBatch _spriteBatch;
		private byte[] _scaledFrameData;
		private byte[] _outputData;
		private Size _outputImageSize;
		private SamplerState _downscaleSampleState;
		private float _tilesProcessed;
		private float _totalTiles;

		public bool IsCapturing
		{
			get
			{
				Monitor.Enter(this._captureLock);
				bool result = this._activeSettings != null;
				Monitor.Exit(this._captureLock);
				return result;
			}
		}

		public CaptureCamera(GraphicsDevice graphics)
		{
			CaptureCamera.CameraExists = true;
			this._graphics = graphics;
			this._spriteBatch = new SpriteBatch(graphics);
			try
			{
				this._frameBuffer = new RenderTarget2D(graphics, 2048, 2048, false, graphics.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			}
			catch
			{
				Main.CaptureModeDisabled = true;
				return;
			}
			this._downscaleSampleState = SamplerState.AnisotropicClamp;
		}

		~CaptureCamera()
		{
			this.Dispose();
		}

		public void Capture(CaptureSettings settings)
		{
			Main.GlobalTimerPaused = true;
			Monitor.Enter(this._captureLock);
			if (this._activeSettings != null)
			{
				throw new InvalidOperationException("Capture called while another capture was already active.");
			}
			this._activeSettings = settings;
			Microsoft.Xna.Framework.Rectangle area = settings.Area;
			float num = 1f;
			if (settings.UseScaling)
			{
				if (area.Width << 4 > 4096)
				{
					num = 4096f / (float)(area.Width << 4);
				}
				if (area.Height << 4 > 4096)
				{
					num = Math.Min(num, 4096f / (float)(area.Height << 4));
				}
				num = Math.Min(1f, num);
				this._outputImageSize = new Size((int)MathHelper.Clamp((float)((int)(num * (float)(area.Width << 4))), 1f, 4096f), (int)MathHelper.Clamp((float)((int)(num * (float)(area.Height << 4))), 1f, 4096f));
				this._outputData = new byte[4 * this._outputImageSize.Width * this._outputImageSize.Height];
				int num2 = (int)Math.Floor((double)(num * 2048f));
				this._scaledFrameData = new byte[4 * num2 * num2];
				this._scaledFrameBuffer = new RenderTarget2D(this._graphics, num2, num2, false, this._graphics.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			}
			else
			{
				this._outputData = new byte[16777216];
			}
			this._tilesProcessed = 0f;
			this._totalTiles = (float)(area.Width * area.Height);
			for (int i = area.X; i < area.X + area.Width; i += 126)
			{
				for (int j = area.Y; j < area.Y + area.Height; j += 126)
				{
					int num3 = Math.Min(128, area.X + area.Width - i);
					int num4 = Math.Min(128, area.Y + area.Height - j);
					int width = (int)Math.Floor((double)(num * (float)(num3 << 4)));
					int height = (int)Math.Floor((double)(num * (float)(num4 << 4)));
					int x = (int)Math.Floor((double)(num * (float)(i - area.X << 4)));
					int y = (int)Math.Floor((double)(num * (float)(j - area.Y << 4)));
					this._renderQueue.Enqueue(new CaptureCamera.CaptureChunk(new Microsoft.Xna.Framework.Rectangle(i, j, num3, num4), new Microsoft.Xna.Framework.Rectangle(x, y, width, height)));
				}
			}
			Monitor.Exit(this._captureLock);
		}

		public void DrawTick()
		{
			Monitor.Enter(this._captureLock);
			if (this._activeSettings == null)
			{
				return;
			}
			if (this._renderQueue.Count > 0)
			{
				CaptureCamera.CaptureChunk captureChunk = this._renderQueue.Dequeue();
				this._graphics.SetRenderTarget(this._frameBuffer);
				this._graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
				Main.instance.DrawCapture(captureChunk.Area, this._activeSettings);
				if (this._activeSettings.UseScaling)
				{
					this._graphics.SetRenderTarget(this._scaledFrameBuffer);
					this._graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
					this._spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, this._downscaleSampleState, DepthStencilState.Default, RasterizerState.CullNone);
					this._spriteBatch.Draw(this._frameBuffer, new Microsoft.Xna.Framework.Rectangle(0, 0, this._scaledFrameBuffer.Width, this._scaledFrameBuffer.Height), Microsoft.Xna.Framework.Color.White);
					this._spriteBatch.End();
					this._graphics.SetRenderTarget(null);
					this._scaledFrameBuffer.GetData<byte>(this._scaledFrameData, 0, this._scaledFrameBuffer.Width * this._scaledFrameBuffer.Height * 4);
					this.DrawBytesToBuffer(this._scaledFrameData, this._outputData, this._scaledFrameBuffer.Width, this._outputImageSize.Width, captureChunk.ScaledArea);
				}
				else
				{
					this._graphics.SetRenderTarget(null);
					this.SaveImage(this._frameBuffer, captureChunk.ScaledArea.Width, captureChunk.ScaledArea.Height, ImageFormat.Png, this._activeSettings.OutputName, string.Concat(new object[]
							{
								captureChunk.Area.X,
								"-",
								captureChunk.Area.Y,
								".png"
							}));
				}
				this._tilesProcessed += (float)(captureChunk.Area.Width * captureChunk.Area.Height);
			}
			if (this._renderQueue.Count == 0)
			{
				this.FinishCapture();
			}
			Monitor.Exit(this._captureLock);
		}

		private unsafe void DrawBytesToBuffer(byte[] sourceBuffer, byte[] destinationBuffer, int sourceBufferWidth, int destinationBufferWidth, Microsoft.Xna.Framework.Rectangle area)
		{
			fixed (byte* ptr = &destinationBuffer[0])
			{
				byte* ptr2 = ptr;
				fixed (byte* ptr3 = &sourceBuffer[0])
				{
					byte* ptr4 = ptr3;
					ptr2 += destinationBufferWidth * area.Y + area.X << 2;
					for (int i = 0; i < area.Height; i++)
					{
						for (int j = 0; j < area.Width; j++)
						{
#if WINDOWS
							ptr2[2] = ptr4[0];
							ptr2[1] = ptr4[1];
							ptr2[0] = ptr4[2];
							ptr2[3] = ptr4[3];
#else
							ptr2[0] = ptr4[0];
							ptr2[1] = ptr4[1];
							ptr2[2] = ptr4[2];
							ptr2[3] = ptr4[3];
#endif
							ptr4 += 4;
							ptr2 += 4;
						}
						ptr4 += sourceBufferWidth - area.Width << 2;
						ptr2 += destinationBufferWidth - area.Width << 2;
					}
				}
			}
		}

		public float GetProgress()
		{
			return this._tilesProcessed / this._totalTiles;
		}

		private bool SaveImage(int width, int height, ImageFormat imageFormat, string filename)
		{
			bool result;
			try
			{
				Directory.CreateDirectory(string.Concat(new object[]
						{
							Main.SavePath,
							Path.DirectorySeparatorChar,
							"Captures",
							Path.DirectorySeparatorChar
						}));
#if WINDOWS
				using (Bitmap bitmap = new Bitmap(width, height))
				{
					System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
					BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
					IntPtr scan = bitmapData.Scan0;
					Marshal.Copy(this._outputData, 0, scan, width * height * 4);
					bitmap.UnlockBits(bitmapData);
					bitmap.Save(filename, imageFormat);
					bitmap.Dispose();
				}
#else
				using (FileStream fileStream = File.Create(filename))
				{
					PlatformUtilities.SavePng(fileStream, width, height, width, height, this._outputData);
				}
#endif
				result = true;
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				result = false;
			}
			return result;
		}

		private void SaveImage(Texture2D texture, int width, int height, ImageFormat imageFormat, string foldername, string filename)
		{
			string text = string.Concat(new object[]
				{
					Main.SavePath,
					Path.DirectorySeparatorChar,
					"Captures",
					Path.DirectorySeparatorChar,
					foldername
				});
			string filename2 = Path.Combine(text, filename);
			Directory.CreateDirectory(text);
			int elementCount = texture.Width * texture.Height * 4;
			texture.GetData<byte>(this._outputData, 0, elementCount);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
#if WINDOWS
						byte b = this._outputData[num + 2];
						this._outputData[num2 + 2] = this._outputData[num];
						this._outputData[num2] = b;
						this._outputData[num2 + 1] = this._outputData[num + 1];
						this._outputData[num2 + 3] = this._outputData[num + 3];
#else
					this._outputData[num2] = this._outputData[num];
					this._outputData[num2 + 1] = this._outputData[num + 1];
					this._outputData[num2 + 2] = this._outputData[num + 2];
					this._outputData[num2 + 3] = this._outputData[num + 3];
#endif
					num += 4;
					num2 += 4;
				}
				num += texture.Width - width << 2;
			}
#if WINDOWS
			using (Bitmap bitmap = new Bitmap(width, height))
			{
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
				IntPtr scan = bitmapData.Scan0;
				Marshal.Copy(this._outputData, 0, scan, width * height * 4);
				bitmap.UnlockBits(bitmapData);
				bitmap.Save(filename2, imageFormat);
			}
#else
			using (FileStream fileStream = File.Create(filename2))
			{
				PlatformUtilities.SavePng(fileStream, width, height, width, height, this._outputData);
			}
#endif
		}

		private void FinishCapture()
		{
			if (this._activeSettings.UseScaling)
			{
				int num = 0;
				while (!this.SaveImage(this._outputImageSize.Width, this._outputImageSize.Height, ImageFormat.Png, string.Concat(new object[]
						{
							Main.SavePath,
							Path.DirectorySeparatorChar,
							"Captures",
							Path.DirectorySeparatorChar,
							this._activeSettings.OutputName,
							".png"
						})))
				{
					GC.Collect();
					Thread.Sleep(5);
					num++;
					Console.WriteLine(Language.GetTextValue("Error.CaptureError"));
					if (num > 5)
					{
						Console.WriteLine(Language.GetTextValue("Error.UnableToCapture"));
						break;
					}
				}
			}
			this._outputData = null;
			this._scaledFrameData = null;
			Main.GlobalTimerPaused = false;
			CaptureInterface.EndCamera();
			if (this._scaledFrameBuffer != null)
			{
				this._scaledFrameBuffer.Dispose();
				this._scaledFrameBuffer = null;
			}
			this._activeSettings = null;
		}

		public void Dispose()
		{
#if CLIENT
			Monitor.Enter(this._captureLock);
			if (this._isDisposed)
			{
				return;
			}
			this._frameBuffer.Dispose();
			if (this._scaledFrameBuffer != null)
			{
				this._scaledFrameBuffer.Dispose();
				this._scaledFrameBuffer = null;
			}
			CaptureCamera.CameraExists = false;
			this._isDisposed = true;
			Monitor.Exit(this._captureLock);
#endif
		}
	}
}
