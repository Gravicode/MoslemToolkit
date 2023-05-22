using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class KalenderPendidikanService : ICrud<KalenderPendidikan>
    {
        NgajiDB db;

        public KalenderPendidikanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.KalenderPendidikans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.KalenderPendidikans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<KalenderPendidikan> FindByKeyword(string Keyword)
        {
            var data = from x in db.KalenderPendidikans
                       where x.Kegiatan.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<KalenderPendidikan> GetAllData()
        {
            return db.KalenderPendidikans.ToList();
        }  
        
        public List<KalenderPendidikan> GetAllData(DateTime Start,DateTime End)
        {
            return db.KalenderPendidikans.Where(x=>x.TanggalMulai >= Start && x.TanggalMulai <= End).ToList();
        }

        public KalenderPendidikan GetDataById(object Id)
        {
            return db.KalenderPendidikans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(KalenderPendidikan data)
        {
            try
            {
                db.KalenderPendidikans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(KalenderPendidikan data)
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
            return db.KalenderPendidikans.Max(x => x.Id);
        }
    }
}
