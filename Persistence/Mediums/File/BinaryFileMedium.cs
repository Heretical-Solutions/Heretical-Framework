using System;
using System.Threading.Tasks;
using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationMedium]
	public class BinaryFileMedium
		: ISerializationMedium,
		  IMediumWithTypeFilter,
		  IHasIODestination,
		  IAsyncSerializationMedium
	{
		private readonly ILogger logger;

		public string FullPath { get; private set; }

		public BinaryFileMedium(
			string fullPath,
			ILogger logger)
		{
			FullPath = fullPath;

			this.logger = logger;
		}

		#region ISerializationMedium

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] result = null;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				value = result.CastFromTo<byte[], TValue>();

				return false;
			}

			result = File.ReadAllBytes(savePath);

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			byte[] result = null;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				value = result.CastFromTo<byte[], object>();

				return false;
			}

			result = File.ReadAllBytes(savePath);

			value = result.CastFromTo<byte[], object>();

			return true;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.WriteAllBytes(savePath, contents);

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<object, byte[]>();

			File.WriteAllBytes(savePath, contents);

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.AppendAllBytes(
				savePath,
				contents);

			return true;
#endif
		}

		public bool Append(
			Type valueType,
			object value)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.AppendAllBytes(
				savePath,
				contents);

			return true;
#endif
		}

		#endregion

		#endregion

		#region IMediumWithTypeFilter

		public bool AllowsType<TValue>()
		{
			return typeof(TValue) == typeof(byte[]);
		}

		public bool AllowsType(
			Type valueType)
		{
			return valueType == typeof(byte[]);
		}

		#endregion

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger);

			if (!IOHelpers.FileExists(
				FullPath,
				logger))
			{
				//Courtesy of https://stackoverflow.com/questions/44656364/when-writing-a-txt-file-in-unity-it-says-sharing-violation-on-path
				File
					.Create(
						FullPath)
					.Dispose();
			}
		}

		public bool IODestinationExists()
		{
			return IOHelpers.FileExists(
				FullPath,
				logger);
		}

		public void CreateIODestination()
		{
			EraseIODestination();

			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger);

			//Courtesy of https://stackoverflow.com/questions/44656364/when-writing-a-txt-file-in-unity-it-says-sharing-violation-on-path
			File
				.Create(
					FullPath)
				.Dispose();
		}

		public void EraseIODestination()
		{
			if (IOHelpers.FileExists(
				FullPath,
				logger))
			{
				File.Delete(FullPath);
			}
		}

		#endregion

		#region IAsyncSerializationMedium

		#region Read

		public async Task<(bool, TValue)> ReadAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			TValue value = default;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				return (false, value);
			}

			byte[] result = await File.ReadAllBytesAsync(
				savePath);

			value = result.CastFromTo<byte[], TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> ReadAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			object value = default;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				return (false, value);
			}

			byte[] result = await File.ReadAllBytesAsync(
				savePath);

			value = result.CastFromTo<byte[], object>();

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> WriteAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.WriteAllBytesAsync(
				savePath,
				contents);

			return true;
		}

		public async Task<bool> WriteAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<object, byte[]>();

			await File.WriteAllBytesAsync(
				savePath,
				contents);

			return true;
		}

		#endregion

		#region Append

		public async Task<bool> AppendAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertMediumIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.AppendAllBytesAsync(
				savePath,
				contents);

			return true;
#endif
		}

		public async Task<bool> AppendAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertMediumIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.AppendAllBytesAsync(
				savePath,
				contents);

			return true;
#endif
		}

		#endregion

		#endregion

		private void AssertMediumIsValid(
			Type valueType)
		{
			if (valueType != typeof(byte[]))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));
		}
	}
}