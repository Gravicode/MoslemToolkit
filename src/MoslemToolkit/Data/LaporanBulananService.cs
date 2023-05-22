using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System.Collections.Generic;
using System.Linq;

namespace MoslemToolkit.Data
{
    public class LaporanBulananService
    {

        NgajiDB db;
        public List<Kelas> DataKelas { set; get; }
        public LaporanBulananService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool LoadReport(int Tahun, int Bulan)
        {
            try
            {
                var datas = from x in db.Kelass.Include(x => x.SiswaPerKelas).ThenInclude(x => x.Jamaah)
                            select x;
                DataKelas = datas.ToList();

                return true;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            return false;
        }
    }
}
