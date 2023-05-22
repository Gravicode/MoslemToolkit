using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KPIBatchService : ICrud<KPIBatch>
    {
        NgajiDB db;

        public KPIBatchService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KPIBatches.Where(x => x.BatchNo == (int)Id).FirstOrDefault());
            db.KPIBatches.Remove(selData);
            db.SaveChanges();
            return true;
        }
         public bool Exists(int BatchNo)
        {
            var selData = db.KPIBatches.Any(x => x.BatchNo == BatchNo);
            return selData;
        }

        public List<KPIBatch> FindByKeyword(string Keyword)
        {
            var data = from x in db.KPIBatches
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<KPIBatch> GetAllData()
        {
            return db.KPIBatches.ToList();
        }

        public KPIBatch GetDataById(object Id)
        {
            return db.KPIBatches.Where(x => x.BatchNo == (int)Id).FirstOrDefault();
        }


        public bool InsertData(KPIBatch data)
        {
            try
            {
                db.KPIBatches.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(KPIBatch data)
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

        public int GetLastId()
        {
            if (db.KPIBatches.Count() <= 0) return 0;
            return db.KPIBatches.Max(x => x.BatchNo);
        }

        long ICrud<KPIBatch>.GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
/*











 */
