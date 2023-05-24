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
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using MoslemToolkit.Helpers;

namespace MoslemToolkit.Pages.PJKBM
{
    public partial class MasterEvaluasiKelas : ComponentBase
    {
        bool HasLogin;
        int currentMonth, currentWeek;
        int Tahun, Semester, StartMonth = 1;
        long KelasId=-1,JamaahId=-1;
        MoslemToolkit.Data.KelasService Kelasservice;
        MoslemToolkit.Data.MateriPerKelasService MateriPerKelasSvc;
        MoslemToolkit.Data.LaporanHasilBelajarService LaporanHasilBelajarSvc;
        MoslemToolkit.Data.NilaiSiswaService NilaiSiswaSvc;
        public List<Kelas> DataKelas;
        public List<Jamaah> DataSiswa;
        public List<int> ListTahun;
        MoslemToolkit.Data.EvaluasiKelasService service;
        string customHeader = string.Empty;
        List<DynamicItem<EvaluasiKelas>> DataEvaluasiKelasItems;
        DynamicItem<LaporanHasilBelajar> SelLaporanHasilBelajarItem;
        List<MateriPerKelas> DataMateriPerKelas;
        EvaluasiKelas itemObject = new EvaluasiKelas();
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        string UserName;
        CultureInfo ci;

