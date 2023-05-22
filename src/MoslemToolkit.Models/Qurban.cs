using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MoslemToolkit.Data
{
    public class Qurban
    {

        public Qurban()
        {
            BL = new List<DataBL>();
            Pembagian = new List<DataPembagian>();
            DataHewanQurban = new List<HewanQurban>();
        }
        public double TotalBerat { get; set; }
        public double BeratTanpaSampil { get; set; }
        public double? TotalBeratBersih { get; set; }
        public DataTable Report { set; get; }
        public InfoQurban Info { get; set; }
        public List<DataBL> BL { get; set; }
        public List<HewanQurban> DataHewanQurban { get; set; }
        public List<DataPembagian> Pembagian { get; set; }
        public int? QtySampil { get; set; } = 0;
        public double? BeratSampil { get; set; } = 30;
    }
}
