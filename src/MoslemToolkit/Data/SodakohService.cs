using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class SodakohService : ICrud<Sodakoh>
    {
        NgajiDB db;

        public SodakohService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Sodakohs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Sodakohs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Sodakoh> FindByKeyword(string Keyword)
        {
            var data = from x in db.Sodakohs
                       where x.Kategori.Contains(Keyword) || x.NamaRekening.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Sodakoh> GetAllData()
        {
            return db.Sodakohs.ToList();
        }

        public Sodakoh GetDataById(object Id)
        {
            return db.Sodakohs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Sodakoh data)
        {
            try
            {
                db.Sodakohs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Sodakoh data)
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
            return db.Sodakohs.Max(x => x.Id);
        }
    }
}
