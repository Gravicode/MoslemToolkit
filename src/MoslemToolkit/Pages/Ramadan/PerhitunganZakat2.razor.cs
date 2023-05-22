using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MoslemToolkit.Models;
using MoslemToolkit.Data;
using Newtonsoft.Json;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Util;

namespace MoslemToolkit.Pages.Ramadan
{
    public partial class PerhitunganZakat2 : ComponentBase
    {
        ZakatFitrahParameter param;
        ReportZakatService Reportservice;
        string PesanStatus;
        int Progress = 0;

        int Tahun;
        MoslemToolkit.Data.MuzakiService service;
        MoslemToolkit.Data.MustahikService MustahikService;
        MoslemToolkit.Data.StateStorageService StateService;
        MoslemToolkit.Data.HargaBerasZakatService HargaService;
        List<Muzaki> DataMuzaki;
        List<Muzaki> MuzakiBeras;
        List<Muzaki> MuzakiUang;
        List<Muzaki> MuzakiBelum;
        List<Mustahik> DataMustahik;
        List<PembagianZakat> Pembagian;
        List<PembagianIteration> PembagianDetail;

        List<StatistikZakat> MuzakiStat;
        List<StatistikZakat> MustahikStat;
        private PieConfig _configMuzaki;
        private PieConfig _configMustahik;
        List<(string Nama, long Jumlah, double Persen)> Stats;
        int TotalIteration;
        //List<PembagianZakat> DetailAsnab;
        long HargaBeras, HargaJualBeras;
        double TotalBeras;
        double TotalDalamBeras;
        double TotalUang;
        double Total;//, SisaUang, SisaBeras, TotalBerasSelain_SB_DA_DS;
        double SubTotal;
        void Refresh()
        {
            RefreshData();

            Kalkulasi();

            DrawChart();
        }

        void DrawChart()
        {
            if (_configMustahik != null && _configMuzaki != null)
            {

                _configMustahik = UpdateChartConfig(_configMustahik, MustahikStat);
                _configMuzaki = UpdateChartConfig(_configMuzaki, MuzakiStat);
            }
            else
            {
                _configMustahik = GetChartConfig("Mustahik", MustahikStat);
                _configMuzaki = GetChartConfig("Muzaki", MuzakiStat);
            }
        }

