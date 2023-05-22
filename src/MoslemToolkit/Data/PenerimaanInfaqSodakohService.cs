using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class PenerimaanInfaqSodakohService : ICrud<PenerimaanInfaqSodakoh>
    {
        NgajiDB db;

        public PenerimaanInfaqSodakohService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.PenerimaanInfaqSodakohs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.PenerimaanInfaqSodakohs.Remove(selData);
            db.SaveChanges();
            return true;
        }
        public List<PenerimaanInfaqSodakoh> GetDataIsrun(object Id)
        {
            var selData = from x in db.PenerimaanInfaqSodakohs.Include(c => c.AkunInfaqSodakoh)
                          where x.HeaderPenerimaanInfaqSodakohId == (long)Id
                          orderby x.AkunInfaqSodakoh.Level, x.AkunInfaqSodakoh.No
                          select x;
            return selData.ToList();
        }
        public List<PenerimaanInfaqSodakoh> GetDataIsrun()
        {
            var items = (from x in db.AkunInfaqSodakohs
                         where x.MasukKeIsrun 
                        orderby x.Level, x.No
                        select x).ToList();
            var datas = new List<PenerimaanInfaqSodakoh>();
            foreach (var item in items)
            {
                var newData = new PenerimaanInfaqSodakoh()
                {
                    AkunInfaqSodakohId = item.Id,
                    AkunInfaqSodakoh = item,
                    NamaAkun = item.Uraian,
                    No = item.No
                };
                datas.Add(newData);
            }
            return datas;
        } public List<PenerimaanInfaqSodakoh> GetDataNonIsrun()
        {
            var datas = new List<PenerimaanInfaqSodakoh>();
            var newData = new PenerimaanInfaqSodakoh()
            {
                No=1,
                AkunInfaqSodakohId = -1,
            };
            datas.Add(newData);
            return datas;
        }
        public List<PenerimaanInfaqSodakoh> FindByKeyword(string Keyword)
        {
            var data = from x in db.PenerimaanInfaqSodakohs.Include(c=>c.Jamaah)
                       where x.Keterangan.Contains(Keyword) || x.Jamaah.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<PenerimaanInfaqSodakoh> GetAllData()
        {
            return db.PenerimaanInfaqSodakohs.Include(c => c.AkunInfaqSodakoh).Include(c=>c.Jamaah).ToList();
        }

        public PenerimaanInfaqSodakoh GetDataById(object Id)
        {
            return db.PenerimaanInfaqSodakohs.Include(c => c.AkunInfaqSodakoh).Include(c => c.Jamaah).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(PenerimaanInfaqSodakoh data)
        {
            try
            {
                db.PenerimaanInfaqSodakohs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }
        public bool InsertData(List<PenerimaanInfaqSodakoh> datas)
        {
            try
            {
                foreach (var data in datas)
                {
                    db.PenerimaanInfaqSodakohs.Add(data);
                }

                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(PenerimaanInfaqSodakoh data)
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
        public bool UpdateData(List<PenerimaanInfaqSodakoh> datas)
        {
            try
            {
                foreach (var data in datas)
                {
                    db.Entry(data).State = EntityState.Modified;
                }
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
            return db.PenerimaanInfaqSodakohs.Max(x => x.Id);
        }
    }
}
