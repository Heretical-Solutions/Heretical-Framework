using System;
using System.Threading.Tasks;
using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationMedium]
    public class TextFileMedium
        : ISerializationMedium,
          IMediumWithTypeFilter,
          IHasIODestination,
          IAsyncSerializationMedium
    {
        private readonly ILogger logger;

        public string FullPath { get; private set; }

        public TextFileMedium(
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

            string result = string.Empty;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                value = result.CastFromTo<string, TValue>();

                return false;
            }

            result = File.ReadAllText(savePath);

            value = result.CastFromTo<string, TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertMediumIsValid(
                valueType);

            string savePath = FullPath;

            string result = string.Empty;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                value = result.CastFromTo<string, object>();

                return false;
            }

            result = File.ReadAllText(savePath);

            value = result.CastFromTo<string, object>();

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

            string contents = value.CastFromTo<TValue, string>();

            File.WriteAllText(savePath, contents);

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertMediumIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            File.WriteAllText(savePath, contents);

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertMediumIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            File.AppendAllText(savePath, contents);

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertMediumIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            File.AppendAllText(savePath, contents);

            return true;
        }

        #endregion

        #endregion

        #region IMediumWithTypeFilter

        public bool AllowsType<TValue>()
        {
            return typeof(TValue) == typeof(string);
        }

        public bool AllowsType(
            Type valueType)
        {
            return valueType == typeof(string);
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

            string result = await File.ReadAllTextAsync(
                savePath);

            value = result.CastFromTo<string, TValue>();

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

            string result = await File.ReadAllTextAsync(
                savePath);

            value = result.CastFromTo<string, object>();

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

            string contents = value.CastFromTo<TValue, string>();

            await File.WriteAllTextAsync(
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

            string contents = value.CastFromTo<object, string>();

            await File.WriteAllTextAsync(
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
            AssertMediumIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            await File.AppendAllTextAsync(
                savePath,
                contents);

            return true;
        }

        public async Task<bool> AppendAsync(
            Type valueType,
            object value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertMediumIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            await File.AppendAllTextAsync(
                savePath,
                contents);

            return true;
        }

        #endregion

        #endregion

        private void AssertMediumIsValid(
            Type valueType)
        {
            if (valueType != typeof(string))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: {valueType.Name}"));
        }
    }
}