using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Terraria.Audio
{
	public class ActiveSound
	{
		private SoundEffectInstance _sound;
		public readonly bool IsGlobal;
		public Vector2 Position;
		public float Volume;
		private SoundStyle _style;

		public SoundEffectInstance Sound
		{
			get
			{
				return this._sound;
			}
		}

		public SoundStyle Style
		{
			get
			{
				return this._style;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return this.Sound.State == SoundState.Playing;
			}
		}

		public ActiveSound(SoundStyle style, Vector2 position)
		{
			this.Position = position;
			this.Volume = 1f;
			this.IsGlobal = false;
			this._style = style;
			this.Play();
		}

		public ActiveSound(SoundStyle style)
		{
			this.Position = Vector2.Zero;
			this.Volume = 1f;
			this.IsGlobal = true;
			this._style = style;
			this.Play();
		}

		private void Play()
		{
			SoundEffectInstance soundEffectInstance = this._style.GetRandomSound().CreateInstance();
			soundEffectInstance.Pitch += this._style.GetRandomPitch();
			Main.PlaySoundInstance(soundEffectInstance);
			this._sound = soundEffectInstance;
			this.Update();
		}

		public void Stop()
		{
			if (this._sound != null)
			{
				this._sound.Stop();
			}
		}

		public void Pause()
		{
			if (this._sound != null && this._sound.State == SoundState.Playing)
			{
				this._sound.Pause();
			}
		}

		public void Resume()
		{
			if (this._sound != null && this._sound.State == SoundState.Paused)
			{
				this._sound.Resume();
			}
		}

		public void Update()
		{
			if (this._sound == null)
			{
				return;
			}
			Vector2 value = Main.screenPosition + new Vector2((float)(Main.screenWidth / 2), (float)(Main.screenHeight / 2));
			float num = 1f;
			if (!this.IsGlobal)
			{
				float num2 = (this.Position.X - value.X) / ((float)Main.screenWidth * 0.5f);
				num2 = MathHelper.Clamp(num2, -1f, 1f);
				this.Sound.Pan = num2;
				float num3 = Vector2.Distance(this.Position, value);
				num = 1f - num3 / ((float)Main.screenWidth * 1.5f);
			}
			num *= this._style.Volume * this.Volume;
			switch (this._style.Type)
			{
				case SoundType.Sound:
					num *= Main.soundVolume;
					break;
				case SoundType.Ambient:
					num *= Main.ambientVolume;
					break;
				case SoundType.Music:
					num *= Main.musicVolume;
					break;
			}
			num = MathHelper.Clamp(num, 0f, 1f);
			this.Sound.Volume = num;
		}
	}
}
