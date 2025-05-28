using System;

namespace HereticalSolutions.Repositories
{
	public static class RepositoryHelpers
	{
		public static bool TryGetByTypeOrInheritor<TKey, TValue>(
			this IReadOnlyRepository<Type, TValue> repository,
			out TValue value)
		{
			var key = typeof(TKey);

			if (key.IsGenericType)
			{
				var genericTypeDefinition = key.GetGenericTypeDefinition();

				if (repository.Has(
					genericTypeDefinition))
				{
					value = repository.Get(genericTypeDefinition);

					return true;
				}
			}
			
			if (repository.Has(
				key))
			{
				value = repository.Get(key);

				return true;
			}

			value = default;

			return false;
		}

		public static bool TryGetByTypeOrInheritor<TValue>(
			this IReadOnlyRepository<Type, TValue> repository,
			Type key,
			out TValue value)
		{
			if (key.IsGenericType)
			{
				var genericTypeDefinition = key.GetGenericTypeDefinition();

				if (repository.Has(
					genericTypeDefinition))
				{
					value = repository.Get(genericTypeDefinition);

					return true;
				}
			}

			if (repository.Has(
				key))
			{
				value = repository.Get(key);

				return true;
			}

			value = default;

			return false;
		}
	}
}