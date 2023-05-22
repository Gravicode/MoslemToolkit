using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class NgajiServiceMemory : ICrud<Ngaji>
    {
        static List<Ngaji> DataNgaji;

        public NgajiServiceMemory()
        {
            if (DataNgaji == null) DataNgaji = new List<Ngaji>();
            DataNgaji.Add(new Ngaji() { Nama = "PeNgajian Rutin", Keterangan = "Untuk semua jamaah cimanggu - hadith bukhori jilid 2 dan alquran", TanggalDari = DateTime.Now, StreamUrl = "https://youtu.be/UyN6e0whyuw", Id = GetLastId() });
            DataNgaji.Add(new Ngaji() { Nama = "Musyawaroh 4s kelompok", Keterangan = "Untuk 4S - bahas tentang pengajian selama musim Covid19", TanggalDari = DateTime.Now, StreamUrl = "", Id = GetLastId() });

        }
        public bool DeleteData(object Id)
        {
            DataNgaji.Remove(DataNgaji.Where(x => x.Id == (int)Id).FirstOrDefault());
            return true;
        }

        public List<Ngaji> FindByKeyword(string Keyword)
        {
            var data = from x in DataNgaji
                       where x.Nama.Contains(Keyword) || x.Keterangan.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Ngaji> GetAllData()
        {
            return DataNgaji.ToList();
        }

        public Ngaji GetDataById(object Id)
        {
            return DataNgaji.Where(x => x.Id == (int)Id).FirstOrDefault();
        }

        public long GetLastId()
        {
            var xx = (from x in DataNgaji
                      orderby x.Id descending
                      select x.Id).FirstOrDefault();
            if (xx <= 0) return 1;
            return xx + 1;
        }

        public bool InsertData(Ngaji data)
        {
            data.Id = GetLastId();
            DataNgaji.Add(data);
            return true;
        }

        public bool UpdateData(Ngaji data)
        {
            var sel = DataNgaji.Where(x => x.Id == data.Id).FirstOrDefault();
            if (sel != null)
            {
                sel.Nama = data.Nama;
                sel.Keterangan = data.Keterangan;
                sel.TanggalDari = data.TanggalDari;
                sel.TanggalSampai = data.TanggalSampai;
                sel.DocumentUrl = data.DocumentUrl;
                sel.StreamUrl = data.StreamUrl;
                return true;

            }
            return false;
        }

    }
}
