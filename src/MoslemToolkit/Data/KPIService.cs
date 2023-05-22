using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KPIService : ICrud<KPI>
    {
        NgajiDB db;

        public KPIService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KPIs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.KPIs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<KPI> FindByKeyword(string Keyword)
        {
            var data = from x in db.KPIs.Include(c=>c.Dapukan)
                       where x.Deskripsi.Contains(Keyword)|| x.Parameter.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<KPI> GetAllData()
        {
            return db.KPIs.Include(c => c.Dapukan).ToList();
        }
        
        public List<KPI> GetDataByDapukan(string Dapukan)
        {
            return db.KPIs.Include(c => c.Dapukan).Where(x=>x.Dapukan.Nama == Dapukan).ToList();
        }

        public KPI GetDataById(object Id)
        {
            return db.KPIs.Include(c => c.Dapukan).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(KPI data)
        {
            try
            {
                db.KPIs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(KPI data)
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
            return db.KPIs.Max(x => x.Id);
        }
    }

}
/*











 */
