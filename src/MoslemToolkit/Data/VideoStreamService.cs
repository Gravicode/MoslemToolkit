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
        static RedisDB DataVideoStream;

        public VideoStreamService()
        {
            if (DataVideoStream == null) DataVideoStream = new RedisDB(AppConstants.RedisCon, 6);
            //DataVideoStream.InsertData<VideoStream>(new VideoStream() { Nama = "PeVideoStreaman Rutin", Keterangan = "Untuk semua jamaah cimanggu - hadith bukhori jilid 2 dan alquran", Tanggal = DateTime.Now, StreamUrl = "https://youtu.be/UyN6e0whyuw", Id = DataVideoStream.GetSequence<VideoStream>() });
            //DataVideoStream.InsertData<VideoStream>(new VideoStream() { Nama = "Musyawaroh 4s kelompok", Keterangan = "Untuk 4S - bahas tentang peVideoStreaman selama musim Covid19", Tanggal = DateTime.Now, StreamUrl = "", DocumentUrl = "", Id = DataVideoStream.GetSequence<VideoStream>() });

        }
        public bool DeleteData(object Id)
        {
            //var selData = (DataVideoStream.GetAllData<VideoStream>().Where(x => x.Id == (int)Id).FirstOrDefault());
            DataVideoStream.DeleteData<VideoStream>((long)Id);
            return true;
        }

        public List<VideoStream> FindByKeyword(string Keyword)
        {
            var data = from x in DataVideoStream.GetAllData<VideoStream>()
                       where x.Nama.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<VideoStream> GetAllData()
        {
            return DataVideoStream.GetAllData<VideoStream>().OrderBy(x => x.Tanggal).ToList();
        }

        public VideoStream GetDataById(object Id)
        {
            return DataVideoStream.GetDataById<VideoStream>((long)Id);
        }

        public long GetLastId()
        {
            var xx = DataVideoStream.GetSequence<VideoStream>();
            return xx + 1;
        }

        public bool InsertData(VideoStream data)
        {
            data.Id = GetLastId();
            DataVideoStream.InsertData<VideoStream>(data);
            return true;
        }

        public bool UpdateData(VideoStream data)
        {
            var sel = DataVideoStream.InsertData<VideoStream>(data);
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
            return false;
        }

    }
}
