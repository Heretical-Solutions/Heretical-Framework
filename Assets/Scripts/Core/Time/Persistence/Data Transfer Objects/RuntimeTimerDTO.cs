//Uncomment usings for Protobuf and CSV support
//using CsvHelper.Configuration.Attributes;
//using ProtoBuf;

namespace HereticalSolutions.Time
{
    [System.Serializable]
    //Uncomment attributes for Protobuf and CSV support
    //[Delimiter(",")]
    //[ProtoContract]
    public class RuntimeTimerDTO
    {
        //[Name("ID")]
        //[ProtoMember(1)]
        public string ID { get; set; }

        //[Name("State")]
        //[ProtoMember(2)]
        public ETimerState State { get; set; }

        //[Name("CurrentTimeElapsed")]
        //[ProtoMember(3)]
        public float CurrentTimeElapsed { get; set; }
        
        //[Name("Accumulate")]
        //[ProtoMember(4)]
        public bool Accumulate { get; set; }
        
        //[Name("Repeat")]
        //[ProtoMember(5)]
        public bool Repeat { get; set; }
        
        //[Name("CurrentDuration")]
        //[ProtoMember(6)]
        public float CurrentDuration { get; set; }
        
        //[Name("DefaultDuration")]
        //[ProtoMember(7)]
        public float DefaultDuration { get; set; }
    }
}