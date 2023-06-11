using System;

//Uncomment usings for Protobuf and CSV support
//using CsvHelper.Configuration.Attributes;
//using ProtoBuf;

namespace HereticalSolutions.Time
{
    [System.Serializable]
    //Uncomment attributes for Protobuf and CSV support
    //[Delimiter(",")]
    //[ProtoContract]
    public class PersistentTimerDTO
    {
        //[Name("ID")]
        //[ProtoMember(1)]
        public string ID { get; set; }

        //[Name("State")]
        //[ProtoMember(2)]
        public ETimerState State { get; set; }

        //[Name("StartTime")]
        //[ProtoMember(3)]
        public DateTime StartTime { get; set; }

        //[Name("EstimatedFinishTime")]
        //[ProtoMember(4)]
        public DateTime EstimatedFinishTime { get; set; }

        //[Name("SavedProgress")]
        //[ProtoMember(5)]
        public TimeSpan SavedProgress { get; set; }
        
        //[Name("Accumulate")]
        //[ProtoMember(6)]
        public bool Accumulate { get; set; }
        
        //[Name("Repeat")]
        //[ProtoMember(7)]
        public bool Repeat { get; set; }
        
        //[Name("CurrentDurationSpan")]
        //[ProtoMember(8)]
        public TimeSpan CurrentDurationSpan { get; set; }
        
        //[Name("DefaultDurationSpan")]
        //[ProtoMember(9)]
        public TimeSpan DefaultDurationSpan { get; set; }
    }
}