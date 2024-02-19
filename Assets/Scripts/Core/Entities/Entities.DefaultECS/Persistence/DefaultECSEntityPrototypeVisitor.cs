using System;
using System.Collections.Generic;

using System.Reflection;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;

using HereticalSolutions.Entities.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.Serialization;

namespace HereticalSolutions.Entities
{
	public class DefaultECSEntityPrototypeVisitor
		: ASaveLoadVisitor<Entity, EntityPrototypeDTO>
	{
		#region Reflection

		private static Type[] componentTypes;

		private static MethodInfo writeComponentMethodInfo;

		private static IReadOnlyRepository<Type, WriteComponentToObjectDelegate> componentWriters;

		//private static Type[] viewComponentTypes;

		//private static MethodInfo addComponentMethodInfo;

		//private static IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> componentAdders;

		#endregion

		private readonly IPrototypesRepository<World, Entity> prototypesRepository;

		public DefaultECSEntityPrototypeVisitor(
			IPrototypesRepository<World, Entity> prototypesRepository,
			ILogger logger = null)
			: base(logger)
		{
			this.prototypesRepository = prototypesRepository;
		}

		#region ILoadVisitorGeneric

		public override bool Load(
			EntityPrototypeDTO DTO,
			out Entity value)
		{
			LazyInitialization();

			prototypesRepository.TryAllocatePrototype(
				DTO.PrototypeID,
				out value);

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						value,
						component);
			}

			return true;
		}

		public override bool Load(
			EntityPrototypeDTO DTO,
			Entity valueToPopulate)
		{
			LazyInitialization();

			foreach (var component in DTO.Components)
			{
				componentWriters
					.Get(component.GetType())
					.Invoke(
						valueToPopulate,
						component);
			}

			return true;
		}

		#endregion

		#region ISaveVisitorGeneric

		public override bool Save(Entity value, out EntityPrototypeDTO DTO)
		{
			var entitySerializationWrapper = new EntitySerializationWrapper(value);

			object[] componentsArray = new object[entitySerializationWrapper.Components.Length];

			for (int i = 0; i < componentsArray.Length; i++)
			{
				componentsArray[i] = entitySerializationWrapper.Components[i].ObjectValue;
			}

			string prototypeID = string.Empty;

			//TODO: optimize
			foreach (var key in prototypesRepository.AllPrototypeIDs)
			{
				if (prototypesRepository.TryGetPrototype(
					key,
					out Entity prototypeEntity))
				{
					if (prototypeEntity.Equals(value))
					{
						prototypeID = key;

						break;
					}
				}
			}

			DTO = new EntityPrototypeDTO
			{
				PrototypeID = prototypeID,

				Components = componentsArray
			};

			return true;
		}

		#endregion

		private void LazyInitialization()
		{
			if (componentTypes == null)
			{
				DefaultECSEntitiesFactory.BuildComponentTypesListWithAttribute<ComponentAttribute>(
					out componentTypes);
			}

			if (writeComponentMethodInfo == null)
			{
				writeComponentMethodInfo =
					typeof(DefaultECSEntityPrototypeVisitor).GetMethod(
						"WriteComponent",
						BindingFlags.Static | BindingFlags.Public);
			}

			if (componentWriters == null)
			{
				componentWriters = BuildComponentWriters(
					writeComponentMethodInfo,
					componentTypes);
			}
		}

		private static IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> BuildComponentAdders(
			MethodInfo addComponentMethodInfo,
			Type[] viewComponentTypes)
		{
			IReadOnlyRepository<Type, AddObjectComponentToEntityDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, AddObjectComponentToEntityDelegate>();

			for (int i = 0; i < viewComponentTypes.Length; i++)
			{
				MethodInfo addComponentGeneric = addComponentMethodInfo.MakeGenericMethod(viewComponentTypes[i]);

				AddObjectComponentToEntityDelegate addComponentGenericDelegate =
					(AddObjectComponentToEntityDelegate)addComponentGeneric.CreateDelegate(
						typeof(AddObjectComponentToEntityDelegate),
						null);

				((IRepository<Type, AddObjectComponentToEntityDelegate>)result).Add(
					viewComponentTypes[i],
					addComponentGenericDelegate);
			}

			return result;
		}

		public static void AddComponent<TComponent>(
			Entity entity,
			object component)
		{
			// Early return for AoT compilation calls
			if (component == null)
				return;

			entity.Set<TComponent>((TComponent)component);
		}

		private static IReadOnlyRepository<Type, WriteComponentToObjectDelegate> BuildComponentWriters(
			MethodInfo writeComponentMethodInfo,
			Type[] componentTypes)
		{
			IReadOnlyRepository<Type, WriteComponentToObjectDelegate> result =
				RepositoriesFactory.BuildDictionaryRepository<Type, WriteComponentToObjectDelegate>();

			for (int i = 0; i < componentTypes.Length; i++)
			{
				MethodInfo writeComponentGeneric = writeComponentMethodInfo.MakeGenericMethod(componentTypes[i]);

				WriteComponentToObjectDelegate writeComponentGenericDelegate =
					(WriteComponentToObjectDelegate)writeComponentGeneric.CreateDelegate(
						typeof(WriteComponentToObjectDelegate),
						null);

				((IRepository<Type, WriteComponentToObjectDelegate>)result).Add(
					componentTypes[i],
					writeComponentGenericDelegate);
			}

			return result;
		}

		public static void WriteComponent<TComponent>(
			Entity entity,
			object componentValue)
		{
			// Early return for AoT compilation calls
			if (componentValue == null)
				return;

			entity.Set<TComponent>((TComponent)componentValue);
		}

		interface IComponentWrapper
		{
			Type Type { get; }

			object ObjectValue { get; }
		}

		private class EntitySerializationWrapper
		{
			private class ComponentWrapper<T> : IComponentWrapper
			{
				public bool IsEnabled { get; }

				public T Value { get; }

				public object ObjectValue
				{
					get => Value;
				}

				public Type Type => typeof(T);

				public ComponentWrapper(bool isEnabled, T value)
				{
					Value = value;

					IsEnabled = isEnabled;
				}
			}

			private sealed class ComponentSerializationReader : IComponentReader
			{
				private readonly Entity _entity;

				private readonly List<IComponentWrapper> _components;

				public ComponentSerializationReader(in Entity entity, List<IComponentWrapper> components)
				{
					_components = components;
					_entity = entity;
				}

				public void OnRead<T>(in T component, in Entity componentOwner) => _components.Add(new ComponentWrapper<T>(_entity.IsEnabled<T>(), component));
			}

			private readonly Entity _entity;
			private readonly List<IComponentWrapper> _components;

			public World World => _entity.World;
			public bool IsAlive => _entity.IsAlive;
			public bool IsEnabled => _entity.IsEnabled();
			public IComponentWrapper[] Components => _components.ToArray();

			public EntitySerializationWrapper(Entity entity)
			{
				_entity = entity;

				_components = new List<IComponentWrapper>();

				entity.ReadAllComponents(new ComponentSerializationReader(_entity, _components));

				_components.Sort((o1, o2) => string.Compare(o1.Type.FullName, o2.Type.FullName));
			}
		}
	}
}