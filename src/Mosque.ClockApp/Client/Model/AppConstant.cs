namespace Mosque.ClockApp.Client.Model
{
    public static class AppConstant
    {
        public static string ShalatApiUrl { get; set; } = "https://api.pray.zone/v2/times/day.json?city=[kota]&date=[tanggal]";
        public static string ShalatApiUrl2 { get; set; } = "https://api.myquran.com/v1/sholat/jadwal/[kode_kota]/[tanggal]";
        public static string KodeKotaApiUrl { get; set; } = "https://api.myquran.com/v1/sholat/kota/cari/[kota]";
        public static string ApiUrl { get; set; }
        public static string ApiKey { get; set; }
        public static string WeatherApiUrl { get; set; } = "https://api.openweathermap.org/data/2.5/weather?q=[kota]&units=metric&cnt=1&APPID=[key]";
        public static string WeatherApiKey { get; set; } = "c43828b5321ba48ef18c490d402a4331";
        public static string AlarmApiUrl { get; set; } 

        public static int SyncTime { get; set; }

        public const string StorageKey = "data-masjid";


    }
}
