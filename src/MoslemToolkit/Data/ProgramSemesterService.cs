using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class ProgramSemesterService : ICrud<ProgramSemester>
    {
        NgajiDB db;

        public ProgramSemesterService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.ProgramSemesters.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.ProgramSemesters.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<ProgramSemester> FindByKeyword(string Keyword)
        {
            var data = from x in db.ProgramSemesters.Include(c=>c.MateriPerKelas.Materi)
                       where x.MateriPerKelas.Materi.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<ProgramSemester> GetAllData()
        {
            return db.ProgramSemesters.ToList();
        }
        
        public List<ProgramSemester> GetAllData(long KelasId, int Tahun, int Semester)
        {
            var selMatKel = db.MateriPerKelass.Include(c=>c.Materi).Where(x=>x.KelasId == KelasId && 
            x.Tahun == Tahun && x.Semester == Semester).ToList();
          
            if (selMatKel != null)
            {
                var hashMateri = new HashSet<long>();
                selMatKel.ForEach(x => { hashMateri.Add(x.Id); });
                return db.ProgramSemesters.Where(x => hashMateri.Contains( x.MateriPerKelasId)).ToList();
            }
            return null;
        } 
        public bool RemoveData(long KelasId, int Tahun, int Semester)
        {
            var selMatKel = db.MateriPerKelass.Include(c=>c.Materi).Where(x=>x.KelasId == KelasId && 
            x.Tahun == Tahun && x.Semester == Semester).FirstOrDefault();
            if (selMatKel != null)
            {
                var datas = db.ProgramSemesters.Where(x => x.MateriPerKelasId == selMatKel.Id).ToList();
                foreach(var item in datas)
                {
                    db.ProgramSemesters.Remove(item);
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool InsertDatas(List<ProgramSemesterItem> datas)
        {
            try
            {
                foreach (var item in datas)
                {
                    if(item.Checked)
                        db.ProgramSemesters.Add(new ProgramSemester() { Bulan = item.Bulan, MateriPerKelasId=item.MateriKelasId, Minggu = item.Minggu });
                }
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public ProgramSemester GetDataById(object Id)
        {
            return db.ProgramSemesters.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(ProgramSemester data)
        {
            try
            {
                db.ProgramSemesters.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(ProgramSemester data)
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
            return db.ProgramSemesters.Max(x => x.Id);
        }
    }
}
