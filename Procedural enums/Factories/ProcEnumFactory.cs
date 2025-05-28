using System;
using System.Collections.Generic;
using System.Reflection;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.ProceduralEnums.Factories
{
	public class ProcEnumFactory
	{
		private readonly OneToOneMapFactory oneToOneMapFactory;

		public ProcEnumFactory(
			OneToOneMapFactory oneToOneMapFactory)
		{
			this.oneToOneMapFactory = oneToOneMapFactory;
		}

		public (string enumString, TAttribute attribute)[]
			ParseAttributeInAssemblies<TAttribute>()
			where TAttribute : AProcEnumAttribute
		{
			var result = new List<(string enumString, TAttribute attribute)>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly == null)
					continue;

				foreach (var type in assembly.GetTypes())
				{
					if (type == null)
						continue;

					foreach (var field in
						type.GetFields(
							BindingFlags.Public
							| BindingFlags.NonPublic
							| BindingFlags.Static
							| BindingFlags.FlattenHierarchy))
					{
						if (field.FieldType != typeof(string) || !field.IsLiteral)
							continue;

						foreach (var attribute in
							field.GetCustomAttributes(
								typeof(TAttribute),
								true))
						{
							var enumAttribute = attribute as TAttribute;

							if (enumAttribute != null)
							{
								string resourceId = (string)field.GetValue(null);

								result.Add((resourceId, enumAttribute));
							}
						}
					}
				}
			}

			return result.ToArray();
		}

		public ProcEnum<string> BuildProcEnumFromAttribute<TAttribute>()
			where TAttribute : AProcEnumAttribute
		{
			var map = oneToOneMapFactory.BuildDictionaryOneToOneMap<int, string>();

			var attributePairs = ParseAttributeInAssemblies<TAttribute>();

			Array.Sort(
				attributePairs,
				(x, y) => x.attribute.Order.CompareTo(y.attribute.Order));

			int currentIndex = attributePairs[0].attribute.Order;

			foreach (var attributePair in attributePairs)
			{
				var enumString = attributePair.enumString;

				if (map.HasRight(enumString))
				{
					throw new Exception(
						$"Enum string {enumString} already exists in the map.");
				}

				currentIndex = Math.Max(
					currentIndex,
					attributePair.attribute.Order);

				while (map.HasLeft(currentIndex))
				{
					currentIndex++;
				}

				map.Add(
					currentIndex,
					enumString);

				currentIndex++;
			}

			return new ProcEnum<string>(
				map);
		}

		public ProcEnum<TValue> BuildProcEnum<TValue>(
			(TValue value, int order)[] values)
		{
			var map = oneToOneMapFactory.BuildDictionaryOneToOneMap<int, TValue>();

			Array.Sort(
				values,
				(x, y) => x.order.CompareTo(y.order));

			int currentIndex = values[0].order;

			foreach (var valueIndexPair in values)
			{
				var value = valueIndexPair.value;

				if (map.HasRight(value))
				{
					throw new Exception(
						$"Enum value {value} already exists in the map.");
				}

				currentIndex = Math.Max(
					currentIndex,
					valueIndexPair.order);

				while (map.HasLeft(currentIndex))
				{
					currentIndex++;
				}

				map.Add(
					currentIndex,
					value);

				currentIndex++;
			}

			return new ProcEnum<TValue>(
				map);
		}
	}
}