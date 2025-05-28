using System;
using System.Collections.Generic;

using HereticalSolutions.Persistence;

using HereticalSolutions.TypeConversion;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
	[Visitor(typeof(IReadOnlyRepository<,>), typeof(RepositoryDTO))]
	public abstract class ARepositoryVisitor<TRepository, TKey, TValue>
		: ISaveVisitor,
		  ILoadVisitor,
		  IPopulateVisitor
		  where TRepository : IReadOnlyRepository<TKey, TValue>
	{
		protected readonly ITypeConverter<string> stringConverter;

		protected readonly ILogger logger;

		public ARepositoryVisitor(
			ITypeConverter<string> stringConverter,
			ILogger logger)
		{
			this.stringConverter = stringConverter;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>(
			TVisitable instance)
		{
			return typeof(TVisitable).IsSameOrInheritor(typeof(IReadOnlyRepository<,>));
		}

		public bool CanVisit(
			Type visitableType,
			object instance)
		{
			return visitableType.IsSameOrInheritor(typeof(IReadOnlyRepository<,>));
		}

		public Type GetDTOType<TVisitable>(
			TVisitable instance)
		{
			if (!typeof(TVisitable).IsSameOrInheritor(typeof(IReadOnlyRepository<,>)))
				return null;

			return typeof(RepositoryDTO);
		}

		public Type GetDTOType(
			Type visitableType,
			object instance)
		{
			if (!visitableType.IsSameOrInheritor(typeof(IReadOnlyRepository<,>)))
				return null;

			return typeof(RepositoryDTO);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (visitable is not TRepository repository)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(TRepository).Name}");

				return false;
			}

			List<KeyValuePairDTO> keyValuePairs = new List<KeyValuePairDTO>();

			foreach (var key in repository.Keys)
			{
				var value = repository.Get(key);

				if (!stringConverter.ConvertToTargetType(
					key.GetType(),
					key,
					out var stringKey))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT KEY TO STRING: {key}");

					dto = null;

					return false;
				}

				if (!stringConverter.ConvertToTargetType(
					value.GetType(),
					value,
					out var stringValue))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE TO STRING: {value}");

					dto = null;

					return false;
				}

				keyValuePairs.Add(
					new KeyValuePairDTO
					{
						Key = stringKey,
						Value = stringValue
					});
			}

			dto = new RepositoryDTO
			{
				KeyValuePairs = keyValuePairs.ToArray()
			};

			return true;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (visitableObject is not TRepository repository)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(TRepository).Name}");

				return false;
			}

			List<KeyValuePairDTO> keyValuePairs = new List<KeyValuePairDTO>();

			foreach (var key in repository.Keys)
			{
				var value = repository.Get(key);

				if (!stringConverter.ConvertToTargetType(
					key.GetType(),
					key,
					out var stringKey))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT KEY TO STRING: {key}");

					dto = null;

					return false;
				}

				if (!stringConverter.ConvertToTargetType(
					value.GetType(),
					value,
					out var stringValue))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE TO STRING: {value}");

					dto = null;

					return false;
				}

				keyValuePairs.Add(
					new KeyValuePairDTO
					{
						Key = stringKey,
						Value = stringValue
					});
			}

			dto = new RepositoryDTO
			{
				KeyValuePairs = keyValuePairs.ToArray()
			};

			return true;
		}

		#endregion

		#region ILoadVisitor

		public bool VisitLoad<TVisitable>(
			object dto,
			out TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (dto is not RepositoryDTO castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(RepositoryDTO)}");

				visitable = default;

				return false;
			}

			var repository = AllocateRepository();

			foreach (var keyValuePair in castedDTO.KeyValuePairs)
			{
				IRepository<TKey, TValue> repositoryCasted =
					repository as IRepository<TKey, TValue>;

				stringConverter.ConvertFromTargetType<TKey>(
					keyValuePair.Key,
					out var key);

				stringConverter.ConvertFromTargetType<TValue>(
					keyValuePair.Value,
					out var value);

				repositoryCasted.Add(
					key,
					value);
			}

			if (repository is not TVisitable repositoryAsVisitable)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(TVisitable).Name}");

				visitable = default;

				return false;
			}

			visitable = repositoryAsVisitable;

			return true;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out object visitableObject,
			IVisitor rootVisitor)
		{
			if (dto is not RepositoryDTO castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(RepositoryDTO)}");

				visitableObject = default;

				return false;
			}

			var repository = AllocateRepository();

			foreach (var keyValuePair in castedDTO.KeyValuePairs)
			{
				IRepository<TKey, TValue> repositoryCasted =
					repository as IRepository<TKey, TValue>;

				stringConverter.ConvertFromTargetType<TKey>(
					keyValuePair.Key,
					out var key);

				stringConverter.ConvertFromTargetType<TValue>(
					keyValuePair.Value,
					out var value);

				repositoryCasted.Add(
					key,
					value);
			}

			visitableObject = repository;

			return true;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable,
			IVisitor rootVisitor)
		{
			if (dto is not RepositoryDTO castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(RepositoryDTO)}");

				return false;
			}

			if (visitable is not IRepository<TKey, TValue> repository)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(IRepository<TKey, TValue>).Name}");

				return false;
			}

			repository.Clear();

			foreach (var keyValuePair in castedDTO.KeyValuePairs)
			{
				stringConverter.ConvertFromTargetType<TKey>(
					keyValuePair.Key,
					out var key);

				stringConverter.ConvertFromTargetType<TValue>(
					keyValuePair.Value,
					out var value);

				repository.Add(
					key,
					value);
			}
			
			return true;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject,
			IVisitor rootVisitor)
		{
			if (dto is not RepositoryDTO castedDTO)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {nameof(RepositoryDTO)}");

				return false;
			}

			if (visitableObject is not IRepository<TKey, TValue> repository)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(IRepository<TKey, TValue>).Name}");

				return false;
			}

			repository.Clear();

			foreach (var keyValuePair in castedDTO.KeyValuePairs)
			{
				stringConverter.ConvertFromTargetType<TKey>(
					keyValuePair.Key,
					out var key);

				stringConverter.ConvertFromTargetType<TValue>(
					keyValuePair.Value,
					out var value);

				repository.Add(
					key,
					value);
			}

			return true;
		}

		#endregion

		protected abstract TRepository AllocateRepository();
	}
}