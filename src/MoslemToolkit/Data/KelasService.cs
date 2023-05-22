using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KelasService : ICrud<Kelas>
    {
        NgajiDB db;

        public KelasService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Kelass.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Kelass.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Kelas> FindByKeyword(string Keyword)
        {
            var data = from x in db.Kelass
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Kelas> GetAllData()
        {
            return db.Kelass.Include(x=>x.Siswas).Include(x=>x.SiswaPerKelas).ToList();
        }
        public List<Jamaah> GetSiswaByKelas(long KelasId)
        {
            var selKelas = db.Kelass.Include(c=>c.Siswas).Where(x=>x.Id == KelasId).FirstOrDefault();
            return selKelas.Siswas.ToList();
        }

        public Kelas GetDataById(object Id)
        {
            return db.Kelass.Include(c=>c.SiswaPerKelas).ThenInclude(x=>x.Jamaah).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Kelas data)
        {
            try
            {
                db.Kelass.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Kelas data)
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
            return db.Kelass.Max(x => x.Id);
        }
    }
}
