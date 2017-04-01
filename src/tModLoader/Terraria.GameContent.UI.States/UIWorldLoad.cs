using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.World.Generation;

namespace Terraria.GameContent.UI.States
{
	public class UIWorldLoad : UIState
	{
		private UIGenProgressBar _progressBar = new UIGenProgressBar();
		private UIHeader _progressMessage = new UIHeader();
		private GenerationProgress _progress;

		public UIWorldLoad(GenerationProgress progress)
		{
			this._progressBar.Top.Pixels = 370f;
			this._progressBar.HAlign = 0.5f;
			this._progressBar.VAlign = 0f;
			this._progressBar.Recalculate();
			this._progressMessage.CopyStyle(this._progressBar);
			UIHeader expr_78_cp_0 = this._progressMessage;
			expr_78_cp_0.Top.Pixels = expr_78_cp_0.Top.Pixels - 70f;
			this._progressMessage.Recalculate();
			this._progress = progress;
			base.Append(this._progressBar);
			base.Append(this._progressMessage);
		}

		public override void OnActivate()
		{
			if (PlayerInput.UsingGamepadUI)
			{
				UILinkPointNavigator.Points[3000].Unlink();
				UILinkPointNavigator.ChangePoint(3000);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this._progressBar.SetProgress(this._progress.TotalProgress, this._progress.Value);
			this._progressMessage.Text = this._progress.Message;
			this.UpdateGamepadSquiggle();
		}

		private void UpdateGamepadSquiggle()
		{
			Vector2 value = new Vector2((float)Math.Cos((double)(Main.GlobalTime * 6.28318548f)), (float)Math.Sin((double)(Main.GlobalTime * 6.28318548f * 2f))) * new Vector2(30f, 15f) + Vector2.UnitY * 20f;
			UILinkPointNavigator.Points[3000].Unlink();
			UILinkPointNavigator.SetPosition(3000, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value);
		}

		public string GetStatusText()
		{
			return string.Format("{0:0.0%} - " + this._progress.Message + " - {1:0.0%}", this._progress.TotalProgress, this._progress.Value);
		}
	}
}
