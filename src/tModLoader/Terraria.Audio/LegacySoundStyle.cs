using Microsoft.Xna.Framework.Audio;
using System;
using Terraria.Utilities;

namespace Terraria.Audio
{
	public class LegacySoundStyle : SoundStyle
	{
		private static UnifiedRandom _random = new UnifiedRandom();
		private int _style;
		private int _styleVariations;
		private int _soundId;

		public int Style
		{
			get
			{
				if (this._styleVariations != 1)
				{
					return LegacySoundStyle._random.Next(this._style, this._style + this._styleVariations);
				}
				return this._style;
			}
		}

		public int Variations
		{
			get
			{
				return this._styleVariations;
			}
		}

		public int SoundId
		{
			get
			{
				return this._soundId;
			}
		}

		public override bool IsTrackable
		{
			get
			{
				return this._soundId == 42;
			}
		}

		public LegacySoundStyle(int soundId, int style, SoundType type = SoundType.Sound)
			: base(type)
		{
			this._style = style;
			this._styleVariations = 1;
			this._soundId = soundId;
		}

		public LegacySoundStyle(int soundId, int style, int variations, SoundType type = SoundType.Sound)
			: base(type)
		{
			this._style = style;
			this._styleVariations = variations;
			this._soundId = soundId;
		}

		private LegacySoundStyle(int soundId, int style, int variations, SoundType type, float volume, float pitchVariance)
			: base(volume, pitchVariance, type)
		{
			this._style = style;
			this._styleVariations = variations;
			this._soundId = soundId;
		}

		public LegacySoundStyle WithVolume(float volume)
		{
			return new LegacySoundStyle(this._soundId, this._style, this._styleVariations, base.Type, volume, base.PitchVariance);
		}

		public LegacySoundStyle WithPitchVariance(float pitchVariance)
		{
			return new LegacySoundStyle(this._soundId, this._style, this._styleVariations, base.Type, base.Volume, pitchVariance);
		}

		public LegacySoundStyle AsMusic()
		{
			return new LegacySoundStyle(this._soundId, this._style, this._styleVariations, SoundType.Music, base.Volume, base.PitchVariance);
		}

		public LegacySoundStyle AsAmbient()
		{
			return new LegacySoundStyle(this._soundId, this._style, this._styleVariations, SoundType.Ambient, base.Volume, base.PitchVariance);
		}

		public LegacySoundStyle AsSound()
		{
			return new LegacySoundStyle(this._soundId, this._style, this._styleVariations, SoundType.Sound, base.Volume, base.PitchVariance);
		}

		public bool Includes(int soundId, int style)
		{
			return this._soundId == soundId && style >= this._style && style < this._style + this._styleVariations;
		}

		public override SoundEffect GetRandomSound()
		{
			if (this.IsTrackable)
			{
				return Main.trackableSounds[this.Style];
			}
			return null;
		}
	}
}
