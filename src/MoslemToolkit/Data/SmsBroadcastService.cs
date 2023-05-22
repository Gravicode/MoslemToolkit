using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class SmsBroadcastService : ICrud<SmsBroadcastData>
    {
        NgajiDB db;

        public SmsBroadcastService()
        {
            if (db == null) db = new NgajiDB();
           

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.SmsBroadcasts.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.SmsBroadcasts.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<SmsBroadcastData> FindByKeyword(string Keyword)
        {
            var data = from x in db.SmsBroadcasts
                       where x.Judul.Contains(Keyword) || x.Pesan.Contains(Keyword) || x.DikirimKe.Contains(Keyword) 
                       select SmsBroadcastData.To(x);
            return data.ToList();
        }

        public List<SmsBroadcastPrimitive> GetAllData2()
        {
            var data = from x in db.SmsBroadcasts
                       select SmsBroadcastPrimitive.From(x);
            return data.ToList();
        }
        public List<SmsBroadcastData> GetAllData()
        {
            var data = from x in db.SmsBroadcasts
                       select SmsBroadcastData.To(x);
            return data.ToList();
        }

        public SmsBroadcastData GetDataById(object Id)
        {
            var item = db.SmsBroadcasts.Where(x => x.Id == (long)Id).FirstOrDefault();
            return item != null ? SmsBroadcastData.To(item) : null;
        }


        public bool InsertData(SmsBroadcast data)
        {
            try
            {
                db.SmsBroadcasts.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(SmsBroadcast data)
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
            return db.SmsBroadcasts.Max(x => x.Id);
        }

        public bool InsertData(SmsBroadcastData data)
        {
            throw new NotImplementedException();
        }

        public bool UpdateData(SmsBroadcastData data)
        {
            try
            {
                var data2 = SmsBroadcastData.From(data);
                db.Entry(data2).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
