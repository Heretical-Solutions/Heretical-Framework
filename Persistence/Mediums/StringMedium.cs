using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationMedium]
    public class StringMedium
        : ISerializationMedium,
          IMediumWithTypeFilter
    {
        private readonly Func<string> valueGetter;

        private readonly Action<string> valueSetter;

        private readonly ILogger logger;

        public StringMedium(
            Func<string> valueGetter,
            Action<string> valueSetter,
            ILogger logger)
        {
            this.valueGetter = valueGetter;

            this.valueSetter = valueSetter;

            this.logger = logger;
        }

        #region ISerializationMedium

        #region Read

        public bool Read<TValue>(
            out TValue value)
        {
            AssertMediumIsValid(
               typeof(TValue));

            value = valueGetter().CastFromTo<string, TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertMediumIsValid(
               valueType);

            value = valueGetter().CastFromTo<string, object>();

            return true;
        }

        #endregion

        #region Write

        public bool Write<TValue>(
            TValue value)
        {
            AssertMediumIsValid(
               typeof(TValue));

            valueSetter(value.CastFromTo<TValue, string>());

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertMediumIsValid(
               valueType);

            valueSetter(value.CastFromTo<object, string>());

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertMediumIsValid(
                typeof(TValue));

            var result = valueGetter();

            result += value.CastFromTo<TValue, string>();

            valueSetter(result);

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertMediumIsValid(
                valueType);

            var result = valueGetter();

            result += value.CastFromTo<object, string>();

            valueSetter(result);

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