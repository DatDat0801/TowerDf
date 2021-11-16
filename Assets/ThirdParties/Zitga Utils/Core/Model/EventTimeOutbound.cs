using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZitgaUtils
{
    public class EventTimeOutbound
    {
        [JsonProperty("0")] public List<EventTime> EventTimes { get; set; }
    }

    public class EventTime
    {
        [JsonProperty("0")] public long StartTime { get; set; }

        [JsonProperty("1")] public long EndTime { get; set; }

        [JsonProperty("2")] public long Season { get; set; }

        [JsonProperty("3")] public int EventType { get; set; }

        [JsonProperty("4")] public int DataId { get; set; }

        [JsonProperty("end")] public bool IsOpened { get; set; }
    }
}