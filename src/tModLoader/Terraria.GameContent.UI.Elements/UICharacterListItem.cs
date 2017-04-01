using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Social;
using Terraria.UI;
using System.Reflection;
using Terraria.ModLoader;

namespace Terraria.GameContent.UI.Elements
{
	public class UICharacterListItem : UIPanel
	{
		private PlayerFileData _data;
		private Texture2D _dividerTexture;
		private Texture2D _innerPanelTexture;
		private UICharacter _playerPanel;
		private UIText _buttonLabel;
		private UIText _deleteButtonLabel;
		private Texture2D _buttonCloudActiveTexture;
		private Texture2D _buttonCloudInactiveTexture;
		private Texture2D _buttonFavoriteActiveTexture;
		private Texture2D _buttonFavoriteInactiveTexture;
		private Texture2D _buttonPlayTexture;
		private Texture2D _buttonDeleteTexture;
		private Texture2D _errorTexture;
		private UIImageButton _deleteButton;

		public bool IsFavorite
		{
			get
			{
				return this._data.IsFavorite;
			}
		}

		public UICharacterListItem(PlayerFileData data, int snapPointIndex)
		{
			this.BorderColor = new Color(89, 116, 213) * 0.7f;
			this._dividerTexture = TextureManager.Load("Images/UI/Divider");
			this._innerPanelTexture = TextureManager.Load("Images/UI/InnerPanelBackground");
			this._buttonCloudActiveTexture = TextureManager.Load("Images/UI/ButtonCloudActive");
			this._buttonCloudInactiveTexture = TextureManager.Load("Images/UI/ButtonCloudInactive");
			this._buttonFavoriteActiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteActive");
			this._buttonFavoriteInactiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteInactive");
			this._buttonPlayTexture = TextureManager.Load("Images/UI/ButtonPlay");
			this._buttonDeleteTexture = TextureManager.Load("Images/UI/ButtonDelete");
			this._errorTexture = Texture2D.FromStream(Main.instance.GraphicsDevice,
				Assembly.GetExecutingAssembly().GetManifestResourceStream("Terraria.ModLoader.UI.ButtonError.png"));
			this.Height.Set(96f, 0f);
			this.Width.Set(0f, 1f);
			base.SetPadding(6f);
			this._data = data;
			this._playerPanel = new UICharacter(data.Player);
			this._playerPanel.Left.Set(4f, 0f);
			this._playerPanel.OnDoubleClick += new UIElement.MouseEvent(this.PlayGame);
			base.OnDoubleClick += new UIElement.MouseEvent(this.PlayGame);
			base.Append(this._playerPanel);
			UIImageButton uIImageButton = new UIImageButton(this._buttonPlayTexture);
			uIImageButton.VAlign = 1f;
			uIImageButton.Left.Set(4f, 0f);
			uIImageButton.OnClick += new UIElement.MouseEvent(this.PlayGame);
			uIImageButton.OnMouseOver += new UIElement.MouseEvent(this.PlayMouseOver);
			uIImageButton.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
			base.Append(uIImageButton);
			UIImageButton uIImageButton2 = new UIImageButton(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
			uIImageButton2.VAlign = 1f;
			uIImageButton2.Left.Set(28f, 0f);
			uIImageButton2.OnClick += new UIElement.MouseEvent(this.FavoriteButtonClick);
			uIImageButton2.OnMouseOver += new UIElement.MouseEvent(this.FavoriteMouseOver);
			uIImageButton2.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
			uIImageButton2.SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
			base.Append(uIImageButton2);
			if (SocialAPI.Cloud != null)
			{
				UIImageButton uIImageButton3 = new UIImageButton(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
				uIImageButton3.VAlign = 1f;
				uIImageButton3.Left.Set(52f, 0f);
				uIImageButton3.OnClick += new UIElement.MouseEvent(this.CloudButtonClick);
				uIImageButton3.OnMouseOver += new UIElement.MouseEvent(this.CloudMouseOver);
				uIImageButton3.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
				base.Append(uIImageButton3);
				uIImageButton3.SetSnapPoint("Cloud", snapPointIndex, null, null);
			}
			UIImageButton uIImageButton4 = new UIImageButton(this._buttonDeleteTexture);
			uIImageButton4.VAlign = 1f;
			uIImageButton4.HAlign = 1f;
			uIImageButton4.OnClick += new UIElement.MouseEvent(this.DeleteButtonClick);
			uIImageButton4.OnMouseOver += new UIElement.MouseEvent(this.DeleteMouseOver);
			uIImageButton4.OnMouseOut += new UIElement.MouseEvent(this.DeleteMouseOut);
			this._deleteButton = uIImageButton4;
			if (!this._data.IsFavorite)
			{
				base.Append(uIImageButton4);
			}
			if (data.customDataFail != null)
			{
				UIImageButton uIImageButton5 = new UIImageButton(this._errorTexture);
				uIImageButton5.VAlign = 1f;
				uIImageButton5.HAlign = 1f;
				uIImageButton5.Left.Set(-24f, 0f);
				uIImageButton5.OnClick += new UIElement.MouseEvent(this.ErrorButtonClick);
				uIImageButton5.OnMouseOver += new UIElement.MouseEvent(this.ErrorMouseOver);
				uIImageButton5.OnMouseOut += new UIElement.MouseEvent(this.DeleteMouseOut);
				base.Append(uIImageButton5);
			}
			this._buttonLabel = new UIText("", 1f, false);
			this._buttonLabel.VAlign = 1f;
			this._buttonLabel.Left.Set(80f, 0f);
			this._buttonLabel.Top.Set(-3f, 0f);
			base.Append(this._buttonLabel);
			this._deleteButtonLabel = new UIText("", 1f, false);
			this._deleteButtonLabel.VAlign = 1f;
			this._deleteButtonLabel.HAlign = 1f;
			this._deleteButtonLabel.Left.Set(data.customDataFail == null ? -30f : -54f, 0f);
			this._deleteButtonLabel.Top.Set(-3f, 0f);
			base.Append(this._deleteButtonLabel);
			uIImageButton.SetSnapPoint("Play", snapPointIndex, null, null);
			uIImageButton2.SetSnapPoint("Favorite", snapPointIndex, null, null);
			uIImageButton4.SetSnapPoint("Delete", snapPointIndex, null, null);
		}

		private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsFavorite)
			{
				this._buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
				return;
			}
			this._buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
		}

