using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class DokumenService : ICrud<Dokumen>
    {
        NgajiDB db;

        public DokumenService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Dokumens.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Dokumens.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Dokumen> FindByKeyword(string Keyword)
        {
            var data = from x in db.Dokumens
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Dokumen> GetAllData()
        {
            return db.Dokumens.ToList();
        }

        public Dokumen GetDataById(object Id)
        {
            return db.Dokumens.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Dokumen data)
        {
            try
            {
                db.Dokumens.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

       

        public bool UpdateData(Dokumen data)
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
            return db.Dokumens.Max(x => x.Id);
        }
    }
}
