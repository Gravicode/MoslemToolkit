using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class NilaiSiswaService : ICrud<NilaiSiswa>
    {
        NgajiDB db;

        public NilaiSiswaService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.NilaiSiswas.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.NilaiSiswas.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<NilaiSiswa> FindByKeyword(string Keyword)
        {
            var data = from x in db.NilaiSiswas.Include(c=>c.Jamaah)
                       where x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<NilaiSiswa> GetAllData()
        {
            return db.NilaiSiswas.ToList();
        }
        public List<NilaiSiswa> GetDataByKelasMateri(DateTime Tanggal,long materiKelasId)
        {
            return db.NilaiSiswas.Include(c=>c.Jamaah).Include(c=>c.MateriPerKelas).Include(c => c.MateriPerKelas.Materi).Include(c => c.MateriPerKelas.Materi.Bab)
                .Where(x=>x.MateriPerKelasId==materiKelasId && x.Tanggal.Day == Tanggal.Day && x.Tanggal.Month == Tanggal.Month && x.Tanggal.Year == Tanggal.Year ).ToList();
        }
        public NilaiSiswa GetDataById(object Id)
        {
            return db.NilaiSiswas.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(NilaiSiswa data)
        {
            try
            {
                db.NilaiSiswas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine( ex.Message);
            }
            return false;
        }

        public SiswaStat GetStat(long JamaahId, DateTime StartDate, DateTime EndDate)
        {
            var stat = new SiswaStat();
            var dataSiswa = db.NilaiSiswas.Where(x => x.Tanggal >= StartDate && x.Tanggal <= EndDate && x.JamaahId == JamaahId);
            var tanggals = (from x in dataSiswa
                           select new DateTime(x.Tanggal.Year,x.Tanggal.Month,x.Tanggal.Day)).Distinct().ToList();
            if (tanggals.Count > 0)
            {
                foreach(var item in tanggals)
                {
                    var sel = dataSiswa.Where(x => x.Tanggal.Day == item.Day && x.Tanggal.Month == item.Month && x.Tanggal.Year == item.Year).FirstOrDefault();
                    switch (sel.Kehadiran)
                    {
                        case StatusKehadiran.Alpha:
                            stat.Alpha++;
                            break;
                        case StatusKehadiran.Hadir:
                            stat.Hadir++;
                            break;
                        case StatusKehadiran.Ijin:
                            stat.Ijin++;
                            break;
                        case StatusKehadiran.Sakit:
                            stat.Sakit++;
                            break;

                    }
                }
            }
            return stat;
        }

        public bool UpdateData(NilaiSiswa data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }
        public long GetLastId()
        {
            return db.NilaiSiswas.Max(x => x.Id);
        }
    }
}
