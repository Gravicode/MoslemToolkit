using GemBox.Spreadsheet;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
   
  
    public class QurbanService
    {
        public static Qurban ReadExcel(string PathFile, int Tahun)
        {
            try
            {
                var Qurban = new Qurban();
                //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

                if (!File.Exists(PathFile)) return null;
                var workbook = ExcelFile.Load(PathFile);


                // Iterate through all worksheets in an Excel workbook.
                var worksheet = workbook.Worksheets["DataPembagian"];
                var counter = 0;
                // Iterate through all rows in an Excel worksheet.
                foreach (var row in worksheet.Rows)
                {
                    if (counter > 0)
                    {
                        var newPembagian = new DataPembagian();
                        newPembagian.KK = row.Cells[0].Value?.ToString();
                        newPembagian.NoUrut = int.Parse(row.Cells[1].Value?.ToString());

                        newPembagian.Nama = row.Cells[2].Value?.ToString();
                        newPembagian.Status = row.Cells[3].Value?.ToString();
                        newPembagian.Sapi = row.Cells[4].Value?.ToString();

                        newPembagian.Golongan = row.Cells[5].Value?.ToString();
                        newPembagian.KP = row.Cells[6].Value?.ToString();
                        newPembagian.Pembagian = double.Parse((row.Cells[7].Value?.ToString() ?? "0"));
                        newPembagian.BL = double.Parse((row.Cells[8].Value?.ToString() ?? "0"));
                        newPembagian.Kantong = row.Cells[9].Value?.ToString();
                        newPembagian.KAKI = double.Parse((row.Cells[10].Value?.ToString() ?? "0"));
                        newPembagian.KEPALA = double.Parse((row.Cells[11].Value?.ToString() ?? "0"));
                        newPembagian.TULANG = double.Parse((row.Cells[12].Value?.ToString() ?? "0"));
                        newPembagian.JEROHAN = double.Parse((row.Cells[13].Value?.ToString() ?? "0"));
                        newPembagian.APRESIASI = double.Parse((row.Cells[14].Value?.ToString() ?? "0"));
                        newPembagian.Tahun = Tahun;
                        Qurban.Pembagian.Add(newPembagian);
                    }
                    counter++;


                }

                worksheet = workbook.Worksheets["DataBL"];
                counter = 0;
                // Iterate through all rows in an Excel worksheet.
                foreach (var row in worksheet.Rows)
                {
                    if (counter > 0)
                    {
                        var newBL = new DataBL();
                        newBL.Urut = int.Parse(row.Cells[0].Value?.ToString());
                        newBL.Nama = row.Cells[1].Value?.ToString();
                        newBL.BERAT = double.Parse((row.Cells[2].Value?.ToString() ?? "0"));
                        newBL.BUNGKUS = double.Parse((row.Cells[3].Value?.ToString() ?? "0"));
                        newBL.KP = row.Cells[4].Value?.ToString();
                        newBL.KETERANGAN = row.Cells[5].Value?.ToString();
                        var jenis = row.Cells[6].Value?.ToString();
                        newBL.Jenis = (TipeBL)Enum.Parse(typeof(TipeBL), jenis, true);
                        newBL.Tahun = Tahun;
                        Qurban.BL.Add(newBL);
                    }
                    counter++;

                }

                worksheet = workbook.Worksheets["DataHewan"];
                counter = 0;
                // Iterate through all rows in an Excel worksheet.
                foreach (var row in worksheet.Rows)
                {
                    if (counter > 0)
                    {
                        var newHewan = new HewanQurban();
                        newHewan.No = int.Parse(row.Cells[0].Value?.ToString());
                        newHewan.Pemilik = row.Cells[1].Value?.ToString();
                        newHewan.Bruto = double.Parse((row.Cells[2].Value?.ToString() ?? "0"));
                        newHewan.PersenNet = double.Parse((row.Cells[3].Value?.ToString() ?? "0"));
                        newHewan.Netto = newHewan.Bruto * newHewan.PersenNet;
                        newHewan.KepalaSapi = row.Cells[5].Value?.ToString();
                        var jenis = row.Cells[6].Value?.ToString();
                        newHewan.Jenis = (JenisHewanQurban)Enum.Parse(typeof(JenisHewanQurban), jenis, true);
                        newHewan.Keterangan = row.Cells[7].Value?.ToString();
                        newHewan.Tahun = Tahun;
                        Qurban.DataHewanQurban.Add(newHewan);
                    }
                    counter++;

                }
                return Qurban;
            }
            catch
            {
                return null;
            }
            
        }
    }
    
}
