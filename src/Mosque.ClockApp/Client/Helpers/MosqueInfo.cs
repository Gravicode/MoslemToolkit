using Mosque.ClockApp.Client.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mosque.ClockApp.Client.Helpers
{
    public class MosqueInfo
    {
        HttpClient client;

        public MosqueInfo()
        {
            client = new HttpClient();
        }

        public async Task<List<MosqueAlarmPrimitive>> GetAlarmData()
        {
            try
            {
                #region alarm
                var dataalarm = await client.GetStringAsync(AppConstant.AlarmApiUrl);
                var obj = JsonConvert.DeserializeObject<List<MosqueAlarmPrimitive>>(dataalarm);
                return obj;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return default;
        }

        public async Task<InfoDetailMasjid> GetInfo(string Kode)
        {
            try
            {
                var url = AppConstant.ApiUrl + Kode;
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<InfoDetailMasjid>(data);
                //get prayer time
                #region api 1
                //var shalah = await GetPrayerTime(DateHelper.GetLocalTimeNow());
                //obj.WaktuShalats.Clear();
                //obj.WaktuShalats.Add(new WaktuShalat("Imsyak", shalah.results.datetime[0].times.Imsak));
                //obj.WaktuShalats.Add(new WaktuShalat("Shubuh", shalah.results.datetime[0].times.Fajr));
                //obj.WaktuShalats.Add(new WaktuShalat("Dzuhur", shalah.results.datetime[0].times.Dhuhr));
                //obj.WaktuShalats.Add(new WaktuShalat("Ashar", shalah.results.datetime[0].times.Asr));
                //obj.WaktuShalats.Add(new WaktuShalat("Maghrib", shalah.results.datetime[0].times.Maghrib));
                //obj.WaktuShalats.Add(new WaktuShalat("Isya", shalah.results.datetime[0].times.Isha));
                #endregion
                #region api 2
                var shalah = await GetPrayerTime2(DateHelper.GetLocalTimeNow());
                obj.WaktuShalats.Clear();
                obj.WaktuShalats.Add(new WaktuShalat("Imsyak", shalah.data.jadwal.imsak));
                obj.WaktuShalats.Add(new WaktuShalat("Shubuh", shalah.data.jadwal.subuh));
                obj.WaktuShalats.Add(new WaktuShalat("Terbit", shalah.data.jadwal.terbit));
                //obj.WaktuShalats.Add(new WaktuShalat("Shubuh", "07:09"));
                obj.WaktuShalats.Add(new WaktuShalat("Dzuhur", shalah.data.jadwal.dzuhur));
                obj.WaktuShalats.Add(new WaktuShalat("Ashar", shalah.data.jadwal.ashar));
                obj.WaktuShalats.Add(new WaktuShalat("Maghrib", shalah.data.jadwal.maghrib));
                obj.WaktuShalats.Add(new WaktuShalat("Isya", shalah.data.jadwal.isya));

                #endregion
                //get weather
                var weather = await GetWeather();

               
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return default;            
        }

        async Task<ShalatObj> GetPrayerTime(DateTime tanggal, string Kota="Jakarta")
        {
            try
            {

                var url = AppConstant.ShalatApiUrl.Replace("[kota]", Kota).Replace("[tanggal]", tanggal.ToString("MM/dd/yyyy"));
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<ShalatObj>(data);
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return default;
           
        } 
        
        async Task<ShalatInfo> GetPrayerTime2(DateTime tanggal, string Kota="Jakarta")
        {
            try
            {
                var url = AppConstant.KodeKotaApiUrl.Replace("[kota]", Kota);
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<KotaInfo>(data);
                if (obj != null)
                {
                    var kode_kota = obj.data.FirstOrDefault().id;

                    var url2 = AppConstant.ShalatApiUrl2.Replace("[kode_kota]", kode_kota).Replace("[tanggal]", tanggal.ToString("yyyy/MM/dd"));
                    var data2 = await client.GetStringAsync(url2);
                    var obj2 = JsonConvert.DeserializeObject<ShalatInfo>(data2);
                    return obj2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return default;
           
        } 
        async Task<WeatherObj> GetWeather(string Kota="Jakarta")
        {
            try
            {
                var url = AppConstant.WeatherApiUrl.Replace("[kota]", Kota).Replace("[key]", AppConstant.WeatherApiKey);
                var data = await client.GetStringAsync(url);
                var obj = JsonConvert.DeserializeObject<WeatherObj>(data);
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return default;
           
        }
    }
}
