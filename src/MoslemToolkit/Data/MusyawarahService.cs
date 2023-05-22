using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class MusyawarahService : ICrud<Musyawarah>
    {
        NgajiDB db;

        public MusyawarahService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Musyawarahs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Musyawarahs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Musyawarah> FindByKeyword(string Keyword)
        {
            var data = from x in db.Musyawarahs
                       where x.Topik.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Musyawarah> GetAllData()
        {
            return db.Musyawarahs.ToList();
        }

        public Musyawarah GetDataById(object Id)
        {
            return db.Musyawarahs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Musyawarah data)
        {
            try
            {
                db.Musyawarahs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {

            }
            return false;
        }



        public bool UpdateData(Musyawarah data)
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
            return db.Musyawarahs.Max(x => x.Id);
        }
    }
}
