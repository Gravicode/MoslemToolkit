using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mosque.ClockApp.Client.Model
{
    #region mosque clock
    public enum TipeCuaca { Cerah, Hujan, Berangin, Panas }
    public class KegiatanMasjid
    {
        public DateTime Tanggal { get; set; }
        public string Nama { get; set; }
    }
    public class ImageItem
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }
    public class PengumumanItem
    {
        public string Keterangan { get; set; }
    }
    public class WaktuShalat
    {
        public WaktuShalat(string Nama, string Waktu)
        {
            this.Nama = Nama;
            this.Waktu = new TimeSpan(int.Parse(Waktu.Split(':')[0]), int.Parse(Waktu.Split(':')[1]), 0);
        }
        public int Urutan { get; set; }
        public string Nama { get; set; }
        public TimeSpan Waktu { get; set; }
    }
    public class InfoDetailMasjid
    {
        public string Imam { get; set; }
        public string Muazin { get; set; }
        public string Khotib { get; set; }
        public List<WaktuShalat> WaktuShalats { set; get; }
        public List<KegiatanMasjid> Kegiatan { get; set; }
        public string Lokasi { get; set; }
        public TipeCuaca Cuaca { get; set; } = TipeCuaca.Cerah;

        public DateTime TanggalHariIni { get; set; }
        public string NamaMasjid { get; set; }
        public List<PengumumanItem> Pengumuman { get; set; }
        public List<ImageItem> ImageUrl { get; set; }
        public string VideoUrl { get; set; }

        public InfoDetailMasjid()
        {
            ImageUrl = new List<ImageItem>();
            Pengumuman = new List<PengumumanItem>();
            WaktuShalats = new List<WaktuShalat>();
            Kegiatan = new List<KegiatanMasjid>();
        }
    }
    #endregion
    #region salat api

    public class ShalatInfo
    {
        public bool status { get; set; }
        public DataSholat data { get; set; }
    }

    public class DataSholat
    {
        public string id { get; set; }
        public string lokasi { get; set; }
        public string daerah { get; set; }
        public Koordinat koordinat { get; set; }
        public Jadwal jadwal { get; set; }
    }

    public class Koordinat
    {
        public float lat { get; set; }
        public float lon { get; set; }
        public string lintang { get; set; }
        public string bujur { get; set; }
    }

    public class Jadwal
    {
        public string tanggal { get; set; }
        public string imsak { get; set; }
        public string subuh { get; set; }
        public string terbit { get; set; }
        public string dhuha { get; set; }
        public string dzuhur { get; set; }
        public string ashar { get; set; }
        public string maghrib { get; set; }
        public string isya { get; set; }
        public string date { get; set; }
    }

    public class KotaInfo
    {
        public bool status { get; set; }
        public DataKota[] data { get; set; }
    }

    public class DataKota
    {
        public string id { get; set; }
        public string lokasi { get; set; }
    }

    public class ShalatObj
    {
        public int code { get; set; }
        public string status { get; set; }
        public Results results { get; set; }
    }

    public class Results
    {
        public DateTimeObj[] datetime { get; set; }
        public Location location { get; set; }
        public Settings settings { get; set; }
    }

    public class Location
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float elevation { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public string timezone { get; set; }
        public float local_offset { get; set; }
    }

    public class Settings
    {
        public string timeformat { get; set; }
        public string school { get; set; }
        public string juristic { get; set; }
        public string highlat { get; set; }
        public float fajr_angle { get; set; }
        public float isha_angle { get; set; }
    }

    public class DateTimeObj
    {
        public Times times { get; set; }
        public DateObj date { get; set; }
    }

    public class Times
    {
        public string Imsak { get; set; }
        public string Sunrise { get; set; }
        public string Fajr { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Sunset { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
        public string Midnight { get; set; }
    }

    public class DateObj
    {
        public int timestamp { get; set; }
        public string gregorian { get; set; }
        public string hijri { get; set; }
    }
    #endregion
    #region weather api

    public class WeatherObj
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    #endregion

    #region alarm
    public class MosqueAlarmData
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string AlarmId { get; set; }
        public TimeSpan Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public List<DayOfWeek> Hari { get; set; }

        public static MosqueAlarm From(MosqueAlarmData item)
        {

            var hari = new List<int>();
            if (item.Hari != null)
            {
                hari = (from x in item.Hari
                        select (int)x).ToList();
            }
            var newItem = new MosqueAlarm()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,

                Hari = string.Join(";", hari),
                Id = item.Id,
                Judul = item.Judul,
                AlarmId = item.AlarmId,
                Waktu = item.Waktu

            };
            return newItem;
        }

        public static MosqueAlarmData To(MosqueAlarm item)
        {

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new MosqueAlarmData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                AlarmId = item.AlarmId,
                Waktu = item.Waktu

            };
            return newItem;
        }
    }
    public class MosqueAlarmPrimitive
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string AlarmId { get; set; }

        public string Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }

        public static MosqueAlarmPrimitive From(MosqueAlarm item)
        {
            if (item == null) return null;
            var newItem = new MosqueAlarmPrimitive()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,

                Id = item.Id,
                Hari = item.Hari,
                Judul = item.Judul
                  ,
                AlarmId = item.AlarmId,

                Waktu = $"{item.Waktu.Hours.ToString("D2")}:{item.Waktu.Minutes.ToString("D2")}"

            };
            return newItem;
        }
        public static MosqueAlarmData To(MosqueAlarmPrimitive item)
        {
            try
            {
                var hari = new List<DayOfWeek>();
                if (!string.IsNullOrEmpty(item.Hari))
                {
                    hari = (from x in item.Hari.Split(';')
                            select (DayOfWeek)Convert.ToInt32(x)).ToList();
                }
                var parsedTime = item.Waktu.Split(':');
                var newItem = new MosqueAlarmData()
                {
                    AktifDari = item.AktifDari,
                    AktifSampai = item.AktifSampai,
                    Berulang = item.Berulang,

                    Hari = hari,
                    Id = item.Id,
                    Judul = item.Judul,
                    AlarmId = item.AlarmId,
                    Waktu = new TimeSpan(int.Parse( parsedTime[0]), int.Parse(parsedTime[1]), 0)  
                };

               
                return newItem;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default;
            }
        }
    }
    public class MosqueAlarm
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string AlarmId { get; set; } = "alarm1";
        public TimeSpan Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }
    }
    #endregion
    public class Model
    {
    }
}
