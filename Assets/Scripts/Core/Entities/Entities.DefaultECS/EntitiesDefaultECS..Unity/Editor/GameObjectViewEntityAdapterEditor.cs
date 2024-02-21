using System;
using System.Linq;

using DefaultEcs;

using UnityEditor;

using UnityEngine;

namespace HereticalSolutions.Entities.Editor
{
	[CustomEditor(typeof(GameObjectViewEntityAdapter))]
	public class GameObjectViewEntityAdapterEditor
		: UnityEditor.Editor
	{
		private IEntityIDEditorHelper[] entityIDEditorHelpers;

		private object entityManager;

		private IContainsEntityWorlds<World, IDefaultECSEntityWorldController> entityWorldsRepository;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			LazyInitialization();


			EditorGUILayout.Space();

			var viewEntityAdapter = (GameObjectViewEntityAdapter)target;

			if (!viewEntityAdapter.Initialized)
			{
				EditorGUILayout.LabelField($"View entity adapter is not initialized");

				return;
			}

			if (entityManager == null)
			{
				foreach (var helper in entityIDEditorHelpers)
				{
					if (helper.TryGetEntityManager(out entityManager))
					{
						entityWorldsRepository = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

						break;
					}
				}
			}

			//Still not found
			if (entityManager == null)
			{
				EditorGUILayout.LabelField($"Could not find entity manager");

				return;
			}

			object entityIDObject = null;

			foreach (var helper in entityIDEditorHelpers)
			{
				if (helper.TryGetEntityID(
					viewEntityAdapter.ViewEntity,
					out entityIDObject))
				{
					break;
				}
			}

			if (entityIDObject != null)
			{
				//var guid = viewEntityAdapter.ViewEntity.Get<GameEntityComponent>().GUID;

				var registryEntity = default(Entity);

				foreach (var helper in entityIDEditorHelpers)
				{
					registryEntity = helper.GetRegistryEntity(
						entityManager,
						entityIDObject);

					if (registryEntity != default)
					{
						break;
					}
				}

				if (registryEntity == default)
				{
					EditorGUILayout.LabelField($"Could not find registry entity");

					return;
				}

				DrawEntity(
					registryEntity,
					"Registry world entity");

				EditorGUILayout.Space();

				foreach (var worldID in entityWorldsRepository.EntityWorldsRepository.AllWorldIDs)
				{
					var localEntity = default(Entity);

					foreach (var helper in entityIDEditorHelpers)
					{
						localEntity = helper.GetEntity(
							entityManager,
							entityIDObject,
							worldID);

						if (localEntity != default)
						{
							break;
						}
					}

					if (localEntity == default)
					{
						continue;
					}

					DrawEntity(
						localEntity,
						worldID);

					EditorGUILayout.Space();
				}
			}
			else
			{
				DrawEntity(
					viewEntityAdapter.ViewEntity,
					"View world entity");
			}
		}

		private void LazyInitialization()
		{
			if (entityIDEditorHelpers == null)
			{
				entityIDEditorHelpers = GetHelpers();
			}
		}

		private static IEntityIDEditorHelper[] GetHelpers()
		{
			var interfaceType = typeof(IEntityIDEditorHelper);

			var types = AppDomain
				.CurrentDomain
				.GetAssemblies()
				.SelectMany(
					s => s.GetTypes())
				.Where(
					p => interfaceType.IsAssignableFrom(p)
					&& p.IsClass
					&& !p.IsAbstract);

			IEntityIDEditorHelper[] result = new IEntityIDEditorHelper[types.Count()];

			for (int i = 0; i < types.Count(); i++)
			{
				result[i] = (IEntityIDEditorHelper)Activator.CreateInstance(types.ElementAt(i));
			}

			return result;
		}

		private void DrawEntity(
			Entity entity,
			string entityName)
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.LabelField(entityName);

			EditorGUILayout.Space();

			var entitySerializationWrapper = new EntitySerializationWrapper(entity);

			foreach (var component in entitySerializationWrapper.Components)
			{
				EditorGUILayout.BeginVertical("Window");

				EditorGUILayout.LabelField($"{component.Type.Name}");

				EditorGUILayout.Space();

				ToDetailedString(component.ObjectValue);

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}

		private static void ToDetailedString(object component)
		{
			if (component == null)
				return;

			var fields = component.GetType().GetFields();

			foreach (var fieldInfo in fields)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(fieldInfo.Name);

				var value = fieldInfo.GetValue(component);

				string valueString = (value == null) ? "NULL" : value.ToString();

				EditorGUILayout.LabelField(valueString);

				EditorGUILayout.EndHorizontal();
			}
		}
	}
}

