using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class CicilanService : ICrud<Cicilan>
    {
        NgajiDB db;

        public CicilanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Cicilans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Cicilans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Cicilan> FindByKeyword(string Keyword)
        {
            var data = from x in db.Cicilans
                       where x.Keterangan.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<Cicilan> GetAllData()
        {
            return db.Cicilans.ToList();
        }

        public Cicilan GetDataById(object Id)
        {
            return db.Cicilans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Cicilan data)
        {
            try
            {
                db.Cicilans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Cicilan data)
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
            return db.Cicilans.Max(x => x.Id);
        }
    }

}
/*











 */
