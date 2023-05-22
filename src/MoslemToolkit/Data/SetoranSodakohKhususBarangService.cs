using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class SetoranSodakohKhususBarangService : ICrud<SetoranSodakohKhususBarang>
    {
        NgajiDB db;

        public SetoranSodakohKhususBarangService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.SetoranSodakohKhususBarangs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.SetoranSodakohKhususBarangs.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<SetoranSodakohKhususBarang> FindByKeyword(string Keyword)
        {
            var data = from x in db.SetoranSodakohKhususBarangs.Include(c => c.AkunSodakohKhusus).Include(c => c.Jamaah)
                       where x.NamaPenyetor.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<SetoranSodakohKhususBarang> GetAllData()
        {
            return db.SetoranSodakohKhususBarangs.Include(c => c.AkunSodakohKhusus).Include(c => c.Jamaah).ToList();
        }

        public SetoranSodakohKhususBarang GetDataById(object Id)
        {
            return db.SetoranSodakohKhususBarangs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(SetoranSodakohKhususBarang data)
        {
            try
            {
                db.SetoranSodakohKhususBarangs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(SetoranSodakohKhususBarang data)
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
            return db.SetoranSodakohKhususBarangs.Max(x => x.Id);
        }
    }
}
