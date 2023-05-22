using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KalenderKelompokService : ICrud<KalenderKelompok>
    {
        NgajiDB db;

        public KalenderKelompokService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KalenderKelompoks.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.KalenderKelompoks.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<KalenderKelompok> FindByKeyword(string Keyword)
        {
            var data = from x in db.KalenderKelompoks
                       where x.Kegiatan.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<KalenderKelompok> GetAllData()
        {
            return db.KalenderKelompoks.ToList();
        } 
       
        

        public List<KalenderKelompok> GetAllData(DateTime Start, DateTime End)
        {
            return db.KalenderKelompoks.Where(x => x.TanggalMulai >= Start && x.TanggalMulai <= End).OrderBy(x=>x.TanggalMulai).ToList();
        }

        public KalenderKelompok GetDataById(object Id)
        {
            return db.KalenderKelompoks.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(KalenderKelompok data)
        {
            try
            {
                db.KalenderKelompoks.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(KalenderKelompok data)
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
            return db.KalenderKelompoks.Max(x => x.Id);
        }
    }
}
