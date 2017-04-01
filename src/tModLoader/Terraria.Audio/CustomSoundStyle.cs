using Microsoft.Xna.Framework.Audio;
using System;
using Terraria.Utilities;

namespace Terraria.Audio
{
	public class CustomSoundStyle : SoundStyle
	{
		private static UnifiedRandom _random = new UnifiedRandom();
		private SoundEffect[] _soundEffects;

		public override bool IsTrackable
		{
			get
			{
				return true;
			}
		}

		public CustomSoundStyle(SoundEffect soundEffect, SoundType type = SoundType.Sound, float volume = 1f, float pitchVariance = 0f)
			: base(volume, pitchVariance, type)
		{
			this._soundEffects = new SoundEffect[]
			{
				soundEffect
			};
		}

		public CustomSoundStyle(SoundEffect[] soundEffects, SoundType type = SoundType.Sound, float volume = 1f, float pitchVariance = 0f)
			: base(volume, pitchVariance, type)
		{
			this._soundEffects = soundEffects;
		}

		public override SoundEffect GetRandomSound()
		{
			return this._soundEffects[CustomSoundStyle._random.Next(this._soundEffects.Length)];
		}
	}
}
