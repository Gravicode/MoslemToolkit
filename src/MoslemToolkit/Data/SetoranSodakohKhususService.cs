using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class SetoranSodakohKhususService : ICrud<SetoranSodakohKhusus>
    {
        NgajiDB db;

        public SetoranSodakohKhususService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.SetoranSodakohKhususs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.SetoranSodakohKhususs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<SetoranSodakohKhusus> FindByKeyword(string Keyword)
        {
            var data = from x in db.SetoranSodakohKhususs.Include(c=>c.AkunSodakohKhusus).Include(c=>c.Jamaah)
                       where x.NamaPenyetor.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<SetoranSodakohKhusus> GetAllData()
        {
            return db.SetoranSodakohKhususs.Include(c => c.AkunSodakohKhusus).Include(c => c.Jamaah).ToList();
        }

        public SetoranSodakohKhusus GetDataById(object Id)
        {
            return db.SetoranSodakohKhususs.Include(c => c.AkunSodakohKhusus).Include(c => c.Jamaah).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(SetoranSodakohKhusus data)
        {
            try
            {
                db.SetoranSodakohKhususs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(SetoranSodakohKhusus data)
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
            return db.SetoranSodakohKhususs.Max(x => x.Id);
        }
    }
}
