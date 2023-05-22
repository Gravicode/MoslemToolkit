using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class TimService : ICrud<Tim>
    {
        NgajiDB db;

        public TimService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Tims.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Tims.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Tim> FindByKeyword(string Keyword)
        {
            var data = from x in db.Tims
                       where x.Nama.Contains(Keyword) || x.Alamat.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Tim> GetAllData()
        {
            return db.Tims.ToList();
        }

        public Tim GetDataById(object Id)
        {
            return db.Tims.Where(x => x.Id == (long)Id).FirstOrDefault();
        } 
        
        public Tim GetByDapukan(string Dapukan)
        {
            return db.Tims.Where(x => x.Tugas.Contains(Dapukan)).FirstOrDefault();
        }


        public bool InsertData(Tim data)
        {
            try
            {
                db.Tims.Add(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }



        public bool UpdateData(Tim data)
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
            return db.Tims.Max(x => x.Id);
        }
    }
}
