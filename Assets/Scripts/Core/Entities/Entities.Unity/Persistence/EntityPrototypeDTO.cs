using System;

namespace HereticalSolutions.Entities
{
	/// <summary>
	/// Represents a data transfer object for an entity prototype.
	/// </summary>
	[Serializable] 
	public struct EntityPrototypeDTO
	{
		/// <summary>
		/// Gets or sets the unique identifier of the prototype.
		/// </summary>
		public string PrototypeID;

		/// <summary>
		/// Gets or sets the array of components associated with the prototype.
		/// </summary>
		public object[] Components;
	}
}