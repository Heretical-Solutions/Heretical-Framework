#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
using Newtonsoft.Json;
#endif

#if CSV_SUPPORT
using CsvHelper.Configuration.Attributes;
#endif

#if PROTOBUF_SUPPORT
using ProtoBuf;
#endif

using System;

namespace HereticalSolutions.Synchronization.Time.Timers.TimeSpanTick
{
#region Serialization attributes        
    [Serializable]
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
    [JsonObject(MemberSerialization.OptIn)]
#endif
#if CSV_SUPPORT
    [Delimiter(HereticalSolutions.Persistence.PersistenceConsts.CSV_DELIMITER)]
#endif
#if PROTOBUF_SUPPORT
    [ProtoContract]
#endif
#endregion
    public class TimeSpanTimerDTO
    {
#region Serialization attributes     
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("ID")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(1)]
#endif
#endregion
        public string ID;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("State")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(2)]
#endif
#endregion
        public ETimerState State;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("StartTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(3)]
#endif
#endregion
        public DateTime StartTime;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("EstimatedFinishTime")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(4)]
#endif
#endregion
        public DateTime EstimatedFinishTime;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("SavedProgress")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(5)]
#endif
#endregion
        public TimeSpan SavedProgress;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Accumulate")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(6)]
#endif
#endregion
        public bool Accumulate;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("Repeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(7)]
#endif
#endregion
        public bool Repeat;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FlushTimeElapsedOnRepeat")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(8)]
#endif
#endregion
        public bool FlushTimeElapsedOnRepeat;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("FireRepeatCallbackOnFinish")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(9)]
#endif
#endregion
        public bool FireRepeatCallbackOnFinish;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("CurrentDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(10)]
#endif
#endregion
        public TimeSpan CurrentDurationSpan;

#region Serialization attributes
#if (JSON_SUPPORT && JSON_OPT_IN_SUPPORT)
        [JsonProperty]
#endif
#if CSV_SUPPORT
        [Name("DefaultDurationSpan")]
#endif
#if PROTOBUF_SUPPORT
        [ProtoMember(11)]
#endif
#endregion
        public TimeSpan DefaultDurationSpan;
    }
}