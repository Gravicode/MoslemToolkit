using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class ProfilInfaqJamaahService : ICrud<ProfilInfaqJamaah>
    {
        NgajiDB db;

        public ProfilInfaqJamaahService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.ProfilInfaqJamaahs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.ProfilInfaqJamaahs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<ProfilInfaqJamaah> FindByKeyword(string Keyword)
        {
            var data = from x in db.ProfilInfaqJamaahs.Include(c=>c.Jamaah)
                       where x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<ProfilInfaqJamaah> GetAllData()
        {
            return db.ProfilInfaqJamaahs.Include(c=>c.Jamaah).ToList();
        }

        public ProfilInfaqJamaah GetDataById(object Id)
        {
            return db.ProfilInfaqJamaahs.Include(c=>c.Jamaah).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(ProfilInfaqJamaah data)
        {
            try
            {
                db.ProfilInfaqJamaahs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(ProfilInfaqJamaah data)
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
            return db.ProfilInfaqJamaahs.Max(x => x.Id);
        }
    }
}
