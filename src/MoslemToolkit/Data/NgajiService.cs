using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class NgajiService : ICrud<Ngaji>
    {
        NgajiDB db;

        public NgajiService()
        {
            if (db == null) db = new NgajiDB();

        }
        public Nasehat GetNasehatHariIni(DateTime date)
        {
            var sel = db.Nasehats.Where(x => x.Tanggal.Day == date.Day && x.Tanggal.Month == date.Month &&
            x.Tanggal.Year == date.Year).FirstOrDefault();
            return sel;
        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Ngajis.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Ngajis.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Ngaji> FindByKeyword(string Keyword)
        {
            var data = from x in db.Ngajis
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Ngaji> GetAllData()
        {
            return db.Ngajis.ToList();
        }
       
        public Ngaji GetDataById(object Id)
        {
            return db.Ngajis.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Ngaji data)
        {
            try
            {
                db.Ngajis.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }


        public bool UpdateData(Ngaji data)
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
            return db.Ngajis.Max(x => x.Id);
        }
    }
}
