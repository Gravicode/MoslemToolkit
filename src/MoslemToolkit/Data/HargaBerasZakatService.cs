using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class HargaBerasZakatService : ICrud<HargaBerasZakat>
    {
        NgajiDB db;

        public HargaBerasZakatService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.HargaBerasZakats.Where(x => x.Tahun == (long)Id).FirstOrDefault());
            db.HargaBerasZakats.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<HargaBerasZakat> FindByKeyword(string Keyword)
        {
            var data = from x in db.HargaBerasZakats
                       where x.Tahun.ToString().Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<HargaBerasZakat> GetAllData()
        {
            return db.HargaBerasZakats.ToList();
        }

        public HargaBerasZakat GetDataById(object Id)
        {
            return db.HargaBerasZakats.Where(x => x.Tahun == (int)Id).FirstOrDefault();
        } 
        public long GetHargaBeras(int Tahun)
        {
            var sel = db.HargaBerasZakats.Where(x => x.Tahun == Tahun).FirstOrDefault();
            if (sel == null)
                return AppConstants.HargaJualBeras;
            else
                return sel.HargaJualBeras;
        }

       
        public bool InsertData(HargaBerasZakat data)
        {
            try
            {
                db.HargaBerasZakats.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(HargaBerasZakat data)
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
            return db.HargaBerasZakats.Max(x => x.Tahun);
        }
    }

}
/*











 */
