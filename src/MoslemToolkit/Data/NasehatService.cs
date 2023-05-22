using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class NasehatService : ICrud<Nasehat>
    {
        NgajiDB db;

        public NasehatService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Nasehats.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Nasehats.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Nasehat> FindByKeyword(string Keyword)
        {
            var data = from x in db.Nasehats
                       where x.Topik.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Nasehat> GetAllData()
        {
            return db.Nasehats.ToList();
        }
        public List<Nasehat> GetAllData(DateTime start, DateTime end)
        {
            return db.Nasehats.Where(x=>x.Tanggal>=start && x.Tanggal <=end).OrderBy(x=>x.Tanggal).ToList();
        }

        public Nasehat GetDataById(object Id)
        {
            return db.Nasehats.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Nasehat data)
        {
            try
            {
                db.Nasehats.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        

        public bool UpdateData(Nasehat data)
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
        public Nasehat GetNasehatHariIni(DateTime date)
        {
            var sel = db.Nasehats.Where(x => x.Tanggal.Day == date.Day && x.Tanggal.Month == date.Month &&
            x.Tanggal.Year == date.Year).FirstOrDefault();
            return sel;
        }
        public long GetLastId()
        {
            return db.Nasehats.Max(x => x.Id);
        }
    }
}
