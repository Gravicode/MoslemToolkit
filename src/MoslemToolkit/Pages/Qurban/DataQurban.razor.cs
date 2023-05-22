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
using Microsoft.AspNetCore.WebUtilities;

namespace MoslemToolkit.Pages.Qurban
{
    public partial class DataQurban : ComponentBase
    {
        string PesanStatus;
        int Progress = 0;
        ReportQurbanService Reportservice;
        StateStorageService StateService;
        PembagianService pembagianSvc;
        InfoQurbanService qurbanSvc;
        BLService blService;
        HewanQurbanService hewanService;
        MoslemToolkit.Data.Qurban DataList;
        enum TipeCheck { Siap, Diterima }
        public string keyword { get; set; }
        int Tahun;
        ReportQurbanParameter param;
        async Task CetakReport()
        {
            try
            {
                param.Tahun = Tahun;
                param.Desa = AppConstants.NAMA_DESA;
                param.Kelompok = AppConstants.NAMA_KELOMPOK;
                param.DataList = DataList;

              

                var bytes = await Reportservice.GenerateReport(param);
                await FileUtil.SaveAs(jsRuntime, "Qurban.pdf", bytes);
            }
            catch (Exception ex)
            {
                toastService.ShowError(ex.Message);
            }

        }
        
        static Random rnd = new Random(Environment.TickCount);

