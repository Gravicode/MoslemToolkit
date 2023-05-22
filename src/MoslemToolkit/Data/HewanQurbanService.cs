using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class HewanQurbanService : ICrud<HewanQurban>
    {
        NgajiDB db;

        public HewanQurbanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.HewanQurbans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.HewanQurbans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<HewanQurban> FindByKeyword(string Keyword)
        {
            var data = from x in db.HewanQurbans
                       where x.Pemilik.Contains(Keyword)
                       select x;
            return data.ToList();
        }
        public bool ClearData(int tahun)
        {
            try
            {
                foreach (var item in db.HewanQurbans.Where(x => x.Tahun == tahun).ToList())
                {
                    db.HewanQurbans.Remove(item);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }
        public List<HewanQurban> GetAllData()
        {
            return db.HewanQurbans.ToList();
        }
        public List<HewanQurban> GetAllData(int tahun)
        {
            return db.HewanQurbans.Where(x=>x.Tahun==tahun).ToList();
        }

        public HewanQurban GetDataById(object Id)
        {
            return db.HewanQurbans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(HewanQurban data)
        {
            try
            {
                db.HewanQurbans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(HewanQurban data)
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
            return db.HewanQurbans.Max(x => x.Id);
        }
    }

}
