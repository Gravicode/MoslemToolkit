using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class Musyawarah5UnsurService : ICrud<Musyawarah5Unsur>
    {
        NgajiDB db;

        public Musyawarah5UnsurService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Musyawarah5Unsurs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Musyawarah5Unsurs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Musyawarah5Unsur> FindByKeyword(string Keyword)
        {
            var data = from x in db.Musyawarah5Unsurs
                       where x.Nama.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Musyawarah5Unsur> GetAllData()
        {
            return db.Musyawarah5Unsurs.ToList();
        }

        public Musyawarah5Unsur GetDataById(object Id)
        {
            return db.Musyawarah5Unsurs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Musyawarah5Unsur data)
        {
            try
            {
                db.Musyawarah5Unsurs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }



        public bool UpdateData(Musyawarah5Unsur data)
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
            return db.Musyawarah5Unsurs.Max(x => x.Id);
        }
    }
}
