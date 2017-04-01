using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.Graphics.Capture
{
	public class CaptureManager
	{
		public static CaptureManager Instance = new CaptureManager();
		private CaptureInterface _interface;
		private CaptureCamera _camera;

		public bool IsCapturing
		{
			get
			{
#if CLIENT
				return this._camera.IsCapturing;
#else
				return false;
#endif
			}
		}

		public bool Active
		{
			get
			{
				return this._interface.Active;
			}
			set
			{
				if (Main.CaptureModeDisabled)
				{
					return;
				}
				if (this._interface.Active != value)
				{
					this._interface.ToggleCamera(value);
				}
			}
		}

		public bool UsingMap
		{
			get
			{
				return this.Active && this._interface.UsingMap();
			}
		}

		public CaptureManager()
		{
			this._interface = new CaptureInterface();
#if CLIENT
			this._camera = new CaptureCamera(Main.instance.GraphicsDevice);
#endif
		}

		public void Scrolling()
		{
			this._interface.Scrolling();
		}

		public void Update()
		{
			this._interface.Update();
		}

		public void Draw(SpriteBatch sb)
		{
			this._interface.Draw(sb);
		}

		public float GetProgress()
		{
			return this._camera.GetProgress();
		}

		public void Capture()
		{
			this.Capture(new CaptureSettings
				{
					Area = new Rectangle(2660, 100, 1000, 1000),
					UseScaling = false
				});
		}

		public void Capture(CaptureSettings settings)
		{
			this._camera.Capture(settings);
		}

		public void DrawTick()
		{
			this._camera.DrawTick();
		}
	}
}
