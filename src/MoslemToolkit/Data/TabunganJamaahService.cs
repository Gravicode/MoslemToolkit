using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class TabunganJamaahService : ICrud<TabunganJamaah>
    {
        NgajiDB db;

        public TabunganJamaahService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.TabunganJamaahs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.TabunganJamaahs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<TabunganJamaah> FindByKeyword(string Keyword)
        {
            var data = from x in db.TabunganJamaahs
                       where x.NamaJamaah.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<TabunganJamaah> GetAllData()
        {
            return db.TabunganJamaahs.Include(c => c.AkunTabunganJamaah).Include(c=>c.Jamaah).ToList();
        }

        public TabunganJamaah GetDataById(object Id)
        {
            return db.TabunganJamaahs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(TabunganJamaah data)
        {
            try
            {
                db.TabunganJamaahs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(TabunganJamaah data)
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
            return db.TabunganJamaahs.Max(x => x.Id);
        }
    }
}
