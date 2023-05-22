using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AkunSodakohKhususService : ICrud<AkunSodakohKhusus>
    {
        NgajiDB db;

        public AkunSodakohKhususService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.AkunSodakohKhususs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.AkunSodakohKhususs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<AkunSodakohKhusus> FindByKeyword(string Keyword)
        {
            var data = from x in db.AkunSodakohKhususs
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<AkunSodakohKhusus> GetAllData()
        {
            return db.AkunSodakohKhususs.ToList();
        }

        public AkunSodakohKhusus GetDataById(object Id)
        {
            return db.AkunSodakohKhususs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(AkunSodakohKhusus data)
        {
            try
            {
                db.AkunSodakohKhususs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(AkunSodakohKhusus data)
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
            return db.AkunSodakohKhususs.Max(x => x.Id);
        }
    }
}
