#if CSV_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

using CsvHelper;
using CsvHelper.Configuration;

namespace HereticalSolutions.Persistence.CSV
{
    [FormatSerializer]
    public class CSVSerializer
        : ATextSerializer
    {
        private readonly bool includeHeader;

        private readonly CsvConfiguration csvConfiguration;

        public CSVSerializer(
            bool includeHeader,
            bool useFieldsInsteadOfProperties,
            ILogger logger)
            : base(
                logger)
        {
            this.includeHeader = includeHeader;

            csvConfiguration = new CsvConfiguration(
                CultureInfo.InvariantCulture)
            {
                MemberTypes = (useFieldsInsteadOfProperties)
                    ? CsvHelper.Configuration.MemberTypes.Fields
                    : CsvHelper.Configuration.MemberTypes.Properties
            };
        }

        protected override bool CanSerializeWithTextWriter => true;

        protected override bool CanDeserializeWithTextReader => true;

        protected override bool SerializeWithTextWriter<TValue>(
            TextStreamMedium textStreamMedium,
            TValue value)
        {
            var valueType = typeof(TValue);

            using (var csvWriter = new CsvWriter(
                textStreamMedium.StreamWriter,
                csvConfiguration))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader<TValue>();

                    csvWriter.NextRecord();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    csvWriter.WriteRecords((IEnumerable)value);
                }
                else
                    csvWriter.WriteRecord<TValue>(value);
            }

