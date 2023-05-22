using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MoslemToolkit.Models;
using MoslemToolkit.Data;
using Newtonsoft.Json;
using System.Data;
using Blazored.Toast.Services;

namespace MoslemToolkit.Pages.PJKBM
{
    public partial class ReportBuku : ComponentBase
    {
        MoslemToolkit.Data.KelasService Kelasservice;
        public List<Kelas> DataKelas;
        BukuParameter param;
        ReportService service;
        string PesanStatus;
        int Progress=0;
        protected override async Task OnInitializedAsync()
        {
            param = new BukuParameter();
            var Today = DateHelper.GetLocalTimeNow();
            if (service == null) service = new ReportService();
            service.StatusChanged += async(object sender, ReportService.StatusChangedEventArgs e)=>
            {
                var msg = $"{e.Progress}% - {e.Message}";
                PesanStatus = msg;
                Progress = e.Progress;
                await InvokeAsync(StateHasChanged);
                //toastService.ShowInfo(msg);

            };
            param.DariTahun = Today.Year;
            param.SampaiTahun = param.DariTahun + 1;
            param.Desa = AppConstants.NAMA_DESA;
            param.Kelompok = AppConstants.NAMA_KELOMPOK;
            param.Daerah = AppConstants.NAMA_DAERAH;
            //semesteran
            if(Today.Month<7)
            {
                param.DariTanggal = new DateTime(Today.Year, 1, 1);
                param.SampaiTanggal = param.DariTanggal.AddMonths(6).AddDays(-1);
            }
            else
            {
                param.DariTanggal = new DateTime(Today.Year, 7, 1);
                param.SampaiTanggal = param.DariTanggal.AddMonths(6).AddDays(-1);
            }

            if (Kelasservice == null) Kelasservice = new KelasService();
            if (DataKelas == null)
            {
                DataKelas = Kelasservice.GetAllData();
            }

        }

      

        async Task Cetak()
        {
            try
            {
                param.KelasNama = Kelasservice.GetDataById(param.KelasId)?.Nama;
                var bytes = await service.GenerateBuku(param);
                await FileUtil.SaveAs(jsRuntime, "Buku.pdf", bytes);
            }
            catch (Exception ex)
            {
                toastService.ShowError(ex.Message);
            }

        }
    }
}
