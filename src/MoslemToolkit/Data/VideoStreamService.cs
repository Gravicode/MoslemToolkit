
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class VideoStreamService : ICrud<VideoStream>
    {
        NgajiDB db;

        public VideoStreamService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.VideoStreams.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.VideoStreams.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<VideoStream> FindByKeyword(string Keyword)
        {
            var data = from x in db.VideoStreams
                       where x.Nama.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<VideoStream> GetAllData()
        {
            return db.VideoStreams.ToList();
        }
        public List<VideoStream> GetAllData(DateTime start, DateTime end)
        {
            return db.VideoStreams.Where(x => x.Tanggal >= start && x.Tanggal <= end).OrderBy(x => x.Tanggal).ToList();
        }

        public VideoStream GetDataById(object Id)
        {
            return db.VideoStreams.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(VideoStream data)
        {
            try
            {
                db.VideoStreams.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }



        public bool UpdateData(VideoStream data)
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
        public VideoStream GetVideoStreamHariIni(DateTime date)
        {
            var sel = db.VideoStreams.Where(x => x.Tanggal.Day == date.Day && x.Tanggal.Month == date.Month &&
            x.Tanggal.Year == date.Year).FirstOrDefault();
            return sel;
        }
        public long GetLastId()
        {
            return db.VideoStreams.Max(x => x.Id);
        }
    }
}
