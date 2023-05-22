using GemBox.Document;
using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MoslemToolkit.Helpers;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MoslemToolkit.Data
{
    public class ReportService 
    {
        NgajiDB db;
        KelasService kelasSvc;
        CultureInfo ci;
        MateriPerKelasService MateriPerKelasSvc ;
        KalenderPendidikanService KalenderPendidikanSvc;
        ProgramSemesterService ProgramSemesterSvc;
        EvaluasiKelasService EvaluasiKelasSvc;
        LaporanHasilBelajarService LaporanHasilBelajarSvc;
        AnekdotService AnekdotSvc;
        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        List<ProgramSemester> DataProgramSemester;
        List<MateriPerKelas> DataMateriPerKelas;
        List<KalenderPendidikan> DataKegiatan;
        static readonly List<CellData> CalendarCells = new List<CellData>() {
        new CellData (5,0),
        new CellData (5,9),
        new CellData (5,18),
        new CellData (19,0),
        new CellData (19,9),
        new CellData (19,18)

        };
        public class StatusChangedEventArgs : EventArgs
        {
            public int Progress { get; set; }
            public string Message { get; set; }
        }
        public ReportService()
        {
            ci = new CultureInfo("id-ID");
            if (db == null) db = new NgajiDB();
            if (kelasSvc == null) kelasSvc = new KelasService();
            if (MateriPerKelasSvc == null) MateriPerKelasSvc = new MateriPerKelasService();
            if (ProgramSemesterSvc == null) ProgramSemesterSvc = new  ProgramSemesterService();
            if (KalenderPendidikanSvc == null) KalenderPendidikanSvc = new  KalenderPendidikanService();
            if (AnekdotSvc == null) AnekdotSvc = new   AnekdotService();
            if (EvaluasiKelasSvc == null) EvaluasiKelasSvc = new  EvaluasiKelasService();
            if (LaporanHasilBelajarSvc == null) LaporanHasilBelajarSvc = new  LaporanHasilBelajarService();

        }

        string GetMonthName(int month)
        {
            if (month < 1 || month > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(month);
            return monthName;
        }

        public async Task<byte[]> GenerateBuku(BukuParameter param, bool IsLinux = false)
        {
            try
            {
                
                List<byte[]> DocParts = new List<byte[]>();
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "Mulai bikin buku..." });
                await Task.Delay(1);
                #region cover
                //1. Cover

                var bytes = ReadDocAsBytes(AppConstants.Report_Cover);
                // Load Word document from file's path.
                var document = DocumentModel.Load(new MemoryStream(bytes));

                // Get Word document's plain text.
                document.Content.Replace("[KELAS]", param.KelasNama);
                document.Content.Replace("[DAERAH]", param.Daerah);
                document.Content.Replace("[DESA]", param.Desa);
                document.Content.Replace("[KELOMPOK]", param.Kelompok);
                document.Content.Replace("[KELAS]", param.KelasNama);
                document.Content.Replace("[DARI]", param.DariTahun.ToString());
                document.Content.Replace("[KE]", param.SampaiTahun.ToString());

                var dataBytes = AddDocToList(document);
                DocParts.Add(dataBytes);
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 10, Message = "Cover selesai..." });
                await Task.Delay(1);
                #region presensi
                //2. presensi

                if (param.SampaiTanggal < param.DariTanggal) throw new Exception("Tanggal sampai harus lebih besar.");

                var selKelas = kelasSvc.GetDataById(param.KelasId);

                bytes = ReadDocAsBytes(AppConstants.Report_Presensi);

                var currentIdx = param.DariTanggal;
                var StartRow = 7;
                Dictionary<string, int> TotalHadir = null;
                while (true)
                {
                    // Load Excel workbook from file's path.
                    ExcelFile workbook = ExcelFile.Load(new MemoryStream(bytes));

                    // Iterate through all worksheets in a workbook.
                    ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                    //replace static data
                    var monthName = ci.DateTimeFormat.GetMonthName(currentIdx.Month);
                    worksheet.Cells.ReplaceText("[KELAS]", param.KelasNama);
                    worksheet.Cells.ReplaceText("[USIA-DARI]", selKelas?.MinUsia.ToString());
                    worksheet.Cells.ReplaceText("[USIA-SAMPAI]", selKelas?.MaxUsia.ToString());
                    worksheet.Cells.ReplaceText("[DAERAH]", param.Daerah);
                    worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                    worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                    worksheet.Cells.ReplaceText("[WALI-KELAS]", selKelas?.WaliKelas);
                    worksheet.Cells.ReplaceText("[BULAN]", monthName);
                    worksheet.Cells.ReplaceText("[TAHUN]", currentIdx.Year.ToString());
                    //iterate data siswa
                    //ambil data kehadiran di bulan berjalan
                    var startDate = new DateTime(currentIdx.Year, currentIdx.Month, 1);
                    var endDate = startDate.AddMonths(1);
                    var kehadiran = (from x in db.NilaiSiswas.Include(c => c.MateriPerKelas).Include(c => c.Jamaah)
                                     where x.Tanggal >= startDate && x.Tanggal < endDate && x.MateriPerKelas.KelasId == param.KelasId
                                     select x).ToList();
                    if (kehadiran.Count == 0)
                    {
                        goto bawah;
                    }
                    //ambil nama siswa dan urutkan
                    var siswas = kehadiran.Select(x => x.Jamaah).Distinct().OrderBy(x => x.Nama).ToList();
                    //tulis ke excel per nama siswa
                    var currentRow = StartRow;
                    var sym = string.Empty;
                    if (TotalHadir == null)
                        TotalHadir = new Dictionary<string, int>();

                    foreach (var siswa in siswas)
                    {
                        TotalHadir["H"] = 0;
                        TotalHadir["S"] = 0;
                        TotalHadir["I"] = 0;
                        TotalHadir["A"] = 0;

                        worksheet.Cells[currentRow, 1].Value = siswa.Nama;
                        var AbsenHarian = kehadiran.Where(x => x.JamaahId == siswa.Id).OrderBy(x => x.Tanggal);
                        var tanggals = AbsenHarian.Select(x => x.Tanggal.Day).Distinct();
                        foreach (var tgl in tanggals)
                        {
                            var hari = AbsenHarian.Where(x => x.Tanggal.Day == tgl).FirstOrDefault();
                            switch (hari.Kehadiran)
                            {
                                case StatusKehadiran.Alpha:
                                    sym = "A";
                                    break;
                                case StatusKehadiran.Hadir:
                                    sym = "H";
                                    break;
                                case StatusKehadiran.Sakit:
                                    sym = "S";
                                    break;
                                case StatusKehadiran.Ijin:
                                    sym = "I";
                                    break;
                                default:
                                    sym = "X";
                                    break;

                            }
                            TotalHadir[sym] += 1;
                            worksheet.Cells[currentRow, 1 + hari.Tanggal.Day].Value = sym;

                        }
                        worksheet.Cells[currentRow, 33].Value = TotalHadir["H"];
                        worksheet.Cells[currentRow, 34].Value = TotalHadir["S"];
                        worksheet.Cells[currentRow, 35].Value = TotalHadir["I"];
                        worksheet.Cells[currentRow, 36].Value = TotalHadir["A"];
                        currentRow++;
                    }
                bawah:
                    dataBytes = AddXlsToList(workbook);
                    DocParts.Add(dataBytes);
                    if (currentIdx.Month == param.SampaiTanggal.Month && currentIdx.Year == param.SampaiTanggal.Year) break;
                    currentIdx = currentIdx.AddMonths(1);
                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 20, Message = "Presensi selesai..." });
                await Task.Delay(1);
                #region jurnal

                bytes = ReadDocAsBytes(AppConstants.Report_Jurnal);
                currentIdx = param.DariTanggal;
                int counter = 0;
                var maxRow = 25;
                StartRow = 5;
                var styleCell = new CellStyle();
                styleCell.HorizontalAlignment = HorizontalAlignmentStyle.Left;
                styleCell.VerticalAlignment = VerticalAlignmentStyle.Center;
                styleCell.WrapText = true;
                styleCell.ShrinkToFit = true;

                styleCell.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

                while (true)
                {
                    counter = 0;
                    // Load Excel workbook from file's path.
                    ExcelFile workbook = ExcelFile.Load(new MemoryStream(bytes));

                    // Iterate through all worksheets in a workbook.
                    ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                    //replace static data
                    var monthName = GetMonthName(currentIdx.Month);
                    worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                    worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                    worksheet.Cells.ReplaceText("[WALI-KELAS]", selKelas?.WaliKelas);
                    worksheet.Cells.ReplaceText("[BULAN]", monthName);
                    var semester = currentIdx.Month >= 1 && currentIdx.Month <= 6 ? "Ganjil" : "Genap";
                    worksheet.Cells.ReplaceText("[SEMESTER]", semester);
                    //iterate data siswa
                    //ambil data kehadiran di bulan berjalan
                    var startDate = new DateTime(currentIdx.Year, currentIdx.Month, 1);
                    var endDate = startDate.AddMonths(1);
                    var kehadiran = (from x in db.NilaiSiswas.Include(c => c.MateriPerKelas).Include(c => c.Jamaah).Include(c => c.MateriPerKelas.Materi)
                                     where x.Tanggal >= startDate && x.Tanggal < endDate && x.MateriPerKelas.KelasId == param.KelasId
                                     select x).ToList();
                    if (kehadiran.Count == 0)
                    {
                        goto bawah2;
                    }
                    //total per hari
                    var harians = kehadiran.Select(x => new DateTime(x.Tanggal.Year, x.Tanggal.Month, x.Tanggal.Day)).Distinct().OrderBy(x => x).ToList();
                    //tulis ke excel per nama siswa
                    var currentRow = StartRow;

                    foreach (var hari in harians)
                    {
                        var materiperhari = kehadiran.Where(x => x.Tanggal.Day == hari.Day && x.Tanggal.Month == hari.Month && x.Tanggal.Year == hari.Year).ToList();
                        var materisatuhari = materiperhari.Select(x => x.MateriPerKelas.Materi.Id).Distinct();
                        foreach (var materi in materisatuhari)
                        {
                            TotalHadir["H"] = materiperhari.Count(x => x.Kehadiran == StatusKehadiran.Hadir && x.MateriPerKelas.Materi.Id == materi);
                            TotalHadir["S"] = materiperhari.Count(x => x.Kehadiran == StatusKehadiran.Sakit && x.MateriPerKelas.Materi.Id == materi);
                            TotalHadir["I"] = materiperhari.Count(x => x.Kehadiran == StatusKehadiran.Ijin && x.MateriPerKelas.Materi.Id == materi);
                            TotalHadir["A"] = materiperhari.Count(x => x.Kehadiran == StatusKehadiran.Alpha && x.MateriPerKelas.Materi.Id == materi);

                            worksheet.Cells[currentRow, 0].Value = counter + 1;
                            worksheet.Cells[currentRow, 1].Value = hari.ToString("dddd, dd/MM/yyyy", ci);
                            worksheet.Cells[currentRow, 2].Value = materiperhari.Where(x => x.MateriPerKelas.Materi.Id == materi).FirstOrDefault().MateriPerKelas.Materi.Nama;
                            worksheet.Cells[currentRow, 4].Value = $"H:{TotalHadir["H"]}. S:{TotalHadir["S"]}, I:{TotalHadir["I"]}, A:{TotalHadir["A"]}";
                            worksheet.Cells[currentRow, 2].Style = styleCell;
                            counter++;
                            currentRow++;

                            //jika sudah melewati maxrow buat new sheet
                            if (currentRow-5 >= maxRow)//&& harians.Count>maxRow
                            {
                                //tulis ke file 
                                dataBytes = AddXlsToList(workbook);
                                DocParts.Add(dataBytes);
                                //reset header and reset row counter
                                currentRow = StartRow;
                                workbook = ExcelFile.Load(new MemoryStream(bytes));

                                // Iterate through all worksheets in a workbook.
                                worksheet = workbook.Worksheets.FirstOrDefault();
                                //replace static data
                                
                                worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                                worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                                worksheet.Cells.ReplaceText("[WALI-KELAS]", selKelas?.WaliKelas);
                                worksheet.Cells.ReplaceText("[BULAN]", monthName);
                                worksheet.Cells.ReplaceText("[SEMESTER]", semester);
                            }
                        }
                    }
                bawah2:
                    dataBytes = AddXlsToList(workbook);
                    DocParts.Add(dataBytes);
                    if (currentIdx.Month == param.SampaiTanggal.Month && currentIdx.Year == param.SampaiTanggal.Year) break;
                    currentIdx = currentIdx.AddMonths(1);
                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 30, Message = "Jurnal selesai..." });
                await Task.Delay(1);
                #region program semester
                var blockCell = new CellStyle();
                blockCell.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                blockCell.VerticalAlignment = VerticalAlignmentStyle.Center;
                blockCell.WrapText = true;
                blockCell.ShrinkToFit = true;
                blockCell.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.Black), SpreadsheetColor.FromName(ColorName.Black));
                blockCell.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

                bytes = ReadDocAsBytes(AppConstants.Report_Prosem);

                if (bytes != null)
                {
                    StartRow = 6;
                    currentIdx = param.DariTanggal;
                    for (int repeat = 0; repeat < 2; repeat++)
                    {
                        ExcelFile workbook = ExcelFile.Load(new MemoryStream(bytes));

                        // Iterate through all worksheets in a workbook.
                        ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();


                        //replace static data
                       
                        worksheet.Cells.ReplaceText("[KELAS]", param.KelasNama.ToUpper());
                        worksheet.Cells.ReplaceText("[USIA-DARI]", selKelas?.MinUsia.ToString());
                        worksheet.Cells.ReplaceText("[USIA-SAMPAI]", selKelas?.MaxUsia.ToString());
                        worksheet.Cells.ReplaceText("[DAERAH]", param.Daerah);
                        worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                        worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                        var semester = currentIdx.Month >= 1 && currentIdx.Month <= 6 ? "Ganjil" : "Genap";
                        worksheet.Cells.ReplaceText("[SEMESTER]", semester.ToUpper());
                        //retrieve data
                        var semesterNum = semester == "Ganjil" ? 1 : 2;
                        DataMateriPerKelas = MateriPerKelasSvc.GetAllData(param.KelasId, param.DariTahun,semesterNum );
                        DataProgramSemester = ProgramSemesterSvc.GetAllData(param.KelasId, param.DariTahun, semesterNum);
                        //rename month
                        for (int i = 0; i < 6; i++)
                        {
                            var row = 4;
                            var col = 3 + (i * 4);
                            worksheet.Cells[row, col].Value = GetMonthName(currentIdx.Month + i);
                        }
                        //fill rows
                        counter = 0;
                        if (DataMateriPerKelas != null)
                        {
                            foreach (var materi in DataMateriPerKelas)
                            {

                                worksheet.Cells[StartRow + counter, 0].Value = counter + 1;
                                worksheet.Cells[StartRow + counter, 1].Value = materi.Materi.Bab.Nama;
                                worksheet.Cells[StartRow + counter, 2].Value = materi.Materi.Nama;
                                var ket = materi.Keterangan;
                                var colCount = 0;
                                for (int i = 0; i < 6; i++)
                                {
                                    var currentMonth = param.DariTanggal.Month + i;
                                    for (int x = 0; x < 4; x++)
                                    {
                                        var currentWeek = x + 1;
                                        if (!(x > 0 && i == 5))
                                        {
                                            var exists = isKelasExist(materi.Id, currentMonth, currentWeek);
                                            if (exists)
                                            {
                                                //write x
                                                worksheet.Cells[StartRow + counter, 3 + colCount].Value = "X";
                                                worksheet.Cells[StartRow + counter, 3 + colCount].Style = blockCell;

                                            }
                                        }
                                    
                                        colCount++;
                                    }
                                }

                                counter++;
                            }
                        }
                        dataBytes = AddXlsToList(workbook);
                        DocParts.Add(dataBytes);
                        currentIdx = currentIdx.AddMonths(6);
                    }

                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 40, Message = "Program semester selesai..." });
                await Task.Delay(1);
                #region kalender pendidikan
                var liburCell = new CellStyle();
                liburCell.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                liburCell.VerticalAlignment = VerticalAlignmentStyle.Center;
                liburCell.WrapText = true;
                liburCell.ShrinkToFit = true;
                liburCell.Font.Color = SpreadsheetColor.FromName(ColorName.Red);              
                liburCell.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.Red), SpreadsheetColor.FromName(ColorName.White));
                liburCell.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

                var dayCell = new CellStyle();
                dayCell.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                dayCell.VerticalAlignment = VerticalAlignmentStyle.Center;
                dayCell.WrapText = true;
                
                dayCell.ShrinkToFit = true;
                liburCell.Font.Color = SpreadsheetColor.FromName(ColorName.Black);
                dayCell.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.White), SpreadsheetColor.FromName(ColorName.White));
                dayCell.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);

                var eventCell = new CellStyle();
                eventCell.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                eventCell.VerticalAlignment = VerticalAlignmentStyle.Center;
                eventCell.WrapText = true;
                eventCell.ShrinkToFit = true;
                liburCell.Font.Color = SpreadsheetColor.FromName(ColorName.Black);
                eventCell.FillPattern.SetPattern(FillPatternStyle.Solid, SpreadsheetColor.FromName(ColorName.LightGreen), SpreadsheetColor.FromName(ColorName.White));
                eventCell.Borders.SetBorders(GemBox.Spreadsheet.MultipleBorders.All, SpreadsheetColor.FromName(ColorName.Black), LineStyle.Medium);


                bytes = ReadDocAsBytes(AppConstants.Report_Kalender);

                if (bytes != null)
                {
                    StartRow = 6;
                    currentIdx = param.DariTanggal;

                    //retrieve data
                    for (int repeat = 0; repeat < 2; repeat++)
                    {
                        ExcelFile workbook = ExcelFile.Load(new MemoryStream(bytes));

                        // Iterate through all worksheets in a workbook.
                        ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();

                        DataKegiatan = KalenderPendidikanSvc.GetAllData(currentIdx, currentIdx.AddMonths(6));

                        //replace static data

                        worksheet.Cells.ReplaceText("[DAERAH]", param.Daerah);
                        worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                        worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                        var semester = currentIdx.Month >= 1 && currentIdx.Month <= 6 ? "Ganjil" : "Genap";
                        worksheet.Cells.ReplaceText("[SEMESTER]", semester.ToUpper());
                        
                        var semesterNum = semester == "Ganjil" ? 1 : 2;
                       
                        //rename month
                        counter = 0;
                        foreach(var item in CalendarCells)
                        {
                            worksheet.Cells[item.Row, item.Col].Value = GetMonthName(currentIdx.Month) + " "+ param.DariTanggal.Year;
                            //fill dates
                            var curCol = item.Col;
                            var curRow = item.Row+2;
                            for(var tgl=new DateTime(currentIdx.Year,currentIdx.Month,1);tgl<currentIdx.AddMonths(1);tgl=tgl.AddDays(1))
                            {
                                curCol = item.Col + (int) tgl.DayOfWeek;
                                worksheet.Cells[curRow, curCol].Value = tgl.Day.ToString();
                                
                                if (isKegiatanExist(tgl))
                                {
                                    worksheet.Cells[curRow, curCol].Style = eventCell;
                                }
                                else
                                {
                                    switch (tgl.DayOfWeek)
                                    {
                                        case DayOfWeek.Sunday:
                                            worksheet.Cells[curRow, curCol].Style = liburCell;
                                            break;
                                        default:
                                            worksheet.Cells[curRow, curCol].Style = dayCell;
                                            break;
                                    }
                                }
                                if(tgl.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    curRow++;
                                    curCol = item.Col;
                                }
                                else
                                 curCol++;
                            }
                            //list kegiatan in month
                            var row = 0;
                            var kegiatanInMonth = DataKegiatan.Where(x => x.TanggalMulai.Month == currentIdx.Month).OrderBy(x=>x.TanggalMulai);
                            foreach(var activity in kegiatanInMonth)
                            {
                                worksheet.Cells[curRow+1+row, item.Col].Value = $"{activity.TanggalMulai.Day}-{activity.TanggalSelesai.Day}";
                                worksheet.Cells[curRow+1+row, item.Col+1].Value = activity.Kegiatan;
                                row++;
                            }
                            currentIdx = currentIdx.AddMonths(1);
                            counter++;
                        }
                        
                        dataBytes = AddXlsToList(workbook);
                        DocParts.Add(dataBytes);
                        
                    }

                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 50, Message = "Kalender pendidikan selesai..." });
                await Task.Delay(1);
                #region anekdot
                bytes = ReadDocAsBytes(AppConstants.Report_Anekdot);

                if (bytes != null)
                {
                    StartRow = 5;
                    var MaxRow = 16;
                    counter = 0;
                    var num = 0;
                    ExcelFile workbook = ExcelFile.Load(new MemoryStream(bytes));

                    // Iterate through all worksheets in a workbook.
                    ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();

                    //retrieve data
                    var DataAnekdot = AnekdotSvc.GetAllData(param.DariTanggal,param.SampaiTanggal,param.KelasId);
                    foreach(var anekdot in DataAnekdot)
                    {
                        num++;
                        if (counter == 0)
                        {
                            //create new
                            workbook = ExcelFile.Load(new MemoryStream(bytes));
                            // Iterate through all worksheets in a workbook.
                            worksheet = workbook.Worksheets.FirstOrDefault();
                            //replace static data
                            worksheet.Cells.ReplaceText("[DARI]", param.DariTahun.ToString());
                            worksheet.Cells.ReplaceText("[KE]", param.SampaiTahun.ToString());
                            worksheet.Cells.ReplaceText("[WALI-KELAS]", selKelas?.WaliKelas);                            
                        }
                        //write row
                        worksheet.Cells[StartRow + counter, 0].Value =num;
                        worksheet.Cells[StartRow + counter, 1].Value =anekdot.Tanggal.ToString("dddd, dd-MMM-yyyy",ci);
                        worksheet.Cells[StartRow + counter, 2].Value = $"P:{anekdot.Permasalahan}{Environment.NewLine}S:{anekdot.Solusi}";

                        counter++;
                        if (counter > MaxRow || counter == DataAnekdot.Count)
                        {
                            counter = 0;
                            dataBytes = AddXlsToList(workbook);
                            DocParts.Add(dataBytes);
                        }

                    }
                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 60, Message = "Anekdot selesai..." });
                await Task.Delay(1);
                #region evaluasi kelas

                bytes = ReadDocAsBytes(AppConstants.Report_Evaluasi);
                currentIdx = param.DariTanggal;
                for (int repeat = 0; repeat < 2; repeat++)
                {
                    // Load Word document from file's path.
                    document = DocumentModel.Load(new MemoryStream(bytes));

                    // replace static section
                    foreach (var section in document.Sections)
                    {
                        foreach (var item in section.HeadersFooters)
                        {
                            item.Content.Replace("[KELAS]", param.KelasNama);
                            item.Content.Replace("[DAERAH]", param.Daerah);
                            item.Content.Replace("[DARI]", param.DariTahun.ToString());
                            item.Content.Replace("[KE]", param.SampaiTahun.ToString());
                            item.Content.Replace("[USIA-DARI]", selKelas?.MinUsia.ToString());
                            item.Content.Replace("[USIA-SAMPAI]", selKelas?.MaxUsia.ToString());
                            item.Content.Replace("[TANGGAL]", DateHelper.GetLocalTimeNow().ToString("dd MMMM yyyy", ci));
                        }
                    }
                    var maxcol = 20;
                    var col = 3;
                    foreach (var section in document.Sections)
                    {
                        foreach (var block in section.Blocks)
                        {
                            if (block.ElementType == ElementType.Table)
                            {
                                var table = block as GemBox.Document.Tables.Table;
                                var semester = currentIdx.Month >= 1 && currentIdx.Month <= 6 ? 1 : 2;
                                //retrieve data
                                var DataEvaluasiKelas = EvaluasiKelasSvc.GetAllData(param.KelasId, param.DariTahun, semester);
                                //clear row table
                                for (int i = table.Rows.Count - 1; i > 1; i--)
                                {
                                    var removerow = table.Rows[i];
                                    table.Rows.Remove(removerow);
                                }
                                //change semester 
                                var selRow = table.Rows[0];
                                if (selRow != null)
                                {
                                    var cell = selRow.Cells[2];
                                    var paragraph = new Paragraph(document, $"PENJABARAN MATERI SEMESTER {semester}");
                                    paragraph.ParagraphFormat.Style.CharacterFormat = new CharacterFormat() { Bold=true };
                                    cell.Blocks.Clear();
                                    cell.Blocks.Add(paragraph);
                                }
                                    //nama siswa
                                    col = 0;
                                var siswas = DataEvaluasiKelas.Select(x => x.Jamaah).OrderBy(x => x.Nama).Distinct().ToList();
                                selRow = table.Rows[1];
                                foreach (var siswa in siswas)
                                {
                                    var cell = selRow.Cells[col];
                                    var paragraph = new Paragraph(document, siswa.Nama);
                                    cell.Blocks.Clear();
                                    cell.Blocks.Add(paragraph);
                                    //cell.CellFormat.FitText = true;
                                    col++;
                                }
                                counter = 0;
                                //add rows
                                foreach (var item in DataEvaluasiKelas)
                                {
                                    // Create a row and add it to table.
                                    var NewRow = new List<object>();
                                    NewRow.Add(counter + 1);
                                    NewRow.Add(item.MateriPerKelas.Materi.Bab.Nama);
                                    NewRow.Add(item.MateriPerKelas.Materi.Nama);
                                    foreach (var siswa in siswas)
                                    {
                                        var selSiswa = DataEvaluasiKelas.Where(x => x.JamaahId == siswa.Id && x.MateriPerKelasId == item.MateriPerKelasId).FirstOrDefault();
                                        NewRow.Add(selSiswa != null ? selSiswa.Nilai : "0");
                                    }
                                    //fill empty cell
                                    for(int i=0;i<maxcol - siswas.Count; i++)
                                    {
                                        NewRow.Add("");
                                    }
                                    AddRowCell(document, table, NewRow.ToArray());
                                    counter++;
                                }
                            }
                        }

                    }

                    dataBytes = AddDocToList(document);
                    DocParts.Add(dataBytes);
                    currentIdx = currentIdx.AddMonths(6);
                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 70, Message = "Evaluasi kelas selesai..." });
                await Task.Delay(1);
                #region laporan hasil belajar

                bytes = ReadDocAsBytes(AppConstants.Report_Raport);
                //get data per tahun
                var DataEvaluasiTahun = EvaluasiKelasSvc.GetAllData(param.KelasId, param.DariTahun);
                var Datasiswas = DataEvaluasiTahun.Select(x => x.Jamaah).Distinct().OrderBy(x => x.Nama).ToList();
                foreach (var siswa in Datasiswas)
                {
                    currentIdx = param.DariTanggal;
                    for (int repeat = 0; repeat < 2; repeat++)
                    {
                        var semester = currentIdx.Month >= 1 && currentIdx.Month <= 6 ? 1 : 2;
                        // Load Word document from file's path.
                        document = DocumentModel.Load(new MemoryStream(bytes));

                        // replace static section

                        document.Content.Replace("[NAMA]", siswa.Nama);
                        document.Content.Replace("[NIS]", siswa.Id.ToString());
                        document.Content.Replace("[KELOMPOK]", AppConstants.NAMA_KELOMPOK);
                        document.Content.Replace("[SEMESTER]", $"{semester} ({(semester == 1 ? "GANJIL" : "GENAP")})");
                        document.Content.Replace("[KELAS]", param.KelasNama);
                        document.Content.Replace("[DAERAH]", param.Daerah);
                        document.Content.Replace("[DARI]", param.DariTahun.ToString());
                        document.Content.Replace("[KE]", param.SampaiTahun.ToString());
                        document.Content.Replace("[USIA-DARI]", selKelas?.MinUsia.ToString());
                        document.Content.Replace("[USIA-SAMPAI]", selKelas?.MaxUsia.ToString());
                        document.Content.Replace("[TANGGAL]", DateHelper.GetLocalTimeNow().ToString("dd MMMM yyyy", ci));

                        var tablecount = 0;
                        var DataEvaluasiSemester = DataEvaluasiTahun.Where(x => x.JamaahId == siswa.Id && x.MateriPerKelas.Semester == semester);
                        foreach (var section in document.Sections)
                        {
                            foreach (var block in section.Blocks)
                            {
                                if (block.ElementType == ElementType.Table)
                                {
                                    var table = block as GemBox.Document.Tables.Table;
                                    //table nilai
                                    if (tablecount == 1)
                                    {
                                        //retrieve data per semester
                                        //clear row table
                                        for (int i = table.Rows.Count - 1; i > 1; i--)
                                        {
                                            var removerow = table.Rows[i];
                                            table.Rows.Remove(removerow);
                                        }
                                        counter = 0;
                                        //add rows
                                        foreach (var item in DataEvaluasiSemester)
                                        {
                                            // Create a row and add it to table.
                                            var NewRow = new List<object>();
                                            NewRow.Add(counter + 1);
                                            NewRow.Add(item.MateriPerKelas.Materi.Bab.Nama);
                                            NewRow.Add(item.MateriPerKelas.Materi.Nama);
                                            NewRow.Add(item.NilaiMutu);
                                            NewRow.Add(item.Keterangan);
                                            AddRowCell(document, table, NewRow.ToArray());
                                            counter++;
                                        }
                                    }
                                    else if(tablecount==2)
                                    //table absen dsb
                                    {
                                        var stat = LaporanHasilBelajarSvc.GetData(param.KelasId, param.DariTahun, semester, siswa.Id);
                                        if (stat != null)
                                        {
                                            ChangeCellValue(document, table, 1, 1, stat.Sakit);
                                            ChangeCellValue(document, table, 2, 1, stat.Ijin);
                                            ChangeCellValue(document, table, 3, 1, stat.Alpha);

                                            ChangeCellValue(document, table, 1, 3, stat.SikapAkhlak);
                                            ChangeCellValue(document, table, 2, 3, stat.Kerajinan);
                                            ChangeCellValue(document, table, 3, 3, stat.KebersihanKerapihan);
                                        }
                                    
                                    }
                                    tablecount++;
                                }
                            }

                        }

                        dataBytes = AddDocToList(document);
                        DocParts.Add(dataBytes);
                        currentIdx = currentIdx.AddMonths(6);
                    }
                }
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 100, Message = "Laporan belajar, buku selesai..." });
                await Task.Delay(1);
                //concatenate doc part..
                dataBytes = PdfHelper.MergePdf(DocParts);

                return await Task.FromResult(dataBytes);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 0, Message = "Terjadi kesalahan:"+ex.Message });
                Console.WriteLine(ex);
                return null;
            }
            void ChangeCellValue(DocumentModel document, GemBox.Document.Tables.Table table, int row,int col, object valueCell)
            {
                try
                {
                    var curRow = table.Rows[row];
                    var cell = curRow.Cells[col];
                    cell.Blocks.Clear();
                    // Create a paragraph and add it to cell.
                    var paragraph = new Paragraph(document, valueCell.ToString());
                    //paragraph.ParagraphFormat.Style.CharacterFormat = new CharacterFormat() { Bold = true, Size = 9 };
                    cell.Blocks.Add(paragraph);
                }
                catch { }
            }
            void AddRowCell(DocumentModel document, GemBox.Document.Tables.Table table, object[] Data)
            {
                var row = new GemBox.Document.Tables.TableRow(document);
                table.Rows.Add(row);
                for (int c = 0; c < Data.Length; c++)
                {
                    // Create a cell and add it to row.
                    var cell = new GemBox.Document.Tables.TableCell(document);
                    //cell.CellFormat.FitText = true;
                    row.Cells.Add(cell);

                    // Create a paragraph and add it to cell.
                    var paragraph = new Paragraph(document, Data[c].ToString());
                    paragraph.ParagraphFormat.Style.CharacterFormat =  new CharacterFormat() { Bold = true, Size=9 };
                    cell.Blocks.Add(paragraph);
                }
            }
            bool isKegiatanExist(DateTime tanggal)
            {
                if (DataKegiatan == null) return false;
                var exists = DataKegiatan.Any(x => tanggal >= x.TanggalMulai && tanggal <= x.TanggalSelesai);
                return exists;
            }
            bool isKelasExist(long MateriKelasId, int Bulan, int Minggu)
            {
                if (DataProgramSemester == null) return false;
                var exists = DataProgramSemester.Any(x => x.MateriPerKelasId == MateriKelasId && x.Bulan == Bulan && x.Minggu == Minggu);
                return exists;
            }
            byte[] AddDocToList(DocumentModel document)
            {
                var pdfSaveOptions = new GemBox.Document.PdfSaveOptions() { ImageDpi = 220 };
                byte[] dataBytes;
                using (var pdfStream = new MemoryStream())
                {
                    document.Save(pdfStream, pdfSaveOptions);
                    dataBytes = pdfStream.ToArray();
                }
                return dataBytes;
            }
            byte[] AddXlsToList(ExcelFile document)
            {
                foreach (var ws in document.Worksheets)
                {
                    ws.PrintOptions.FitWorksheetWidthToPages = 1;
                    ws.PrintOptions.FitWorksheetHeightToPages = 1;
                }
                var pdfSaveOptions = new GemBox.Spreadsheet.PdfSaveOptions() { ImageDpi = 220 };
                byte[] dataBytes;
                using (var pdfStream = new MemoryStream())
                {
                    document.Save(pdfStream, pdfSaveOptions);
                    dataBytes = pdfStream.ToArray();
                }
                return dataBytes;
            }
            byte[] ReadDocAsBytes(string FilePath)
            {
                if (!System.IO.File.Exists(FilePath)) throw new Exception($"Template {Path.GetFileNameWithoutExtension(FilePath)} is not found");
                var _temp = string.Empty;
                if (IsLinux)
                {
                    _temp = FilePath.Replace("\\", "/");
                }
                else
                {
                    _temp = FilePath;
                }
                var bytes = System.IO.File.ReadAllBytes(_temp);
                return bytes;
            }
        }

    }
}
