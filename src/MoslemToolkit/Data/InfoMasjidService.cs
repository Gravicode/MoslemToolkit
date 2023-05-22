using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class InfoMasjidService : ICrud<InfoMasjid>
    {
        NgajiDB db;

        public InfoMasjidService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.InfoMasjids.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.InfoMasjids.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<InfoMasjid> FindByKeyword(string Keyword)
        {
            var data = from x in db.InfoMasjids
                       where x.KodeMasjid.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<InfoMasjid> GetAllData()
        {
            return db.InfoMasjids.ToList();
        } 
        public InfoMasjid GetData(string Kode)
        {
            if (string.IsNullOrEmpty(Kode)) 
                return db.InfoMasjids.FirstOrDefault();
            else
                return db.InfoMasjids.Where(x=>x.KodeMasjid == Kode).FirstOrDefault();
        }

        public InfoMasjid GetDataById(object Id)
        {
            return db.InfoMasjids.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(InfoMasjid data)
        {
            try
            {
                db.InfoMasjids.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(InfoMasjid data)
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
            return db.InfoMasjids.Max(x => x.Id);
        }
    }

}
/*











 */
