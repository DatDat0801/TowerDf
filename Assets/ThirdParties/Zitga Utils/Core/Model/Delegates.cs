namespace ZitgaUtils
{
    public class Delegates
    {
        public delegate void OnGetLocationInfo(int logicCode, LocationInbound inbound);
        
        public delegate void OnGetEventData(int logicCode, EventTimeOutbound outbound);
    }
}
