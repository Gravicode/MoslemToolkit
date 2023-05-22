using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AbsensiSiswaService : ICrud<AbsensiSiswa>
    {
        NgajiDB db;

        public AbsensiSiswaService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.AbsensiSiswas.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.AbsensiSiswas.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<AbsensiSiswa> FindByKeyword(string Keyword)
        {
            var data = from x in db.AbsensiSiswas.Include(c=>c.Jamaah)
                       where x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<AbsensiSiswa> GetAllData()
        {
            return db.AbsensiSiswas.ToList();
        }

        public List<AbsensiSiswa> GetDataByDate(long MaterPerKelasId, DateTime from, DateTime to)
        {
                return db.AbsensiSiswas.Include(c=>c.MateriPerKelas).Where(x => x.Tanggal >= from && x.Tanggal <= to && x.MateriPerKelas.Id == MaterPerKelasId).ToList();

        }

        public AbsensiSiswa GetDataById(object Id)
        {
            return db.AbsensiSiswas.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(AbsensiSiswa data)
        {
            try
            {
                db.AbsensiSiswas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        public bool Update(AbsensiSiswa data)
        {
            try
            {
                var item = db.AbsensiSiswas.Include(c=>c.Jamaah).Include(c=>c.MateriPerKelas).Where(x => x.Jamaah.Id == data.Jamaah.Id && x.MateriPerKelas.Id == data.MateriPerKelas.Id && x.Tanggal.Day == data.Tanggal.Day && x.Tanggal.Month == data.Tanggal.Month && x.Tanggal.Year == data.Tanggal.Year).FirstOrDefault();
                if (item != null)
                {
                    item.Tanggal = data.Tanggal;
                    item.PhotoUrl = data.PhotoUrl;
                 
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

        public bool Absen(AbsensiSiswa data, bool Overwrite = false)
        {
            var IsExist = db.AbsensiSiswas.Include(c => c.Jamaah).Include(c => c.MateriPerKelas).Any(
                          x => x.Jamaah.Id == data.Jamaah.Id && x.MateriPerKelas.Id == data.MateriPerKelas.Id && x.Tanggal.Month == data.Tanggal.Month
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

        public bool UpdateData(AbsensiSiswa data)
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
            return db.AbsensiSiswas.Max(x => x.Id);
        }
    }
}
