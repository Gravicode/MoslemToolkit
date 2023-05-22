using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class MustahikService : ICrud<Mustahik>
    {
        NgajiDB db;

        public MustahikService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Mustahiks.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Mustahiks.Remove(selData);
            db.SaveChanges();
            return true;
        } 
        
        public bool ClearData(int Year)
        {
            var datas = db.Mustahiks.Where(x => x.Tahun == Year).ToList();
            foreach(var selData in datas)
                db.Mustahiks.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Mustahik> FindByKeyword(string Keyword)
        {
            var data = from x in db.Mustahiks
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Mustahik> GetAllData()
        {
            return db.Mustahiks.ToList();
        }    public List<Mustahik> GetAllData(int Year)
        {
            return db.Mustahiks.Where(x=>x.Tahun==Year).ToList();
        }



        public Mustahik GetDataById(object Id)
        {
            return db.Mustahiks.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Mustahik data)
        {
            try
            {
                db.Mustahiks.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }


        public bool UpdateData(Mustahik data)
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
            return db.Mustahiks.Max(x => x.Id);
        }

        public bool UploadExcel(int Tahun, string PathFile)
        {

            var Qurban = new Qurban();
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            if (!File.Exists(PathFile)) return false;
            var workbook = ExcelFile.Load(PathFile);


            // Iterate through all worksheets in an Excel workbook.
            var worksheet = workbook.Worksheets["Mustahik"];
            var counter = 0;
            List<Mustahik> dataBaru = new List<Mustahik>();
            // Iterate through all rows in an Excel worksheet.
            foreach (var row in worksheet.Rows)
            {
                if (counter > 0)
                {
                    var newData = new Mustahik();
                    newData.Tahun = Tahun;
                    newData.No = Convert.ToInt32( row.Cells[0].Value);
                    newData.Nama = row.Cells[1].Value?.ToString();
                    newData.Jumlah = Convert.ToInt32(row.Cells[2].Value);
                    TipeAsnab pos;
                    Enum.TryParse(row.Cells[3].Value?.ToString(), out pos);
                    newData.TipeAsnab = pos;
                    newData.Beras = Convert.ToInt32(row.Cells[4].Value);
                    newData.Uang = Convert.ToInt32(row.Cells[4].Value);
                    dataBaru.Add(newData);
                }
                counter++;


            }
            if (dataBaru.Count > 0)
            {
                //delete existing
                var existing = db.Mustahiks.Where(x => x.Tahun == Tahun).ToList();
                existing.ForEach(x => { db.Mustahiks.Remove(x); });
                db.SaveChanges();
                //add new
                dataBaru.ForEach(x => { db.Mustahiks.Add(x); });
                db.SaveChanges();

                return true;
            }

            return false;

        }
        public byte[] ExportToExcel()
        {
            // If using Professional version, put your serial key below.
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            var workbook = new ExcelFile();
            var worksheet = workbook.Worksheets.Add("Mustahik");
            var datas = GetAllData();
            int row = 1;

            var styleHeader = new CellStyle();
            styleHeader.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleHeader.VerticalAlignment = VerticalAlignmentStyle.Center;
            styleHeader.Font.Weight = ExcelFont.BoldWeight;
            styleHeader.Font.Color = Color.Black;
            styleHeader.WrapText = true;
            styleHeader.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            worksheet.Cells[0, 0].Value = "No";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "Jumlah";
            worksheet.Cells[0, 3].Value = "Asnab";
            worksheet.Cells[0, 4].Value = "Zakat Beras";
            worksheet.Cells[0, 5].Value = "Titip Zakat (uang)";

            worksheet.Cells[0, 0].Style = styleHeader;
            worksheet.Cells[0, 1].Style = styleHeader;
            worksheet.Cells[0, 2].Style = styleHeader;
            worksheet.Cells[0, 3].Style = styleHeader;
            worksheet.Cells[0, 3].Style = styleHeader;
            worksheet.Cells[0, 4].Style = styleHeader;
            worksheet.Cells[0, 5].Style = styleHeader;

            var style = new CellStyle();
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Font.Weight = ExcelFont.NormalWeight;
            style.Font.Color = Color.Black;
            style.WrapText = true;
            style.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            foreach (var item in datas)
            { /*
               *     worksheet.Cells[0, 0].Value = "No";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "Jumlah";
            worksheet.Cells[0, 3].Value = "Asnab";
            worksheet.Cells[0, 4].Value = "Zakat Beras";
            worksheet.Cells[0, 5].Value = "Titip Zakat (uang)";
             */
                worksheet.Cells[row, 0].Value = item.No;
                worksheet.Cells[row, 1].Value = item.Nama;
                worksheet.Cells[row, 2].Value = item.Jumlah;
                worksheet.Cells[row, 3].Value = item.TipeAsnab.ToString();
                worksheet.Cells[row, 4].Value = item.Beras;
                worksheet.Cells[row, 5].Value = item.Uang;
              
                worksheet.Cells[row, 0].Style = style;
                worksheet.Cells[row, 1].Style = style;
                worksheet.Cells[row, 2].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 4].Style = style;
                worksheet.Cells[row, 5].Style = style;
                row++;
            }
            var tmpfile = Path.GetTempFileName() + ".xlsx";

            workbook.Save(tmpfile);
            return File.ReadAllBytes(tmpfile);
        }
    }
}
