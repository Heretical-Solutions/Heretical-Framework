using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.TypeConversion
{
	public abstract class ATypeConverter<TTarget>
		: ITypeConverter<TTarget>
	{
		protected readonly IReadOnlyRepository<Type, Delegate>
			convertFromTargetTypeDelegateRepository;

		protected readonly IReadOnlyRepository<Type, Delegate>
			convertToTargetTypeDelegateRepository;

		protected readonly ILogger logger;

		public ATypeConverter(
			IReadOnlyRepository<Type, Delegate> convertFromTargetTypeDelegateRepository,
			IReadOnlyRepository<Type, Delegate> convertToTargetTypeDelegateRepository,
			ILogger logger)
		{
			this.convertFromTargetTypeDelegateRepository = 
				convertFromTargetTypeDelegateRepository;

			this.convertToTargetTypeDelegateRepository = 
				convertToTargetTypeDelegateRepository;

			this.logger = logger;
		}

		#region ITypeConverter

		public bool ConvertFromTargetType<TValue>(
			TTarget source,
			out TValue value)
		{
			Delegate @delegate;

			if (convertFromTargetTypeDelegateRepository.TryGet(
				typeof(TValue),
				out @delegate))
			{
				Func<TTarget, TValue> convertFromTargetTypeDelegate =
					(Func<TTarget, TValue>)@delegate;

				if (convertFromTargetTypeDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM {typeof(TTarget).Name} DELEGATE FOR TYPE {typeof(TValue).Name}");

					value = default;

					return false;
				}

				value = convertFromTargetTypeDelegate.Invoke(
					source);

				return true;
			}

			if (FallbackConvertFromTargetType<TValue>(
				source,
				out value))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM {typeof(TTarget).Name} FOR TYPE {typeof(TValue).Name}");

			value = default;

			return false;
		}

		public bool ConvertFromTargetType(
			Type valueType,
			TTarget source,
			out object value)
		{
			Delegate @delegate;

			if (convertFromTargetTypeDelegateRepository.TryGet(
				valueType,
				out @delegate))
			{
				Func<TTarget, object> convertFromTargetTypeDelegate =
					(Func<TTarget, object>)@delegate;

				if (convertFromTargetTypeDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM {typeof(TTarget).Name} DELEGATE FOR TYPE {valueType}");

					value = default;

					return false;
				}

				value = convertFromTargetTypeDelegate.Invoke(
					source);

				return true;
			}

			if (FallbackConvertFromTargetType(
				valueType,
				source,
				out value))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM {typeof(TTarget).Name} FOR TYPE {valueType.Name}");

			value = default;

			return false;
		}

		public bool ConvertToTargetType<TValue>(
			TValue value,
			out TTarget result)
		{
			if (convertToTargetTypeDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				Func<TValue, TTarget> convertToTargetTypeDelegate =
					(Func<TValue, TTarget>)@delegate;

				if (convertToTargetTypeDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO {typeof(TTarget).Name} DELEGATE FOR TYPE {typeof(TValue).Name}");

					result = default;

					return false;
				}

				result = convertToTargetTypeDelegate.Invoke(
					value);

				return true;
			}

			if (FallbackConvertToTargetType<TValue>(
				value,
				out result))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO {typeof(TTarget).Name} FOR TYPE {typeof(TValue).Name}");

			return false;
		}

		public bool ConvertToTargetType(
			Type valueType,
			object value,
			out TTarget result)
		{
			if (convertToTargetTypeDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				Func<object, TTarget> convertToTargetTypeDelegate =
					(Func<object, TTarget>)@delegate;

				if (convertToTargetTypeDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO {typeof(TTarget).Name} DELEGATE FOR TYPE {valueType.Name}");

					result = default;

					return false;
				}

				result = convertToTargetTypeDelegate.Invoke(
					value);

				return true;
			}

			if (FallbackConvertToTargetType(
				valueType,
				value,
				out result))
			{
				return true;
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO {typeof(TTarget).Name} FOR TYPE {valueType.Name}");

			return false;
		}

		#endregion

		protected abstract bool FallbackConvertFromTargetType<TValue>(
			TTarget source,
			out TValue value);

		protected abstract bool FallbackConvertFromTargetType(
			Type valueType,
			TTarget source,
			out object value);

		protected abstract bool FallbackConvertToTargetType<TValue>(
			TValue value,
			out TTarget result);

		protected abstract bool FallbackConvertToTargetType(
			Type valueType,
			object valueObject,
			out TTarget result);
	}
}