using System;
using System.IO;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.TypeConversion
{
	public class ByteArrayConverter
		: ATypeConverter<byte[]>
	{	
		public ByteArrayConverter(
			IReadOnlyRepository<Type, Delegate> convertFromTargetTypeDelegateRepository,
			IReadOnlyRepository<Type, Delegate> convertToTargetTypeDelegateRepository,
			ILogger logger)
			: base (
				convertFromTargetTypeDelegateRepository,
				convertToTargetTypeDelegateRepository,
				logger)
		{
		}

		protected override bool FallbackConvertFromTargetType<TValue>(
			byte[] byteArray,
			out TValue value)
		{
			if (typeof(TValue) == typeof(byte[]))
			{
				value = byteArray.CastFromTo<byte[], TValue>();

				return true;
			}

			using (var ms = new MemoryStream(byteArray))
			{
				// Create a binary reader
				using (var reader = new BinaryReader(ms))
				{
					// Read the value based on its type
					if (typeof(TValue) == typeof(bool))
					{
						value = reader.ReadBoolean().CastFromTo<bool, TValue>();
					}
					else if (typeof(TValue) == typeof(byte))
					{
						value = reader.ReadByte().CastFromTo<byte, TValue>();
					}
					else if (typeof(TValue) == typeof(char))
					{
						value = reader.ReadChar().CastFromTo<char, TValue>();
					}
					else if (typeof(TValue) == typeof(decimal))
					{
						value = reader.ReadDecimal().CastFromTo<decimal, TValue>();
					}
					else if (typeof(TValue) == typeof(double))
					{
						value = reader.ReadDouble().CastFromTo<double, TValue>();
					}
					else if (typeof(TValue) == typeof(short))
					{
						value = reader.ReadInt16().CastFromTo<short, TValue>();
					}
					else if (typeof(TValue) == typeof(int))
					{
						value = reader.ReadInt32().CastFromTo<int, TValue>();
					}
					else if (typeof(TValue) == typeof(long))
					{
						value = reader.ReadInt64().CastFromTo<long, TValue>();
					}
					else if (typeof(TValue) == typeof(sbyte))
					{
						value = reader.ReadSByte().CastFromTo<sbyte, TValue>();
					}
					else if (typeof(TValue) == typeof(float))
					{
						value = reader.ReadSingle().CastFromTo<float, TValue>();
					}
					else if (typeof(TValue) == typeof(ushort))
					{
						value = reader.ReadUInt16().CastFromTo<ushort, TValue>();
					}
					else if (typeof(TValue) == typeof(uint))
					{
						value = reader.ReadUInt32().CastFromTo<uint, TValue>();
					}
					else if (typeof(TValue) == typeof(string))
					{
						value = reader.ReadString().CastFromTo<string, TValue>();
					}
				}
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {typeof(TValue).Name}");

			value = default;

			return false;
		}

		protected override bool FallbackConvertFromTargetType(
			Type valueType,
			byte[] byteArray,
			out object value)
		{
			if (valueType == typeof(byte[]))
			{
				value = byteArray.CastFromTo<byte[], object>();

				return true;
			}

			using (var ms = new MemoryStream(byteArray))
			{
				// Create a binary reader
				using (var reader = new BinaryReader(ms))
				{
					// Read the value based on its type
					if (valueType == typeof(bool))
					{
						value = reader.ReadBoolean().CastFromTo<bool, object>();
					}
					else if (valueType == typeof(byte))
					{
						value = reader.ReadByte().CastFromTo<byte, object>();
					}
					else if (valueType == typeof(char))
					{
						value = reader.ReadChar().CastFromTo<char, object>();
					}
					else if (valueType == typeof(decimal))
					{
						value = reader.ReadDecimal().CastFromTo<decimal, object>();
					}
					else if (valueType == typeof(double))
					{
						value = reader.ReadDouble().CastFromTo<double, object>();
					}
					else if (valueType == typeof(short))
					{
						value = reader.ReadInt16().CastFromTo<short, object>();
					}
					else if (valueType == typeof(int))
					{
						value = reader.ReadInt32().CastFromTo<int, object>();
					}
					else if (valueType == typeof(long))
					{
						value = reader.ReadInt64().CastFromTo<long, object>();
					}
					else if (valueType == typeof(sbyte))
					{
						value = reader.ReadSByte().CastFromTo<sbyte, object>();
					}
					else if (valueType == typeof(float))
					{
						value = reader.ReadSingle().CastFromTo<float, object>();
					}
					else if (valueType == typeof(ushort))
					{
						value = reader.ReadUInt16().CastFromTo<ushort, object>();
					}
					else if (valueType == typeof(uint))
					{
						value = reader.ReadUInt32().CastFromTo<uint, object>();
					}
					else if (valueType == typeof(string))
					{
						value = reader.ReadString().CastFromTo<string, object>();
					}
				}
			}

			logger?.LogError(
				GetType(),
				$"UNSUPPORTED TYPE: {valueType.Name}");

			value = default;

			return false;
		}

		protected override bool FallbackConvertToTargetType<TValue>(
			TValue value,
			out byte[] byteArray)
		{
			if (typeof(TValue) == typeof(byte[]))
			{
				byteArray = value.CastFromTo<TValue, byte[]>();

				return true;
			}

			using (var ms = new MemoryStream())
			{
				using (var writer = new BinaryWriter(ms))
				{
					// Read the value based on its type
					if (typeof(TValue) == typeof(bool))
					{
						writer.Write(value.CastFromTo<TValue, bool>());
					}
					else if (typeof(TValue) == typeof(byte))
					{
						writer.Write(value.CastFromTo<TValue, byte>());
					}
					else if (typeof(TValue) == typeof(char))
					{
						writer.Write(value.CastFromTo<TValue, char>());
					}
					else if (typeof(TValue) == typeof(decimal))
					{
						writer.Write(value.CastFromTo<TValue, decimal>());
					}
					else if (typeof(TValue) == typeof(double))
					{
						writer.Write(value.CastFromTo<TValue, double>());
					}
					else if (typeof(TValue) == typeof(short))
					{
						writer.Write(value.CastFromTo<TValue, short>());
					}
					else if (typeof(TValue) == typeof(int))
					{
						writer.Write(value.CastFromTo<TValue, int>());
					}
					else if (typeof(TValue) == typeof(long))
					{
						writer.Write(value.CastFromTo<TValue, long>());
					}
					else if (typeof(TValue) == typeof(sbyte))
					{
						writer.Write(value.CastFromTo<TValue, sbyte>());
					}
					else if (typeof(TValue) == typeof(float))
					{
						writer.Write(value.CastFromTo<TValue, float>());
					}
					else if (typeof(TValue) == typeof(ushort))
					{
						writer.Write(value.CastFromTo<TValue, ushort>());
					}
					else if (typeof(TValue) == typeof(uint))
					{
						writer.Write(value.CastFromTo<TValue, uint>());
					}
					else if (typeof(TValue) == typeof(string))
					{
						writer.Write(value.CastFromTo<TValue, string>());
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"UNSUPPORTED TYPE: {typeof(TValue).Name}");

						byteArray = default;

						return false;
					}
				}

				byteArray = ms.ToArray();
			}

			return true;
		}

		protected override bool FallbackConvertToTargetType(
			Type valueType,
			object valueObject,
			out byte[] byteArray)
		{
			if (valueType == typeof(byte[]))
			{
				byteArray = valueObject.CastFromTo<object, byte[]>();

				return true;
			}

			using (var ms = new MemoryStream())
			{
				using (var writer = new BinaryWriter(ms))
				{
					// Read the value based on its type
					if (valueType == typeof(bool))
					{
						writer.Write(valueObject.CastFromTo<object, bool>());
					}
					else if (valueType == typeof(byte))
					{
						writer.Write(valueObject.CastFromTo<object, byte>());
					}
					else if (valueType == typeof(char))
					{
						writer.Write(valueObject.CastFromTo<object, char>());
					}
					else if (valueType == typeof(decimal))
					{
						writer.Write(valueObject.CastFromTo<object, decimal>());
					}
					else if (valueType == typeof(double))
					{
						writer.Write(valueObject.CastFromTo<object, double>());
					}
					else if (valueType == typeof(short))
					{
						writer.Write(valueObject.CastFromTo<object, short>());
					}
					else if (valueType == typeof(int))
					{
						writer.Write(valueObject.CastFromTo<object, int>());
					}
					else if (valueType == typeof(long))
					{
						writer.Write(valueObject.CastFromTo<object, long>());
					}
					else if (valueType == typeof(sbyte))
					{
						writer.Write(valueObject.CastFromTo<object, sbyte>());
					}
					else if (valueType == typeof(float))
					{
						writer.Write(valueObject.CastFromTo<object, float>());
					}
					else if (valueType == typeof(ushort))
					{
						writer.Write(valueObject.CastFromTo<object, ushort>());
					}
					else if (valueType == typeof(uint))
					{
						writer.Write(valueObject.CastFromTo<object, uint>());
					}
					else if (valueType == typeof(string))
					{
						writer.Write(valueObject.CastFromTo<object, string>());
					}
					else
					{
						logger?.LogError(
							GetType(),
							$"UNSUPPORTED TYPE: {valueType.Name}");

						byteArray = default;

						return false;
					}
				}

				byteArray = ms.ToArray();
			}

			return true;
		}
	}
}