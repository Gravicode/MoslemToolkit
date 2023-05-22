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

namespace MoslemToolkit.Pages.PJKBM
{
    public partial class MasterProgramSemester : ComponentBase
    {
        bool HasLogin;
        int currentMonth, currentWeek;
        int Tahun, Semester, StartMonth = 1;
        long KelasId;
        MoslemToolkit.Data.KelasService Kelasservice;
        MoslemToolkit.Data.MateriPerKelasService MateriPerKelasSvc;
        public List<Kelas> DataKelas;
        public List<int> ListTahun;
        MoslemToolkit.Data.ProgramSemesterService service;
        string customHeader = string.Empty;
        List<ProgramSemester> DataProgramSemester;
        List<MateriPerKelas> DataMateriPerKelas;
        List<ProgramSemesterItem> DataProgramSemesterItems;
        ProgramSemester itemObject = new ProgramSemester();
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
            if (service == null) service = new ProgramSemesterService();
            if (MateriPerKelasSvc == null) MateriPerKelasSvc = new MateriPerKelasService();

            //DataProgramSemester = service.GetAllData();
            ListTahun = MateriPerKelasSvc.GetAllData().Select(x => x.Tahun).Distinct().OrderBy(x => x).ToList();

        }

        string GetMonthName(int Index)
        {
            if (Index < 1 || Index > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(Index);
            return monthName;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (DataProgramSemester != null && DataProgramSemester.Count > 0)
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
                        NavMgr.NavigateTo($"/login");
                    }
                    else
                    {
                        HasLogin = RoleInfo.HasRole(UserName, Roles.Pengurus) || RoleInfo.HasRole(UserName, Roles.PJKBM);
                        if (HasLogin)
                            StateHasChanged();
                    }
                }
            }
        }
        private async Task Simpan()
        {
            //save here
            var isDeleted = service.RemoveData(KelasId, Tahun, Semester);
            if (isDeleted)
            {
                var result = service.InsertDatas(DataProgramSemesterItems);
            }
            var updated = MateriPerKelasSvc.UpdateDatas(DataMateriPerKelas);
            toastService.ShowSuccess("Data Saved.");
            //StateHasChanged();
        }
        private async Task Lihat()
        {
            //save here
            StartMonth = Semester == 1 ? 1 : 7;
            DataProgramSemester = service.GetAllData(KelasId, Tahun, Semester);
            DataMateriPerKelas = MateriPerKelasSvc.GetAllData(KelasId, Tahun, Semester);
            if (DataProgramSemesterItems == null) DataProgramSemesterItems = new List<ProgramSemesterItem>();
            DataProgramSemesterItems.Clear();
            foreach (var mat in DataMateriPerKelas)
            {
                for (int i = 0; i < 6; i++)
                {
                    currentMonth = StartMonth + i;
                    for (int x = 0; x < 4; x++)
                    {
                        currentWeek = x + 1;
                        DataProgramSemesterItems.Add(new ProgramSemesterItem() { Bulan = currentMonth, Minggu = currentWeek, MateriKelasId = mat.Id, Checked = isKelasExist(mat.Id, currentMonth, currentWeek) });
                    }
                }
            }
            StateHasChanged();

        }

        bool isKelasExist(long MateriKelasId, int Bulan, int Minggu)
        {
            if (DataProgramSemester == null) return false;
            var exists = DataProgramSemester.Any(x => x.MateriPerKelasId == MateriKelasId && x.Bulan == Bulan && x.Minggu == Minggu);
            return exists;
        }

        ProgramSemesterItem GetProgramItem(long MateriKelasId, int Bulan, int Minggu)
        {
            if (DataProgramSemesterItems == null) return null;
            var one = DataProgramSemesterItems.Where(x => x.MateriKelasId == MateriKelasId && x.Bulan == Bulan && x.Minggu == Minggu).FirstOrDefault();
            return one;
        }

        private async void DataChanged()
        {
            DataProgramSemester = service.GetAllData();
            StateHasChanged();
        }


        private void ClearChat(ProgramSemester item)
        {
            ChatMessageService svc = new ChatMessageService();
            if (item != null)
            {
                svc.ClearChat(item.Id);

            }
        }


        private async System.Threading.Tasks.Task applyPager(string TableName)
        {
            // await jsRuntime.InvokeAsync<object>("applyPager", TableName);
        }

    }
}
