using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Terraria.Graphics.Shaders
{
	public class HairShaderDataSet
	{
		protected List<HairShaderData> _shaderData = new List<HairShaderData>();
		protected Dictionary<int, short> _shaderLookupDictionary = new Dictionary<int, short>();
		protected byte _shaderDataCount;

		public T BindShader<T>(int itemId, T shaderData) where T : HairShaderData
		{
			if (this._shaderDataCount == 255)
			{
				throw new Exception("Too many shaders bound.");
			}
			this._shaderLookupDictionary[itemId] = (short)(this._shaderDataCount += 1);
			this._shaderData.Add(shaderData);
			return shaderData;
		}

		public void Apply(short shaderId, Player player, DrawData? drawData = null)
		{
			if (shaderId != 0 && shaderId <= (short)this._shaderDataCount)
			{
				this._shaderData[(int)(shaderId - 1)].Apply(player, drawData);
				return;
			}
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}

		public Color GetColor(short shaderId, Player player, Color lightColor)
		{
			if (shaderId != 0 && shaderId <= (short)this._shaderDataCount)
			{
				return this._shaderData[(int)(shaderId - 1)].GetColor(player, lightColor);
			}
			return new Color(lightColor.ToVector4() * player.hairColor.ToVector4());
		}

		public HairShaderData GetShaderFromItemId(int type)
		{
			if (this._shaderLookupDictionary.ContainsKey(type))
			{
				return this._shaderData[(int)(this._shaderLookupDictionary[type] - 1)];
			}
			return null;
		}

		public short GetShaderIdFromItemId(int type)
		{
			if (this._shaderLookupDictionary.ContainsKey(type))
			{
				return this._shaderLookupDictionary[type];
			}
			return -1;
		}
	}
}