            return true;
        }

        protected override bool SerializeWithTextWriter(
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject)
        {
            using (var csvWriter = new CsvWriter(
                textStreamMedium.StreamWriter,
                csvConfiguration))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader(valueType);

                    csvWriter.NextRecord();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    csvWriter.WriteRecords((IEnumerable)valueObject);
                }
                else
                    csvWriter.WriteRecord(valueObject);
            }

            return true;
        }

        protected override bool DeserializeWithTextReader<TValue>(
            TextStreamMedium textStreamMedium,
            out TValue value)
        {
            value = default(TValue);

            var valueType = typeof(TValue);

            using (var csvReader = new CsvReader(
                textStreamMedium.StreamReader,
                csvConfiguration))
            {
                csvReader.Read();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    value = records.CastFromTo<IEnumerable<object>, TValue>();
                }
                else
                {
                    if (includeHeader)
                        csvReader.Read();

                    value = csvReader.GetRecord<TValue>();
                }
            }

            return true;
        }

        protected override bool DeserializeWithTextReader(
            TextStreamMedium textStreamMedium,
            Type valueType,
            out object valueObject)
        {
            valueObject = default(object);

            using (var csvReader = new CsvReader(
                textStreamMedium.StreamReader,
                csvConfiguration))
            {
                csvReader.Read();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    valueObject = records;
                }
                else
                {
                    if (includeHeader)
                        csvReader.Read();

                    valueObject = csvReader.GetRecord(valueType);
                }
            }

            return true;
        }

        protected virtual async Task<bool> SerializeWithTextWriterAsync<TValue>(
            TextStreamMedium textStreamMedium,
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var valueType = typeof(TValue);

            using (var csvWriter = new CsvWriter(
                textStreamMedium.StreamWriter,
                csvConfiguration))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader<TValue>();

                    await csvWriter.NextRecordAsync();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    await csvWriter.WriteRecordsAsync((IEnumerable)value);
                }
                else
                    csvWriter.WriteRecord<TValue>(value);
            }

            return true;
        }

        protected virtual async Task<bool> SerializeWithTextWriterAsync(
            TextStreamMedium textStreamMedium,
            Type valueType,
            object valueObject,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            using (var csvWriter = new CsvWriter(
                textStreamMedium.StreamWriter,
                csvConfiguration))
            {
                if (includeHeader)
                {
                    csvWriter.WriteHeader(valueType);

                    await csvWriter.NextRecordAsync();
                }

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    await csvWriter.WriteRecordsAsync((IEnumerable)valueObject);
                }
                else
                    csvWriter.WriteRecord(valueObject);
            }

            return true;
        }

        protected virtual async Task<(bool, TValue)> DeserializeWithTextReaderAsync<TValue>(
            TextStreamMedium textStreamMedium,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var value = default(TValue);

            var valueType = typeof(TValue);

            using (var csvReader = new CsvReader(
                textStreamMedium.StreamReader,
                csvConfiguration))
            {
                await csvReader.ReadAsync();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    value = records.CastFromTo<IEnumerable<object>, TValue>();
                }
                else
                {
                    if (includeHeader)
                        await csvReader.ReadAsync();

                    value = csvReader.GetRecord<TValue>();
                }
            }

            return (true, value);
        }

        protected virtual async Task<(bool, object)> DeserializeWithTextReaderAsync(
            TextStreamMedium textStreamMedium,
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var valueObject = default(object);

            using (var csvReader = new CsvReader(
                textStreamMedium.StreamReader,
                csvConfiguration))
            {
                await csvReader.ReadAsync();

                if (includeHeader)
                    csvReader.ReadHeader();

                if (valueType.IsTypeGenericArray()
                    || valueType.IsTypeEnumerable()
                    || valueType.IsTypeGenericEnumerable())
                {
                    var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                        ? valueType.GetGenericArrayUnderlyingType()
                        : valueType.GetGenericEnumerableUnderlyingType();

                    var records = csvReader.GetRecords(underlyingType);

                    valueObject = records;
                }
                else
                {
                    if (includeHeader)
                        await csvReader.ReadAsync();

                    valueObject = csvReader.GetRecord(valueType);
                }
            }

            return (true, valueObject);
        }

        protected override string SerializeToString<TValue>(
            TValue value)
        {
            string csv;

            var valueType = typeof(TValue);

            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(
                    stringWriter,
                    csvConfiguration))
                {
                    if (includeHeader)
                    {
                        csvWriter.WriteHeader<TValue>();

                        csvWriter.NextRecord();
                    }

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)value);
                    }
                    else
                        csvWriter.WriteRecord<TValue>(value);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        protected override string SerializeToString(
            Type valueType,
            object valueObject)
        {
            string csv;

            using (StringWriter stringWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(
                    stringWriter,
                    csvConfiguration))
                {
                    if (includeHeader)
                    {
                        csvWriter.WriteHeader(valueType);

                        csvWriter.NextRecord();
                    }

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        csvWriter.WriteRecords((IEnumerable)valueObject);
                    }
                    else
                        csvWriter.WriteRecord(valueObject);
                }

                csv = stringWriter.ToString();
            }

            return csv;
        }

        protected override bool DeserializeFromString<TValue>(
            string csv,
            out TValue value)
        {
            value = default(TValue);

            var valueType = typeof(TValue);

            using (StringReader stringReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(
                    stringReader,
                    csvConfiguration))
                {
                    csvReader.Read();

                    csvReader.ReadHeader();

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                            ? valueType.GetGenericArrayUnderlyingType()
                            : valueType.GetGenericEnumerableUnderlyingType();

                        var records = csvReader.GetRecords(underlyingType);

                        value = records.CastFromTo<IEnumerable<object>, TValue>();
                    }
                    else
                    {
                        csvReader.Read();

                        value = csvReader.GetRecord<TValue>();
                    }
                }
            }

            return true;
        }

        protected override bool DeserializeFromString(
            string csv,
            Type valueType,
            out object valueObject)
        {
            valueObject = default(object);

            using (StringReader stringReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(
                    stringReader,
                    csvConfiguration))
                {
                    csvReader.Read();

                    csvReader.ReadHeader();

                    if (valueType.IsTypeGenericArray()
                        || valueType.IsTypeEnumerable()
                        || valueType.IsTypeGenericEnumerable())
                    {
                        var underlyingType = (valueType.IsTypeGenericArray() || valueType.IsTypeEnumerable())
                            ? valueType.GetGenericArrayUnderlyingType()
                            : valueType.GetGenericEnumerableUnderlyingType();

                        var records = csvReader.GetRecords(underlyingType);

                        valueObject = records;
                    }
                    else
                    {
                        csvReader.Read();

                        valueObject = csvReader.GetRecord(valueType);
                    }
                }
            }

            return true;
        }
    }
}

#endif