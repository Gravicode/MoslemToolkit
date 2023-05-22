using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class NgajiDB : DbContext
    {

        public NgajiDB()
        {
        }

        public NgajiDB(DbContextOptions<NgajiDB> options)
            : base(options)
        {
        }
        public DbSet<TargetPerKelas> TargetPerKelass { get; set; }
        public DbSet<SiswaPerKelas> SiswaPerKelass { get; set; }
        public DbSet<CashflowQurban> CashflowQurbans { get; set; }
        public DbSet<HargaBerasZakat> HargaBerasZakats { get; set; }
        public DbSet<MosqueAlarm> MosqueAlarms { get; set; }
        public DbSet<KPI> KPIs { get; set; }
        public DbSet<KPIScoreHeader> KPIScoreHeaders { get; set; }
        public DbSet<KPIScore> KPIScores { get; set; }
        public DbSet<KPIBatch> KPIBatches { get; set; }
        public DbSet<Musyawarah5Unsur> Musyawarah5Unsurs { get; set; }
        public DbSet<AkunInfaqSodakoh> AkunInfaqSodakohs { get; set; }
        public DbSet<HeaderPenerimaanInfaqSodakoh> HeaderPenerimaanInfaqSodakohs { get; set; }
        public DbSet<PenerimaanInfaqSodakoh> PenerimaanInfaqSodakohs { get; set; }
        public DbSet<PengeluaranInfaqSodakoh> PengeluaranInfaqSodakohs { get; set; }
        public DbSet<ProfilInfaqJamaah> ProfilInfaqJamaahs { get; set; }
        public DbSet<AkunSodakohKhusus> AkunSodakohKhususs { get; set; }
        public DbSet<SetoranSodakohKhusus> SetoranSodakohKhususs { get; set; }
        public DbSet<SetoranSodakohKhususBarang> SetoranSodakohKhususBarangs { get; set; }
        public DbSet<AkunTabunganJamaah> AkunTabunganJamaahs { get; set; }
        public DbSet<TabunganJamaah> TabunganJamaahs { get; set; }

        public DbSet<Cicilan> Cicilans { get; set; }
        public DbSet<Pinjaman> Pinjamans { get; set; }
        public DbSet<InfoMasjid> InfoMasjids { get; set; }
        public DbSet<LaporanHasilBelajar> LaporanHasilBelajars { get; set; }
        public DbSet<EvaluasiKelas> EvaluasiKelass { get; set; }
        public DbSet<KalenderPendidikan> KalenderPendidikans { get; set; }
        public DbSet<KalenderKelompok> KalenderKelompoks { get; set; }
        public DbSet<ProgramSemester> ProgramSemesters { get; set; }
        public DbSet<Bab> Babs { get; set; }
        public DbSet<Materi> Materis { get; set; }
        public DbSet<Kelas> Kelass { get; set; }
        public DbSet<Anekdot> Anekdots { get; set; }
        public DbSet<MateriPerKelas> MateriPerKelass { get; set; }
        public DbSet<NilaiSiswa> NilaiSiswas { get; set; }
        public DbSet<AbsensiSiswa> AbsensiSiswas { get; set; }

        public DbSet<StateStorage> StateStorages { get; set; }
        public DbSet<Mustahik> Mustahiks { get; set; }
        public DbSet<Muzaki> Muzakis { get; set; }
        public DbSet<Asnab> Asnabs { get; set; }
        public DbSet<ProsentaseZakat> ProsentaseZakats { get; set; }
        public DbSet<SmsBroadcast> SmsBroadcasts { get; set; }
        public DbSet<Aset> Asets { get; set; }
        public DbSet<Pengumuman> Pengumumans { get; set; }
        public DbSet<Dapukan> Dapukans { get; set; }
        public DbSet<Tim> Tims { get; set; }
        public DbSet<Jamaah> Jamaahs { get; set; }
        public DbSet<Acara> Acaras { get; set; }
        public DbSet<Musyawarah> Musyawarahs { get; set; }
        public DbSet<BuktiTransfer> BuktiTransfers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<VideoStream> VideoStreams { get; set; }
        public DbSet<Ngaji> Ngajis { get; set; }
        public DbSet<Absensi> Absensis { get; set; }
        public DbSet<Dokumen> Dokumens { get; set; }
        public DbSet<Nasehat> Nasehats { get; set; }
        public DbSet<ProgramRamadan> ProgramRamadans { get; set; }
        public DbSet<TargetProgram> TargetPrograms { get; set; }
        public DbSet<InfoQurban> InfoQurbans { get; set; }
        public DbSet<HewanQurban> HewanQurbans { get; set; }
        public DbSet<DataBL> DataBLs { get; set; }
        public DbSet<DataPembagian> DataPembagians { get; set; }
        public DbSet<Sodakoh> Sodakohs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*
            builder.Entity<DataEventRecord>().HasKey(m => m.DataEventRecordId);
            builder.Entity<SourceInfo>().HasKey(m => m.SourceInfoId);

            // shadow properties
            builder.Entity<DataEventRecord>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<SourceInfo>().Property<DateTime>("UpdatedTimestamp");
            */
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            /*
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<SourceInfo>();
            updateUpdatedProperty<DataEventRecord>();
            */
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(AppConstants.SQLConn, ServerVersion.AutoDetect(AppConstants.SQLConn));
            }
        }
        private void updateUpdatedProperty<T>() where T : class
        {
            /*
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
            */
        }

    }
}
