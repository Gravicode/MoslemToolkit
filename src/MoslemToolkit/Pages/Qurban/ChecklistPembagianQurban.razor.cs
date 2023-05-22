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
using BlazorInputFile;
using System.Data;

namespace MoslemToolkit.Pages.Qurban
{
    public partial class ChecklistPembagianQurban : ComponentBase
    {
        ReportQurbanService Reportservice;
        string PesanStatus;
        int Progress = 0;
        
        StateStorageService StateService;
        PembagianService pembagianSvc;
        InfoQurbanService qurbanSvc;
        BLService blService;
        HewanQurbanService hewanService;
        MoslemToolkit.Data.Qurban DataList;
        List<PembagianPerKP> DataPembagianKP;
        enum TipeCheck { Siap, Diterima }
        public string keyword { get; set; }
        int Tahun;


        void Refresh()
        {
            RefreshData();

            //Kalkulasi();
        }
        static Random rnd = new Random(Environment.TickCount);


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //if (DataList != null)
            //{

            //    await jsRuntime.InvokeAsync<object>("TestDataTablesAdd", "#gridData");
            //}

        }

        async Task LoadState()
        {
            try
            {
                var keyname = $"checklist-qurban-{Tahun}";
                var keyval = StateService.GetDataByKey(keyname);
                if (keyval != null)
                {
                    var data = JsonConvert.DeserializeObject<MoslemToolkit.Data.Qurban>(keyval.ValueString);
                    if (data != null)
                    {
                        DataList = data;
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
                var state = DataList;
                var json = JsonConvert.SerializeObject(state);
                var keyval = new StateStorage();
                keyval.NameKey = $"checklist-qurban-{Tahun}";
                keyval.ValueString = json;
                var hasil = StateService.SetData(keyval);
                if (hasil)
                    toastService.ShowInfo("state tersimpan");

            }
            catch (Exception)
            {
                toastService.ShowError("save state gagal.");
            }
        }


        async void DownloadFile()
        {

            if (DataPembagianKP != null && DataPembagianKP.Count > 0)
            {
               

                var dt = new System.Data.DataTable("data");
                dt.Columns.Add("No");
                dt.Columns.Add("Nama");
                dt.Columns.Add("Golongan");
                dt.Columns.Add("Status");
                dt.Columns.Add("KP");
                dt.Columns.Add("Kantong BL");
                dt.Columns.Add("Pembagian (KG)");
                dt.Columns.Add("BL");
                dt.Columns.Add("Kaki");
                dt.Columns.Add("Kepala");
                dt.Columns.Add("Tulang");
                dt.Columns.Add("Jerohan");
                dt.Columns.Add("Apresiasi (KG)");


                var count = 0;
                foreach (var row in DataPembagianKP)
                    foreach (var item in row.Pembagian)
                    {
                        var newRow = dt.NewRow();
                        newRow[0] = ++count;
                        newRow[1] = item.Nama;
                        newRow[2] = item.Golongan;
                        newRow[3] = item.Status;
                        newRow[4] = item.KP;
                        newRow[5] = item.Kantong;
                        newRow[6] = item.Pembagian;
                        newRow[7] = item.BL;
                        newRow[8] = item.KAKI;
                        newRow[9] = item.KEPALA;
                        newRow[10] = item.TULANG;
                        newRow[11] = item.JEROHAN;
                        newRow[12] = item.APRESIASI;


                        dt.Rows.Add(newRow);
                    }
                dt.AcceptChanges();
                var ExportedFile = ExportData.ExportToExcel("Pembagian", dt);
                if (!string.IsNullOrEmpty(ExportedFile))
                {
                    var bytes = File.ReadAllBytes(ExportedFile);
                    await FileUtil.SaveAs(jsRuntime, "DataQurban.xlsx", bytes);
                }
                else
                {
                    toastService.ShowError("Tidak bisa download, terjadi kesalahan.");
                }
            }
            else
                toastService.ShowError("Tidak ada data untuk di download.");


        }
        protected override async Task OnInitializedAsync()
        {
            if (pembagianSvc == null || blService == null)
            {
                pembagianSvc = new PembagianService();
                blService = new BLService();
                hewanService = new HewanQurbanService();
                Tahun = DateTime.Now.Year;
            }
            if (StateService == null) StateService = new StateStorageService();
            if (qurbanSvc == null) qurbanSvc = new InfoQurbanService();
            if (DataList == null)
            {
                DataList = new MoslemToolkit.Data.Qurban();
            }
            RefreshData();
            if (Reportservice == null) Reportservice = new ReportQurbanService();
            Reportservice.StatusChanged += async (object sender, ReportQurbanService.StatusChangedEventArgs e) =>
            {
                var msg = $"{e.Progress}% - {e.Message}";
                PesanStatus = msg;
                Progress = e.Progress;
                await InvokeAsync(StateHasChanged);
                //toastService.ShowInfo(msg);

            };            
        }

        void RefreshData()
        {

            DataList.Pembagian = pembagianSvc.GetAllData(Tahun);
            //DataList.BL = blService.GetAllData(Tahun);
            //DataList.DataHewanQurban = hewanService.GetAllData(Tahun);
            //DataList.Info = qurbanSvc.GetData(Tahun);
            //DataList.QtySampil = DataList.DataHewanQurban?.Count(x => x.Jenis == JenisHewanQurban.Sapi);
            //calculate pembagian per kp
            if (DataPembagianKP == null) DataPembagianKP = new List<PembagianPerKP>();
            else
                DataPembagianKP.Clear();
            var bagian = DataList.Pembagian.Where(x => x.Pembagian > 0).Select(x => x.Pembagian).Distinct().OrderByDescending(x => x);
            foreach (var item in bagian)
            {
                var newItem = new PembagianPerKP();
                newItem.Berat = item;
                var daftar = (from x in DataList.Pembagian
                              where x.Pembagian == item
                              orderby x.Nama
                              select x).ToList();
                newItem.KP = daftar.FirstOrDefault().KP;
                newItem.Pembagian = daftar;
                DataPembagianKP.Add(newItem);
            }
        }
        async Task CetakReport()
        {
            try
            {
                if (DataPembagianKP == null)
                {
                    toastService.ShowError("Data pembagian kosong");
                    return;
                }

                var bytes = await Reportservice.GenerateReportPerKP(DataPembagianKP);
                await FileUtil.SaveAs(jsRuntime, "Pembagian.pdf", bytes);
            }
            catch (Exception ex)
            {
                toastService.ShowError(ex.Message);
            }

        }

        async Task CaptureScreen()
        {
            await jsRuntime.InvokeAsync<object>("HtmlToImage", new[] { "canvas1" });
        }

    }

    
}