namespace Net_2kBot.Modules
{
    public static class Global
    {
        public static long LastCall;
        public static long TimeNow;
        public static int Cd = 40;
        public static string[]? Ops;
        public static string[]? Blocklist;
        public static string Path = Directory.GetCurrentDirectory();
        public static string[] Ignores =
                {
                    "748029973",
                    "2265453790",
                    "2286003479",
                    "3594648576",
                    "3573523379",
                    "1351158016",
                    "3604629098"
                };
        public const string ApiKey = "";
        public static string Qq = "";
        public static string VerifyKey = "";
    };
}
