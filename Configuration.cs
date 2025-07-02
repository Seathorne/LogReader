namespace LogReader
{
    internal static class Configuration
    {
        public static TimeZoneInfo TimeZone;
        
        static Configuration()
        {
            TimeZone = TimeZoneInfo.Local;
        }
    }
}
