using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class DapukanService : ICrud<Dapukan>
    {
        NgajiDB db;

        public DapukanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Dapukans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Dapukans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Dapukan> FindByKeyword(string Keyword)
        {
            var data = from x in db.Dapukans
                       where x.Nama.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<Dapukan> GetAllData()
        {
            return db.Dapukans.OrderBy(x=>x.OrderNo).ToList();
        }

        public Dapukan GetDataById(object Id)
        {
            return db.Dapukans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Dapukan data)
        {
            try
            {
                db.Dapukans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }



        public bool UpdateData(Dapukan data)
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
            return db.Dapukans.Max(x => x.Id);
        }
    }
}
