using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Terraria.Cinematics
{
	public class CinematicManager
	{
		public static CinematicManager Instance = new CinematicManager();
		private List<Film> _films = new List<Film>();

		public void Update(GameTime gameTime)
		{
			if (this._films.Count > 0)
			{
				if (!this._films[0].IsActive)
				{
					this._films[0].OnBegin();
				}
				if (Main.hasFocus && !Main.gamePaused && !this._films[0].OnUpdate(gameTime))
				{
					this._films[0].OnEnd();
					this._films.RemoveAt(0);
				}
			}
		}

		public void PlayFilm(Film film)
		{
			this._films.Add(film);
		}

		public void StopAll()
		{
		}
	}
}
