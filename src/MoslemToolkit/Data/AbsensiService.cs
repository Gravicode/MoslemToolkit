using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AbsensiService : ICrud<Absensi>
    {
        NgajiDB db;

        public AbsensiService()
        {
            if (db == null) db = new NgajiDB();
         
        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Absensis.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Absensis.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Absensi> FindByKeyword(string Keyword)
        {
            var data = from x in db.Absensis
                       where x.Nama.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<Absensi> GetAllData()
        {
            return db.Absensis.ToList();
        }

        public List<Absensi> GetDataByDate(long AcaraId, DateTime from, DateTime to, TipeAbsensi Tipe)
        {
            if (Tipe == TipeAbsensi.Semua)
                return db.Absensis.Where(x => x.Tanggal >= from && x.Tanggal <= to && x.RefId == AcaraId).ToList();
            else
                return db.Absensis.Where(x => x.Tanggal >= from && x.Tanggal <= to && x.RefId == AcaraId && x.Tipe == Tipe).ToList();

        }

        public Absensi GetDataById(object Id)
        {
            return db.Absensis.Where(x=>x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Absensi data)
        {
            try { 
            db.Absensis.Add(data);
            db.SaveChanges();
            return true;
        }
            catch
            {

            }
            return false;
        }

        public bool Update(Absensi data)
        {
            try
            {
                var item = db.Absensis.Where(x => x.RefId == data.RefId && x.Nama == data.Nama && x.Tanggal.Day == data.Tanggal.Day && x.Tanggal.Month == data.Tanggal.Month && x.Tanggal.Year == data.Tanggal.Year).FirstOrDefault();
                if (item != null)
                {
                    item.Tanggal = data.Tanggal;
                    item.PhotoUrl = data.PhotoUrl;
                    item.JumlahOrang = data.JumlahOrang;
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool Absen(Absensi data,bool Overwrite=false)
        {
            var IsExist = db.Absensis.Any(
                          x => x.RefId == data.RefId && x.Nama == data.Nama && x.Tanggal.Month == data.Tanggal.Month
                          && x.Tanggal.Year == data.Tanggal.Year && x.Tanggal.Day == data.Tanggal.Day);
            if (!IsExist)
            {
                InsertData(data);
                return true;
            }

            if (IsExist && Overwrite)
            {
                Update(data);
                return true;
            }
            return false;
        }

        public bool UpdateData(Absensi data)
        {
            try { 
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
            return db.Absensis.Max(x => x.Id);
        }
    }
}
