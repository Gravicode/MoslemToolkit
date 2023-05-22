using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class TargetPerKelasService : ICrud<TargetPerKelas>
    {
        NgajiDB db;

        public TargetPerKelasService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.TargetPerKelass.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.TargetPerKelass.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<TargetPerKelas> FindByKeyword(string Keyword)
        {
            var data = from x in db.TargetPerKelass.Include(c => c.Materi)
                       where x.Materi.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<TargetPerKelas> GetAllData()
        {
            return db.TargetPerKelass.Include(c => c.Materi).Include(c => c.Materi.Bab).AsNoTracking().ToList();
        }
        public List<TargetPerKelas> GetAllData(long KelasId, int Tahun, int Semester)
        {
            return db.TargetPerKelass.Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.KelasId == KelasId
            && x.Tahun == Tahun && x.Semester == Semester).OrderBy(x => x.Materi.Bab.No).ThenBy(x => x.Materi.No).ToList();
        }

        public List<TargetPerKelas> GetDataByKelasId(long kelasId, DateTime? date = null)
        {
            if (kelasId < 0)
                return GetAllData();
            else
                if (date.HasValue)
                return db.TargetPerKelass.Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.KelasId == kelasId && x.Tahun == date.Value.Year && x.Semester == DateHelper.GetSemesterFromDate(date.Value)).AsNoTracking().ToList();
            else
                return db.TargetPerKelass.Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.KelasId == kelasId).AsNoTracking().ToList();
        }

        public TargetPerKelas GetDataById(object Id)
        {
            return db.TargetPerKelass.Include(c => c.Kelas).Include(c => c.Materi).Include(c => c.Materi.Bab).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(TargetPerKelas data)
        {
            try
            {
                db.TargetPerKelass.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool UpdateData(Kelas kelas, List<TargetMateriTemp> materis, int semester, int tahun)
        {
            try
            {
                //materi baru
                var materihash = new HashSet<long>();
                materis.ForEach(x => { materihash.Add(x.Id); });
                //remove all data
                var removed = from x in db.TargetPerKelass
                              where x.KelasId == kelas.Id && !materihash.Contains(x.MateriId)
                              select x;
                foreach (var item in removed)
                {
                    db.Remove(item);
                }
                db.SaveChanges();
                //update data
                var updated = from x in db.TargetPerKelass
                              where x.KelasId == kelas.Id && materihash.Contains(x.MateriId)
                              select x;
                foreach(var item in updated)
                {
                    var sel = materis.Where(x => x.Id == item.MateriId).FirstOrDefault();
                    item.TargetNilai = sel.TargetNilai;
                    item.TargetDeskripsi = sel.TargetDeskripsi;
                }
                //add data
                var exists = (from x in db.TargetPerKelass
                              where x.KelasId == kelas.Id && materihash.Contains(x.MateriId)
                              select x).ToList();
                var materiexist = new HashSet<long>();
                int counter = 1;
                exists.ForEach(x => { materiexist.Add(x.MateriId); x.No = counter++; });
                foreach (var mat in materis)
                {
                    if (!materiexist.Contains(mat.Id))
                    {
                        var newItem = new TargetPerKelas() { No = counter++, TargetDeskripsi = mat.TargetDeskripsi, TargetNilai = mat.TargetNilai, KelasId = kelas.Id, MateriId = mat.Id, Semester = semester, Tahun = tahun };
                        db.TargetPerKelass.Add(newItem);
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
        public bool UpdateData(TargetPerKelas data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

               
                return true;
            }
            catch
            {

            }
            return false;
        }
        public bool UpdateDatas(List<TargetPerKelas> data)
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
            return db.TargetPerKelass.Max(x => x.Id);
        }
    }
}
