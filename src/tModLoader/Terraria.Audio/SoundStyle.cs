using Microsoft.Xna.Framework.Audio;
using System;
using Terraria.Utilities;

namespace Terraria.Audio
{
	public abstract class SoundStyle
	{
		private static UnifiedRandom _random = new UnifiedRandom();
		private float _volume;
		private float _pitchVariance;
		private SoundType _type;

		public float Volume
		{
			get
			{
				return this._volume;
			}
		}

		public float PitchVariance
		{
			get
			{
				return this._pitchVariance;
			}
		}

		public SoundType Type
		{
			get
			{
				return this._type;
			}
		}

		public abstract bool IsTrackable
		{
			get;
		}

		public SoundStyle(float volume, float pitchVariance, SoundType type = SoundType.Sound)
		{
			this._volume = volume;
			this._pitchVariance = pitchVariance;
			this._type = type;
		}

		public SoundStyle(SoundType type = SoundType.Sound)
		{
			this._volume = 1f;
			this._pitchVariance = 0f;
			this._type = type;
		}

		public float GetRandomPitch()
		{
			return SoundStyle._random.NextFloat() * this.PitchVariance - this.PitchVariance * 0.5f;
		}

		public abstract SoundEffect GetRandomSound();
	}
}
