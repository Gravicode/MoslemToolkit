using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class MateriService : ICrud<Materi>
    {
        NgajiDB db;

        public MateriService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Materis.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Materis.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Materi> FindByKeyword(string Keyword)
        {
            var data = from x in db.Materis
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Materi> GetAllData()
        {
            return db.Materis.Include(c=>c.Bab).ToList();
        }

        public Materi GetDataById(object Id)
        {
            return db.Materis.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Materi data)
        {
            try
            {
                db.Materis.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Materi data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                //db.ChangeTracker.DetectChanges();
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
            return db.Materis.Max(x => x.Id);
        }
    }
}
