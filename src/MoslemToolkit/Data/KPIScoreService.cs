using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KPIScoreService : ICrud<KPIScore>
    {
        NgajiDB db;

        public KPIScoreService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KPIScores.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.KPIScores.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<KPIScore> FindByKeyword(string Keyword)
        {
            var data = from x in db.KPIScores.Include(c => c.Kpi)
                       where x.Keterangan.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<KPIScore> GetAllData()
        {
            return db.KPIScores.Include(c => c.Kpi).ToList();
        }

        public KPIScore GetDataById(object Id)
        {
            return db.KPIScores.Include(c => c.Kpi).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(KPIScore data)
        {
            try
            {
                db.KPIScores.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(KPIScore data)
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
            return db.KPIScores.Max(x => x.Id);
        }
    }

}
/*











 */
