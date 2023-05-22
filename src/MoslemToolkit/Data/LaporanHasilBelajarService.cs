using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class LaporanHasilBelajarService : ICrud<LaporanHasilBelajar>
    {
        NgajiDB db;

        public LaporanHasilBelajarService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.LaporanHasilBelajars.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.LaporanHasilBelajars.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<LaporanHasilBelajar> FindByKeyword(string Keyword)
        {
            var data = from x in db.LaporanHasilBelajars.Include(c=>c.Jamaah)
                       where x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<LaporanHasilBelajar> GetAllData()
        {
            return db.LaporanHasilBelajars.ToList();
        }

        public LaporanHasilBelajar GetDataById(object Id)
        {
            return db.LaporanHasilBelajars.Where(x => x.Id == (long)Id).FirstOrDefault();
        }

        public LaporanHasilBelajar GetData(long KelasId, int Tahun, int Semester, long JamaahId)
        {
            return db.LaporanHasilBelajars.Include(c=>c.Jamaah).Include(c=>c.Kelas).Where(c => c.KelasId == KelasId && c.Tahun == Tahun && c.Semester == Semester && c.JamaahId == JamaahId).FirstOrDefault();

        }

        public bool InsertData(LaporanHasilBelajar data)
        {
            try
            {
                db.LaporanHasilBelajars.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(LaporanHasilBelajar data)
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
            return db.LaporanHasilBelajars.Max(x => x.Id);
        }
    }
}
