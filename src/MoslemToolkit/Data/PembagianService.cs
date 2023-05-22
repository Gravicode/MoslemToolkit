using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class PembagianService : ICrud<DataPembagian>
    {
        NgajiDB db;

        public PembagianService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool ClearData(int tahun)
        {
            try
            {
                foreach (var item in db.DataPembagians.Where(x=>x.Tahun == tahun).ToList())
                {
                    db.DataPembagians.Remove(item);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }
        public bool DeleteData(object Id)
        {
            var selData = (db.DataPembagians.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.DataPembagians.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<DataPembagian> FindByKeyword(string Keyword)
        {
            var data = from x in db.DataPembagians
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<DataPembagian> GetAllData()
        {
            return db.DataPembagians.ToList();
        } 
        public List<DataPembagian> GetAllData(int tahun)
        {
            return db.DataPembagians.Where(x=>x.Tahun == tahun).ToList();
        }

        public DataPembagian GetDataById(object Id)
        {
            return db.DataPembagians.Where(x => x.Id == (long)Id).FirstOrDefault();
        }

        public async Task<bool> UpdateRef(int Tahun)
        {
            try
            {
                HewanQurbanService svc = new HewanQurbanService();
                var hewans = svc.GetAllData(Tahun);
                var datas = await (from x in db.DataPembagians
                                   where x.Tahun == Tahun && x.HewanQurban == null
                                   select x).ToListAsync();
                for (int i = 0; i < datas.Count; i++)
                {
                    var item = datas[i];
                    item.HewanQurban = hewans.FirstOrDefault(x => x.Pemilik == item.Sapi);
                }
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
           
        }
        public bool InsertData(DataPembagian data)
        {
            try
            {
                db.DataPembagians.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

     

        public bool UpdateData(DataPembagian data)
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
            return db.DataPembagians.Max(x => x.Id);
        }
    }
}
