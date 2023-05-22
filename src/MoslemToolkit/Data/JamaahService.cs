using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Data;
using MoslemToolkit.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class JamaahService : ICrud<Jamaah>
    {
        NgajiDB db;


        public static List<Jamaah> ReadExcel(string PathFile)
        {
            try
            {
                var DataJamaah = new List<Jamaah>();
                //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

                if (!File.Exists(PathFile)) return null;
                var workbook = ExcelFile.Load(PathFile);


                // Iterate through all worksheets in an Excel workbook.
                var worksheet = workbook.Worksheets[0];
                var counter = 0;
                // Iterate through all rows in an Excel worksheet.
                foreach (var row in worksheet.Rows)
                {
                    if (counter > 0)
                    {
                        var newJamaah = new Jamaah();
                        //newJamaah.KK = row.Cells[0].Value?.ToString();
                        //newJamaah.NoUrut = int.Parse(row.Cells[1].Value?.ToString());

                        newJamaah.Nama = row.Cells[1].Value?.ToString();
                        newJamaah.Alamat = row.Cells[2].Value?.ToString();
                        newJamaah.Telepon = row.Cells[3].Value?.ToString();
                        newJamaah.Email = row.Cells[4].Value?.ToString();
                        newJamaah.Posisi = string.IsNullOrEmpty(row.Cells[5].Value?.ToString()) ? PosisiKeluarga.ANAK : (PosisiKeluarga)Enum.Parse(typeof(PosisiKeluarga), row.Cells[5].Value?.ToString());
                        newJamaah.TempatLahir = row.Cells[6].Value?.ToString();
                        if(!string.IsNullOrEmpty(row.Cells[7].Value?.ToString()))
                        {
                            DateTime.TryParse(row.Cells[7].Value?.ToString(), out var dt);
                            newJamaah.TanggalLahir = dt;
                        }
                        newJamaah.Status = string.IsNullOrEmpty(row.Cells[8].Value?.ToString()) ? StatusPernikahan.Single : (StatusPernikahan)Enum.Parse(typeof(StatusPernikahan), row.Cells[8].Value?.ToString());

                        newJamaah.Kelamin = string.IsNullOrEmpty(row.Cells[9].Value?.ToString()) ? 'N' : row.Cells[9].Value.ToString()[0];
                        newJamaah.KK = row.Cells[10].Value?.ToString();

                        newJamaah.Gol = row.Cells[11].Value?.ToString();
                        newJamaah.Ayah = row.Cells[12].Value?.ToString();
                        newJamaah.Ibu = row.Cells[13].Value?.ToString();
                        newJamaah.Id = string.IsNullOrEmpty(row.Cells[14].Value?.ToString()) ? 0 : long.Parse( row.Cells[14].Value.ToString());
                        newJamaah.MotherId = string.IsNullOrEmpty(row.Cells[15].Value?.ToString()) ? 0 : long.Parse( row.Cells[15].Value.ToString());
                        newJamaah.FatherId = string.IsNullOrEmpty(row.Cells[16].Value?.ToString()) ? 0 : long.Parse( row.Cells[16].Value.ToString());

                        DataJamaah.Add(newJamaah);
                    }
                    counter++;


                }

                return DataJamaah;
            }
            catch
            {
                return null;
            }

        }
        public JamaahService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Jamaahs.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Jamaahs.Remove(selData);
            db.SaveChanges();
            return true;
        } 
        
        public bool ImportData(List<Jamaah> NewData)
        {
            try
            {
                var currentData = db.Jamaahs.ToList();
                foreach (var item in NewData)
                {
                    var existing = currentData.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existing == null)
                    {
                        db.Jamaahs.Add(item);
                    }
                    else
                    {
                        existing.Nama = item.Nama;
                        existing.Alamat = item.Alamat;
                        existing.Telepon = item.Telepon;
                        existing.Email = item.Email;
                        existing.Posisi = item.Posisi;
                        existing.TempatLahir = item.TempatLahir;
                        existing.TanggalLahir = item.TanggalLahir;
                        existing.Status = item.Status;
                        existing.Kelamin = item.Kelamin;
                        existing.KK = item.KK;
                        existing.Gol = item.Gol;
                        existing.Ayah = item.Ayah;
                        existing.Ibu = item.Ibu;
                        existing.MotherId = item.MotherId;
                        existing.FatherId = item.FatherId;
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public List<Jamaah> FindByKeyword(string Keyword)
        {
            var data = from x in db.Jamaahs
                       where x.Nama.Contains(Keyword) || x.Alamat.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Jamaah> GetAllData()
        {
            return db.Jamaahs.OrderBy(x=>x.Nama).ToList();
        }

        public Jamaah GetDataById(object Id)
        {
            return db.Jamaahs.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Jamaah data)
        {
            try
            {
                db.Jamaahs.Add(data);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


        public byte[] ExportToExcel()
        {
            // If using Professional version, put your serial key below.
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            var workbook = new ExcelFile();
            var worksheet = workbook.Worksheets.Add("Jamaah");
            var datas = GetAllData();
            int row = 1;

            var styleHeader = new CellStyle();
            styleHeader.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleHeader.VerticalAlignment = VerticalAlignmentStyle.Center;
            styleHeader.Font.Weight = ExcelFont.BoldWeight;
            styleHeader.Font.Color = Color.Black;
            styleHeader.WrapText = true;
            styleHeader.Borders.SetBorders(MultipleBorders.Left  | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            worksheet.Cells[0, 0].Value = "No";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "Alamat";
            worksheet.Cells[0, 3].Value = "Telepon";
            worksheet.Cells[0, 4].Value = "Email";
            worksheet.Cells[0, 5].Value = "Posisi";
            worksheet.Cells[0, 6].Value = "TempatLahir";
            worksheet.Cells[0, 7].Value = "TanggalLahir";
            worksheet.Cells[0, 8].Value = "Status";
            worksheet.Cells[0, 9].Value = "Kelamin";
            worksheet.Cells[0, 10].Value = "KK";
            worksheet.Cells[0, 11].Value = "Gol";
            worksheet.Cells[0, 12].Value = "Ayah";
            worksheet.Cells[0, 13].Value = "Ibu";
            worksheet.Cells[0, 14].Value = "Id";
            worksheet.Cells[0, 15].Value = "MotherId";
            worksheet.Cells[0, 16].Value = "FatherId";

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
            worksheet.Cells[0, 12].Style = styleHeader;
            worksheet.Cells[0, 13].Style = styleHeader;
            worksheet.Cells[0, 14].Style = styleHeader;
            worksheet.Cells[0, 15].Style = styleHeader;
            worksheet.Cells[0, 16].Style = styleHeader;

            var style = new CellStyle();
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Font.Weight = ExcelFont.NormalWeight;
            style.Font.Color = Color.Black;
            style.WrapText = true;
            style.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            foreach (var item in datas)
            {
                worksheet.Cells[row, 0].Value = row ;
                worksheet.Cells[row, 1].Value = item.Nama;
                worksheet.Cells[row, 2].Value = item.Alamat;
                worksheet.Cells[row, 3].Value = item.Telepon;
                worksheet.Cells[row, 4].Value = item.Email;
                worksheet.Cells[row, 5].Value = item.Posisi.ToString();
                worksheet.Cells[row, 6].Value = item.TempatLahir;
                worksheet.Cells[row, 7].Value = item.TanggalLahir;
                worksheet.Cells[row, 8].Value = item.Status.ToString();
                worksheet.Cells[row, 9].Value = item.Kelamin.ToString();
                worksheet.Cells[row, 10].Value = item.KK?.ToString();
                worksheet.Cells[row, 11].Value = item.Gol?.ToString();
                worksheet.Cells[row, 12].Value = item.Ayah?.ToString();
                worksheet.Cells[row, 13].Value = item.Ibu?.ToString();
                worksheet.Cells[row, 14].Value = item.Id.ToString();
                worksheet.Cells[row, 15].Value = item.MotherId.ToString();
                worksheet.Cells[row, 16].Value = item.FatherId.ToString();
           
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
                worksheet.Cells[row, 12].Style = style;
                worksheet.Cells[row, 13].Style = style;
                worksheet.Cells[row, 14].Style = style;
                worksheet.Cells[row, 15].Style = style;
                worksheet.Cells[row, 16].Style = style;
                row++;
            }
            var tmpfile = Path.GetTempFileName() + ".xlsx";
            
            workbook.Save(tmpfile);
            return File.ReadAllBytes(tmpfile);
        }
        public bool UpdateData(Jamaah data)
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
            return db.Jamaahs.Max(x => x.Id);
        }
    }
}
