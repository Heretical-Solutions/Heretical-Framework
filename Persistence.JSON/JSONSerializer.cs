#if JSON_SUPPORT

using System;
using System.Collections.Generic;

using System.Threading.Tasks;

using System.Linq;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HereticalSolutions.Persistence.JSON
{
    [FormatSerializer]
    public class JSONSerializer
        : ATextSerializer
    {
        private readonly JsonSerializerSettings writeSerializerSettings;

        private readonly JsonSerializerSettings readSerializerSettings;

        public JSONSerializer(
            ILogger logger)
            : base(
                logger)
        {
            writeSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,   //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,                               //Tell me something... Why did I comment this out before?
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };

            writeSerializerSettings.Converters.Add(
                new Newtonsoft.Json.Converters.StringEnumConverter());

            readSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,                               //Tell me something... Why did I comment this out before?
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,   //COMMENTED OUT BECAUSE THIS OPTION IS NOT PRESENT IN JSON.NET.AOT
                MaxDepth = 10,

                //Courtesy of https://stackoverflow.com/questions/39383098/ignore-missing-types-during-deserialization-of-list
                SerializationBinder = new JsonSerializationBinder(
                    new DefaultSerializationBinder()),

                Error = (sender, args) =>
                {
                    if (args.CurrentObject == args.ErrorContext.OriginalObject
                        && args.ErrorContext.Error.InnerExceptionsAndSelf().OfType<JsonSerializationBinderException>().Any()
                        && args.ErrorContext.OriginalObject.GetType().GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        logger?.LogError<JSONSerializer>(
                            $"EXCEPTION WAS THROWN DURING DESERIALIZATION: {args.ErrorContext.Error.Message}");

                        args.ErrorContext.Handled = true;
                    }
                }
            };
        }

        #region IFormatSerializer

        public override bool Populate<TValue>(
            ISerializationCommandContext context,
            TValue value)
        {
            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                value,
                readSerializerSettings);

            return true;
        }

        public override bool Populate(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject)
        {
            if (!TryDeserialize<string>(
                context,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                valueObject,
                readSerializerSettings);

            return true;
        }

        #endregion

        #region IBlockFormatSerializer

        public override bool PopulateBlock<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                value,
                readSerializerSettings);

            return result;
        }

        public override bool PopulateBlock(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize)
        {
            bool result = false;

            if (!TryDeserializeBlock<string>(
                context,
                blockOffset,
                blockSize,
                out string json))
            {
                return false;
            }

            JsonConvert.PopulateObject(
                json,
                valueObject,
                readSerializerSettings);

            return result;
        }

        #endregion

        #region IAsyncFormatSerializer

        public override async Task<bool> PopulateAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            JsonConvert.PopulateObject(
                byteArrayResult.Item2,
                value,
                readSerializerSettings);

            return result;
        }

        public override async Task<bool> PopulateAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            var byteArrayResult = await TryDeserializeAsync<string>(
                context,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            JsonConvert.PopulateObject(
                byteArrayResult.Item2,
                valueObject,
                readSerializerSettings);

            return result;
        }

        #endregion

        #region IAsyncBlockFormatSerializer

        public async Task<bool> PopulateBlockAsync<TValue>(
            ISerializationCommandContext context,
            TValue value,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                blockOffset,
                blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            JsonConvert.PopulateObject(
                byteArrayResult.Item2,
                value,
                readSerializerSettings);

            return result;
        }

        public async Task<bool> PopulateBlockAsync(
            Type valueType,
            ISerializationCommandContext context,
            object valueObject,
            int blockOffset,
            int blockSize,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            bool result = false;

            var byteArrayResult = await TryDeserializeBlockAsync<string>(
                context,
                blockOffset,
                blockSize,

                asyncContext);

            if (!byteArrayResult.Item1)
            {
                return false;
            }

            JsonConvert.PopulateObject(
                byteArrayResult.Item2,
                valueObject,
                readSerializerSettings);

            return result;
        }

        #endregion

        protected override string SerializeToString<TValue>(
            TValue value)
        {
            return JsonConvert.SerializeObject(
                value,
                Formatting.Indented,
                writeSerializerSettings);
        }

        protected override string SerializeToString(
            Type valueType,
            object valueObject)
        {
            return JsonConvert.SerializeObject(
                valueObject,
                Formatting.Indented,
                writeSerializerSettings);
        }

        protected override bool DeserializeFromString<TValue>(
            string json,
            out TValue value)
        {
            value = JsonConvert.DeserializeObject<TValue>(
                json,
                readSerializerSettings);

            return true;
        }

        protected override bool DeserializeFromString(
            string json,
            Type valueType,
            out object valueObject)
        {
            valueObject = JsonConvert.DeserializeObject(
                json,
                valueType,
                readSerializerSettings);

            return true;
        }
    }
}

#endif