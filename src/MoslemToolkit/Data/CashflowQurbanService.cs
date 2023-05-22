using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class CashflowQurbanService : ICrud<CashflowQurban>
    {
        NgajiDB db;

        public CashflowQurbanService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.CashflowQurbans.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.CashflowQurbans.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<CashflowQurban> FindByKeyword(string Keyword)
        {
            var data = from x in db.CashflowQurbans
                       where x.Keterangan.Contains(Keyword) || x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<CashflowQurban> GetAllData()
        {
            return db.CashflowQurbans.ToList();
        }
        
        public List<int> GetTahuns()
        {
            return db.CashflowQurbans.Select(x=>x.Tanggal.Year).Distinct().ToList();
        }
        public List<CashflowQurban> GetDataByYear(int Tahun)
        {
            var datas= db.CashflowQurbans.Where(x=>x.Tanggal.Year == Tahun).OrderBy(x=>x).ToList();
            return datas;
        } 
       
        public CashflowQurban GetDataById(object Id)
        {
            return db.CashflowQurbans.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(CashflowQurban data)
        {
            try
            {
                db.CashflowQurbans.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(CashflowQurban data)
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
            return db.CashflowQurbans.Max(x => x.Id);
        }
    }

}
/*











 */