        protected override async Task OnInitializedAsync()
        {
            ci = new CultureInfo("id-ID");
            if (Kelasservice == null) Kelasservice = new KelasService();
            if (DataKelas == null)
            {
                DataKelas = Kelasservice.GetAllData();
            }
            if (service == null) service = new EvaluasiKelasService();
            if (MateriPerKelasSvc == null) MateriPerKelasSvc = new MateriPerKelasService();
            if (LaporanHasilBelajarSvc == null) LaporanHasilBelajarSvc = new  LaporanHasilBelajarService();
            if (NilaiSiswaSvc == null) NilaiSiswaSvc = new   NilaiSiswaService();
           

            //DataEvaluasiKelas = service.GetAllData();
            ListTahun = MateriPerKelasSvc.GetAllData().Select(x => x.Tahun).Distinct().OrderBy(x => x).ToList();

        }

        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (DataEvaluasiKelasItems != null && DataEvaluasiKelasItems.Count > 0)
            {
                await jsRuntime.InvokeAsync<object>("FreezeTableHeader", new object[] { "#gridData" });
            }
            if (firstRender)
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    try
                    {
                        UserName = await localStorage.GetItemAsync<string>(AppConstants.NameKey);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    if (string.IsNullOrEmpty(UserName))
                    {
                        NavMgr.NavigateTo($"/auth/login");
                    }
                    else
                    {
                        HasLogin = RoleInfo.HasRole(UserName, Roles.Pengurus) || RoleInfo.HasRole(UserName, Roles.Pengajar);
                        if (HasLogin)
                            StateHasChanged();
                    }
                }
            }
        }
        private async Task HitungMutu()
        {
            if (DataEvaluasiKelasItems != null)
            {
                for(int i = 0; i < DataEvaluasiKelasItems.Count; i++)
                {
                    var item = DataEvaluasiKelasItems[i].Data;
                    item.NilaiMutu = ScoreHelper.getScore(item.Nilai);
                }
            }
        }
            private async Task Simpan()
        {
            //save here
            if (DataEvaluasiKelasItems != null)
            {
                var dataNew = DataEvaluasiKelasItems.Where(x => x.IsNew).Select(x => x.Data).ToList();
                var dataUpdate = DataEvaluasiKelasItems.Where(x => !x.IsNew).Select(x => x.Data).ToList();
                var isUpdated = service.UpdateDatas(dataUpdate);

                var isInserted = service.InsertDatas(dataNew);

                if (SelLaporanHasilBelajarItem != null)
                {
                    if (SelLaporanHasilBelajarItem.IsNew)
                        LaporanHasilBelajarSvc.InsertData(SelLaporanHasilBelajarItem.Data);
                    else
                        LaporanHasilBelajarSvc.UpdateData(SelLaporanHasilBelajarItem.Data);
                }

                if (isInserted) await Lihat();
            }
            toastService.ShowSuccess("Data Saved.");
            //StateHasChanged();
        }
        private async Task UpdateStat()
        {
            if (SelLaporanHasilBelajarItem == null) return;
            var stat = GetStat();
            SelLaporanHasilBelajarItem.Data.Alpha = stat.Alpha;
            SelLaporanHasilBelajarItem.Data.Sakit = stat.Sakit;
            SelLaporanHasilBelajarItem.Data.Ijin = stat.Ijin;
            SelLaporanHasilBelajarItem.Data.Hadir = stat.Hadir;
            

        }
        private async Task Lihat()
        {
            //save here
            StartMonth = Semester == 2 ? 1 : 7;
            var DataEvaluasiKelas = service.GetAllData(KelasId, Tahun, Semester,JamaahId);
            DataMateriPerKelas = MateriPerKelasSvc.GetAllData(KelasId, Tahun, Semester);
            if (DataEvaluasiKelasItems == null) DataEvaluasiKelasItems = new List<DynamicItem<EvaluasiKelas>>();
            DataEvaluasiKelasItems.Clear();
            foreach (var mat in DataMateriPerKelas)
            {
                var selItem = DataEvaluasiKelas.Where(x => x.MateriPerKelasId == mat.Id).FirstOrDefault();
                if (selItem == null)
                {
                    DataEvaluasiKelasItems.Add(new DynamicItem<EvaluasiKelas>() { IsNew=true, Data = new EvaluasiKelas() { JamaahId=JamaahId, Keterangan="", KeteranganDari=UserName, MateriPerKelasId=mat.Id, MateriPerKelas = mat, Nilai=0, NilaiMutu="", Tanggal = DateHelper.GetLocalTimeNow() } });
                }
                else
                {
                    DataEvaluasiKelasItems.Add(new DynamicItem<EvaluasiKelas>() { Data = selItem, IsNew=false });
                }
            }
            if (SelLaporanHasilBelajarItem == null) SelLaporanHasilBelajarItem = new DynamicItem<LaporanHasilBelajar>();
            var selBelajar = LaporanHasilBelajarSvc.GetData(KelasId, Tahun, Semester, JamaahId);
            if(selBelajar == null)
            {
                //ganjil                
                var stat = GetStat();
                SelLaporanHasilBelajarItem.Data = new LaporanHasilBelajar() { Tahun = Tahun, Semester = Semester, KelasId = KelasId, JamaahId = JamaahId, SikapAkhlak="-", KebersihanKerapihan = "-", Kerajinan = "-", Alpha = stat.Alpha, Hadir = stat.Hadir, Ijin = stat.Ijin, Sakit = stat.Sakit };
                SelLaporanHasilBelajarItem.IsNew = true;
            }
            else
            {
                SelLaporanHasilBelajarItem.Data = selBelajar;
                SelLaporanHasilBelajarItem.IsNew = false;
            }
            StateHasChanged();

        }

        SiswaStat GetStat()
        {
            DateTime start;
            DateTime end;
            if (Semester == 1)
            {
                start = new DateTime(Tahun, 7, 1);
                end = start.AddMonths(6).AddDays(-1);
            }
            else
            {
                start = new DateTime(Tahun, 1, 1);
                end = start.AddMonths(6).AddDays(-1);
            }
            var stat = NilaiSiswaSvc.GetStat(JamaahId, start, end);
            return stat;
        }
        
        void PilihKelas(ChangeEventArgs e)
        {
            var SelectedKelasId = long.Parse( e.Value.ToString());
            KelasId = SelectedKelasId;
            if (SelectedKelasId > -1)
            {
                DataSiswa = Kelasservice.GetSiswaByKelas(SelectedKelasId);
            }
            else
                JamaahId = -1;
            //Console.WriteLine("It is definitely: " + SelectedKelasId);
        }

        private async System.Threading.Tasks.Task applyPager(string TableName)
        {
            // await jsRuntime.InvokeAsync<object>("applyPager", TableName);
        }


    }
}
