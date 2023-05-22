using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MoslemToolkit;
using MoslemToolkit.Shared;
using MoslemToolkit.Shared.Usercontrols;
using BlazorInputFile;
using Blazored.Toast;
using Blazored.Toast.Services;
using Blazored.Typeahead;
using ChartJs.Blazor;
using System.IO;
using MoslemToolkit.Models;
using MoslemToolkit.Data;
using System.Linq;
using Newtonsoft.Json;
namespace MoslemToolkit.Pages.Ramadan
{
    public partial class PembagianZakatMal
    {
        int Tahun;
        double TotalAmil=0, TotalAsnab=0;
        int JumlahAmil=1;
        List<Mustahik> DataMustahik;
        List<AmilShare> DataAmil;
        MoslemToolkit.Data.MustahikService MustahikService;
        List<PembagianZakat> Pembagian;

        public class AmilShare
        {
            public string Nama { get; set; }
            public int No { get; set; }
            public double Persen { get; set; }
            public double Total { get; set; }
        }
        protected override async Task OnInitializedAsync()
        {
            if (MustahikService == null) MustahikService = new MustahikService();
            Tahun = DateTime.Now.Year;
            Refresh();
        }
        void Refresh()
        {
            RefreshData();

            Kalkulasi();

        }

        void Kalkulasi()
        {
            #region asnab
            //hitung tabel pembagian
            Pembagian = new List<PembagianZakat>();
            /*
            AppConstants.PembagianDefault.ForEach(x =>
            {
                if (!x.IsAsnab)
                {
                    var jenis = x.IsAsnab ? "Aznab" : "Amil";
                    Pembagian.Add(new PembagianZakat()
                    {
                        IdMustahik = -1,
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
            });*/
            //DetailAsnab = new List<PembagianZakat>();
            //var PctAsnab = AppConstants.PembagianDefault.Where(x => x.IsAsnab).FirstOrDefault().Persen;
            var TotalPengaliAsnab = DataMustahik.Sum(x => (x.Jumlah * AppConstants.AsnabDefault[x.TipeAsnab].FaktorPengali));
            DataMustahik.ForEach(x =>
            {
                var pct = ((AppConstants.AsnabDefault[x.TipeAsnab].FaktorPengali * x.Jumlah) / TotalPengaliAsnab);// * PctAsnab;
                Pembagian.Add(new PembagianZakat()
                {
                    IdMustahik = x.Id,
                    Nama = x.Nama,
                    Jenis = "Aznab",
                    Persen = pct,
                    Beras = 0,
                    Uang = pct*TotalAsnab,
                    KonversiUangKeBeras = 0,
                    TotalDalamBeras = 0,
                    BerasPembulatan = 0,
                    UangPembulatan = 0
                });
            });
            #endregion

            #region amil
            if (JumlahAmil == DataAmil.Count)
            {
                foreach(var item in DataAmil)
                {
                    item.Total = item.Persen * TotalAmil;
                }
            }
            else
            {
              
                if (JumlahAmil > 0)
                {
                    DataAmil.Clear();
                    double pct = 1d / JumlahAmil;
                    for (int i = 0; i < JumlahAmil; i++)
                    {
                        DataAmil.Add(new AmilShare() { No = i + 1, Nama = $"Amil-{i+1}", Persen = pct, Total = pct * TotalAmil });
                    }
                }
            }
            #endregion
        }
        void RefreshData()
        {
            if (DataAmil == null)
            {
                DataAmil = new List<AmilShare>();
                if (JumlahAmil > 0)
                {
                    double pct = 1d / JumlahAmil;
                    for (int i = 0; i < JumlahAmil; i++)
                    {
                        DataAmil.Add(new AmilShare() { No = i + 1, Nama = $"Amil-{i+1}", Persen = pct, Total = pct * TotalAmil });
                    }
                }
            }
            DataMustahik = MustahikService.GetAllData(Tahun);
        }
    }
}