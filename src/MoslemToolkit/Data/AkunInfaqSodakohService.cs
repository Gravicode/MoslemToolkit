using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AkunInfaqSodakohService : ICrud<AkunInfaqSodakoh>
    {
        NgajiDB db;

        public AkunInfaqSodakohService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.AkunInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.AkunInfaqSodakohs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<AkunInfaqSodakoh> FindByKeyword(string Keyword)
        {
            var data = from x in db.AkunInfaqSodakohs
                       where x.Uraian.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<AkunInfaqSodakoh> GetAllData()
        {
            return db.AkunInfaqSodakohs.ToList();
        }
        public List<AkunInfaqSodakoh> GetNonRutin()
        {
            return db.AkunInfaqSodakohs.Where(x=>!x.Rutin).OrderBy(x=>x.Uraian).ToList();
        }

        public AkunInfaqSodakoh GetDataById(object Id)
        {
            return db.AkunInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(AkunInfaqSodakoh data)
        {
            try
            {
                db.AkunInfaqSodakohs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(AkunInfaqSodakoh data)
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
            return db.AkunInfaqSodakohs.Max(x => x.Id);
        }
    }
}
