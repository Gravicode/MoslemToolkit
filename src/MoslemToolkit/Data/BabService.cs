using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class BabService : ICrud<Bab>
    {
        NgajiDB db;

        public BabService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Babs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Babs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Bab> FindByKeyword(string Keyword)
        {
            var data = from x in db.Babs
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Bab> GetAllData()
        {
            return db.Babs.ToList();
        }

        public Bab GetDataById(object Id)
        {
            return db.Babs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Bab data)
        {
            try
            {
                db.Babs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Bab data)
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
            return db.Babs.Max(x => x.Id);
        }
    }
}
