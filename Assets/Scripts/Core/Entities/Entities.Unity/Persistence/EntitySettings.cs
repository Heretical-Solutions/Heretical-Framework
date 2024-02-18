using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.Serializers;

using HereticalSolutions.Persistence.Factories;

using UnityEngine;

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Represents the settings for an entity.
	/// </summary>
	[CreateAssetMenu(fileName = "Entity settings", menuName = "Settings/Entities/Entity settings", order = 0)]
	public class EntitySettings : ScriptableObject
	{
		private JSONSerializer jsonSerializer;

		private StringArgument stringArgument;

		/// <summary>
		/// The JSON string representation of the entity.
		/// </summary>
		public string EntityJson;

		/// <summary>
		/// The EntityPrototypeDTO object representing the entity's prototype.
		/// </summary>
		public EntityPrototypeDTO PrototypeDTO
		{
			get
			{
				if (jsonSerializer == null)
					jsonSerializer = UnityPersistenceFactory.BuildSimpleUnityJSONSerializer();

				if (stringArgument == null)
					stringArgument = new StringArgument();

				stringArgument.Value = EntityJson;

				bool success = jsonSerializer.Deserialize(
					stringArgument,
					typeof(EntityPrototypeDTO),
					out object newEntityDTO);

				return (EntityPrototypeDTO)newEntityDTO;
			}
		}
	}
}