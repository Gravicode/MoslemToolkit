using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class BLService : ICrud<DataBL>
    {
        NgajiDB db;

        public BLService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.DataBLs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.DataBLs.Remove(selData);
            db.SaveChanges();
            return true;
        }
        public bool ClearData(int tahun)
        {
            try
            {
                foreach (var item in db.DataBLs.Where(x => x.Tahun == tahun).ToList())
                {
                    db.DataBLs.Remove(item);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }
        public List<DataBL> FindByKeyword(string Keyword)
        {
            var data = from x in db.DataBLs
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<DataBL> GetAllData()
        {
            return db.DataBLs.ToList();
        }
         public List<DataBL> GetAllData(int Tahun)
        {
            return db.DataBLs.Where(x=>x.Tahun == Tahun).ToList();
        }

        public DataBL GetDataById(object Id)
        {
            return db.DataBLs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(DataBL data)
        {
            try
            {
                db.DataBLs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

       

        public bool UpdateData(DataBL data)
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
            return db.DataBLs.Max(x => x.Id);
        }
    }
}
