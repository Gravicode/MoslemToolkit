using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class PengeluaranInfaqSodakohService : ICrud<PengeluaranInfaqSodakoh>
    {
        NgajiDB db;

        public PengeluaranInfaqSodakohService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.PengeluaranInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.PengeluaranInfaqSodakohs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<PengeluaranInfaqSodakoh> FindByKeyword(string Keyword)
        {
            var data = from x in db.PengeluaranInfaqSodakohs.Include(c=>c.AkunInfaqSodakoh)
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<PengeluaranInfaqSodakoh> GetAllData()
        {
            return db.PengeluaranInfaqSodakohs.Include(c => c.AkunInfaqSodakoh).OrderByDescending(x => x.Tanggal).ToList();
        }

        public PengeluaranInfaqSodakoh GetDataById(object Id)
        {
            return db.PengeluaranInfaqSodakohs.Include(c => c.AkunInfaqSodakoh).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(PengeluaranInfaqSodakoh data)
        {
            try
            {
                db.PengeluaranInfaqSodakohs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(PengeluaranInfaqSodakoh data)
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
            return db.PengeluaranInfaqSodakohs.Max(x => x.Id);
        }
    }
}
