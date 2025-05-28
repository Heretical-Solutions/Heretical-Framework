using System;

namespace HereticalSolutions.TypeConversion
{
	public interface ITypeConverter<TTarget>
	{
		bool ConvertFromTargetType<TValue>(
			TTarget source,
			out TValue value);

		bool ConvertFromTargetType(
			Type valueType,
			TTarget source,
			out object value);

		bool ConvertToTargetType<TValue>(
			TValue value,
			out TTarget result);

		bool ConvertToTargetType(
			Type valueType,
			object value,
			out TTarget result);
	}
}