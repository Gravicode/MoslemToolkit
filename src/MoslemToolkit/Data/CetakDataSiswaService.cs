using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class CetakDataSiswaService
    {
        NgajiDB db;

        public CetakDataSiswaService()
        {
            if (db == null) db = new NgajiDB();

        }

        public async Task<List<Jamaah>> FindByKeyword(long kelasid, string Keyword)
        {
            var listSiswa = new List<Jamaah>();
            var data = await (from x in db.Kelass.Include(c => c.SiswaPerKelas).ThenInclude(x=>x.Jamaah)
                              where x.Id == kelasid  
                       select x).FirstOrDefaultAsync();
            var filtered = data.SiswaPerKelas.Where(x => x.Jamaah.Nama.Contains(Keyword)).ToList();
            foreach (var item in filtered)
            {
                listSiswa.Add(item.Jamaah);
            }
            return listSiswa;
          
        }

      
        public async Task<List<Jamaah>> GetAllData(long kelasid)
        {
            var listSiswa = new List<Jamaah>();
            var data = await (from x in db.Kelass.Include(c => c.SiswaPerKelas).ThenInclude(x => x.Jamaah)
                              where x.Id == kelasid
                              select x).FirstOrDefaultAsync();
            foreach(var item in data.SiswaPerKelas)
            {
                listSiswa.Add(item.Jamaah);
            }
            return listSiswa;
        }

        public async Task<byte[]> ExportToExcel(long kelasid)
        {
            // If using Professional version, put your serial key below.
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            var workbook = new ExcelFile();
            var worksheet = workbook.Worksheets.Add("Jamaah");
            var datas = await GetAllData(kelasid);
            int row = 1;

            var styleHeader = new CellStyle();
            styleHeader.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleHeader.VerticalAlignment = VerticalAlignmentStyle.Center;
            styleHeader.Font.Weight = ExcelFont.BoldWeight;
            styleHeader.Font.Color = SpreadsheetColor.FromName(ColorName.Black);
            styleHeader.WrapText = true;
            styleHeader.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, SpreadsheetColor.FromName( ColorName.Black), LineStyle.Thin);

            worksheet.Cells[0, 0].Value = "No";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "Alamat";
            worksheet.Cells[0, 3].Value = "Telepon";
            worksheet.Cells[0, 4].Value = "Email";
            worksheet.Cells[0, 5].Value = "Posisi";
            worksheet.Cells[0, 6].Value = "TanggalLahir";
            worksheet.Cells[0, 7].Value = "Status";
            worksheet.Cells[0, 8].Value = "Kelamin";
            worksheet.Cells[0, 9].Value = "KK";
            worksheet.Cells[0, 10].Value = "Gol";
            worksheet.Cells[0, 11].Value = "Foto";

            worksheet.Cells[0, 0].Style = styleHeader;
            worksheet.Cells[0, 1].Style = styleHeader;
            worksheet.Cells[0, 2].Style = styleHeader;
            worksheet.Cells[0, 3].Style = styleHeader;
            worksheet.Cells[0, 4].Style = styleHeader;
            worksheet.Cells[0, 5].Style = styleHeader;
            worksheet.Cells[0, 6].Style = styleHeader;
            worksheet.Cells[0, 7].Style = styleHeader;
            worksheet.Cells[0, 8].Style = styleHeader;
            worksheet.Cells[0, 9].Style = styleHeader;
            worksheet.Cells[0, 10].Style = styleHeader;
            worksheet.Cells[0, 11].Style = styleHeader;

            var style = new CellStyle();
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Font.Weight = ExcelFont.NormalWeight;
            style.Font.Color = SpreadsheetColor.FromName(ColorName.Black);
            style.WrapText = true;
            style.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, SpreadsheetColor.FromName( ColorName.Black), LineStyle.Thin);

            foreach (var item in datas)
            {
                worksheet.Cells[row, 0].Value = row;
                worksheet.Cells[row, 1].Value = item.Nama;
                worksheet.Cells[row, 2].Value = item.Alamat;
                worksheet.Cells[row, 3].Value = item.Telepon;
                worksheet.Cells[row, 4].Value = item.Email;
                worksheet.Cells[row, 5].Value = item.Posisi.ToString();
                worksheet.Cells[row, 6].Value = item.TanggalLahir;
                worksheet.Cells[row, 7].Value = item.Status.ToString();
                worksheet.Cells[row, 8].Value = item.Kelamin.ToString();
                worksheet.Cells[row, 9].Value = item.KK?.ToString();
                worksheet.Cells[row, 10].Value = item.Gol?.ToString();
                var phototmp = await getImageUrl(item.PhotoUrl);
                if(!string.IsNullOrEmpty(phototmp))
                    worksheet.Pictures.Add(phototmp, worksheet.Cells[row, 11].Name);


                worksheet.Cells[row, 0].Style = style;
                worksheet.Cells[row, 1].Style = style;
                worksheet.Cells[row, 2].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 4].Style = style;
                worksheet.Cells[row, 5].Style = style;
                worksheet.Cells[row, 6].Style = style;
                worksheet.Cells[row, 7].Style = style;
                worksheet.Cells[row, 8].Style = style;
                worksheet.Cells[row, 9].Style = style;
                worksheet.Cells[row, 10].Style = style;
                worksheet.Cells[row, 11].Style = style;
                row++;
            }
            var tmpfile = Path.GetTempFileName() + ".xlsx";

            workbook.Save(tmpfile);
            return File.ReadAllBytes(tmpfile);
        }
        HttpClient client;
        async Task<string> getImageUrl(string PhotoUrl)
        {
            if (string.IsNullOrEmpty(PhotoUrl)) return null;
            if (client == null) client = new HttpClient();
            try
            {
                var bytes = await client.GetByteArrayAsync(PhotoUrl);
                if (bytes != null)
                {
                    var tmpPath = Path.GetTempFileName() + Path.GetExtension(PhotoUrl);
                    File.WriteAllBytes(tmpPath, bytes);
                    return tmpPath;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }
    }
}
