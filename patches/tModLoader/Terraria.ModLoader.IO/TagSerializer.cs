﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Terraria.ModLoader.IO
{
	public abstract class TagSerializer
	{
		public abstract Type Type { get; }
		public abstract Type TagType { get; }

		public abstract object Serialize(object value);
		public abstract object Deserialize(object tag);
		public abstract IList SerializeList(IList value);
		public abstract IList DeserializeList(IList value);

		private static IDictionary<Type, TagSerializer> serializers = new Dictionary<Type, TagSerializer>();
		private static IDictionary<string, Type> typeNameCache = new Dictionary<string, Type>();

		static TagSerializer() {
			Reload();
		}

		internal static void Reload() {
			serializers.Clear();
			typeNameCache.Clear();
			AddSerializer(new BoolTagSerializer());
			AddSerializer(new UShortTagSerializer());
			AddSerializer(new UIntTagSerializer());
			AddSerializer(new ULongTagSerializer());
			AddSerializer(new Vector2TagSerializer());
			AddSerializer(new Vector3TagSerializer());
			AddSerializer(new ColorSerializer());
		}

		public static bool TryGetSerializer(Type type, out TagSerializer serializer) {
			if (serializers.TryGetValue(type, out serializer))
				return true;

			if (typeof(TagSerializable).IsAssignableFrom(type)) {
				var sType = typeof(TagSerializableSerializer<>).MakeGenericType(type);
				serializers[type] = serializer = (TagSerializer) sType.GetConstructor(new Type[0]).Invoke(new object[0]);
				return true;
			}

			return false;
		}

		public static void AddSerializer(TagSerializer serializer) {
			serializers.Add(serializer.Type, serializer);
		}

		protected static Type GetType(string name) {
			Type type;
			if (typeNameCache.TryGetValue(name, out type))
				return type;

			type = Type.GetType(name);
			if (type != null)
				return typeNameCache[name] = type;

			foreach (var mod in ModLoader.LoadedMods) {
				type = mod.Code?.GetType(name);
				if (type != null)
					return typeNameCache[name] = type;
			}

			return null;
		}
	}

	public abstract class TagSerializer<T, S> : TagSerializer
	{
		public override Type Type => typeof(T);
		public override Type TagType => typeof(S);

		public abstract S Serialize(T value);
		public abstract T Deserialize(S tag);

		public override object Serialize(object value) {
			return Serialize((T)value);
		}

		public override object Deserialize(object tag) {
			return Deserialize((S)tag);
		}

		public override IList SerializeList(IList value) {
			return ((IList<T>)value).Select(Serialize).ToList();
		}

		public override IList DeserializeList(IList value) {
			return ((IList<S>)value).Select(Deserialize).ToList();
		}
	}

	public class UShortTagSerializer : TagSerializer<ushort, short>
	{
		public override short Serialize(ushort value) => (short)value;
		public override ushort Deserialize(short tag) => (ushort)tag;
	}

	public class UIntTagSerializer : TagSerializer<uint, int>
	{
		public override int Serialize(uint value) => (int)value;
		public override uint Deserialize(int tag) => (uint)tag;
	}

	public class ULongTagSerializer : TagSerializer<ulong, long>
	{
		public override long Serialize(ulong value) => (long)value;
		public override ulong Deserialize(long tag) => (ulong)tag;
	}

	public class BoolTagSerializer : TagSerializer<bool, byte>
	{
		public override byte Serialize(bool value) => (byte)(value ? 1 : 0);
		public override bool Deserialize(byte tag) => tag != 0;
	}

	public class Vector2TagSerializer : TagSerializer<Vector2, TagCompound>
	{
		public override TagCompound Serialize(Vector2 value) => new TagCompound {
			["x"] = value.X,
			["y"] = value.Y,
		};

		public override Vector2 Deserialize(TagCompound tag) => new Vector2(tag.GetFloat("x"), tag.GetFloat("y"));
	}

	public class Vector3TagSerializer : TagSerializer<Vector3, TagCompound>
	{
		public override TagCompound Serialize(Vector3 value) => new TagCompound {
			["x"] = value.X,
			["y"] = value.Y,
			["z"] = value.Z,
		};

		public override Vector3 Deserialize(TagCompound tag) => new Vector3(tag.GetFloat("x"), tag.GetFloat("y"), tag.GetFloat("z"));
	}

	public class ColorSerializer : TagSerializer<Color, int>
	{
		public override int Serialize(Color value) {
			return (int)value.PackedValue;
		}

		public override Color Deserialize(int tag) {
			return new Color(tag & 0xFF, tag >> 8 & 0xFF, tag >> 16 & 0xFF, tag >> 24 & 0xFF);
		}
	}
}
