using System;

using HereticalSolutions.Repositories;

using Newtonsoft.Json;

namespace HereticalSolutions.Persistence.Serializers
{
	public class JSONSerializer : ISerializer
	{
		/// <summary>
		/// JSON.Net serialization settings for writing
		/// </summary>
		private readonly JsonSerializerSettings writeSerializerSettings;

		/// <summary>
		/// JSON.Net serialization settings for reading
		/// </summary>
		private readonly JsonSerializerSettings readSerializerSettings;

		private readonly IReadOnlyObjectRepository strategyRepository;

		public JSONSerializer(IReadOnlyObjectRepository strategyRepository)
		{
			writeSerializerSettings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
			};
			
			writeSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

			readSerializerSettings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				MaxDepth = 10
			};

			this.strategyRepository = strategyRepository;
		}

		#region ISerializer
		
		public bool Serialize<TValue>(ISerializationArgument argument, TValue DTO)
		{
			string json = JsonConvert.SerializeObject(
				DTO,
				Formatting.Indented,
				writeSerializerSettings);
			
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[JSONSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, json);
		}

		public bool Serialize(ISerializationArgument argument, Type DTOType, object DTO)
		{
			string json = JsonConvert.SerializeObject(
				DTO,
				Formatting.Indented,
				writeSerializerSettings);
			
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[JSONSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

			return concreteStrategy.Serialize(argument, json);
		}

		public bool Deserialize<TValue>(ISerializationArgument argument, out TValue DTO)
		{
			DTO = (TValue)Activator.CreateInstance(typeof(TValue));
			
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[JSONSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(argument, out var json))
				return false;

			JsonConvert.PopulateObject(json, DTO, readSerializerSettings);

			return true;
		}

		public bool Deserialize(ISerializationArgument argument, Type DTOType, out object DTO)
		{
			DTO = Activator.CreateInstance(DTOType);
			
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[JSONSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IJsonSerializationStrategy)strategyObject;

			if (!concreteStrategy.Deserialize(argument, out var json))
				return false;

			JsonConvert.PopulateObject(json, DTO, readSerializerSettings);

			return true;
		}

		public void Erase(ISerializationArgument argument)
		{
			if (!strategyRepository.TryGet(argument.GetType(), out var strategyObject))
				throw new Exception($"[JSONSerializer] COULD NOT RESOLVE STRATEGY BY ARGUMENT: {argument.GetType().ToString()}");

			var concreteStrategy = (IJsonSerializationStrategy)strategyObject;
			
			concreteStrategy.Erase(argument);
		}
		
		#endregion
	}
}