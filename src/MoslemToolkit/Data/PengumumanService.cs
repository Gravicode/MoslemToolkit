using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class PengumumanService : ICrud<Pengumuman>
    {
        NgajiDB db;

        public PengumumanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Pengumumans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Pengumumans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Pengumuman> FindByKeyword(string Keyword)
        {
            var data = from x in db.Pengumumans
                       where x.Judul.Contains(Keyword) || x.Isi.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Pengumuman> GetAllData()
        {
            return db.Pengumumans.ToList();
        }

        public Pengumuman GetDataById(object Id)
        {
            return db.Pengumumans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Pengumuman data)
        {
            try
            {
                db.Pengumumans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Pengumuman data)
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
            return db.Pengumumans.Max(x => x.Id);
        }
    }

}
/*











 */
