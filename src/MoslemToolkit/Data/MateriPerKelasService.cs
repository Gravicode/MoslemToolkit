using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class MateriPerKelasService : ICrud<MateriPerKelas>
    {
        NgajiDB db;

        public MateriPerKelasService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MateriPerKelass.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MateriPerKelass.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MateriPerKelas> FindByKeyword(string Keyword)
        {
            var data = from x in db.MateriPerKelass.Include(c=>c.Materi)
                       where x.Materi.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<MateriPerKelas> GetAllData()
        {
            return db.MateriPerKelass.Include(c=>c.Materi).Include(c => c.Materi.Bab).AsNoTracking().ToList();
        } 
        public List<MateriPerKelas> GetAllData(long KelasId, int Tahun, int Semester)
        {
            return db.MateriPerKelass.Include(c=>c.Materi).Include(c => c.Materi.Bab).Where(x=>x.KelasId==KelasId
            && x.Tahun == Tahun && x.Semester == Semester).OrderBy(x=>x.Materi.Bab.No).ThenBy(x=>x.Materi.No).ToList();
        }

        public List<MateriPerKelas> GetDataByKelasId(long kelasId,DateTime? date=null)
        {
            if (kelasId < 0) 
                return GetAllData();
            else
                if(date.HasValue)
                return db.MateriPerKelass.Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.KelasId == kelasId && x.Tahun == date.Value.Year && x.Semester == DateHelper.GetSemesterFromDate(date.Value)).AsNoTracking().ToList();
            else
                return db.MateriPerKelass.Include(c => c.Materi).Include(c=>c.Materi.Bab).Where(x=>x.KelasId==kelasId).AsNoTracking().ToList();
        }

        public MateriPerKelas GetDataById(object Id)
        {
            return db.MateriPerKelass.Include(c=>c.Kelas).Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(MateriPerKelas data)
        {
            try
            {
                db.MateriPerKelass.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool CheckIfMateriHasNilai(long MateriKelasId)
        {
            var data = db.MateriPerKelass.Where(x => x.Id == MateriKelasId).Include(x=>x.NilaiSiswas).FirstOrDefault();
            if(data?.NilaiSiswas!=null && data?.NilaiSiswas.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckIfMateriHasNilai(Kelas kelas, List<Materi> materis)
        {
            //materi baru
            var materihash = new HashSet<long>();
            materis.ForEach(x => { materihash.Add(x.Id); });
            //remove all data
            var removed = from x in db.MateriPerKelass.Include(x=>x.NilaiSiswas)
                          where x.KelasId == kelas.Id && !materihash.Contains(x.MateriId.Value)
                          select x;
            foreach (var item in removed)
            {
                if(item?.NilaiSiswas!=null & item?.NilaiSiswas.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public bool UpdateData(Kelas kelas,List<Materi> materis,int semester,int tahun)
        {
            try
            {
                //materi baru
                var materihash = new HashSet<long>();
                materis.ForEach(x => { materihash.Add(x.Id); });
                //remove all data
                var removed = from x in db.MateriPerKelass
                               where x.KelasId == kelas.Id && !materihash.Contains(x.MateriId.Value)
                               select x;
                foreach (var item in removed)
                {
                    db.Remove(item);
                }
                db.SaveChanges();
                //add data
                var exists = (from x in db.MateriPerKelass
                              where x.KelasId == kelas.Id && materihash.Contains(x.MateriId.Value)
                              select x).ToList();
                var materiexist = new HashSet<long>();
                int counter = 1;
                exists.ForEach(x => { materiexist.Add(x.MateriId.Value);x.No = counter++; });
                foreach(var mat in materis)
                {
                    if (!materiexist.Contains(mat.Id))
                    {
                        var newItem = new MateriPerKelas() { No = counter++, IsActive = true, KelasId = kelas.Id, MateriId = mat.Id, Semester = semester, Tahun = tahun };
                        db.MateriPerKelass.Add(newItem);
                    }
                   
                }
                db.SaveChanges();

                return true;
            }
            catch
            {

            }
            return false;
        }
        public bool UpdateData(MateriPerKelas data)
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
        public bool UpdateDatas(List<MateriPerKelas> data)
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
        public long GetLastId()
        {
            return db.MateriPerKelass.Max(x => x.Id);
        }
    }
}
