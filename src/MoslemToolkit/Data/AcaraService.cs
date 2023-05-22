using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AcaraService : ICrud<Acara>
    {
        NgajiDB db;

        public AcaraService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Acaras.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Acaras.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Acara> FindByKeyword(string Keyword)
        {
            var data = from x in db.Acaras
                       where x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Acara> GetAllData()
        {
            return db.Acaras.ToList();
        }

        public Acara GetDataById(object Id)
        {
            return db.Acaras.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Acara data)
        {
            try
            {
                db.Acaras.Add(data);
                db.SaveChanges();
                return true;
            }catch{

            }
            return false;

        }

       

        public bool UpdateData(Acara data)
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
            return db.Acaras.Max(x => x.Id);
        }
    }
    
}
/*











 */
