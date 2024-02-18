using System;
using System.Collections.Generic;
using System.Reflection;

using DefaultEcs;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.Factories;
using HereticalSolutions.Persistence.Serializers;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;

using UnityEngine;

namespace HereticalSolutions.Entities.Editor
{
	[CustomEditor(typeof(EntitySettings))]
	[CanEditMultipleObjects]
	public class EntitySettingsEditor : UnityEditor.Editor
	{
		private ComponentTypesProvider componentTypesProvider;

		private EntityPrototypeDTO entityPrototypeDTO;

		private JSONSerializer jsonSerializer;

		private StringArgument stringArgument;

		private GUIStyle structNameLabelStyle;

		void OnEnable()
		{
			if (componentTypesProvider == null)
			{
				componentTypesProvider = new ComponentTypesProvider();

				componentTypesProvider.OnComponentSelected += OnComponentTypeToAddSelected;
			}

			if (jsonSerializer == null)
				jsonSerializer = UnityPersistenceFactory.BuildSimpleUnityJSONSerializer();

			if (stringArgument == null)
				stringArgument = new StringArgument();

			if (structNameLabelStyle == null)
			{
				structNameLabelStyle = new GUIStyle();
				structNameLabelStyle.fontSize = 12;
				structNameLabelStyle.fontStyle = FontStyle.Bold;
				structNameLabelStyle.normal.textColor = Color.white;
			}

			//globalDirty = false;

			Deserialize();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			bool dirty = false;

			string prototypeID = EditorGUILayout.TextField(
				entityPrototypeDTO.PrototypeID);

			if (prototypeID != entityPrototypeDTO.PrototypeID)
			{
				entityPrototypeDTO.PrototypeID = prototypeID;

				dirty = true;
			}

			if (DrawComponents())
				dirty = true;

			EditorGUILayout.Space(10f);

			if (GUILayout.Button("ERASE"))
			{
				Erase();
			}

			if (GUILayout.Button("ADD COMPONENT"))
			{
				SearchWindow.Open(
					new SearchWindowContext(
						GUIUtility.GUIToScreenPoint(
							Event.current.mousePosition)),
					componentTypesProvider);
			}

			if (dirty)
			{
				Serialize();

				dirty = false;
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool DrawComponents()
		{
			if (entityPrototypeDTO.Components == null)
			{
				Erase();
			}

			bool localDirty = false;

			for (int i = 0; i < entityPrototypeDTO.Components.Length; i++)
			{
				if (DrawStruct(
						ref entityPrototypeDTO.Components[i],
						i))
					localDirty = true;
			}

			return localDirty;
		}

		private bool DrawStruct(
			ref object structObject,
			int index,
			int level = 0)
		{
			Type structType = structObject.GetType();

			var fields = structType.GetFields(
				BindingFlags.Public
				| BindingFlags.Instance);

			if (level == 0)
				EditorGUILayout.BeginVertical("Box");
			else
				EditorGUILayout.BeginVertical("Window");

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(
				structType.Name,
				structNameLabelStyle);

			bool modified = false;

			if (level == 0
				&& index != -1)
			{
				if (GUILayout.Button("UP"))
				{
					MoveComponentUp(index);

					modified = true;
				}

				if (GUILayout.Button("DOWN"))
				{
					MoveComponentDown(index);

					modified = true;
				}

				if (GUILayout.Button("REMOVE"))
				{
					RemoveComponent(index);

					modified = true;
				}
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space(10f);

			bool localDirty = false;

			if (!modified)
			{
				foreach (var fieldInfo in fields)
				{
					if (!fieldInfo.IsDefined(typeof(HideInInspector), true)) //HideInInspector filtered out
					{
						if (DrawField(
								ref structObject,
								fieldInfo,
								level))
							localDirty = true;
					}
				}
			}

			EditorGUILayout.EndVertical();

			return localDirty;
		}

		private bool DrawField(
			ref object structObject,
			FieldInfo fieldInfo,
			int level)
		{
			var value = fieldInfo.GetValue(structObject);

			var valueType = fieldInfo.FieldType;

			var fieldName = fieldInfo.Name;

			bool localDirty = false;

			switch (value)
			{
				case Guid guidValue:
					break;

				case Entity entityValue:
					break;

				case Enum enumValue:
					var enumResult = EditorGUILayout.EnumPopup(fieldName, enumValue);

					if (!Equals(enumResult, enumValue))
					{
						fieldInfo.SetValue(structObject, enumResult);

						localDirty = true;
					}

					break;

				case int intValue:

					int intResult = EditorGUILayout.IntField(
						fieldName,
						(int)value);

					if (intResult != intValue)
					{
						fieldInfo.SetValue(structObject, intResult);

						localDirty = true;
					}

					break;

				case long longValue:

					long longResult = EditorGUILayout.LongField(
						fieldName,
						(long)value);

					if (longResult != longValue)
					{
						fieldInfo.SetValue(structObject, longResult);

						localDirty = true;
					}

					break;

				case float floatValue:

					float floatResult = EditorGUILayout.FloatField(
						fieldName,
						(float)value);

					if (floatResult != floatValue)
					{
						fieldInfo.SetValue(structObject, floatResult);

						localDirty = true;
					}

					break;

				case double doubleValue:

					double doubleResult = EditorGUILayout.DoubleField(
						fieldName,
						(double)value);

					if (doubleResult != doubleValue)
					{
						fieldInfo.SetValue(structObject, doubleResult);

						localDirty = true;
					}

					break;

				case bool boolValue:

					bool boolResult = EditorGUILayout.Toggle(
						fieldName,
						(bool)value);

					if (boolResult != boolValue)
					{
						fieldInfo.SetValue(structObject, boolResult);

						localDirty = true;
					}

					break;

				//FOR SOME REASON ITS NOT EVEN ENTERING THIS CASE
				//NEITHER string NOR String WORK
				/*
                case string stringValue:

                    string stringResult = EditorGUILayout.TextField(
                        fieldName,
                        (string)value);

                    if (stringResult != stringValue)
                    {
                        fieldInfo.SetValue(structObject, stringResult);

                        localDirty = true;
                    }

                    break;
                */

				case Vector2 vector2Value:

					Vector2 vector2Result = EditorGUILayout.Vector2Field(
						fieldName,
						(Vector2)value);

					if (vector2Result != vector2Value)
					{
						fieldInfo.SetValue(structObject, vector2Result);

						localDirty = true;
					}

					break;

				case Vector3 vector3Value:

					Vector3 vector3Result = EditorGUILayout.Vector3Field(
						fieldName,
						(Vector3)value);

					if (vector3Result != vector3Value)
					{
						fieldInfo.SetValue(structObject, vector3Result);

						localDirty = true;
					}

					break;

				case LayerMask layerMask:
					LayerMask temp =
						EditorGUILayout.MaskField(
							fieldName,
							InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask),
							InternalEditorUtility.layers);

					var result = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(temp);

					if (result != layerMask)
					{
						fieldInfo.SetValue(structObject, result);

						localDirty = true;
					}

					break;

				/*case Array array:
                    if(array == null)
                        array = (Array)Activator.CreateInstance(fieldInfo.FieldType);

                    foreach (var property in array)
                    {
                        foreach (var propertyInfo in property.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            EditorGUILayout.LabelField(propertyInfo.Name);
                        }
                    }
                    
                    break;*/

				default:

					if (valueType == typeof(string))
					{
						var stringValue = (string)value;

						string stringResult = EditorGUILayout.TextField(
							fieldName,
							(string)value);

						if (stringResult != stringValue)
						{
							fieldInfo.SetValue(structObject, stringResult);

							localDirty = true;
						}

						break;
					}

					//TODO: check
					/*
					if (fieldInfo.FieldType.GetInterface(typeof(IEnumerable<>).FullName) != null
						&& fieldInfo.FieldType.GetElementType() != typeof(Entity))
					{
						var array = fieldInfo.GetValue(structObject);

						if (array == null)
							array = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), 0);

						var newArray = DrawArray(fieldName, (Array)array, out localDirty);

						if (localDirty)
						{
							fieldInfo.SetValue(structObject, newArray);
							//localDirty = true;
						}

						break;
					}

					bool isEmptyScriptable = valueType.Name.Contains("Config"); //don't know how to show scriptable field with Null value to set it

					if (value as ScriptableObject || isEmptyScriptable)
					{
						var configValue = (ScriptableObject)value;

						var objectResult = EditorGUILayout.ObjectField(fieldName, configValue, typeof(ScriptableObject), false);

						if (!Equals(objectResult, configValue))
						{
							fieldInfo.SetValue(structObject, objectResult);

							localDirty = true;
						}

						break;
					}
					*/

					bool isStruct = valueType.IsValueType && !valueType.IsPrimitive;

					if (isStruct)
					{
						EditorGUILayout.BeginVertical();

						EditorGUILayout.LabelField(fieldName);

						if (DrawStruct(
								ref value,
								-1,
								level + 1))
							localDirty = true;

						EditorGUILayout.EndVertical();

						if (localDirty)
							fieldInfo.SetValue(
								structObject, value);
					}

					break;
			}

			return localDirty;
		}

		/*
		private Array DrawArray(string fieldName, Array arrayValue, out bool dirty)
		{
			dirty = false;

			EditorGUILayout.LabelField(fieldName);

			EditorGUILayout.LabelField("Length: " + arrayValue.Length);

			for (int i = 0; i < arrayValue.Length; i++)
			{
				var element = arrayValue.GetValue(i);

				bool localDirty = false;

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.BeginVertical();

				if (DrawStruct(ref element, -1))
					localDirty = true;

				if (localDirty)
					arrayValue.SetValue(element, i);

				if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
				{
					arrayValue = RemoveElement(arrayValue, i, out dirty);
					break;
				}

				EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();

				dirty = localDirty;
			}

			if (GUILayout.Button("Add Element"))
			{
				arrayValue = AddNewElement(arrayValue, out dirty);
			}

			return arrayValue;
		}
		*/

		/*
		private Array AddNewElement(Array arrayValue, out bool dirty)
		{
			dirty = false;

			var newArray = Array.CreateInstance(arrayValue.GetType().GetElementType(), arrayValue.Length + 1);

			for (int i = 0; i < arrayValue.Length; i++)
			{
				newArray.SetValue(arrayValue.GetValue(i), i);
			}

			var elementType = arrayValue.GetType().GetElementType();
			var newElement = Activator.CreateInstance(elementType);

			newArray.SetValue(newElement, arrayValue.Length);

			dirty = true;

			return newArray;
		}

		private Array RemoveElement(Array arrayValue, int index, out bool dirty)
		{
			dirty = false;
			var newArray = Array.CreateInstance(arrayValue.GetType().GetElementType(), arrayValue.Length - 1);

			for (int i = 0, j = 0; i < arrayValue.Length; i++)
			{
				if (i != index)
				{
					newArray.SetValue(arrayValue.GetValue(i), j);
					j++;

					dirty = true;
				}
			}

			return newArray;
		}
		*/

		private void Erase()
		{
			entityPrototypeDTO = new EntityPrototypeDTO();

			entityPrototypeDTO.Components = new object[0];

			Serialize();
		}

		private void Serialize()
		{
			jsonSerializer.Serialize<EntityPrototypeDTO>(
				stringArgument,
				entityPrototypeDTO);

			((EntitySettings)target).EntityJson = stringArgument.Value;

			EditorUtility.SetDirty(target);
		}

		private void Deserialize()
		{
			stringArgument.Value = ((EntitySettings)target).EntityJson;

			bool success = jsonSerializer.Deserialize(
				stringArgument,
				typeof(EntityPrototypeDTO),
				out object newEntityDTO);

			if (success)
				entityPrototypeDTO = (EntityPrototypeDTO)newEntityDTO;

			//FOR SOME REASON THIS ONE FAILS TO POPULATE
			/*
            bool success = jsonSerializer.Deserialize<EntityDTO>(
                stringArgument,
                out EntityDTO newEntityDTO);
            
            if (success)
                entityDTO = newEntityDTO;
            */
		}

		private void OnComponentTypeToAddSelected(Type componentType)
		{
			object[] newComponentsArray = new object[entityPrototypeDTO.Components.Length + 1];

			Array.Copy(
				entityPrototypeDTO.Components,
				0,
				newComponentsArray,
				0,
				entityPrototypeDTO.Components.Length);

			newComponentsArray[newComponentsArray.Length - 1] = Activator.CreateInstance(
				componentType);

			entityPrototypeDTO.Components = newComponentsArray;

			Serialize();
		}

		private void RemoveComponent(int index)
		{
			object[] newComponentsArray = new object[entityPrototypeDTO.Components.Length - 1];

			Array.Copy(
				entityPrototypeDTO.Components,
				0,
				newComponentsArray,
				0,
				index);

			Array.Copy(
				entityPrototypeDTO.Components,
				index + 1,
				newComponentsArray,
				index,
				entityPrototypeDTO.Components.Length - index - 1);

			entityPrototypeDTO.Components = newComponentsArray;

			Serialize();
		}

		private void MoveComponentUp(int index)
		{
			if (index == 0)
				return;

			object temp = entityPrototypeDTO.Components[index];

			entityPrototypeDTO.Components[index] = entityPrototypeDTO.Components[index - 1];

			entityPrototypeDTO.Components[index - 1] = temp;

			Serialize();
		}

		private void MoveComponentDown(int index)
		{
			if (index == entityPrototypeDTO.Components.Length - 1)
				return;

			object temp = entityPrototypeDTO.Components[index];

			entityPrototypeDTO.Components[index] = entityPrototypeDTO.Components[index + 1];

			entityPrototypeDTO.Components[index + 1] = temp;

			Serialize();
		}
	}
}