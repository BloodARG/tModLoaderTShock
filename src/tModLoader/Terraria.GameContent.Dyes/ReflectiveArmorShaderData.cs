using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Dyes
{
	public class ReflectiveArmorShaderData : ArmorShaderData
	{
		public ReflectiveArmorShaderData(Ref<Effect> shader, string passName)
			: base(shader, passName)
		{
		}

		public override void Apply(Entity entity, DrawData? drawData)
		{
			if (entity == null)
			{
				base.Shader.Parameters["uLightSource"].SetValue(Vector3.Zero);
			}
			else
			{
				float num = 0f;
				if (drawData.HasValue)
				{
					num = drawData.Value.rotation;
				}
				Vector2 value = entity.position;
				float num2 = (float)entity.width;
				float num3 = (float)entity.height;
				value += new Vector2(num2, num3) * 0.1f;
				num2 *= 0.8f;
				num3 *= 0.8f;
				Vector3 subLight = Lighting.GetSubLight(value + new Vector2(num2 * 0.5f, 0f));
				Vector3 subLight2 = Lighting.GetSubLight(value + new Vector2(0f, num3 * 0.5f));
				Vector3 subLight3 = Lighting.GetSubLight(value + new Vector2(num2, num3 * 0.5f));
				Vector3 subLight4 = Lighting.GetSubLight(value + new Vector2(num2 * 0.5f, num3));
				float num4 = subLight.X + subLight.Y + subLight.Z;
				float num5 = subLight2.X + subLight2.Y + subLight2.Z;
				float num6 = subLight3.X + subLight3.Y + subLight3.Z;
				float num7 = subLight4.X + subLight4.Y + subLight4.Z;
				Vector2 vector = new Vector2(num6 - num5, num7 - num4);
				float num8 = vector.Length();
				if (num8 > 1f)
				{
					num8 = 1f;
					vector /= num8;
				}
				if (entity.direction == -1)
				{
					vector.X *= -1f;
				}
				vector = vector.RotatedBy((double)(-(double)num), default(Vector2));
				Vector3 value2 = new Vector3(vector, 1f - (vector.X * vector.X + vector.Y * vector.Y));
				value2.X *= 2f;
				value2.Y -= 0.15f;
				value2.Y *= 2f;
				value2.Normalize();
				value2.Z *= 0.6f;
				base.Shader.Parameters["uLightSource"].SetValue(value2);
			}
			base.Apply(entity, drawData);
		}
	}
}
