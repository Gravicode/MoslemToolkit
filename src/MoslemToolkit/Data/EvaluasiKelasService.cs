using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class EvaluasiKelasService : ICrud<EvaluasiKelas>
    {
        NgajiDB db;

        public EvaluasiKelasService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.EvaluasiKelass.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.EvaluasiKelass.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<EvaluasiKelas> FindByKeyword(string Keyword)
        {
            var data = from x in db.EvaluasiKelass.Include(c=>c.Jamaah)
                       where x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<EvaluasiKelas> GetAllData()
        {
            return db.EvaluasiKelass.ToList();
        } 
        public List<EvaluasiKelas> GetAllData(long KelasId,int Tahun, int Semester, long JamaahId)
        {
            return db.EvaluasiKelass.Include(c=>c.MateriPerKelas.Materi.Bab).Include(c=>c.Jamaah)
                .Where(x=>x.JamaahId == JamaahId && x.MateriPerKelas.KelasId==KelasId && x.MateriPerKelas.Tahun == Tahun && x.MateriPerKelas.Semester == Semester).ToList();
        } 
        public List<EvaluasiKelas> GetAllData(long KelasId,int Tahun, int Semester)
        {
            return db.EvaluasiKelass.Include(c=>c.MateriPerKelas.Materi.Bab).Include(c=>c.Jamaah)
                .Where(x=> x.MateriPerKelas.KelasId==KelasId && x.MateriPerKelas.Tahun == Tahun && x.MateriPerKelas.Semester == Semester).OrderBy(x=>x.MateriPerKelas.Materi.Bab.No).ThenBy(x=>x.MateriPerKelas.No).ToList();
        }

        public List<EvaluasiKelas> GetAllData(long KelasId, int Tahun)
        {
            return db.EvaluasiKelass.Include(c => c.MateriPerKelas.Materi.Bab).Include(c => c.Jamaah)
                .Where(x => x.MateriPerKelas.KelasId == KelasId && x.MateriPerKelas.Tahun == Tahun ).OrderBy(x => x.MateriPerKelas.Materi.Bab.No).ThenBy(x => x.MateriPerKelas.No).ToList();
        }
        public bool InsertDatas(List<EvaluasiKelas> datas)
        {
            try
            {
                foreach (var item in datas)
                {
                    item.MateriPerKelas = null;
                    db.EvaluasiKelass.Add(item);
                }
                db.SaveChanges();
                
                return true;
            }
            catch( Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        public bool UpdateDatas(List<EvaluasiKelas> data)
        {
            try
            {
                foreach (var item in data)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();


                return true;
            }
            catch
            {

            }
            return false;
        }
        public EvaluasiKelas GetDataById(object Id)
        {
            return db.EvaluasiKelass.Where(x => x.Id == (long)Id).FirstOrDefault();
        }
        public bool RemoveData(long KelasId, int Tahun, int Semester,long JamaahId)
        {
            var selMatkel = db.MateriPerKelass.Where(x => x.KelasId == KelasId &&
            x.Tahun == Tahun && x.Semester == Semester).FirstOrDefault();
            if (selMatkel != null)
            {
                var removed = db.EvaluasiKelass.Where(x => x.MateriPerKelasId == selMatkel.Id
                && x.JamaahId == JamaahId).ToList();
                foreach(var item in removed)
                {
                    db.EvaluasiKelass.Remove(item);
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

            public bool InsertData(EvaluasiKelas data)
        {
            try
            {
                db.EvaluasiKelass.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(EvaluasiKelas data)
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
            return db.EvaluasiKelass.Max(x => x.Id);
        }
    }
}
