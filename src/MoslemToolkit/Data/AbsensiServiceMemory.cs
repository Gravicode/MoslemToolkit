using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class AbsensiServiceMemory : ICrud<Absensi>
    {
        static List<Absensi> DataAbsensi;

        public AbsensiServiceMemory()
        {
            if (DataAbsensi == null) DataAbsensi = new List<Absensi>();
          
        }
        public bool DeleteData(object Id)
        {
            DataAbsensi.Remove(DataAbsensi.Where(x => x.Id == (int)Id).FirstOrDefault());
            return true;
        }

        public List<Absensi> FindByKeyword(string Keyword)
        {
            var data = from x in DataAbsensi
                       where x.Nama.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<Absensi> GetAllData()
        {
            return DataAbsensi.ToList();
        }

        public Absensi GetDataById(object Id)
        {
            return DataAbsensi.Where(x => x.Id == (int)Id).FirstOrDefault();
        }

        public long GetLastId()
        {
            var xx = (from x in DataAbsensi
                      orderby x.Id descending
                      select x.Id).FirstOrDefault();
            if (xx <= 0) return 1;
            return xx + 1;
        }

        public bool InsertData(Absensi data)
        {
            data.Id = GetLastId();
            DataAbsensi.Add(data);
            return true;
        }

        public bool UpdateData(Absensi data)
        {
            var sel = DataAbsensi.Where(x => x.Id == data.Id).FirstOrDefault();
            if (sel != null)
            {
                sel.Nama = data.Nama;
                sel.RefId = data.RefId;
                sel.Tanggal = data.Tanggal;
               
                return true;

            }
            return false;
        }

    }
}