        public enum UrutanSapis { Reset, Acak, Berat };
        void UrutSapi(UrutanSapis Mode)
        {
            if (DataList != null && DataList.DataHewanQurban != null)
            {
                switch (Mode){
                case UrutanSapis.Reset:
                        for (var i = 0; i < DataList.DataHewanQurban.Count; i++)
                        {
                            DataList.DataHewanQurban[i].NoUrutPotong = i + 1;
                        }
                        break;
                    case UrutanSapis.Acak:
                        for (var i = 0; i < DataList.DataHewanQurban.Count; i++)
                        {
                            DataList.DataHewanQurban[i].NoUrutPotong = i + 1;
                        }
                        for (var i = 0; i < DataList.DataHewanQurban.Count; i++)
                        {
                            var tukeran = rnd.Next(0, DataList.DataHewanQurban.Count - 1);
                            var temp = DataList.DataHewanQurban[i].NoUrutPotong;
                            DataList.DataHewanQurban[i].NoUrutPotong = DataList.DataHewanQurban[tukeran].NoUrutPotong;
                            DataList.DataHewanQurban[tukeran].NoUrutPotong = temp;
                        }                        
                        break;
                    case UrutanSapis.Berat:
                        var ByBerat = DataList.DataHewanQurban.OrderByDescending(x => x.Bruto).ToList();
                        var orderno = 1;
                        foreach (var item in ByBerat)
                        {
                            item.NoUrutPotong = orderno++;
                        }
                        break;
                }
            }
            else
            {
                toastService.ShowError("Upload data hewan qurban terlebih dahulu.");
            }
        }
        
       
        void ChangeDataBL(TipeCheck tipe, DataBL item, object checkedValue)
        {
            var res = (bool)checkedValue;

            if (tipe == TipeCheck.Diterima)
            {
                item.SudahDiterima = res;
            }
            else
            {
                item.SudahSiap = res;
            }
            blService.UpdateData(item);
        }
        void ChangeDataPembagian(TipeCheck tipe, DataPembagian item, object checkedValue)
        {
            var res = (bool)checkedValue;

            if (tipe == TipeCheck.Diterima)
            {
                item.SudahDiterima = res;
            }
            else
            {
                item.SudahSiap = res;
            }
            pembagianSvc.UpdateData(item);
        }
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
                var keyname = $"qurban-{Tahun}";
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
                keyval.NameKey = $"qurban-{Tahun}";
                keyval.ValueString = json;
                var hasil = StateService.SetData(keyval);
                if (hasil)
                    toastService.ShowInfo("state tersimpan");

            }
            catch (Exception ex)
            {
                toastService.ShowError($"save state gagal: {ex}");
            }
        }

        enum TipeDataQurban { Pembagian, BL, HewanQurban };
        async void DownloadFile(TipeDataQurban tipe)
        {
            if (tipe == TipeDataQurban.Pembagian)
            {
                if (DataList != null && DataList.Pembagian.Count > 0)
                {

                    var dt = new System.Data.DataTable("data");
                    dt.Columns.Add("No");
                    dt.Columns.Add("KK");
                    dt.Columns.Add("No. Urut");
                    dt.Columns.Add("Nama");
                    dt.Columns.Add("Status");
                    dt.Columns.Add("Sapi");

                    dt.Columns.Add("Golongan");
                    dt.Columns.Add("KP");
                    dt.Columns.Add("Pembagian (KG)");
                    dt.Columns.Add("BL");
                    dt.Columns.Add("Kantong BL");

                    dt.Columns.Add("Kaki");
                    dt.Columns.Add("Kepala");
                    dt.Columns.Add("Tulang");
                    dt.Columns.Add("Jerohan");
                    dt.Columns.Add("Apresiasi (KG)");
                    dt.Columns.Add("Sudah Siap");
                    dt.Columns.Add("Sudah Diterima");

                    var count = 0;
                    foreach (var item in DataList.Pembagian)
                    {
                        var newRow = dt.NewRow();
                        newRow[0] = ++count;
                        newRow[1] = item.KK;
                        newRow[2] = item.NoUrut;
                        newRow[3] = item.Nama;
                        newRow[4] = item.Status;
                        newRow[5] = item.Sapi;
                        newRow[6] = item.Golongan;
                        newRow[7] = item.KP;
                        newRow[8] = item.Pembagian;
                        newRow[9] = item.BL;
                        newRow[10] = item.Kantong;
                        newRow[11] = item.KAKI;
                        newRow[12] = item.KEPALA;
                        newRow[13] = item.TULANG;
                        newRow[14] = item.JEROHAN;
                        newRow[15] = item.APRESIASI;
                        newRow[16] = item.SudahSiap ? "Ya" : "Tidak";
                        newRow[17] = item.SudahDiterima ? "Ya" : "Tidak";


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
            else if (tipe == TipeDataQurban.BL)
            {
                if (DataList != null && DataList.BL.Count > 0)
                {

                    var dt = new System.Data.DataTable("data");
                    dt.Columns.Add("No Urut");
                    dt.Columns.Add("Nama");
                    dt.Columns.Add("Berat (Kg)");
                    dt.Columns.Add("Bungkus");
                    dt.Columns.Add("KP");
                    dt.Columns.Add("Keterangan");
                    dt.Columns.Add("Jenis");
                    dt.Columns.Add("Sudah Siap");
                    dt.Columns.Add("Sudah Diterima");

                    var count = 0;
                    foreach (var item in DataList.BL)
                    {
                        var newRow = dt.NewRow();
                        newRow[0] = ++count;

                        newRow[1] = item.Nama;
                        newRow[2] = item.BERAT;
                        newRow[3] = item.BUNGKUS;
                        newRow[4] = item.KP;
                        newRow[5] = item.KETERANGAN;
                        newRow[6] = item.Jenis.ToString();

                        newRow[7] = item.SudahSiap ? "Ya" : "Tidak";
                        newRow[8] = item.SudahDiterima ? "Ya" : "Tidak";


                        dt.Rows.Add(newRow);
                    }
                    dt.AcceptChanges();
                    var ExportedFile = ExportData.ExportToExcel("DaftarBL", dt);
                    if (!string.IsNullOrEmpty(ExportedFile))
                    {
                        var bytes = File.ReadAllBytes(ExportedFile);
                        await FileUtil.SaveAs(jsRuntime, "DataBL.xlsx", bytes);
                    }
                    else
                    {
                        toastService.ShowError("Tidak bisa download, terjadi kesalahan.");
                    }
                }
                else
                    toastService.ShowError("Tidak ada data untuk di download.");
            }
            else if (tipe == TipeDataQurban.HewanQurban)
            {
                if (DataList != null && DataList.DataHewanQurban.Count > 0)
                {

                    var dt = new System.Data.DataTable("hewan-qurban");
                    dt.Columns.Add("No");
                    dt.Columns.Add("Pemilik");
                    dt.Columns.Add("Bruto (Kg)");
                    dt.Columns.Add("Persen Nett");
                    dt.Columns.Add("Netto");
                    dt.Columns.Add("Kepala Sapi");
                    dt.Columns.Add("Jenis");

                    var count = 0;
                    foreach (var item in DataList.DataHewanQurban)
                    {
                        var newRow = dt.NewRow();
                        newRow[0] = item.No;

                        newRow[1] = item.Pemilik;
                        newRow[2] = item.Bruto;
                        newRow[3] = item.PersenNet.ToString("n2");
                        newRow[4] = item.Netto;
                        newRow[5] = item.KepalaSapi;
                        newRow[6] = item.Jenis.ToString();


                        dt.Rows.Add(newRow);
                    }
                    dt.AcceptChanges();
                    var ExportedFile = ExportData.ExportToExcel("DaftarHewanQurban", dt);
                    if (!string.IsNullOrEmpty(ExportedFile))
                    {
                        var bytes = File.ReadAllBytes(ExportedFile);
                        await FileUtil.SaveAs(jsRuntime, "DataHewanQurban.xlsx", bytes);
                    }
                    else
                    {
                        toastService.ShowError("Tidak bisa download, terjadi kesalahan.");
                    }
                }
                else
                    toastService.ShowError("Tidak ada data untuk di download.");
            }
        }
        protected override async Task OnInitializedAsync()
        {
            if (pembagianSvc == null || blService == null)
            {
                pembagianSvc = new PembagianService();
                blService = new BLService();
                hewanService = new  HewanQurbanService();
                Tahun = DateTime.Now.Year;
            }
            if (StateService == null) StateService = new StateStorageService();
            if (qurbanSvc == null) qurbanSvc = new  InfoQurbanService();
            if (Reportservice == null) Reportservice = new ReportQurbanService ();
            Reportservice.StatusChanged += async (object sender, ReportQurbanService.StatusChangedEventArgs e) =>
            {
                var msg = $"{e.Progress}% - {e.Message}";
                PesanStatus = msg;
                Progress = e.Progress;
                await InvokeAsync(StateHasChanged);
                //toastService.ShowInfo(msg);

            };
            if (DataList == null)
            {
                DataList = new MoslemToolkit.Data.Qurban();
            }
            var uri = NavMgr.ToAbsoluteUri(NavMgr.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("tahun", out var _tahun))
            {
                Tahun = Convert.ToInt32(_tahun);
            }
            RefreshData();
            param = new ReportQurbanParameter();
        }

        void RefreshData()
        {
            
                DataList.Pembagian = pembagianSvc.GetAllData(Tahun);
                DataList.BL = blService.GetAllData(Tahun);
                DataList.DataHewanQurban = hewanService.GetAllData(Tahun);
                DataList.Info = qurbanSvc.GetData(Tahun);
                DataList.QtySampil = DataList.DataHewanQurban?.Count(x => x.Jenis == JenisHewanQurban.Sapi);
            
                GenerateReport();
        }
        async Task Sync()
        {
            try
            {

                pembagianSvc.ClearData(Tahun);
                blService.ClearData(Tahun);
                hewanService.ClearData(Tahun);

                foreach (var item in DataList.DataHewanQurban)
                {
                    hewanService.InsertData(item);
                }

                foreach (var item in DataList.Pembagian)
                {
                    pembagianSvc.InsertData(item);
                }

                foreach (var item in DataList.BL)
                {
                    blService.InsertData(item);
                }
                await pembagianSvc.UpdateRef(Tahun);
                toastService.ShowInfo("Sync completed");
            }
            catch (Exception ex)
            {

                toastService.ShowError("Gagal sync:" + ex);
            }

        }

        async Task HandleFileSelected(IFileListEntry[] files)
        {
            var file = files.FirstOrDefault();
            if (file != null)
            {
                // Just load into .NET memory to show it can be done
                // Alternatively it could be saved to disk, or parsed in memory, or similar

                var ms = new MemoryStream();
                await file.Data.CopyToAsync(ms);
                //var bytes = ImageHelper.FixedSize(ms, 300, 300, false);
                //string newName = $"dokumen_{DateTime.Now.ToString("dd_MM_yyyy")}_{Path.GetFileNameWithoutExtension(file.Name)}{Path.GetExtension(file.Name)}";
                var bytes = ms.ToArray();
                ms.Dispose();
                var tempFile = Path.GetTempFileName() + ".xlsx";

                File.WriteAllBytes(tempFile, bytes);

                DataList = QurbanService.ReadExcel(tempFile,Tahun);
                if (DataList.Info == null)
                {
                    DataList.Info = qurbanSvc.GetData(Tahun);
                }
                if (DataList == null) 
                    toastService.ShowError( "Ada kesalahan pada data");
                else
                    toastService.ShowSuccess("Upload excel berhasil");
                StateHasChanged();
            }
        }
        async Task CaptureScreen()
        {
            await jsRuntime.InvokeAsync<object>("HtmlToImage", new[] { "canvas1" });
        }
        void GenerateReport()
        {
            double TotalBeratReport = 0;
            var dt = new DataTable("report-qurban");
            dt.Columns.Add("No");
            dt.Columns.Add("Uraian");
            dt.Columns.Add("Jumlah");
            dt.Columns.Add("Satuan");
            dt.AcceptChanges();
            var newItem = dt.NewRow();
            newItem[0] =1; 
            newItem[1] ="Jumlah Sapi"; 
            newItem[2] = DataList.DataHewanQurban?.Count(x=>x.Jenis == JenisHewanQurban.Sapi); 
            newItem[3] ="Ekor";
            dt.Rows.Add(newItem);

            newItem = dt.NewRow();
            newItem[0] = 2;
            newItem[1] = "Jumlah Kambing";
            newItem[2] = DataList.DataHewanQurban?.Count(x => x.Jenis == JenisHewanQurban.Kambing);
            newItem[3] = "Ekor";
            dt.Rows.Add(newItem);

            var temp = DataList.Pembagian?.Sum(x => x.Pembagian);
            TotalBeratReport += temp.Value;
            newItem = dt.NewRow();
            newItem[0] = 3;
            newItem[1] = "Porsi Jamaah";
            newItem[2] = temp?.ToString("n2");
            newItem[3] = "Kg";
            dt.Rows.Add(newItem);

            temp = DataList.Pembagian?.Sum(x => x.APRESIASI);
            TotalBeratReport += temp.Value;
            newItem = dt.NewRow();
            newItem[0] = 4;
            newItem[1] = "Porsi Apresiasi Jamaah";
            newItem[2] = temp?.ToString("n2");
            newItem[3] = "Kg";
            dt.Rows.Add(newItem);

            temp = DataList.Pembagian?.Sum(x => x.BL);
            TotalBeratReport += temp.Value;
            newItem = dt.NewRow();
            newItem[0] = 5;
            newItem[1] = "Budi Luhur Sekitar Lingkungan Jamaah";
            newItem[2] = temp?.ToString("n2");
            newItem[3] = "Kg";
            dt.Rows.Add(newItem);

            temp = DataList.QtySampil * DataList.BeratSampil;
            TotalBeratReport += temp.Value;
            newItem = dt.NewRow();
            newItem[0] = 6;
            newItem[1] = $"Porsi ({DataList.QtySampil} @ {DataList.BeratSampil}kg) Sampil Jamaah";
            newItem[2] = temp?.ToString("n2");
            newItem[3] = "Ekor";
            dt.Rows.Add(newItem);
            var TotalBeratSampil = temp.Value;
            //budi luhur
           
            var jenisBl = DataList.BL.Select(x => x.Jenis.ToString()).Distinct().ToList();
            var i = 0;
            foreach(var jenis in jenisBl)
            {
                var selData = DataList.BL.Where(x => x.Jenis.ToString() == jenis).ToList();
                temp = selData.Sum(x => x.BUNGKUS * x.BERAT);
                TotalBeratReport += temp.Value;
              
                newItem = dt.NewRow();
                newItem[0] = 7+i;
                newItem[1] = $"Budi Luhur {jenis}";
                newItem[2] = temp?.ToString("n2");
                newItem[3] = "Kg";
                dt.Rows.Add(newItem);
                i++;

            }

            DataList.Report = dt;
            DataList.TotalBerat = TotalBeratReport;
            DataList.BeratTanpaSampil = TotalBeratReport - TotalBeratSampil;
            DataList.TotalBeratBersih = DataList?.DataHewanQurban.Where(x=>x.Jenis==JenisHewanQurban.Sapi).Sum(x => x.Netto);
        }
    }
}