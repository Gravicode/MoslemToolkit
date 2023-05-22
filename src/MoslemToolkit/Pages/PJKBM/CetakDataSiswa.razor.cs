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
using Microsoft.JSInterop;

namespace MoslemToolkit.Pages.PJKBM
{
    public partial class CetakDataSiswa : ComponentBase
    {
        MoslemToolkit.Data.KelasService Kelasservice;
        public List<Kelas> DataKelas;
        public List<Jamaah> DataSiswa;
        [Parameter]
        public long kelasid { set; get; }
        CetakDataSiswaService service;
        string PesanStatus;
        protected override async Task OnInitializedAsync()
        {
            
            var Today = DateHelper.GetLocalTimeNow();
            if (Kelasservice == null) Kelasservice = new KelasService();
            if (DataKelas == null)
            {
                DataKelas = Kelasservice.GetAllData();
            }

            if (service == null) service = new CetakDataSiswaService();
            await RefreshData();

        }
        public async Task RefreshData()
        {
            if (kelasid > -1)
            {
                DataSiswa = await service.GetAllData(kelasid);
            }
        }
        async Task CetakImage()
        {
            await jsRuntime.InvokeVoidAsync("HtmlToImage", "data-siswa" );
        }

        async Task CetakExcel()
        {
            try
            {
                var bytes = await service.ExportToExcel(kelasid);
                await FileUtil.SaveAs(jsRuntime, "Siswa.xlsx", bytes);
            }
            catch (Exception ex)
            {
                toastService.ShowError(ex.Message);
            }

        }
    }
}
