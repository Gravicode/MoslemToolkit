using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MoslemToolkit.Models
{
    #region common
    [DataContract]
    public class InputCls
    {
        [DataMember(Order = 1)]
        public string[] Param { get; set; }
        [DataMember(Order = 2)]
        public Type[] ParamType { get; set; }
    }
    [DataContract]
    public class OutputCls
    {
        [DataMember(Order = 1)]
        public bool Result { get; set; }
        [DataMember(Order = 2)]
        public string Message { get; set; }
        [DataMember(Order = 3)]
        public string Data { get; set; }
    }
    [DataContract]
    public class UserProfile
    {
        [DataMember(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [DataMember(Order = 2)]
        public string Username { get; set; }
        [DataMember(Order = 3)]
        public string Password { get; set; }
        [DataMember(Order = 4)]
        public string FullName { get; set; }
        [DataMember(Order = 5)]
        public string? Phone { get; set; }
        [DataMember(Order = 6)]
        public string? Email { get; set; }
        [DataMember(Order = 7)]
        public string? Alamat { get; set; }
        [DataMember(Order = 8)]
        public string? KTP { get; set; }
        [DataMember(Order = 9)]
        public string? PicUrl { get; set; }
        [DataMember(Order = 10)]
        public bool Aktif { get; set; } = true;

        [DataMember(Order = 11)]
        public Roles Role { set; get; } = Roles.User;

    }

    public enum Roles { Admin, User, Pengurus, Keuangan, Pengajar }
    #endregion
    #region mosque 
    public class MosqueAlarmData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Judul { get; set; }
        public string AlarmId { get; set; }
        public TimeSpan Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public List<DayOfWeek> Hari { get; set; }

        public static MosqueAlarm From(MosqueAlarmData item)
        {

            var hari = new List<int>();
            if (item.Hari != null)
            {
                hari = (from x in item.Hari
                        select (int)x).ToList();
            }
            var newItem = new MosqueAlarm()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,

                Hari = string.Join(";", hari),
                Id = item.Id,
                Judul = item.Judul,
                AlarmId = item.AlarmId,
                Waktu = item.Waktu

            };
            return newItem;
        }

        public static MosqueAlarmData To(MosqueAlarm item)
        {

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new MosqueAlarmData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                AlarmId = item.AlarmId,
                Waktu = item.Waktu

            };
            return newItem;
        }
    }
    public class MosqueAlarmPrimitive
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string AlarmId { get; set; }

        public string Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }

        public static MosqueAlarmPrimitive From(MosqueAlarm item)
        {
            if (item == null) return null;
            var newItem = new MosqueAlarmPrimitive()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,

                Id = item.Id,
                Hari = item.Hari,
                Judul = item.Judul
                  ,
                AlarmId = item.AlarmId,

                Waktu = $"{item.Waktu.Hours.ToString("D2")}:{item.Waktu.Minutes.ToString("D2")}"

            };
            return newItem;
        }
        public static MosqueAlarmData To(MosqueAlarmPrimitive item)
        {

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new MosqueAlarmData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,

                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                AlarmId = item.AlarmId,
                Waktu = new TimeSpan(int.Parse(item.Waktu.Substring(0, 2)), int.Parse(item.Waktu.Substring(2, 2)), 0)

            };
            return newItem;
        }
    }
    public class MosqueAlarm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Required]
        public string Judul { get; set; }
        [Required]
        public string AlarmId { get; set; } = "alarm1";
        public TimeSpan Waktu { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string? Hari { get; set; }
    }

    public enum TipeCuaca { Cerah, Hujan, Berangin, Panas }

    public class CCTVImage
    {
        public string CctvName { get; set; }
        public string Image64 { get; set; }
    }
    public class KegiatanMasjid
    {
        public DateTime Tanggal { get; set; }
        public string Nama { get; set; }
    }
    public class ImageItem
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }
    public class PengumumanItem
    {
        public string Keterangan { get; set; }
    }
    public class WaktuShalat
    {
        public int Urutan { get; set; }
        public string Nama { get; set; }
        public TimeSpan Waktu { get; set; }
    }
    public class InfoDetailMasjid
    {
        public string Imam { get; set; }
        public string Muazin { get; set; }
        public string Khotib { get; set; }
        public List<WaktuShalat> WaktuShalats { set; get; }
        public List<KegiatanMasjid> Kegiatan { get; set; }
        public string Lokasi { get; set; }
        public TipeCuaca Cuaca { get; set; } = TipeCuaca.Cerah;

        public DateTime TanggalHariIni { get; set; }
        public string NamaMasjid { get; set; }
        public List<PengumumanItem> Pengumuman { get; set; }
        public List<ImageItem> ImageUrl { get; set; }
        public string VideoUrl { get; set; }

        public InfoDetailMasjid()
        {
            ImageUrl = new List<ImageItem>();
            Pengumuman = new List<PengumumanItem>();
            WaktuShalats = new List<WaktuShalat>();
            Kegiatan = new List<KegiatanMasjid>();
        }
    }
    #endregion
    #region helper model

    public class TargetMateriTemp : Materi
    {// Copy Constructor
        public TargetMateriTemp()
        {

        }
        public TargetMateriTemp(Materi c)

        {

            // copy base class properties.

            foreach (PropertyInfo prop in c.GetType().GetProperties())

            {

                PropertyInfo prop2 = c.GetType().GetProperty(prop.Name);

                prop2.SetValue(this, prop.GetValue(c, null), null);

            }

        }
        public bool Selected { get; set; }
        public int TargetNilai { get; set; }
        public string TargetDeskripsi { get; set; }
    }
    public class PembagianPerKP
    {
        public PembagianPerKP()
        {
            Pembagian = new List<DataPembagian>();
        }
        public double Berat { get; set; }
        public string KP { get; set; }
        public List<DataPembagian> Pembagian { get; set; }
    }
    public class ReportQurbanParameter
    {
        public string Desa { get; set; }
        public string Kelompok { get; set; }
        public int Tahun { get; set; }
        public DateTime Tanggal { get; set; }

        public MoslemToolkit.Data.Qurban DataList { set; get; }
    }
    public class ZakatFitrahParameter
    {
        public string Desa { get; set; }
        public string Kelompok { get; set; }
        public int Tahun { get; set; }
        public DateTime Tanggal { get; set; }

        public List<Muzaki> DataMuzaki { get; set; }
        public List<Muzaki> MuzakiBeras { get; set; }
        public List<Muzaki> MuzakiUang { get; set; }
        public List<Muzaki> MuzakiBelum { get; set; }
        public List<Mustahik> DataMustahik { get; set; }
        public List<PembagianZakat> Pembagian { get; set; }
        public List<PembagianIteration> PembagianDetail { get; set; }

        public List<StatistikZakat> MuzakiStat { get; set; }
        public List<StatistikZakat> MustahikStat { get; set; }
        public List<(string Nama, long Jumlah, double Persen)> Stats { get; set; }
        public int TotalIteration { get; set; }
        public long HargaBeras { set; get; }
        public long HargaJualBeras { get; set; }
        public double TotalBeras { get; set; }
        public double TotalDalamBeras { get; set; }
        public double TotalUang { get; set; }
        public double Total { get; set; }//, SisaUang, SisaBeras, TotalBerasSelain_SB_DA_DS{ get; set; }
        public double SubTotal { get; set; }
    }
    public class StatistikZakat
    {
        public string Nama { get; set; }
        public int Total { get; set; }
        public double Prosentase { get; set; }
    }

    public class SiswaStat
    {
        public int Hadir { get; set; }
        public int Sakit { get; set; }
        public int Ijin { get; set; }
        public int Alpha { get; set; }
    }
    public class CellData
    {

        public CellData(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }
        public int Row { get; set; }
        public int Col { get; set; }
    }

    public class DynamicItem<T> where T : class
    {
        public bool IsNew { get; set; }
        public T Data { get; set; }
    }

    public class ProgramSemesterItem
    {
        public bool Checked { get; set; }
        public int Bulan { get; set; }
        public int Minggu { get; set; }
        public long MateriKelasId { get; set; }
    }
    public class BukuParameter
    {
        public string Daerah { get; set; }
        public string Desa { get; set; }
        public string Kelompok { get; set; }
        public long KelasId { get; set; }
        public int DariTahun { get; set; }
        public int SampaiTahun { get; set; }
        public string KelasNama { get; set; }

        public DateTime DariTanggal { get; set; }
        public DateTime SampaiTanggal { get; set; }
    }
    #endregion
    #region pjkbm

    public class KalenderPendidikan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string Kegiatan { get; set; }

        public string Keterangan { get; set; }
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalSelesai { get; set; }

    }
    public class ProgramSemester
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public int Minggu { get; set; }
        public int Bulan { get; set; }

        [ForeignKey("MateriPerKelas")]
        public long MateriPerKelasId { get; set; }
        public MateriPerKelas MateriPerKelas { get; set; }

    }
    public class Bab
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public int No { get; set; }
        public string Nama { get; set; }
        public ICollection<Materi> Materis { get; set; }

    }
    public class Materi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public int No { get; set; }
        public string Nama { get; set; }
        public string Keterangan { get; set; }

        [ForeignKey("Bab")]
        public long BabId { get; set; }
        public Bab Bab { get; set; }
        public ICollection<MateriPerKelas> MateriPerKelas { get; set; }
    }
    public class Anekdot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public DateTime Tanggal { get; set; }
        public string Permasalahan { get; set; }
        public string Solusi { get; set; }
        public string Pendidik { get; set; }

        [ForeignKey("Kelas")]
        public long KelasId { get; set; }
        public Kelas Kelas { get; set; }
    }
    public class Kelas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string Nama { get; set; }

        public int MinUsia { get; set; }
        public int MaxUsia { get; set; }
        public string Kelompok { get; set; }
        public string WaliKelas { get; set; }
        public ICollection<MateriPerKelas> MateriPerKelass { get; set; }
        public ICollection<Jamaah> Siswas { get; set; }
        public ICollection<SiswaPerKelas> SiswaPerKelas { get; set; }
        public ICollection<Anekdot> Anekdots { get; set; }
        public ICollection<LaporanHasilBelajar> LaporanHasilBelajars { get; set; }

    }

    public class SiswaPerKelas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [ForeignKey("Kelas")]
        public long KelasId { get; set; }
        public Kelas Kelas { get; set; }

        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        public int Tahun { get; set; }
    }
    public class MateriPerKelas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public int No { get; set; }
        [ForeignKey("Materi")]
        public long? MateriId { get; set; }
        public Materi? Materi { get; set; }
        [ForeignKey("Kelas")]
        public long? KelasId { get; set; }
        public Kelas? Kelas { get; set; }
        public bool IsActive { get; set; }
        public int Semester { get; set; }
        public int Tahun { get; set; }
        public string? Keterangan { get; set; }
        public ICollection<NilaiSiswa> NilaiSiswas { get; set; }
        public ICollection<ProgramSemester> ProgramSemesters { get; set; }

    }

    public class TargetPerKelas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public int No { get; set; }
        [ForeignKey("Materi")]
        public long MateriId { get; set; }
        public Materi Materi { get; set; }
        [ForeignKey("Kelas")]
        public long KelasId { get; set; }
        public Kelas Kelas { get; set; }
        public int Semester { get; set; }
        public int Tahun { get; set; }
        public int TargetNilai { get; set; }
        [MaxLength(800)]
        public string TargetDeskripsi { get; set; }

        public TargetPerKelas()
        {
            var now = DateTime.Now; //DateHelper.GetLocalTimeNow();
            Tahun = now.Year;
            Semester = now.Month <= 6 ? 1 : 2;

        }

    }
    public class AbsensiSiswa
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime Tanggal { get; set; }
        public int Bulan { get; set; }
        public int Minggu { get; set; }
        public int Tahun { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        [ForeignKey("MateriPerKelas")]
        public long MateriPerKelasId { get; set; }
        public MateriPerKelas MateriPerKelas { get; set; }
    }
    public enum StatusKehadiran { Hadir = 0, Ijin, Sakit, Alpha };
    public class NilaiSiswa
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public DateTime Tanggal { get; set; }
        public bool Hadir { get; set; } = false;
        public StatusKehadiran Kehadiran { get; set; } = StatusKehadiran.Hadir;
        public int Nilai { get; set; } = 0;
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        public string Keterangan { get; set; }
        public string KeteranganDari { get; set; }
        public string Respon { get; set; }
        public string ResponDari { get; set; }
        [ForeignKey("MateriPerKelas")]
        public long MateriPerKelasId { get; set; }
        public MateriPerKelas MateriPerKelas { get; set; }
    }
    public class EvaluasiKelas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public int Nilai { get; set; } = 0;
        public string NilaiMutu { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        public string Keterangan { get; set; }
        public string KeteranganDari { get; set; }

        [ForeignKey("MateriPerKelas")]
        public long MateriPerKelasId { get; set; }
        public MateriPerKelas MateriPerKelas { get; set; }
    }

    public class LaporanHasilBelajar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }

        [ForeignKey("Kelas")]
        public long KelasId { get; set; }
        public Kelas Kelas { get; set; }

        public string SikapAkhlak { get; set; }
        public string Kerajinan { get; set; }
        public string KebersihanKerapihan { get; set; }

        public int Tahun { get; set; }
        public int Semester { get; set; }
        public int Sakit { get; set; }
        public int Ijin { get; set; }
        public int Hadir { get; set; }
        public int Alpha { get; set; }

    }
    #endregion
    #region KPI
    public class KPI
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [ForeignKey("Dapukan")]
        public long DapukanId { get; set; }

        public Dapukan Dapukan { get; set; }
        public string Parameter { get; set; }

        public string? Kategori { get; set; }
        public int MinScore { get; set; } = 1;
        public int MaxScore { get; set; } = 5;
        public string Deskripsi { get; set; }
        public ICollection<KPIScore> KPIScores { get; set; }

    }

    public enum PeriodeKPI
    {
        Bulanan, Semesteran, Tahunan
    }
    public class KPIBatch
    {

        [Key, Column(Order = 0)]
        public int BatchNo { get; set; }
        public DateTime? Tanggal { get; set; }
        public PeriodeKPI PeriodePenilaian { get; set; } = PeriodeKPI.Bulanan;
        public string? Keterangan { get; set; }

        public ICollection<KPIScoreHeader> KPIScoreHeaders { get; set; }
    }

    public class KPIScoreHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [ForeignKey("KPIBatch")]
        public int? BatchNo { get; set; }

        public KPIBatch KPIBatch { get; set; }

        public string? Dapukan { get; set; }
        public string? Nama { get; set; }
        public DateTime? TanggalPenilaian { get; set; }
        public string? PenilaianOleh { get; set; }
        public string? Keterangan { get; set; }

        public ICollection<KPIScore> KPIScores { get; set; }

    }
    public class KPIScore
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [ForeignKey("KPIScoreHeader")]
        public long KPIScoreHeaderId { get; set; }

        public KPIScoreHeader KPIScoreHeader { get; set; }

        [ForeignKey("KPI")]
        public long KPIId { get; set; }

        public KPI Kpi { get; set; }

        public int Nilai { get; set; }
        public string? Keterangan { get; set; }

    }

    #endregion
    public class Musyawarah5Unsur
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string? Nama { get; set; }
        public string? Keterangan { get; set; }
        public string? DocumentUrl { get; set; }
        public string? CreatedBy { get; set; }
    }                
    public enum LevelWilayah { Pusat, Daerah, Desa, Kelompok, Lainnya }
    public enum GrupNeraca { Pusat, Daerah, Desa, Kelompok, KesraMubaleg, Masjid, DanaKematian, Lainnya }
    public enum TipeAkunSodakoh { Infaq, Sodakoh, Tabungan, Pembangunan, HajiUmroh, PPG, Sarana, Organisasi, Qurban, KesraMubaleg, Kematian, Lainnya }
    public class AkunInfaqSodakoh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public int No { get; set; }
        public string Uraian { get; set; }
        public string Keterangan { get; set; }
        [Range(0, 100)]
        public double PersenDariSetoran { get; set; }
        public double TargetJatahan { get; set; }
        public TipeAkunSodakoh Tipe { get; set; }
        public GrupNeraca Grup { get; set; }

        public bool Rutin { get; set; } = true;
        public LevelWilayah Level { get; set; }
        public bool MasukKeIsrun { get; set; } = true;

    }

    public class HeaderPenerimaanInfaqSodakoh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string DiterimaOleh { get; set; }
        public string Penyetor { get; set; }
        public string Keterangan { get; set; }
        public double Total { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
    }
    public class PenerimaanInfaqSodakoh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public int No { get; set; }
        [ForeignKey("AkunInfaqSodakoh")]
        public long AkunInfaqSodakohId { get; set; }
        public AkunInfaqSodakoh AkunInfaqSodakoh { get; set; }
        [ForeignKey("HeaderPenerimaanInfaqSodakoh")]
        public long HeaderPenerimaanInfaqSodakohId { get; set; }
        public HeaderPenerimaanInfaqSodakoh HeaderPenerimaanInfaqSodakoh { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        public string NamaJamaah { get; set; }
        public string NamaAkun { get; set; }
        public string Keterangan { get; set; }
        public double Jumlah { get; set; }
    }
    public class PengeluaranInfaqSodakoh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public int No { get; set; }
        public AkunInfaqSodakoh AkunInfaqSodakoh { get; set; }
        [ForeignKey("AkunInfaqSodakoh")]
        public long AkunInfaqSodakohId { get; set; }
        public string NamaAkun { get; set; }
        public string Keterangan { get; set; }
        public double Jumlah { get; set; }
        public string DikeluarkanOleh { get; set; }

    }

    public enum FrequensiInfaq { Rutin, Jarang, KePusat, TidakPernah }
    public class ProfilInfaqJamaah
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public Jamaah Jamaah { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public FrequensiInfaq Frekuensi { get; set; }
        public string Pekerjaan { get; set; }

    }
    public enum TargetSodakoh
    {
        Jamaah, Aghniya, Pengurus, OrangTua, Lainnya
    }
    public class AkunSodakohKhusus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public string Nama { get; set; }
        public string Keterangan { get; set; }
        public double TargetNilai { get; set; }
        public DateTime DariTanggal { get; set; }
        public DateTime SampaiTanggal { get; set; }
        public string PenanggungJawab { get; set; }
        public TargetSodakoh Target { get; set; } = TargetSodakoh.Jamaah;

    }
    public class SetoranSodakohKhusus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }

        public DateTime Tanggal { get; set; }
        public Jamaah Jamaah { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public string NamaPenyetor { get; set; }
        public AkunSodakohKhusus AkunSodakohKhusus { get; set; }
        [ForeignKey("AkunSodakohKhusus")]
        public long AkunSodakohKhususId { get; set; }
        public double Jumlah { get; set; }
        public string Keterangan { get; set; }
        public string DiterimaOleh { get; set; }
    }
    public class SetoranSodakohKhususBarang
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public Jamaah Jamaah { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public string NamaPenyetor { get; set; }
        public AkunSodakohKhusus AkunSodakohKhusus { get; set; }
        [ForeignKey("AkunSodakohKhusus")]
        public long AkunSodakohKhususId { get; set; }
        public string NamaBarang { get; set; }
        public double Jumlah { get; set; }
        public string Satuan { get; set; }
        public string Keterangan { get; set; }
        public string DiterimaOleh { get; set; }
    }
    public class AkunTabunganJamaah
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }

        public string Nama { get; set; }
        public string Keterangan { get; set; }
        public DateTime DariTanggal { get; set; }
        public DateTime SampaiTanggal { get; set; }

    }
    public class TabunganJamaah
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]

        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public AkunTabunganJamaah AkunTabunganJamaah { get; set; }
        [ForeignKey("AkunTabunganJamaah")]
        public long AkunTabunganJamaahId { get; set; }
        public Jamaah Jamaah { get; set; }
        [ForeignKey("Jamaah")]
        public long JamaahId { get; set; }
        public string NamaJamaah { get; set; }
        public double Jumlah { get; set; }
        public string Keterangan { get; set; }
        public string DiterimaOleh { get; set; }

    }
    public enum TipePinjaman { Hutang, Piutang }
    public enum StatusPinjaman { Lunas, BelumLunas, Gagal }

    public class Pinjaman
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Nama { get; set; }
        public string Keterangan { get; set; }
        public string Peminjam { get; set; }
        public string PemberiPinjaman { get; set; }
        public double JumlahPinjaman { get; set; }
        public DateTime TanggalPinjam { get; set; }
        public DateTime TanggalPengembalian { get; set; }
        public int JangkaWaktuHari { get; set; }
        public TipePinjaman Tipe { get; set; }
        public StatusPinjaman Status { get; set; }
        public ICollection<Cicilan> Cicilans { get; set; }
    }

    public class Cicilan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public double Jumlah { get; set; }
        public string DibayarOleh { get; set; }
        public string Keterangan { get; set; }
        [ForeignKey("Pinjaman")]
        public long? PinjamanId { get; set; }
        public Pinjaman Pinjaman { get; set; }
    }
    public enum TipeAbsensi
    {
        Online = 0, Offline, Semua
    }
    public class WebCamOptions
    {
        public int Width { get; set; } = 320;
        public string VideoID { get; set; }
        public string CanvasID { get; set; }
        public string Filter { get; set; } = null;
    }
    public class PerhitunganZakatState
    {
        public long HargaJualBeras { get; set; }
        public List<PembagianIteration> PembagianDetail { get; set; }
    }
    public class StateStorage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string NameKey { get; set; }
        public string ValueString { get; set; }
    }
    //[Table("prosentasezakats")]
    public class ProsentaseZakat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Nama { get; set; }
        public double Persen { get; set; }
        public bool IsAsnab { get; set; } = false;
    }
    public enum TipeAsnab { Fakir = 1, Miskin, Ghorim, IbnuSabil, Mualaf };
    //[Table("asnabs")]
    public class Asnab
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public double FaktorPengali { get; set; }
    }
    public class InfoMasjid
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string KodeMasjid { get; set; }
        public string DataMasjid { get; set; }
    }

    public class KalenderKelompok
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public string? Kegiatan { get; set; }

        public string? Keterangan { get; set; }
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalSelesai { get; set; }

    }
    public class Mustahik
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public int No { get; set; }
        public string? Nama { get; set; }
        public int Jumlah { get; set; }
        public TipeAsnab TipeAsnab { get; set; }
        public double Beras { get; set; }
        public double Uang { get; set; }
        public int Tahun { get; set; }
    }
    public class Muzaki
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public int NoUrut { get; set; }
        public string? KK { get; set; }
        public string? Nama { get; set; }
        public PosisiKeluarga Posisi { get; set; }

        public bool IsMustahik { get; set; } = true;
        public int ZakatBeras { get; set; } = 0;
        public double TitipZakat { get; set; } //= AppConstants.HargaJualBeras;
        public double SelisihBeras { get; set; } = 0;
        public double SelisihTitipan { get; set; } = 0;


        public string? Amil { get; set; }
        public double DanaTalangan { get; set; } = 0;
        public bool SudahZakat { get; set; } = false;
        public bool SudahRealisasi { get; set; } = false;
        public bool SudahTercatat { get; set; } = false;
        public int Tahun { get; set; }
    }
    public class DetailIteration
    {
        public string Nama { get; set; }
        public double Terima { get; set; }
        public double Dijual { get; set; }
        public double Sisa { get; set; }
        public double SisaSebelumnya { get; set; }
        public double Uang { get; set; }

        public List<Muzaki> DibeliOleh { get; set; }
    }
    public class PembagianIteration
    {
        public int No { get; set; }
        public double TotalPembagian { get; set; }
        public double TotalBerasDijual { get; set; }
        public double TotalBerasSisa { get; set; }
        public double TotalBerasSisaSebelumnya { get; set; }
        public double TotalUang { get; set; }
        public List<DetailIteration> Detail { get; set; }
        //public List<Muzaki> Orang;
        public PembagianIteration()
        {
            //Orang = new List<Muzaki>();
            Detail = new List<DetailIteration>();
        }

    }

    public class HargaBerasZakat
    {
        [Key, Column(Order = 0)]
        public int Tahun { get; set; }
        public long HargaJualBeras { get; set; }
    }
    public class PembagianZakat
    {
        /*
                            <td>Nama</td>
                    <td>Persen</td>
                    <td>Beras</td>
                    <td>Uang</td>
                    <td>Konversi Uang Ke Beras (Sok)</td>
                    <td>Total Dalam Beras (Sok)</td>
                    <td>Beras (Pembulatan)</td>
                    <td>Uang (Pembulatan)</td>

         */
        public long IdMustahik { get; set; }
        public string Jenis { get; set; }
        public string Nama { get; set; }
        public double Persen { get; set; }
        public double Beras { get; set; }
        public double Uang { get; set; }
        public double KonversiUangKeBeras { get; set; }
        public double TotalDalamBeras { get; set; }
        public double BerasPembulatan { get; set; }
        public double UangPembulatan { get; set; }

    }
    public class Aset
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [Required]
        public string KodeBarang { get; set; }
        [Required]
        public string NamaBarang { get; set; }

        [Required]
        public string? Merek { get; set; }

        [Required]
        public string? Ukuran { get; set; }
        public string? NoSertifikat { get; set; }
        public string? Lokasi { get; set; }

        [Required]
        public int Jumlah { get; set; } = 1;

        public string? CaraPerolehan { get; set; }


        public DateTime? TanggalPerolehan { get; set; }

        [Required]
        public string Kondisi { get; set; }
        public string? Keterangan { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
    }
    public enum PosisiKeluarga
    {
        KK, ISTRI, ANAK, CUCU, KELUARGA, KEPONAKAN, MT, SIMPATISAN, MERTUA, IBU
    }

    public enum StatusPernikahan
    {
        Single, Berkeluarga
    }
    public class SmsRecipient
    {
        public string Nama { get; set; }
        public string Phone { get; set; }

        public bool Pilih { get; set; } = false;
    }
    public class SmsBroadcastPrimitive
    {
        public long Id { get; set; }
        public string Judul { get; set; }
        public string Pesan { get; set; }

        public string DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public string WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }

        public static SmsBroadcastPrimitive From(SmsBroadcast item)
        {
            if (item == null) return null;
            var newItem = new SmsBroadcastPrimitive()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = item.DikirimKe,
                Id = item.Id,
                Hari = item.Hari,
                Judul = item.Judul
                  ,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = $"{item.WaktuKirim.Hours.ToString("D2")}:{item.WaktuKirim.Minutes.ToString("D2")}"

            };
            return newItem;
        }
        public static SmsBroadcastData To(SmsBroadcastPrimitive item)
        {
            var dikirim = new List<string>();
            if (!string.IsNullOrEmpty(item.DikirimKe))
            {
                dikirim = item.DikirimKe.Split(';').ToList();
            }

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new SmsBroadcastData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = dikirim,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = new TimeSpan(int.Parse(item.WaktuKirim.Substring(0, 2)), int.Parse(item.WaktuKirim.Substring(2, 2)), 0)

            };
            return newItem;
        }
    }
    public class SmsBroadcast
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Required]
        public string Judul { get; set; }
        [Required]
        public string Pesan { get; set; }

        public string DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public TimeSpan WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public string Hari { get; set; }
    }
    public class DaySelect
    {
        public string Name { get; set; }
        public DayOfWeek Day { get; set; }
        public bool Pilih { get; set; }
    }
    public class SmsBroadcastData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Judul { get; set; }
        public string Pesan { get; set; }
        public List<string> DikirimKe { get; set; }
        public int Terkirim { get; set; }
        public TimeSpan WaktuKirim { get; set; }
        public DateTime AktifDari { get; set; }
        public DateTime AktifSampai { get; set; }
        public bool Berulang { get; set; }
        public List<DayOfWeek> Hari { get; set; }

        public static SmsBroadcast From(SmsBroadcastData item)
        {
            var dikirim = new List<string>();
            if (item.DikirimKe != null)
            {
                dikirim = (from x in item.DikirimKe
                           select x).ToList();
            }
            var hari = new List<int>();
            if (item.Hari != null)
            {
                hari = (from x in item.Hari
                        select (int)x).ToList();
            }
            var newItem = new SmsBroadcast()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = string.Join(";", dikirim),
                Hari = string.Join(";", hari),
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = item.WaktuKirim

            };
            return newItem;
        }

        public static SmsBroadcastData To(SmsBroadcast item)
        {
            var dikirim = new List<string>();
            if (!string.IsNullOrEmpty(item.DikirimKe))
            {
                dikirim = item.DikirimKe.Split(';').ToList();
            }

            var hari = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(item.Hari))
            {
                hari = (from x in item.Hari.Split(';')
                        select (DayOfWeek)Convert.ToInt32(x)).ToList();
            }
            var newItem = new SmsBroadcastData()
            {
                AktifDari = item.AktifDari,
                AktifSampai = item.AktifSampai,
                Berulang = item.Berulang,
                DikirimKe = dikirim,
                Hari = hari,
                Id = item.Id,
                Judul = item.Judul,
                Pesan = item.Pesan,
                Terkirim = item.Terkirim,
                WaktuKirim = item.WaktuKirim

            };
            return newItem;
        }
    }
    public class Pengumuman
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        [Required]
        public string Judul { get; set; }
        [Required]
        public string Isi { get; set; }
        [Required]
        public DateTime Tanggal { get; set; }
        public string CreatedBy { get; set; }
    }
    public class Dapukan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }


        public string Nama { get; set; }
        public int OrderNo { get; set; }
    }
    public class Jamaah
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public StatusPernikahan? Status { get; set; } = StatusPernikahan.Berkeluarga;
        public string? KK { get; set; }
        public string? Nama { get; set; }
        public string? Ayah { get; set; }
        public string? Ibu { get; set; }
        public PosisiKeluarga? Posisi { get; set; } = PosisiKeluarga.KK;
        public string? PhotoUrl { get; set; }
        public string? Gol { get; set; }
        public char? Kelamin { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string? TempatLahir { get; set; }
        public string? Alamat { get; set; }
        public string? Telepon { get; set; }
        public string? Email { get; set; }
        public string? Pendidikan { get; set; }
        public string? Pekerjaan { get; set; }
        public string? CreatedBy { get; set; }
        public ICollection<NilaiSiswa> NilaiSiswa { get; set; }
        public ICollection<AbsensiSiswa> AbsensiSiswa { get; set; }
        public ICollection<EvaluasiKelas> EvaluasiKelas { get; set; }
        public ICollection<LaporanHasilBelajar> LaporanHasilBelajar { get; set; }

        [ForeignKey("Kelas")]
        public long? KelasId { get; set; }
        public Kelas? Kelas { get; set; }
        public ICollection<SiswaPerKelas> KelasSiswa { get; set; }
        public long? FatherId { get; set; }
        public long? MotherId { get; set; }

    }
    public class Tim
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Required]
        public string Tugas { get; set; }
        [Required]
        public string Nama { get; set; }

        public string? Alamat { get; set; }
        public string? Telepon { get; set; }
        public int Urutan { get; set; }
    }
    public class Musyawarah
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string Topik { get; set; }
        public string? Keterangan { get; set; }
        public string DocumentUrl { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class BuktiTransfer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string Tujuan { get; set; }
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
        //[Required(ErrorMessage = "Nama Pengirim wajib isi")]
        public string NamaPengirim { get; set; }
        public string NoRekening { get; set; }
        public string NamaRekening { get; set; }
        public string NamaBank { get; set; }
        public string NomorHp { get; set; }
        public string DibuatOleh { get; set; }
        // [Required(ErrorMessage = "Jumlah wajib isi")]
        public double Jumlah { get; set; }
        //[Required(ErrorMessage = "Wajib upload bukti")]
        public string LampiranUrl { get; set; }
    }


    public class Sodakoh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string NamaBank { get; set; }
        public string NoRekening { get; set; }
        public string NamaRekening { get; set; }
        public string NamaGopay { get; set; }
        public string NomorGopay { get; set; }
        public string GopayQRUrl { get; set; }
        public string Kategori { get; set; }

    }
    public class Acara
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime TanggalMulai { get; set; }
        public DateTime TanggalSelesai { get; set; }
        public string Keterangan { get; set; }

    }
    public class ChatMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public long ThreadId { get; set; }
        public string UserName { get; set; }
        public DateTime Tanggal { get; set; }
        public string Message { get; set; }
    }
    public class VideoStream
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string? Nama { get; set; }
        public string? Keterangan { get; set; }
        public DateTime Tanggal { get; set; }
        public string? EmbedHTML { get; set; }

    }
    public class Ngaji
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string? Nama { get; set; }
        public string? Keterangan { get; set; }
        public DateTime TanggalDari { get; set; }
        public DateTime TanggalSampai { get; set; }

        public string? StreamUrl { get; set; }
        public string? DocumentUrl { get; set; }
    }

    public class Absensi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string Nama { get; set; }
        public string PhotoUrl { get; set; }
        public long RefId { get; set; }
        public int JumlahOrang { get; set; } = 1;
        public TipeAbsensi Tipe { get; set; } = TipeAbsensi.Online;
    }
    public class Dokumen
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Required(ErrorMessage = "Isi tanggal")]
        public DateTime Tanggal { get; set; }
        [Required(ErrorMessage = "Isi nama dokumen")]
        public string Nama { get; set; }
        [Required(ErrorMessage = "Pilih kategori")]
        public string Kategori { get; set; }
        [Required(ErrorMessage = "Isi keterangan")]
        public string Keterangan { get; set; }
        public string FileUrl { get; set; }
        [Required(ErrorMessage = "Isi di upload oleh")]
        public string CreatedBy { get; set; }
    }

    public class Nasehat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        [Required(ErrorMessage = "Isi Tanggal")]
        public DateTime Tanggal { get; set; }
        [Required(ErrorMessage = "Isi Topik")]
        public string Topik { get; set; }
        [Required(ErrorMessage = "Isi Penasehat 1")]
        public string Penasehat1 { get; set; }
        [Required(ErrorMessage = "Isi Penasehat 2")]
        public string Penasehat2 { get; set; }
    }

    public class ProgramRamadan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime TanggalUpdate { get; set; }
        public string Nama { get; set; }
        //public List<TargetProgram> Capaian { get; set; }
        public ICollection<TargetProgram> TargetPrograms { get; set; }
    }

    public class TargetProgram
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public ProgramRamadan ProgramRamadan { set; get; }

        public string Nama { get; set; }
        public int Skor { get; set; }
        public bool Sukses { get; set; } = false;
    }
    public enum JenisHewanQurban { Sapi, Kambing, Kerbau, Unta }
    public class HewanQurban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { set; get; }

        public int No { set; get; }
        public string? Pemilik { set; get; }
        public double Bruto { set; get; }
        public double Netto { set; get; }
        public double PersenNet { set; get; }
        public string? KepalaSapi { set; get; }
        public int Tahun { set; get; }
        public int? NoUrutPotong { set; get; } = 0;
        public JenisHewanQurban Jenis { get; set; } = JenisHewanQurban.Sapi;
        public string? Keterangan { get; set; }

        public ICollection<DataPembagian> DataPembagians { get; set; }

    }
    public class InfoQurban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { set; get; }
        public int Tahun { set; get; }
        public DateTime TglMulai { set; get; }
        public DateTime TglSelesai { set; get; }
        public string PanitiaUrl { set; get; }
        public string InfoLainUrl { set; get; }

    }
    public class DataBL
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { set; get; }

        public int Urut { set; get; }
        public string? Nama { set; get; }
        public double BERAT { set; get; }
        public double BUNGKUS { set; get; }
        public string? KP { set; get; }
        public string? KETERANGAN { set; get; }
        public int Tahun { set; get; }
        public TipeBL Jenis { set; get; } = TipeBL.Lainnya;
        public bool SudahSiap { get; set; }
        public bool SudahDiterima { get; set; }

    }

    public enum TipeBL { Desa, Jamaah, Pejabat, Keamanan, Warga, Sampil, Lainnya }

    public class DataPembagian
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { set; get; }
        public string? KK { set; get; }
        public string? Nama { set; get; }
        public string? Status { set; get; }
        public string? Golongan { set; get; }
        public string? KP { set; get; }
        public double Pembagian { set; get; }
        public double BL { set; get; }
        public double KAKI { set; get; }
        public double KEPALA { set; get; }
        public double TULANG { set; get; }
        public double JEROHAN { set; get; }
        public double APRESIASI { set; get; }
        public int Tahun { set; get; }
        public int NoUrut { set; get; }
        public string? Kantong { set; get; }
        public bool SudahSiap { get; set; }
        public bool SudahDiterima { get; set; }

        public string? Sapi { get; set; }

        public HewanQurban? HewanQurban { get; set; }

    }

    public class CashflowQurban
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public DateTime Tanggal { get; set; }
        public string Nama { get; set; }
        public CashflowType Tipe { get; set; } = CashflowType.Keluar;
        public string Kategori { get; set; }
        public double HargaSatuan { get; set; }
        public double Quantity { get; set; }
        public double Total { get; set; }
        public string Keterangan { get; set; }
        public string DibayarOleh { get; set; }
        public string DiterimaOleh { get; set; }
        public string Satuan { get; set; }

    }
    public enum CashflowType { Masuk, Keluar };
}