        PieConfig GetChartConfig(string Title,List<StatistikZakat> Data)
        {
            var config = new PieConfig
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = Title
                    }
                }
            };

            foreach (var item in Data)
            {
                config.Data.Labels.Add(item.Nama);
            }
            Random rnd = new Random(Environment.TickCount);
            var backgroundColours = new List<string>();
            foreach(var x in Enumerable.Range(1, Data.Count))
            {
                backgroundColours.Add(ColorUtil.ColorHexString((byte)rnd.Next(0,255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
            }
            PieDataset<int> dataset = new PieDataset<int>(Data.Select(x=>x.Total).ToArray())
            {
                BackgroundColor = backgroundColours.ToArray()
            };
            config.Data.Datasets.Add(dataset);
            return config;
        }

        PieConfig UpdateChartConfig(PieConfig config, List<StatistikZakat> Data)
        {
            config.Data.Labels.Clear();

            foreach (var item in Data)
            {
                config.Data.Labels.Add(item.Nama);
            }
            Random rnd = new Random(Environment.TickCount);
            var backgroundColours = new List<string>();
            foreach (var x in Enumerable.Range(1, Data.Count))
            {
                backgroundColours.Add(ColorUtil.ColorHexString((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
            }
            PieDataset<int> dataset = new PieDataset<int>(Data.Select(x => x.Total).ToArray())
            {
                BackgroundColor = backgroundColours.ToArray()
            };

            config.Data.Datasets.Clear();
            config.Data.Datasets.Add(dataset);
            return config;
        }

        void HitungTotalPembagianDariIterasi()
        {
            if(Pembagian!=null && PembagianDetail != null)
            {
                var Sisa = TotalDalamBeras;
                var currentTotal = TotalBeras;
                var totalSisa = 0d;
                var totalUang = 0d;
                //re-kalkulasi total jual per iterasi
                var totalDijual = 0d;
                var sisaSebelumnya = 0d;
                var totalSisaSebelumnya = 0d;
                int count = 0;
                int row = 0;
                var MuzakiCounter = 0;
                var untukDijual = TotalDalamBeras - TotalBeras;
                foreach (var item in PembagianDetail)
                {
                    totalSisa = 0d;
                    totalUang = 0d;
                    totalDijual = 0d;
                    totalSisaSebelumnya = 0d;
                    row = 0;
                    item.TotalPembagian = currentTotal;
                    Sisa -= currentTotal;
                    foreach (var detail in item.Detail)
                    {
                        //re-kalkulasi berdasarkan total penjualan sebelumnya
                        detail.Terima = Pembagian[row].Persen * item.TotalPembagian;

                        sisaSebelumnya = count<=0 ? 0 : PembagianDetail[item.No - 2].Detail[row].Sisa;
                        totalDijual += detail.Dijual;
                        detail.Sisa = (detail.Terima+sisaSebelumnya) - detail.Dijual;
                        detail.Uang = detail.Dijual * HargaBeras;
                        detail.SisaSebelumnya = sisaSebelumnya;
                        totalSisa += detail.Sisa;
                        totalUang += detail.Uang;
                        totalSisaSebelumnya += sisaSebelumnya;
                        untukDijual -= detail.Dijual;
                        //urut pembeli
                        if (detail.DibeliOleh == null) detail.DibeliOleh = new List<Muzaki>();
                        detail.DibeliOleh.Clear();
                        if (detail.Dijual > 0)
                        {
                            int To = MuzakiCounter + (int)detail.Dijual;
                            for(int i = MuzakiCounter; i < To; i++)
                            {
                                if (i < MuzakiUang.Count)
                                {
                                    detail.DibeliOleh.Add(MuzakiUang[i]);
                                }
                            }
                            MuzakiCounter = To;
                        }

                        row++;
                    }
                    item.TotalBerasSisa = totalSisa;
                    item.TotalUang = totalUang;
                    item.TotalBerasDijual = totalDijual;
                    item.TotalBerasSisaSebelumnya = totalSisaSebelumnya;
                    currentTotal = totalDijual;
                    count++;
                }
                //jika current total (!=0) masih sisa, maka perlu menambah iterasi lagi
                var totalDiterima = 0d;
                
                if (currentTotal > 0)
                {
                    while (Sisa > 0)
                    {
                        Sisa -= currentTotal;
                        TotalIteration++;
                        totalDijual = 0d;
                        totalDiterima = 0d;
                        totalSisa = 0d;
                        totalUang = 0d;
                        totalSisaSebelumnya = 0d;
                        var iteration = new PembagianIteration();
                        iteration.No = TotalIteration;
                        iteration.TotalPembagian = currentTotal;
                        if (currentTotal <= 0) break;
                        row = 0;
                        foreach (var item in Pembagian)
                        {
                            //ambil sisa iterasi sebelumnya
                            sisaSebelumnya = TotalIteration <= 1 ? 0 : PembagianDetail[TotalIteration - 2].Detail[row].Sisa;
                            var terima = currentTotal * item.Persen;
                            var dijual = Math.Floor(terima + sisaSebelumnya);
                            if (untukDijual - dijual < 0)
                            {
                                dijual = untukDijual;
                            }

                            untukDijual -= dijual;

                            var detail = new DetailIteration()
                            {
                                Nama = item.Nama,
                                Terima = terima,
                                Dijual = dijual,
                                Sisa = (terima + sisaSebelumnya) - dijual,
                                Uang = dijual * HargaBeras,
                                SisaSebelumnya = sisaSebelumnya

                            };
                            totalSisaSebelumnya += sisaSebelumnya;
                            //urut pembeli
                            if (detail.DibeliOleh == null) detail.DibeliOleh = new List<Muzaki>();
                            detail.DibeliOleh.Clear();
                            if (detail.Dijual > 0)
                            {
                                int To = MuzakiCounter + (int)detail.Dijual;
                                for (int i = MuzakiCounter; i < To; i++)
                                {
                                    if (i < MuzakiUang.Count)
                                    {
                                        detail.DibeliOleh.Add(MuzakiUang[i]);
                                    }
                                }
                                MuzakiCounter = To;
                            }
                            
                            iteration.Detail.Add(detail);
                            totalDijual += dijual;
                            totalDiterima += terima;
                            totalUang += dijual * HargaBeras;
                            totalSisa += (terima + sisaSebelumnya) - dijual;
                            row++;
                        }
                        iteration.TotalBerasDijual = totalDijual;
                        iteration.TotalBerasSisa = totalSisa;
                        iteration.TotalUang = totalUang;
                        iteration.TotalBerasSisaSebelumnya = totalSisaSebelumnya;
                        PembagianDetail.Add(iteration);

                        currentTotal = totalDijual;
                    }
                }

                totalSisa = 0d;
                totalUang = 0d;
                //apply sum iterasi pembagian ke pembagian
                for (var i = 0; i < Pembagian.Count; i++)
                {
                    //totalSisa = 0d;
                    totalUang = 0d;   
                    var current = Pembagian[i];
                    foreach(var item in PembagianDetail)
                    {
                        //totalSisa += item.Detail[i].Sisa;
                        totalUang += item.Detail[i].Uang;
                    }
                    totalSisa = PembagianDetail[PembagianDetail.Count - 1].Detail[i].Sisa;
                    current.Uang = totalUang;
                    current.Beras = totalSisa;
                }
                //validasi jumlah penjualan
                untukDijual = TotalDalamBeras - TotalBeras;
                totalDijual = 0d;
                foreach(var item in PembagianDetail)
                {
                    totalDijual += item.TotalBerasDijual;
                }
                if (totalDijual > untukDijual)
                {
                    toastService.ShowWarning("Jumlah beras yang dijual melebihi dana yang dititip oleh jamaah.");
                }
                else if(totalDijual < untukDijual)
                {
                    toastService.ShowWarning("Titipan uang jamaah ada yang belum dibelikan beras.");
                }
            }
        }

        async Task SimpanData()
        {
            try
            {
            //simpan data mustahik
            foreach(var item in Pembagian)
            {
                if (item.IdMustahik != -1) {
                    var node = MustahikService.GetDataById(item.IdMustahik);
                    node.Uang = item.Uang;
                    node.Beras = (int)item.Beras;
                    MustahikService.UpdateData(node);
                }
               
            }
                foreach (var item in DataMuzaki)
                {
                    service.UpdateData(item);
                }
                toastService.ShowInfo("data tersimpan.");
            }
            catch (Exception)
            {
                toastService.ShowError("sync failed.");
            }

        }
        void Kalkulasi()
        {
            Stats = service.GetStats(Tahun);
            HargaBeras = HargaService.GetHargaBeras(Tahun);
            HargaJualBeras = HargaBeras;
            if (DataMuzaki != null)
            {
                TotalBeras = DataMuzaki.Where(x => x.Tahun == Tahun && x.ZakatBeras > 0).Sum(x => x.ZakatBeras);
                TotalUang = DataMuzaki.Where(x => x.Tahun == Tahun && x.TitipZakat > 0).Sum(x => x.TitipZakat);
                TotalDalamBeras = TotalBeras + (TotalUang / HargaBeras);
                //stat muzaki
                var tempSimp = (from x in DataMuzaki
                                where x.KK.StartsWith("ZSIMP")
                                select x.Id).Count();
                MuzakiStat = MuzakiStat == null ? new List<StatistikZakat>() : MuzakiStat;
                MuzakiStat.Clear();
                MuzakiStat.Add(new StatistikZakat() { Nama = "Jamaah", Total = DataMuzaki.Count - tempSimp, Prosentase = (double)(DataMuzaki.Count - tempSimp) / DataMuzaki.Count });
                MuzakiStat.Add(new StatistikZakat() { Nama = "Simpatisan", Total = tempSimp, Prosentase = (double)tempSimp / DataMuzaki.Count });
                var tipeAsnabs = (from x in DataMustahik
                                  select x.TipeAsnab).Distinct().ToArray();
                //stat mustahik
                MustahikStat = MustahikStat == null ? new List<StatistikZakat>() : MustahikStat;
                MustahikStat.Clear();
                var jmlMustahik = DataMustahik.Sum(x => x.Jumlah);
                foreach (var asnab in tipeAsnabs)
                {
                    var tempMustahik = (from x in DataMustahik
                                    where x.TipeAsnab == asnab
                                    select x.Jumlah).Sum();

                    MustahikStat.Add(new StatistikZakat() { Nama = asnab.ToString(), Total = tempMustahik, Prosentase = (double)tempMustahik / jmlMustahik });

                }

                //hitung tabel pembagian
                Pembagian = new List<PembagianZakat>();

                AppConstants.PembagianDefault.ForEach(x =>
                {
                    if (!x.IsAsnab)
                    {
                        var jenis = x.IsAsnab ? "Aznab" : "Amil";
                        Pembagian.Add(new PembagianZakat()
                        {
                            IdMustahik=-1,
                            Nama = x.Nama,
                            Jenis = jenis,
                            Persen = x.Persen,
                            Beras = 0,
                            Uang = 0,
                            KonversiUangKeBeras = 0,
                            TotalDalamBeras = 0,
                            BerasPembulatan = 0,
                            UangPembulatan = 0
                        });
                    }
                });
                //DetailAsnab = new List<PembagianZakat>();
                var PctAsnab = AppConstants.PembagianDefault.Where(x => x.IsAsnab).FirstOrDefault().Persen;
                var TotalPengaliAsnab = DataMustahik.Sum(x => (x.Jumlah * AppConstants.AsnabDefault[x.TipeAsnab].FaktorPengali));
                DataMustahik.ForEach(x =>
                {
                    var pct = ((AppConstants.AsnabDefault[x.TipeAsnab].FaktorPengali * x.Jumlah) / TotalPengaliAsnab) * PctAsnab;
                    Pembagian.Add(new PembagianZakat()
                    {
                        IdMustahik = x.Id,
                        Nama = x.Nama,
                        Jenis="Aznab",
                        Persen = pct,
                        Beras = 0,
                        Uang = 0,
                        KonversiUangKeBeras = 0,
                        TotalDalamBeras = 0,
                        BerasPembulatan = 0,
                        UangPembulatan = 0
                    });
                });
                var totalDijual = 0d;
                var totalDiterima = 0d;
                var totalSisa = 0d;
                var totalUang = 0d;
                TotalIteration = 0;
                PembagianDetail = new List<PembagianIteration>();
                var currentTotal = TotalBeras;
                var Sisa = TotalDalamBeras;
                var untukDijual = TotalDalamBeras - TotalBeras;
                var muzakkis = DataMuzaki;
                int row = 0;
                if (TotalBeras <= 0)
                {
                    toastService.ShowError("Jika semua jamaah menitipkan uang tidak dapat menggunakan form ini untuk pembagian zakat, gunakan form 1.");
                    return;
                }

                while (Sisa > 0)
                {
                    Sisa -= currentTotal;
                    TotalIteration++;
                    totalDijual = 0d;
                    totalDiterima = 0d;
                    totalSisa = 0d;
                    totalUang = 0d;
                    var iteration = new PembagianIteration();
                    iteration.No = TotalIteration;
                    iteration.TotalPembagian = currentTotal;
                    if (currentTotal <= 0) break;
                    row = 0;
                    foreach(var item in Pembagian)
                    {
                        //ambil sisa iterasi sebelumnya
                        var sisaSebelumnya = TotalIteration <= 1 ? 0 : PembagianDetail[TotalIteration - 2].Detail[row].Sisa;
                        var terima = currentTotal * item.Persen;
                        var dijual = Math.Floor(terima+sisaSebelumnya);
                        if (untukDijual - dijual < 0)
                        {
                            dijual = untukDijual;
                        }

                        untukDijual -= dijual;
                        
                        var detail = new DetailIteration()
                        {
                            Nama = item.Nama,
                            Terima = terima,
                            Dijual = dijual,
                            Sisa = (terima+sisaSebelumnya) - dijual,
                            Uang = dijual * HargaBeras,
                            SisaSebelumnya=sisaSebelumnya
                            
                        };
                        iteration.Detail.Add(detail);
                        totalDijual += dijual;
                        totalDiterima += terima;
                        totalUang += dijual * HargaBeras;
                        totalSisa += (terima + sisaSebelumnya) - dijual;
                        row++;
                    }
                    iteration.TotalBerasDijual = totalDijual;
                    iteration.TotalBerasSisa = totalSisa;
                    iteration.TotalUang = totalUang;
                    /*
                    if (TotalIteration == 1)//pembagian beras
                    {
                        iteration.Orang = (from x in muzakkis
                                           where x.ZakatBeras > 0
                                           orderby x.KK, x.Nama
                                           select x).ToList();
                        foreach (var org in iteration.Orang)
                        {
                            muzakkis.Remove(org);
                        }
                    }
                    else//pembagian uang jadi beras
                    {
                        iteration.Orang = (from x in muzakkis
                                           where x.TitipZakat > 0
                                           orderby x.KK,x.Nama
                                           select x).Take((int)currentTotal).ToList();
                        foreach (var org in iteration.Orang)
                        {
                            muzakkis.Remove(org);
                        }

                    }
                    */
                    PembagianDetail.Add(iteration);
                   
                    currentTotal = totalDijual;
                }
                HitungTotalPembagianDariIterasi();
            }
        }
        void ClearPenjualan()
        {
            if (PembagianDetail != null)
            {
                foreach(var iterasi in PembagianDetail)
                {
                    foreach(var item in iterasi.Detail)
                    {
                        item.Dijual = 0;
                    }
                }

                HitungTotalPembagianDariIterasi();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            if (service == null) service = new MuzakiService();
            if (MustahikService == null) MustahikService = new MustahikService();
            if (StateService == null) StateService = new StateStorageService();
            if (HargaService == null) HargaService = new HargaBerasZakatService();
            Tahun = DateTime.Now.Year;
            Refresh();
            //report
            param = new ZakatFitrahParameter();
            var Today = DateHelper.GetLocalTimeNow();
            if (Reportservice == null) Reportservice = new  ReportZakatService();
            Reportservice.StatusChanged += async (object sender, ReportZakatService.StatusChangedEventArgs e) =>
            {
                var msg = $"{e.Progress}% - {e.Message}";
                PesanStatus = msg;
                Progress = e.Progress;
                await InvokeAsync(StateHasChanged);
                //toastService.ShowInfo(msg);

            };
        }
        async Task CaptureScreen()
        {
            await jsRuntime.InvokeAsync<object>("HtmlToImage", new[] { "canvas1" });
        }
        async Task LoadState()
        {
            try
            {
                var keyname = $"zakatfitrah-{Tahun}";
                var keyval = StateService.GetDataByKey(keyname);
                if (keyval != null)
                {
                    var data = JsonConvert.DeserializeObject< PerhitunganZakatState>(keyval.ValueString);
                    if (data != null)
                    {
                        PembagianDetail = data.PembagianDetail;
                        HargaJualBeras = data.HargaJualBeras;
                        HitungTotalPembagianDariIterasi();
                        toastService.ShowInfo("load state berhasil");
                        return;
                    }
                }
                toastService.ShowInfo("load state gagal, data tidak ada.");


            }
            catch (Exception)
            {
                toastService.ShowError("save state gagal.");
            }
        }
        async Task SaveState()
        {
            try
            {
                var state = new PerhitunganZakatState();
                state.PembagianDetail = PembagianDetail;
                state.HargaJualBeras = HargaJualBeras;
                var json = JsonConvert.SerializeObject(state);
                var keyval = new StateStorage();
                keyval.NameKey = $"zakatfitrah-{Tahun}";
                keyval.ValueString = json;
                var hasil = StateService.SetData(keyval);
                if(hasil)
                    toastService.ShowInfo("state tersimpan");

            }
            catch (Exception)
            {
                toastService.ShowError("save state gagal.");
            }
        }
        void RefreshData()
        {
            DataMuzaki = service.GetAllData(Tahun);
            MuzakiBeras = (from x in DataMuzaki
                               where x.ZakatBeras > 0
                               orderby x.KK, x.Nama
                               select x).ToList();
            MuzakiUang = (from x in DataMuzaki
                           where x.TitipZakat > 0
                           orderby x.KK, x.Nama
                           select x).ToList();
            
            MuzakiBelum = service.GetMuzakiBelumSetor(Tahun).OrderBy(x=> x.KK).ThenBy(x => x.Nama).ToList();

            DataMustahik = MustahikService.GetAllData(Tahun);
        }

        async Task CetakReport()
        {
            try
            {
                param.Tahun = Tahun;
                param.Desa = AppConstants.NAMA_DESA;
                param.Kelompok = AppConstants.NAMA_KELOMPOK;
                param.Total = Total;
                param.SubTotal = SubTotal;
                param.HargaBeras = HargaBeras;
                param.HargaJualBeras = HargaJualBeras;
                param.TotalBeras = TotalBeras;
                param.TotalDalamBeras = TotalDalamBeras;
                param.TotalIteration = TotalIteration;
                param.TotalUang = TotalUang;
                param.Stats = Stats;
                param.MuzakiStat = MuzakiStat;
                param.MustahikStat = MustahikStat;
                param.Pembagian = Pembagian;
                param.PembagianDetail = PembagianDetail; 
                param.MuzakiBelum = MuzakiBelum;
                param.MuzakiUang = MuzakiUang;
                param.MuzakiBeras = MuzakiBeras;
               
                var bytes = await Reportservice.GenerateReport(param);
                await FileUtil.SaveAs(jsRuntime, "Report.pdf", bytes);
            }
            catch (Exception ex)
            {
                toastService.ShowError(ex.Message);
            }

        }
    }

    
}
