using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Mosque.ClockApp.Client;
using Mosque.ClockApp.Client.Shared;
using Blazored.LocalStorage;
using Blazored.Toast;
using Blazored.Toast.Services;
using Mosque.ClockApp.Client.Helpers;
using Mosque.ClockApp.Client.Model;
using Newtonsoft.Json;
using System.Globalization;

namespace Mosque.ClockApp.Client.Pages
{
    public partial class Mosque3
    {
        int EventCounter = 0;
        int Jam = 0, Menit = 0, Detik = 0;
        CultureInfo ci = new CultureInfo("id-ID");
        BlazorTimer timer;
        BlazorTimer timer2;
        MosqueInfo info;
        InfoDetailMasjid detail;
        List<MosqueAlarmPrimitive> AlarmData;
        string TanggalMasehi;
        string TanggalHijriah;
        [Parameter]
        public string Kode { set; get; } = "ALYUSRON";
        protected override void OnInitialized()
        {
            if (info == null)
            {
                info = new MosqueInfo();
            }

            if (timer == null)
            {
                timer = new BlazorTimer();
                timer.OnElapsed += TimerTick;
                timer.SetTimer(1000);
            }

            if (timer2 == null)
            {
                timer2 = new BlazorTimer();
                timer2.OnElapsed += DataUpdate;
                timer2.SetTimer(5 * 60 * 1000); //5*
            }
        }

        async void DataUpdate()
        {
            await RefreshData();
            await InvokeAsync(StateHasChanged);
        }

        async void TimerTick()
        {
            var now = DateHelper.GetLocalTimeNow();
            Jam = now.Hour;
            Menit = now.Minute;
            Detik = now.Second;
            if (detail != null && detail?.Kegiatan.Count > 0)
            {
                EventCounter++;
                if (EventCounter >= detail.Kegiatan.Count)
                    EventCounter = 0;
            }

            //alarm
            if (detail != null)
            {
                foreach (var item in detail?.WaktuShalats)
                {
                    if (item.Waktu.Hours == Jam && item.Waktu.Minutes == Menit && (now - LastPlay).TotalMinutes > 1)
                    {
                        LastPlay = now;
                        await PlaySound();
                    }
                }
            }

            //other alarm
            if (AlarmData != null)
            {
                foreach (var n in AlarmData)
                {
                    var item = MosqueAlarmPrimitive.To(n);
                    now = DateHelper.GetLocalTimeNow();
                    //var waktu = now.ToString("HH:mm");
                    var selHari = new HashSet<DayOfWeek>(item.Hari);
                    var alarmId = item.AlarmId;
                    //waktu == $"{item.Waktu.Hours.ToString("D2")}:{item.Waktu.Minutes.ToString("D2")}" 
                    if (now >= item.AktifDari && now <= item.AktifSampai && now.Hour == item.Waktu.Hours && now.Minute == item.Waktu.Minutes && selHari.Contains(now.DayOfWeek) && (now - LastPlay).TotalMinutes > 1)
                    {
                        LastPlay = now;
                        try
                        {
                            toastService.ShowInfo(item.Judul);
                            await PlayAlarm(alarmId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"id: {item.Id}, fail to alarm, {ex}");
                        }

                    }

                }
            }

            await InvokeAsync(StateHasChanged);
        //RefreshData();
        }

        DateTime LastPlay = DateTime.MinValue;
        async Task RefreshData()
        {
            var tgl = DateHelper.GetLocalTimeNow();
            var tglM = DateHelper.ConvertDateCalendar(tgl, "Hijri", "ar-sa");
            //Selasa, 14 September 2021
            TanggalMasehi = tgl.ToString("dddd, dd MMMM yyyy", ci);
            TanggalHijriah = tglM;
            //localStorage.SetItem("name", "John Smith");
            var config = string.Empty;
            try
            {
                //calling service
                detail = await info.GetInfo(Kode);
                AlarmData = await info.GetAlarmData();
                //put in local storage
                config = JsonConvert.SerializeObject(detail);
                localStorage.SetItem(AppConstant.StorageKey, config);
            }
            catch (Exception ex)
            {
                config = string.Empty;
            }

            if (string.IsNullOrEmpty(config))
            {
                try
                {
                    config = localStorage.GetItem<string>(AppConstant.StorageKey);
                    detail = JsonConvert.DeserializeObject<InfoDetailMasjid>(config);
                }
                catch (Exception ex)
                {
                    detail = null;
                }
            }

            EventCounter = 0;
            Dictionary<string, string> prayerdata = new Dictionary<string, string>();
            foreach (var item in detail.WaktuShalats)
            {
                prayerdata.Add(item.Nama, $"{String.Format("{0:00}", item.Waktu.Hours)}:{String.Format("{0:00}", item.Waktu.Minutes)}");
            }

            var imgurls = new List<string>();
            detail?.ImageUrl.ForEach(x => imgurls.Add(x.Url));
            await jsRuntime.InvokeAsync<object>("LoadPage", 3, prayerdata, imgurls, detail?.VideoUrl);
        }

        protected override async Task OnInitializedAsync()
        {
        }

        public async Task PlaySound()
        {
            await jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
        }

        public async Task PlayAlarm(string Sound)
        {
            await jsRuntime.InvokeAsync<string>("PlayAudio", Sound);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await RefreshData();
                StateHasChanged();
            }
        }
    }
}