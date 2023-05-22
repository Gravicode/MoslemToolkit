using Microsoft.Extensions.Configuration;
using MoslemToolkit.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmsBroadcastService
{
    class Program
    {
        static string ApiUrl;
        static List<SmsBroadcastPrimitive> DataSms;
        static int Delay = 1000;
        static HttpClient client;
        static async Task Main(string[] args)
        {
            ReadConfig();
            
            if (client == null)
                client = new HttpClient();
            
            //var hasil = await SmsService.SendSms("test sms", "08174810345");

            Console.WriteLine("SMS Broadcast v1.0 is ready.");
            Thread th1 = new Thread(new ThreadStart(Loop));
            th1.Start();
            Console.WriteLine("press any key to stop..");
            Console.ReadLine();
            th1.Abort();
        }

        static async void Loop()
        {
            while (true)
            {
                Console.WriteLine($">> check point at {DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss")}");
                try
                {
                    var res = await client.GetAsync(ApiUrl);
                    if (res.IsSuccessStatusCode)
                    {
                        var json = await res.Content.ReadAsStringAsync();
                        DataSms = JsonConvert.DeserializeObject<List<SmsBroadcastPrimitive>>(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("api error:"+ex);
                }
                if (DataSms != null)
                {
                    foreach(var n in DataSms)
                    {
                        var item = SmsBroadcastPrimitive.To(n);
                        var now = DateTime.Now;
                        var waktu = now.ToString("HH:mm");
                        var selHari = new HashSet<DayOfWeek>(item.Hari);
                        var pesan = item.Pesan;
                        if (now >= item.AktifDari && now <= item.AktifSampai && waktu == $"{item.WaktuKirim.Hours.ToString("D2")}:{item.WaktuKirim.Minutes.ToString("D2")}" && selHari.Contains(now.DayOfWeek) )
                        {

                            Parallel.For(0, item.DikirimKe.Count, async(cnt) =>
                            {
                                var nohp = item.DikirimKe[cnt];
                                try
                                {
                                    var hasil = await SmsService.SendSms(pesan, nohp);
                                    if (hasil)
                                    {
                                        Console.WriteLine($"id: {item.Id}, success send sms to:{nohp}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"id: {item.Id}, fail to send sms to:{nohp}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"id: {item.Id}, fail to send sms to :{nohp}, {ex}");
                                }
                            }); // Parallel.For
                        }

                    }
                }

                Thread.Sleep(Delay);
            }
        }

        static void ReadConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("config.json", optional: false);

                IConfiguration Configuration = builder.Build();

                MailService.MailUser = Configuration["MailSettings:MailUser"];
                MailService.MailPassword = Configuration["MailSettings:MailPassword"];
                MailService.MailServer = Configuration["MailSettings:MailServer"];
                MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
                MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
                MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
                MailService.UseSendGrid = true;


                SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
                SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];
                SmsService.TokenKey = Configuration["SmsSettings:TokenKey"];
                Delay = int.Parse(Configuration["App:Delay"]);
                ApiUrl = (Configuration["App:ApiUrl"]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("read config failed:" + ex);
            }
            Console.WriteLine("Read config successfully.");
        }
    }

    public class SmsBroadcast
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string Pesan { get; set; }

        public string DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public TimeSpan WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }
    }
    public class SmsBroadcastData
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string Pesan { get; set; }
        public List<string> DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public TimeSpan WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public List<DayOfWeek> Hari { get; set; }

        public static SmsBroadcast From(SmsBroadcastData item)
        {
            var dikirim = new List<string>();
            if (item.DikirimKe != null)
            {
                dikirim = (from x in item.DikirimKe
                           select x).ToList();
            }
            var hari = new List<int>();
            if (item.Hari != null)
            {
                hari = (from x in item.Hari
                        select (int)x).ToList();
            }
            var newItem = new SmsBroadcast()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = string.Join(";", dikirim),
                Hari = string.Join(";", hari),
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = item.WaktuKirim

            };
            return newItem;
        }

        public static SmsBroadcastData To(SmsBroadcast item)
        {
            var dikirim = new List<string>();
            if (!string.IsNullOrEmpty(item.DikirimKe))
            {
                dikirim = item.DikirimKe.Split(';').ToList();
            }

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new SmsBroadcastData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = dikirim,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = item.WaktuKirim

            };
            return newItem;
        }
    }
    public class SmsBroadcastPrimitive
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string Pesan { get; set; }

        public string DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public string WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }

        public static SmsBroadcastPrimitive From(SmsBroadcast item)
        {
            if (item == null) return null;
            var newItem = new SmsBroadcastPrimitive()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = item.DikirimKe,
                Id = item.Id,
                Hari = item.Hari,
                Judul = item.Judul
                  ,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = $"{item.WaktuKirim.Hours.ToString("D2")}:{item.WaktuKirim.Minutes.ToString("D2")}"

            };
            return newItem;
        }
        public static SmsBroadcastData To(SmsBroadcastPrimitive item)
        {
            var dikirim = new List<string>();
            if (!string.IsNullOrEmpty(item.DikirimKe))
            {
                dikirim = item.DikirimKe.Split(';').ToList();
            }

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new SmsBroadcastData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = dikirim,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = new TimeSpan(int.Parse(item.WaktuKirim.Substring(0, 2)), int.Parse(item.WaktuKirim.Substring(3, 2)), 0)

            };
            return newItem;
        }
    }

}
