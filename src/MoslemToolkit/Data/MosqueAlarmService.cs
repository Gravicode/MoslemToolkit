using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class MosqueAlarmService : ICrud<MosqueAlarmData>
    {
        NgajiDB db;

        public MosqueAlarmService()
        {
            if (db == null) db = new NgajiDB();


        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MosqueAlarms.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MosqueAlarms.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MosqueAlarmData> FindByKeyword(string Keyword)
        {
            var data = from x in db.MosqueAlarms
                       where x.Judul.Contains(Keyword) 
                       select MosqueAlarmData.To(x);
            return data.ToList();
        }

        public List<MosqueAlarmPrimitive> GetAllData2()
        {
            var data = from x in db.MosqueAlarms.AsNoTracking()
                       select MosqueAlarmPrimitive.From(x);
            return data.ToList();
        }
        public List<MosqueAlarmData> GetAllData()
        {
            var data = from x in db.MosqueAlarms.AsNoTracking()
                       select MosqueAlarmData.To(x);
            return data.ToList();
        }

        public MosqueAlarmData GetDataById(object Id)
        {
            var item = db.MosqueAlarms.Where(x => x.Id == (long)Id).FirstOrDefault();
            return item != null ? MosqueAlarmData.To(item) : null;
        }


        public bool InsertData(MosqueAlarm data)
        {
            try
            {
                db.MosqueAlarms.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(MosqueAlarm data)
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
            return db.MosqueAlarms.Max(x => x.Id);
        }

        public bool InsertData(MosqueAlarmData data)
        {
            throw new NotImplementedException();
        }

        public bool UpdateData(MosqueAlarmData data)
        {
            try
            {
                var data2 = MosqueAlarmData.From(data);
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
