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
    public class MuzakiService : ICrud<Muzaki>
    {
        NgajiDB db;

        public MuzakiService()
        {
            if (db == null) db = new NgajiDB();

        }
        public bool ClearData(int Year)
        {
            var datas = db.Muzakis.Where(x => x.Tahun == Year).ToList();
            foreach (var selData in datas)
                db.Muzakis.Remove(selData);
            db.SaveChanges();
            return true;
        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Muzakis.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Muzakis.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Muzaki> FindByKeyword(string Keyword)
        {
            var data = from x in db.Muzakis
                       where x.Nama.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Muzaki> GetAllData()
        {
            return db.Muzakis.ToList();
        }
        public List<Muzaki> GetAllData(int Year)
        {
            return db.Muzakis.Where(x=>x.Tahun==Year && (x.ZakatBeras>0 || x.TitipZakat>0)).ToList();
        }
        public List<Muzaki> GetMuzakiBelumSetor(int Year)
        {
            return db.Muzakis.Where(x=>x.Tahun==Year && (x.ZakatBeras<=0 && x.TitipZakat<=0)).ToList();
        }

        public List<(string Nama, long Jumlah, double Persen)> GetStats(int Year)
        {
            var belumSetor = db.Muzakis.Where(x => x.Tahun == Year && (x.ZakatBeras <= 0 && x.TitipZakat <= 0)).Count(); 
            var beras = db.Muzakis.Where(x => x.Tahun == Year && x.ZakatBeras > 0).Count(); 
            var titipUang = db.Muzakis.Where(x => x.Tahun == Year &&  x.TitipZakat > 0).Count();
            var total = belumSetor+ beras + titipUang;
            var Stats = new List<(string Nama, long Jumlah, double Persen)>();
            if (total <= 0) return default;
            Stats.Add(("Prosentase Setor Beras",beras, (double)beras / total * 100));
            Stats.Add(("Prosentase Titip Uang",titipUang, (double)titipUang / total * 100));
            Stats.Add(("Prosentase Belum Setor",belumSetor, (double)belumSetor / total * 100));
            return Stats;
        }

        public List<Muzaki> GetAllData(int Year, int FilterId)
        {
            switch (FilterId)
            {

                case 1: //beras
                    return db.Muzakis.Where(x => x.Tahun == Year && x.ZakatBeras > 0).OrderBy(x=>x.KK).ToList();
                    break;
                case 2: //uang
                    return db.Muzakis.Where(x => x.Tahun == Year && x.TitipZakat > 0).OrderBy(x => x.KK).ToList();
                    break;
                case 3: //belum
                    return db.Muzakis.Where(x => x.Tahun == Year && x.ZakatBeras <= 0 && x.TitipZakat <= 0).OrderBy(x => x.KK).ToList();
                    break;
                case 0: //all data
                default:
                    return db.Muzakis.Where(x => x.Tahun == Year).OrderBy(x => x.KK).ToList();
                    break;

            }
        }


        public Muzaki GetDataById(object Id)
        {
            return db.Muzakis.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Muzaki data)
        {
            try
            {
                db.Muzakis.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;
        }

        
        public bool UpdateData(Muzaki data)
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
            return db.Muzakis.Max(x => x.Id);
        }

        public  bool UploadExcel(int Tahun, string PathFile)
        {
          
            var Qurban = new Qurban();
            //SpreadsheetInfo.SetLicense(AppConstants.GemLic);

            if (!File.Exists(PathFile)) return false;
            var workbook = ExcelFile.Load(PathFile);


            // Iterate through all worksheets in an Excel workbook.
            var worksheet = workbook.Worksheets["Muzaki"];
            var counter = 0;
            List<Muzaki> dataBaru = new List<Muzaki>();
            // Iterate through all rows in an Excel worksheet.
            foreach (var row in worksheet.Rows)
            {
                if (counter > 0)
                {
                    var newPembagian = new Muzaki();
                    newPembagian.Tahun = Tahun;
                    newPembagian.KK = row.Cells[0].Value?.ToString();
                    newPembagian.NoUrut = Convert.ToInt32(row.Cells[1].Value);
                    newPembagian.Nama = row.Cells[2].Value?.ToString();

                    PosisiKeluarga pos;
                    Enum.TryParse(row.Cells[3].Value?.ToString(),out pos);
                    newPembagian.Posisi = pos;
                    newPembagian.IsMustahik = row.Cells[4].Value?.ToString()=="AKTIF" ? true:false;
                    newPembagian.ZakatBeras = Convert.ToInt32( row.Cells[5].Value);
                    newPembagian.TitipZakat = Convert.ToDouble(row.Cells[6].Value);
                    newPembagian.SelisihBeras = Convert.ToDouble(row.Cells[7].Value);
                    newPembagian.SelisihTitipan = Convert.ToDouble(row.Cells[8].Value);
                    newPembagian.Amil = row.Cells[9].Value?.ToString();
                    newPembagian.DanaTalangan = Convert.ToDouble(row.Cells[10].Value);
                    newPembagian.SudahZakat = row.Cells[11].Value?.ToString()=="1"?true:false;
                    newPembagian.SudahRealisasi = row.Cells[12].Value?.ToString()=="1"?true:false;
                    newPembagian.SudahTercatat = row.Cells[13].Value?.ToString() == "1" ? true : false ;
                    dataBaru.Add(newPembagian);
                }
                counter++;


            }
            if (dataBaru.Count > 0)
            {
                //delete existing
                var existing = db.Muzakis.Where(x => x.Tahun == Tahun).ToList();
                existing.ForEach(x => { db.Muzakis.Remove(x); });
                db.SaveChanges();
                //add new
                dataBaru.ForEach(x => { db.Muzakis.Add(x); });
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
            var worksheet = workbook.Worksheets.Add("Muzaki");
            var datas = GetAllData();
            int row = 1;

            var styleHeader = new CellStyle();
            styleHeader.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            styleHeader.VerticalAlignment = VerticalAlignmentStyle.Center;
            styleHeader.Font.Weight = ExcelFont.BoldWeight;
            styleHeader.Font.Color = Color.Black;
            styleHeader.WrapText = true;
            styleHeader.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);
           
            worksheet.Cells[0, 0].Value = "KK";
            worksheet.Cells[0, 1].Value = "Nama";
            worksheet.Cells[0, 2].Value = "No Urut";
            worksheet.Cells[0, 3].Value = "Status";
            worksheet.Cells[0, 4].Value = "Mustahik";
            worksheet.Cells[0, 5].Value = "Zakat Beras";
            worksheet.Cells[0, 6].Value = "Titip Zakat (uang)";
            worksheet.Cells[0, 7].Value = "Selisih Beras";
            worksheet.Cells[0, 8].Value = "Selisih Titipan";
            worksheet.Cells[0, 9].Value = "Amil";
            worksheet.Cells[0, 10].Value = "Dana Talangan";
            worksheet.Cells[0, 11].Value = "Sudah Zakat";
            worksheet.Cells[0, 12].Value = "Sudah Realisasi";
            worksheet.Cells[0, 13].Value = "Sudah Tercatat";

            worksheet.Cells[0, 0].Style = styleHeader;
            worksheet.Cells[0, 1].Style = styleHeader;
            worksheet.Cells[0, 2].Style = styleHeader;
            worksheet.Cells[0, 3].Style = styleHeader;
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

            var style = new CellStyle();
            style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            style.VerticalAlignment = VerticalAlignmentStyle.Center;
            style.Font.Weight = ExcelFont.NormalWeight;
            style.Font.Color = Color.Black;
            style.WrapText = true;
            style.Borders.SetBorders(MultipleBorders.Left | MultipleBorders.Right | MultipleBorders.Top | MultipleBorders.Bottom, Color.Black, LineStyle.Thin);

            foreach (var item in datas)
            { /*
             newPembagian.KK = row.Cells[0
             newPembagian.Nama = row.Cells
             newPembagian.NoUrut = Convert
             PosisiKeluarga pos;
             Enum.TryParse(row.Cells[3].Va
             newPembagian.Posisi = pos;
             newPembagian.IsMustahik = row
             newPembagian.ZakatBeras = Con
             newPembagian.TitipZakat = Con
             newPembagian.SelisihBeras = C
             newPembagian.SelisihTitipan =
             newPembagian.Amil = row.Cells
             newPembagian.DanaTalangan = C
             newPembagian.SudahZakat = row
             newPembagian.SudahRealisasi =
             newPembagian.SudahTercatat = 
             */
                worksheet.Cells[row, 0].Value = item.KK;
                worksheet.Cells[row, 1].Value = item.Nama;
                worksheet.Cells[row, 2].Value = item.NoUrut;
                worksheet.Cells[row, 3].Value = item.Posisi.ToString();
                worksheet.Cells[row, 3].Value = item.IsMustahik?"AKTIF":"NONAKTIF";
                worksheet.Cells[row, 4].Value = item.Posisi.ToString();
                worksheet.Cells[row, 5].Value = item.ZakatBeras;
                worksheet.Cells[row, 6].Value = item.TitipZakat;
                worksheet.Cells[row, 7].Value = item.SelisihBeras;
                worksheet.Cells[row, 8].Value = item.SelisihTitipan;
                worksheet.Cells[row, 9].Value = item.Amil;
                worksheet.Cells[row, 10].Value = item.DanaTalangan.ToString();
                worksheet.Cells[row,11].Value = item.SudahZakat?"Sudah":"Belum";
                worksheet.Cells[row, 12].Value = item.SudahRealisasi?"Sudah":"Belum";
                worksheet.Cells[row, 13].Value = item.SudahTercatat?"Sudah":"Belum";

                worksheet.Cells[row, 0].Style = style;
                worksheet.Cells[row, 1].Style = style;
                worksheet.Cells[row, 2].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 3].Style = style;
                worksheet.Cells[row, 4].Style = style;
                worksheet.Cells[row, 5].Style = style;
                worksheet.Cells[row, 6].Style = style;
                worksheet.Cells[row, 7].Style = style;
                worksheet.Cells[row, 8].Style = style;
                worksheet.Cells[row, 9].Style = style;
                worksheet.Cells[row, 10].Style = style;
                worksheet.Cells[row,11].Style = style;
                worksheet.Cells[row, 12].Style = style;
                worksheet.Cells[row, 13].Style = style;
                row++;
            }
            var tmpfile = Path.GetTempFileName() + ".xlsx";

            workbook.Save(tmpfile);
            return File.ReadAllBytes(tmpfile);
        }
    }
}
