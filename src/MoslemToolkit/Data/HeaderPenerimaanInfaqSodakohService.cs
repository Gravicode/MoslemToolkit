using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class HeaderPenerimaanInfaqSodakohService : ICrud<HeaderPenerimaanInfaqSodakoh>
    {
        NgajiDB db;

        public HeaderPenerimaanInfaqSodakohService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.HeaderPenerimaanInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.HeaderPenerimaanInfaqSodakohs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<HeaderPenerimaanInfaqSodakoh> FindByKeyword(string Keyword)
        {
            var data = from x in db.HeaderPenerimaanInfaqSodakohs
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<HeaderPenerimaanInfaqSodakoh> GetAllData()
        {
            return db.HeaderPenerimaanInfaqSodakohs.OrderByDescending(x=>x.Tanggal).ToList();
        }

        public HeaderPenerimaanInfaqSodakoh GetDataById(object Id)
        {
            return db.HeaderPenerimaanInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(HeaderPenerimaanInfaqSodakoh data)
        {
            try
            {
                db.HeaderPenerimaanInfaqSodakohs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(HeaderPenerimaanInfaqSodakoh data)
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
            return db.HeaderPenerimaanInfaqSodakohs.Max(x => x.Id);
        }
    }
}
