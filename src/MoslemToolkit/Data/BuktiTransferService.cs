using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class BuktiTransferService : ICrud<BuktiTransfer>
    {
        NgajiDB db;

        public BuktiTransferService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.BuktiTransfers.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.BuktiTransfers.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<BuktiTransfer> FindByKeyword(string Keyword)
        {
            var data = from x in db.BuktiTransfers
                       where x.Keterangan.Contains(Keyword) || x.NamaPengirim.Contains(Keyword) || x.Tujuan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<BuktiTransfer> GetAllData()
        {
            return db.BuktiTransfers.ToList();
        }
        public List<BuktiTransfer> GetDataByUsername(string uname)
        {
            return db.BuktiTransfers.Where(x=>x.DibuatOleh == uname).ToList();
        }
        public BuktiTransfer GetDataById(object Id)
        {
            return db.BuktiTransfers.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(BuktiTransfer data)
        {
            try
            {
                db.BuktiTransfers.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(BuktiTransfer data)
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
            return db.BuktiTransfers.Max(x => x.Id);
        }
    }
}
