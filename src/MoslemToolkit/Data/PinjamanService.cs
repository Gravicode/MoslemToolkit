using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class PinjamanService : ICrud<Pinjaman>
    {
        NgajiDB db;

        public PinjamanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Pinjamans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Pinjamans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Pinjaman> FindByKeyword(string Keyword)
        {
            var data = from x in db.Pinjamans.Include(c=>c.Cicilans)
                       where x.Keterangan.Contains(Keyword) || x.Nama.Contains(Keyword) || x.Peminjam.Contains(Keyword) || x.PemberiPinjaman.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Pinjaman> GetAllData()
        {
            return db.Pinjamans.Include(c => c.Cicilans).ToList();
        }

        public Pinjaman GetDataById(object Id)
        {
            return db.Pinjamans.Include(c => c.Cicilans).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Pinjaman data)
        {
            try
            {
                db.Pinjamans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Pinjaman data)
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
            return db.Pinjamans.Max(x => x.Id);
        }
    }

}
/*











 */
