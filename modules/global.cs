namespace Net_2kBot.Modules
{
    public static class global
    {
        public static long last_call;
        public static long time_now;
        public static int cd = 40;
        public static string[]? ops;
        public static string[]? blocklist;
        public static string path = Directory.GetCurrentDirectory();
        public static string[] ignores =
                {
                    "748029973",
                    "2265453790",
                    "2286003479",
                    "3594648576",
                    "3573523379",
                    "1351158016",
                    "3604629098"
                };
        public static string api_key = "";
        public static string qq = "";
        public static string verify_key = "";
    };
}
