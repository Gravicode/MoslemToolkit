using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AsetService : ICrud<Aset>
    {
        NgajiDB db;

        public AsetService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Asets.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Asets.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Aset> FindByKeyword(string Keyword)
        {
            var data = from x in db.Asets
                       where x.NamaBarang.Contains(Keyword) || x.Merek.Contains(Keyword) || x.Lokasi.Contains(Keyword) || x.NoSertifikat.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Aset> GetAllData()
        {
            return db.Asets.ToList();
        }

        public Aset GetDataById(object Id)
        {
            return db.Asets.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Aset data)
        {
            try
            {
                db.Asets.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Aset data)
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
            return db.Asets.Max(x => x.Id);
        }
    }
}
