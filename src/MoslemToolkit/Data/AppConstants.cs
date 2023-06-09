﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoslemToolkit.Models;
namespace MoslemToolkit.Data
{
    public class AppConstants
    {
        public static string DefaultPass = "123qweasd";
        public static string JamMasjidImagePrefix = "";
        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/ngaji-online";
        public const int FACE_WIDTH = 180;
        public const int FACE_HEIGHT = 135;
        public const string FACE_SUBSCRIPTION_KEY = "a068e60df8254cc5a187e3e8c644f316";
        public const string FACE_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/";
        public static string ReportPJKBM = "";
        public static string ReportKPI = "";
        public static string LaporanPencapaianPJKBM = "";
        public static string LaporanZakatFitrah = "";

        public static string KUReportUrl = "";
        public static string KUInfaqBulananUrl = "";
        public static string ReportQurbanUrl = "";
        public static string SQLConn = "";
        public static string JamMasjidUrl = "https://mosque-clock.my.id/";
        public static string BlobConn { get; set; }
        public const string GemLic = "EDWG-SKFA-D7J1-LDQ5";
        public const string NameKey = "Nama";
        public const string PassKeyJamaah = "123qwe";
        public const string PassKeyAdmin = "123qweasd";
        public const string PassKeyKeuangan = "123qweasd";
        public const string PassKeyPengajar = "123qweasd";
        public static string RedisCon { set; get; }
        public static string KehadiranReportUrl { get; set; } = "";
        public static string Evaluasi4sReportUrl { get; set; } = "";
        public static int HargaJualBeras { get; set; } = 40000;

        public static string Report_Cover;
        public static string Report_Presensi;
        public static string Report_Jurnal; 
        public static string Report_Prosem;
        public static string Report_Kalender;
        public static string Report_Anekdot;
        public static string Report_Evaluasi;
        public static string Report_Raport; 
        public static string NAMA_DAERAH; 
        public static string NAMA_DESA; 
        public static string NAMA_KELOMPOK; 

        public static string SCORE_DATA;

        //public static string USER_ADMIN = "admin";
        //public static string USER_PJKBM = "pengajar";
        //public static string USER_KU = "keuangan";

        public static string[] KategoriCashflowQurban = new string[]
  {
        "Konsumsi","Perlengkapan Qurban", "Infrastruktur Qurban", "Jasa Potong", "Tenda",  "Jual Kulit", "Barang Lainnya", "Jasa Lainnya"
  };
        public static readonly string[] Pekerjaans = new[]{
"Tidak Bekerja",
"Belum Bekerja",
"Karyawan Swasta",
"PNS",
"Pedagang",
"Pengusaha",
"Freelance",
"Pendidik",
"Tenaga Sabilillah",
"Lainnya"

        };

        public static readonly string[] Pendidikans = new[]{
"Tidak Sekolah",
"TK",
"SD",
"SMP",
"SMA/SMK",
"D1/D2/D3",
"S1",
"S2",
"S3",
"Professor",
"Putus Sekolah"

        };
        public static List<ProsentaseZakat> PembagianDefault = new List<ProsentaseZakat>()
        {
            new ProsentaseZakat(){ Id=1, IsAsnab=false, Nama="Amil", Persen=0.1 },
            new ProsentaseZakat(){ Id=2, IsAsnab=false, Nama="Penyelenggaraan", Persen=0.05 },
            new ProsentaseZakat(){ Id=3, IsAsnab=false, Nama="Alat", Persen=0.03 },
            new ProsentaseZakat(){ Id=4, IsAsnab=false, Nama="Lainnya", Persen=0.02 },
            new ProsentaseZakat(){ Id=5, IsAsnab=true, Nama="Asnab", Persen=0.8 },

        };

        public static Dictionary<TipeAsnab,Asnab> AsnabDefault = new  Dictionary<TipeAsnab, Asnab> 
        {
            { TipeAsnab.Fakir, new  Asnab(){  Id=(int)TipeAsnab.Fakir, Nama="Fakir", FaktorPengali=1 } },
            { TipeAsnab.Miskin,new Asnab (){ Id=(int)TipeAsnab.Miskin, Nama="Miskin", FaktorPengali=2 }},
            { TipeAsnab.Ghorim,new Asnab (){ Id=(int)TipeAsnab.Ghorim, Nama="Ghorim", FaktorPengali=3 } },
            { TipeAsnab.IbnuSabil,new Asnab (){ Id=(int)TipeAsnab.IbnuSabil, Nama="Ibnu Sabil", FaktorPengali=1 } },
            { TipeAsnab.Mualaf, new  Asnab(){  Id=(int)TipeAsnab.Mualaf, Nama="Mualaf", FaktorPengali=1 } },

        };
        public static List<TargetProgram> DataProgram = new List<TargetProgram>()
        {
            new TargetProgram (){ Nama="Khataman Quran", Skor=100 },
            new TargetProgram (){ Nama="Doa-doa harian", Skor=50 },
            new TargetProgram (){ Nama="Doa-doa khusus", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hadith Tirmizi", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hadith Bukhori", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hadith Muslim", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hadith Nasai", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hadith Abu Daud", Skor=50 },
            new TargetProgram (){ Nama="Khatam Tata Cara Sholat", Skor=50 },
            new TargetProgram (){ Nama="Khatam Tata Cara Bersuci", Skor=50 },
            new TargetProgram (){ Nama="Khatam Tata Cara Berwudlu", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hukum Pernikahan", Skor=50 },
            new TargetProgram (){ Nama="Hafalan surat pendek ", Skor=50 },
            new TargetProgram (){ Nama="Khatam Hukum Keuangan Syariah", Skor=50 },
        };
        public static string[] Tugas4s = new string[]
    {
        "Ketua DKM",
        "Wakil DKM",
        "Keuangan",
        "Sekretaris",
        "Tim Aset dan Perlengkapan",
        "Tim Qurban",
        "Tim Zakat",
        "Tim Kegiatan Masjid",
        "Tim Pengajar",
    };


        #region zakat fitrah
        public static string Zakat_Fitrah_Cover;
        public static string Zakat_Fitrah_Content;

        #endregion 
        
        #region qurban
        public static string Qurban_Cover;
        public static string Qurban_Content;

        #endregion
    }
}
