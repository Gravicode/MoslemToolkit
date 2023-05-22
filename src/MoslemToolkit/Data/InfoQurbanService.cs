using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class InfoQurbanService : ICrud<InfoQurban>
    {
        NgajiDB db;

        public InfoQurbanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.InfoQurbans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.InfoQurbans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<InfoQurban> FindByKeyword(string Keyword)
        {
            var data = from x in db.InfoQurbans
                       where x.Tahun.ToString().Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<InfoQurban> GetAllData()
        {
            return db.InfoQurbans.ToList();
        }
          public InfoQurban GetData(int Tahun)
        {
            return db.InfoQurbans.Where(x=>x.Tahun == Tahun).FirstOrDefault();
        }

        public InfoQurban GetDataById(object Id)
        {
            return db.InfoQurbans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(InfoQurban data)
        {
            try
            {
                db.InfoQurbans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(InfoQurban data)
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
            return db.InfoQurbans.Max(x => x.Id);
        }
    }

}
/*











 */