		private void CloudMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsCloudSave)
			{
				this._buttonLabel.SetText(Language.GetTextValue("UI.MoveOffCloud"));
				return;
			}
			this._buttonLabel.SetText(Language.GetTextValue("UI.MoveToCloud"));
		}

		private void PlayMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonLabel.SetText(Language.GetTextValue("UI.Play"));
		}

		private void DeleteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			this._deleteButtonLabel.SetText(Language.GetTextValue("UI.Delete"));
		}

		private void ErrorMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			this._deleteButtonLabel.SetText(this._data.customDataFail.modName + " Error");
		}

		private void DeleteMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			this._deleteButtonLabel.SetText("");
		}

		private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonLabel.SetText("");
		}

		private void CloudButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsCloudSave)
			{
				this._data.MoveToLocal();
			}
			else
			{
				this._data.MoveToCloud();
			}
			((UIImageButton)evt.Target).SetImage(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
			if (this._data.IsCloudSave)
			{
				this._buttonLabel.SetText(Language.GetTextValue("UI.MoveOffCloud"));
				return;
			}
			this._buttonLabel.SetText(Language.GetTextValue("UI.MoveToCloud"));
		}

		private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			for (int i = 0; i < Main.PlayerList.Count; i++)
			{
				if (Main.PlayerList[i] == this._data)
				{
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
					Main.selectedPlayer = i;
					Main.menuMode = 5;
					return;
				}
			}
		}

		private void PlayGame(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement != evt.Target)
			{
				return;
			}
			if (this._data.Player.loadStatus == 0)
			{
				Main.SelectPlayer(this._data);
			}
		}

		private void FavoriteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._data.ToggleFavorite();
			((UIImageButton)evt.Target).SetImage(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
			((UIImageButton)evt.Target).SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
			if (this._data.IsFavorite)
			{
				this._buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
				base.RemoveChild(this._deleteButton);
			}
			else
			{
				this._buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
				base.Append(this._deleteButton);
			}
			UIList uIList = this.Parent.Parent as UIList;
			if (uIList != null)
			{
				uIList.UpdateOrder();
			}
		}

		private void ErrorButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			ErrorLogger.LogException(this._data.customDataFail.InnerException);
		}

		public override int CompareTo(object obj)
		{
			UICharacterListItem uICharacterListItem = obj as UICharacterListItem;
			if (uICharacterListItem == null)
			{
				return base.CompareTo(obj);
			}
			if (this.IsFavorite && !uICharacterListItem.IsFavorite)
			{
				return -1;
			}
			if (!this.IsFavorite && uICharacterListItem.IsFavorite)
			{
				return 1;
			}
			if (this._data.Name.CompareTo(uICharacterListItem._data.Name) != 0)
			{
				return this._data.Name.CompareTo(uICharacterListItem._data.Name);
			}
			return this._data.GetFileName(true).CompareTo(uICharacterListItem._data.GetFileName(true));
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			this.BackgroundColor = new Color(73, 94, 171);
			this.BorderColor = new Color(89, 116, 213);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			this.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			this.BorderColor = new Color(89, 116, 213) * 0.7f;
		}

		private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
		{
			spriteBatch.Draw(this._innerPanelTexture, position, new Rectangle?(new Rectangle(0, 0, 8, this._innerPanelTexture.Height)), Color.White);
			spriteBatch.Draw(this._innerPanelTexture, new Vector2(position.X + 8f, position.Y), new Rectangle?(new Rectangle(8, 0, 8, this._innerPanelTexture.Height)), Color.White, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(this._innerPanelTexture, new Vector2(position.X + width - 8f, position.Y), new Rectangle?(new Rectangle(16, 0, 8, this._innerPanelTexture.Height)), Color.White);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			CalculatedStyle dimensions = this._playerPanel.GetDimensions();
			float num = dimensions.X + dimensions.Width;
			Utils.DrawBorderString(spriteBatch, this._data.Name, new Vector2(num + 6f, dimensions.Y - 2f), Color.White, 1f, 0f, 0f, -1);
			spriteBatch.Draw(this._dividerTexture, new Vector2(num, innerDimensions.Y + 21f), null, Color.White, 0f, Vector2.Zero, new Vector2((base.GetDimensions().X + base.GetDimensions().Width - num) / 8f, 1f), SpriteEffects.None, 0f);
			Vector2 vector = new Vector2(num + 6f, innerDimensions.Y + 29f);
			float num2 = 200f;
			Vector2 vector2 = vector;
			this.DrawPanel(spriteBatch, vector2, num2);
			spriteBatch.Draw(Main.heartTexture, vector2 + new Vector2(5f, 2f), Color.White);
			vector2.X += 10f + (float)Main.heartTexture.Width;
			Utils.DrawBorderString(spriteBatch, this._data.Player.statLifeMax2 + " HP", vector2 + new Vector2(0f, 3f), Color.White, 1f, 0f, 0f, -1);
			vector2.X += 65f;
			spriteBatch.Draw(Main.manaTexture, vector2 + new Vector2(5f, 2f), Color.White);
			vector2.X += 10f + (float)Main.manaTexture.Width;
			Utils.DrawBorderString(spriteBatch, this._data.Player.statManaMax2 + " MP", vector2 + new Vector2(0f, 3f), Color.White, 1f, 0f, 0f, -1);
			vector.X += num2 + 5f;
			Vector2 vector3 = vector;
			float num3 = 110f;
			this.DrawPanel(spriteBatch, vector3, num3);
			string text = "";
			Color color = Color.White;
			switch (this._data.Player.difficulty)
			{
				case 0:
					text = Language.GetTextValue("UI.Softcore");
					break;
				case 1:
					text = Language.GetTextValue("UI.Mediumcore");
					color = Main.mcColor;
					break;
				case 2:
					text = Language.GetTextValue("UI.Hardcore");
					color = Main.hcColor;
					break;
			}
			vector3 += new Vector2(num3 * 0.5f - Main.fontMouseText.MeasureString(text).X * 0.5f, 3f);
			Utils.DrawBorderString(spriteBatch, text, vector3, color, 1f, 0f, 0f, -1);
			vector.X += num3 + 5f;
			Vector2 vector4 = vector;
			float num4 = innerDimensions.X + innerDimensions.Width - vector4.X;
			this.DrawPanel(spriteBatch, vector4, num4);
			TimeSpan playTime = this._data.GetPlayTime();
			int num5 = playTime.Days * 24 + playTime.Hours;
			string text2 = ((num5 < 10) ? "0" : "") + num5 + playTime.ToString("\\:mm\\:ss");
			vector4 += new Vector2(num4 * 0.5f - Main.fontMouseText.MeasureString(text2).X * 0.5f, 3f);
			Utils.DrawBorderString(spriteBatch, text2, vector4, Color.White, 1f, 0f, 0f, -1);
		}
	}
}
