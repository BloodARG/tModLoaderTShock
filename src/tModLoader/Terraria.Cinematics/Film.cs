using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Terraria.Cinematics
{
	public class Film
	{
		private class Sequence
		{
			private FrameEvent _frameEvent;
			private int _duration;
			private int _start;

			public FrameEvent Event
			{
				get
				{
					return this._frameEvent;
				}
			}

			public int Duration
			{
				get
				{
					return this._duration;
				}
			}

			public int Start
			{
				get
				{
					return this._start;
				}
			}

			public Sequence(FrameEvent frameEvent, int start, int duration)
			{
				this._frameEvent = frameEvent;
				this._start = start;
				this._duration = duration;
			}
		}

		private int _frame;
		private int _frameCount;
		private int _nextSequenceAppendTime;
		private bool _isActive;
		private List<Film.Sequence> _sequences = new List<Film.Sequence>();

		public int Frame
		{
			get
			{
				return this._frame;
			}
		}

		public int FrameCount
		{
			get
			{
				return this._frameCount;
			}
		}

		public int AppendPoint
		{
			get
			{
				return this._nextSequenceAppendTime;
			}
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
		}

		public void AddSequence(int start, int duration, FrameEvent frameEvent)
		{
			this._sequences.Add(new Film.Sequence(frameEvent, start, duration));
			this._nextSequenceAppendTime = Math.Max(this._nextSequenceAppendTime, start + duration);
			this._frameCount = Math.Max(this._frameCount, start + duration);
		}

		public void AppendSequence(int duration, FrameEvent frameEvent)
		{
			this.AddSequence(this._nextSequenceAppendTime, duration, frameEvent);
		}

		public void AddSequences(int start, int duration, params FrameEvent[] frameEvents)
		{
			for (int i = 0; i < frameEvents.Length; i++)
			{
				FrameEvent frameEvent = frameEvents[i];
				this.AddSequence(start, duration, frameEvent);
			}
		}

		public void AppendSequences(int duration, params FrameEvent[] frameEvents)
		{
			int nextSequenceAppendTime = this._nextSequenceAppendTime;
			for (int i = 0; i < frameEvents.Length; i++)
			{
				FrameEvent frameEvent = frameEvents[i];
				this._sequences.Add(new Film.Sequence(frameEvent, nextSequenceAppendTime, duration));
				this._nextSequenceAppendTime = Math.Max(this._nextSequenceAppendTime, nextSequenceAppendTime + duration);
				this._frameCount = Math.Max(this._frameCount, nextSequenceAppendTime + duration);
			}
		}

		public void AppendEmptySequence(int duration)
		{
			this.AddSequence(this._nextSequenceAppendTime, duration, new FrameEvent(Film.EmptyFrameEvent));
		}

		public void AppendKeyFrame(FrameEvent frameEvent)
		{
			this.AddKeyFrame(this._nextSequenceAppendTime, frameEvent);
		}

		public void AppendKeyFrames(params FrameEvent[] frameEvents)
		{
			int nextSequenceAppendTime = this._nextSequenceAppendTime;
			for (int i = 0; i < frameEvents.Length; i++)
			{
				FrameEvent frameEvent = frameEvents[i];
				this._sequences.Add(new Film.Sequence(frameEvent, nextSequenceAppendTime, 1));
			}
			this._frameCount = Math.Max(this._frameCount, nextSequenceAppendTime + 1);
		}

		public void AddKeyFrame(int frame, FrameEvent frameEvent)
		{
			this._sequences.Add(new Film.Sequence(frameEvent, frame, 1));
			this._frameCount = Math.Max(this._frameCount, frame + 1);
		}

		public void AddKeyFrames(int frame, params FrameEvent[] frameEvents)
		{
			for (int i = 0; i < frameEvents.Length; i++)
			{
				FrameEvent frameEvent = frameEvents[i];
				this.AddKeyFrame(frame, frameEvent);
			}
		}

		public bool OnUpdate(GameTime gameTime)
		{
			if (this._sequences.Count == 0)
			{
				return false;
			}
			foreach (Film.Sequence current in this._sequences)
			{
				int num = this._frame - current.Start;
				if (num >= 0 && num < current.Duration)
				{
					current.Event(new FrameEventData(this._frame, current.Start, current.Duration));
				}
			}
			return ++this._frame != this._frameCount;
		}

		public virtual void OnBegin()
		{
			this._isActive = true;
		}

		public virtual void OnEnd()
		{
			this._isActive = false;
		}

		private static void EmptyFrameEvent(FrameEventData evt)
		{
		}
	}
}
