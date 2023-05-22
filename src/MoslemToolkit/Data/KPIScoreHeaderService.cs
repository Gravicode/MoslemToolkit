using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KPIScoreHeaderService : ICrud<KPIScoreHeader>
    {
        NgajiDB db;

        public KPIScoreHeaderService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KPIScoreHeaders.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.KPIScoreHeaders.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<KPIScoreHeader> FindByKeyword(string Keyword)
        {
            var data = from x in db.KPIScoreHeaders.Include(c => c.KPIBatch).Include(c => c.KPIScores)
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<KPIScoreHeader> GetAllData()
        {
            return db.KPIScoreHeaders.Include(c => c.KPIBatch).Include(c => c.KPIScores).ToList();
        }

        public KPIScoreHeader GetDataById(object Id)
        {
            return db.KPIScoreHeaders.Include(c => c.KPIBatch).Include(c => c.KPIScores).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(KPIScoreHeader data)
        {
            try
            {
                db.KPIScoreHeaders.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(KPIScoreHeader data)
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
            return db.KPIScoreHeaders.Max(x => x.Id);
        }
    }

}
/*











 */
