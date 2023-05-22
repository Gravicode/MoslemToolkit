using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AnekdotService : ICrud<Anekdot>
    {
        NgajiDB db;

        public AnekdotService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Anekdots.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Anekdots.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Anekdot> FindByKeyword(string Keyword)
        {
            var data = from x in db.Anekdots
                       where x.Permasalahan.Contains(Keyword) || x.Pendidik.Contains(Keyword) || x.Solusi.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Anekdot> GetAllData()
        {
            return db.Anekdots.Include(x => x.Kelas).ToList();
        }
        public List<Anekdot> GetAllData(DateTime Start,DateTime End,long KelasId)
        {
            return db.Anekdots.Include(x => x.Kelas).Where(x=>x.Tanggal >= Start && x.Tanggal <= End && x.KelasId == KelasId).ToList();
        }

        public Anekdot GetDataById(object Id)
        {
            return db.Anekdots.Include(c => c.Kelas).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Anekdot data)
        {
            try
            {
                db.Anekdots.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(Anekdot data)
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
            return db.Anekdots.Max(x => x.Id);
        }
    }
}
