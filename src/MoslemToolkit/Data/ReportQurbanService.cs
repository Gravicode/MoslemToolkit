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
    public class ReportQurbanService
    {
        NgajiDB db;
        CultureInfo ci;
        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public class StatusChangedEventArgs : EventArgs
        {
            public int Progress { get; set; }
            public string Message { get; set; }
        }
        public ReportQurbanService()
        {
            ci = new CultureInfo("id-ID");
            if (db == null) db = new NgajiDB();
            //if (kelasSvc == null) kelasSvc = new KelasService();

        }

        string GetMonthName(int month)
        {
            if (month < 1 || month > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(month);
            return monthName;
        }

        public async Task<byte[]> GenerateReport(ReportQurbanParameter param, bool IsLinux = false)
        {
            try
            {

                List<byte[]> DocParts = new List<byte[]>();
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "Mulai bikin laporan..." });
                await Task.Delay(1);
                #region cover
                //1. Cover

                var bytes = ReadDocAsBytes(AppConstants.Qurban_Cover);
                // Load Word document from file's path.
                var document = DocumentModel.Load(new MemoryStream(bytes));

                // Get Word document's plain text.
                document.Content.Replace("[DESA]", param.Desa);
                document.Content.Replace("[KELOMPOK]", param.Kelompok);
                document.Content.Replace("[Tahun]", param.Tahun.ToString());

                var dataBytes = AddDocToList(document);
                DocParts.Add(dataBytes);
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 10, Message = "Cover selesai..." });
                await Task.Delay(1);
                
                #region content
                var content_template = ReadHtmlAsText(AppConstants.Qurban_Content);
                var content = new System.Text.StringBuilder();
                #region panitia
                if (param.DataList?.Info?.PanitiaUrl != null)
                {
                    var panitia = $@"
        <h2>Panitia dan Tugas</h2>

        <div class=""row"">

            <img style=""object-fit:scale-down"" src='{MoslemToolkit.Helpers.ImageHelper.GetImageAsBase64Url(param.DataList?.Info.PanitiaUrl)}' alt=""panitia"" />
        </div>
        ";
                    content.AppendLine(panitia);

                    StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 20, Message = "Data Panitia selesai..." });
                    await Task.Delay(1);
                }
              

                #endregion
                #region info lain
                if (param.DataList?.Info?.InfoLainUrl != null)
                {
                    var infolain = $@"
        <h2>Info Lainnya</h2>

        <div class=""row"">

            <img style=""object-fit:scale-down"" src='{MoslemToolkit.Helpers.ImageHelper.GetImageAsBase64Url(param.DataList?.Info?.InfoLainUrl)}' alt=""info-lain"" />
        </div>
        ";
                    content.AppendLine(infolain);
                }

                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 30, Message = "Info Lain selesai..." });
                await Task.Delay(1);
                #endregion

                #region data hewan
                var data_hewan = @"<h2>Data Hewan Qurban</h2>

    <div class=""row"">
        <div class=""table-responsive"">
            <table class=""table table-bordered table-stripped"">
                <tr>
                    <td>No</td>
                    <td>Pemilik</td>
                    <td>Bruto (kg)</td>
                    <td>Persen Nett</td>
                    <td>Netto (kg)</td>
                    <td>Kepala Sapi</td>
                    <td>Jenis</td>
                    <td>No Urut Potong</td>
                    <td>Keterangan</td>
                </tr>";
                if (param.DataList?.DataHewanQurban != null)
                {
                    foreach (var item in param.DataList?.DataHewanQurban)
                    {
                        data_hewan += $@"
                        <tr>
                            <td>{item.No}</td>
                            <td>{item.Pemilik}</td>
                            <td>{item.Bruto.ToString("n2")} kg</td>
                            <td>{item.PersenNet.ToString("n2")} %</td>
                            <td>{item.Netto.ToString("n2")} kg</td>
                            <td>{item.KepalaSapi}</td>
                            <td>{item.Jenis.ToString()}</td>
                            <td>{item.NoUrutPotong?.ToString()}</td>
                            <td>{item.Keterangan?.ToString()}</td>
                        </tr>";
                    }
                    data_hewan += $@"
                    <tr>
                        <td colspan=""2"">Total</td>
                        <td><b>{param.DataList?.DataHewanQurban.Sum(x => x.Bruto).ToString("n2")} kg</b></td>
                        <td></td>
                        <td><b>{param.DataList?.DataHewanQurban.Sum(x => x.Netto).ToString("n2")} kg</b></td>
                        <td colspan=""4""></td>
                    </tr>";
                }
                data_hewan += @"
            </table>
        </div>
    </div>
    <hr />";
                content.AppendLine(data_hewan);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 40, Message = "Data Hewan selesai..." });
                await Task.Delay(1);
                #endregion

                #region Data Pembagian
                var data_pembagian = $@"<br/>
                <h2>Data Pembagian</h2>
   
    <div class=""row"">

        <div class=""table-responsive"">

            <table id=""gridData"" name=""gridData"" class=""table table-striped w-auto"">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>Nama</th>
                        <th>Siap</th>
                        <th>Trm</th>
                        <th>KK</th>
                        <th>Status</th>
                        <th>Sapi</th>
                        <th>Gol</th>
                        <th>KP</th>
                        <th>Bagi</th>
                        <th>BL</th>
                        <th>KtgBL</th>
                        <th>Kaki</th>
                        <th>Kpl</th>
                        <th>Tlg</th>
                        <th>Jrn</th>
                        <th>Apr</th>
                    </tr>
                </thead>
                <tbody>";
                    if (param.DataList != null)
                    {
                        int counter = 1;
                        var filtered = param.DataList.Pembagian.Where(x=>x.Pembagian>0).OrderBy(x => x.KK);
                        var attr = new Dictionary<string, object>();
                        var attr2 = new Dictionary<string, object>();
                    foreach (var item in filtered)
                    {
                        attr = item.SudahSiap ? new Dictionary<string, object>()
                            {
                            { "checked", "" }
                            } : new Dictionary<string, object>();
                        attr2 = item.SudahDiterima ? new Dictionary<string, object>()
                            {
                            { "checked", "" }
                            } : new Dictionary<string, object>();
                        data_pembagian += $@"
                            <tr>
                                <td>{(counter++)}</td>
                                <td>{item.Nama}</td>
                                <td>{(item.SudahSiap ? "Ok" : "")}</td>
                                <td>{(item.SudahDiterima ? "Ok" : "")}</td>
                                <td>{item.KK}</td>
                                <td>{item.Status}</td>
                                <td>{item.Sapi}</td>
                                <td>{item.Golongan}</td>
                                <td>{item.KP}</td>
                                <td>{item.Pembagian}</td>
                                <td>{item.BL}</td>
                                <td>{item.Kantong}</td>
                                <td>{item.KAKI}</td>
                                <td>{item.KEPALA}</td>
                                <td>{item.TULANG}</td>
                                <td>{item.JEROHAN}</td>
                                <td>{item.APRESIASI}</td>

                            </tr>";
                    }
                    data_pembagian+=$@"
                        <tr>
                            <td colspan=""9"">Total</td>
                            <td><b>{filtered?.Sum(x=>x.Pembagian).ToString("n2")} kg</b></td>
                            <td><b>{filtered?.Sum(x=>x.BL).ToString("n2")} kg</b></td>
                            <td></td>
                            <td><b>{filtered?.Sum(x=>x.KAKI).ToString("n2")}</b></td>
                            <td><b>{filtered?.Sum(x=>x.KEPALA).ToString("n2")}</b></td>
                            <td><b>{filtered?.Sum(x=>x.TULANG).ToString("n2")}</b></td>
                            <td><b>{filtered?.Sum(x=>x.JEROHAN).ToString("n2")}</b></td>
                            <td><b>{filtered?.Sum(x=>x.APRESIASI).ToString("n2")} kg</b></td>
                        </tr>";
                    }
                    else
                    {
                    data_pembagian+=@"
                        <tr>
                            <td colspan=""14"">
                                No Data
                            </td>
                        </tr>";
                    }
                data_pembagian += @"
                </tbody>
            </table>

        </div>

    </div>";
                content.AppendLine(data_pembagian);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 50, Message = "Data Pembagian selesai..." });
                await Task.Delay(1);
                #endregion

                #region Data Pembagian Khusus
                var data_bl = @"<br/>
                 <h2>Data Pembagian Khusus</h2>
    <div class=""row"">

        <div class=""table-responsive"">
            <table class=""table table-bordered table-stripped w-auto"">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nama</th>
                        <th>Siap</th>
                        <th>Diterima</th>
                        <th>Berat (kg)</th>
                        <th>Bungkus (Pcs)</th>
                        <th>KP</th>
                        <th>Ket.</th>
                        <th>Jenis</th>
                        <th>Total Kg</th>
                    </tr>
                </thead>
                <tbody>";
                    if (param.DataList != null)
                    {
                        int counter = 1;
                        var attr = new Dictionary<string, object>();
                        var attr2 = new Dictionary<string, object>();

                        foreach (var item in param.DataList.BL.OrderBy(x => x.KP).ThenBy(x=>x.Nama))
                        {
                            attr = item.SudahSiap ? new Dictionary<string, object>()
                            {
                            { "checked", "" }
                            } : new Dictionary<string, object>();
                            attr2 = item.SudahDiterima ? new Dictionary<string, object>()
                            {
                            { "checked", "" }
                            } : new Dictionary<string, object>();
                        data_bl+=$@"
                            <tr>
                                <td>{(counter++)}</td>
                                <td>{item.Nama}</td>
                                <td>{(item.SudahSiap?"Ok":"")}</td>
                                <td>{(item.SudahDiterima ? "Ok" : "")}</td>
                                <td>{item.BERAT}</td>
                                <td>{item.BUNGKUS}</td>
                                <td>{item.KP}</td>
                                <td>{item.KETERANGAN}</td>
                                <td>{item.Jenis.ToString()}</td>
                                <td>{((item.BERAT * item.BUNGKUS).ToString("n2"))}</td>

                            </tr>";
                        }
                        data_bl+=$@"
                        <tr>
                            <td colspan=""4"">Total</td>
                            <td><b>{param.DataList?.BL.Sum(x=>x.BERAT).ToString("n2")} kg</b></td>
                            <td><b>{param.DataList?.BL.Sum(x=>x.BUNGKUS).ToString("n2")} pcs</b></td>
                            <td colspan=""3""></td>
                            <td><b>{param.DataList?.BL.Sum(x=>x.BUNGKUS * x.BERAT).ToString("n2")} kg</b></td>
                            
                        </tr>";
                    }
                    else
                    {
                    data_bl+=@"
                        <tr>
                            <td colspan=""9"">
                                No Data
                            </td>
                        </tr>";
                    }
                data_bl += @"
                </tbody>
            </table>
        </div>
    </div>
    <hr />";
                content.AppendLine(data_bl);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 60, Message = "Data Pembagian Khusus selesai..." });
                await Task.Delay(1);
                #endregion

                #region Alokasi Sampil
                var data_sampil = $@"<br/>
                <h2>Alokasi Sampil Jamaah</h2>
    <div class=""row"">
        <table class=""table table-bordered table-stripped"">
            <tr>
                <td>Keterangan</td>
                <td>Nilai</td>
            </tr>
            <tr>
                <td>Jumlah Sampil (pcs)</td>
                <td>{param.DataList.QtySampil?.ToString("n0")} pcs</td>
            </tr>
            <tr>
                <td>Berat Sampil (kg)</td>
                <td>{param.DataList.BeratSampil?.ToString("n0")} kg</td>

            </tr>
            <tr>
                <td>Total Berat Sampil</td>
                <td>{((param.DataList.BeratSampil * param.DataList.QtySampil)?.ToString("n0"))} kg</td>
            </tr>
           
        </table>
    </div>
    <hr />";
                content.AppendLine(data_sampil);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 70, Message = "Data Sampil selesai..." });
                await Task.Delay(1);
                #endregion

                #region simpulan
                var simpulan = $@"<br/>
                <h2>Ringkasan Laporan Qurban {param.Tahun}</h2>

    <div class=""row"">
        <div class=""table-responsive"">";
            if (param.DataList?.Report != null)
            {
                    simpulan+= @"
                <table class=""table table-bordered table-stripped"">
                    <tr>";

                        foreach (System.Data.DataColumn dc in param.DataList?.Report.Columns)
                        {
                        simpulan += $@"
                            <td>{dc.ColumnName}</td>";
                        }
                    simpulan += "</tr>";

                    foreach (System.Data.DataRow item in param.DataList?.Report.Rows)
                    {
                        simpulan += $@"
                        <tr>
                            <td>{item[0]}</td>
                            <td>{item[1]}</td>
                            <td>{item[2]}</td>
                            <td>{item[3]}</td>
                        </tr>";
                    }
                    simpulan+=$@"
                    <tr>
                        <td colspan=""2"">Total</td>

                        <td><b>{param.DataList?.TotalBerat.ToString("n2")} kg</b></td>
                        <td></td>
                    </tr>

                </table>";
            }
            simpulan+=$@"
        </div>
        <table class=""table table-bordered table-stripped"">
            <tr>
                <td>Total Berat Pembagian</td>
                <td>{param.DataList?.TotalBerat.ToString("n2")} kg</td>
            </tr>
            <tr>
                <td>Total Berat Pembagian Tanpa Porsi Sampil</td>
                <td>{param.DataList?.BeratTanpaSampil.ToString("n2")} kg</td>
            </tr>
            <tr>
                <td>Total Berat Bersih Sapi</td>
                <td>{param.DataList?.TotalBeratBersih?.ToString("n2")} kg</td>
            </tr>
            <tr>
                <td>Selisih (Total Berat Bersih Sapi - Total Berat Pembagian)</td>
                <td>{((param.DataList?.TotalBeratBersih-param.DataList?.TotalBerat)?.ToString("n2"))} kg</td>

            </tr>

        </table>
    </div>";
                content.AppendLine(simpulan);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 80, Message = "Data Simpulan selesai..." });
                await Task.Delay(1);
                #endregion
                content_template = content_template.Replace("[CONTENT]", content.ToString());
                var htmlLoadOptions = new GemBox.Document.HtmlLoadOptions() { };
                using (var htmlStream = new MemoryStream(htmlLoadOptions.Encoding.GetBytes(content_template)))
                {
                    // Load input HTML text as stream.
                    document = DocumentModel.Load(htmlStream, htmlLoadOptions);
                    // When reading any HTML content a single Section element is created,
                    // which can be used to specify various Word document's page options.
                    // The same can also be achieved with HTML document itself,
                    // by using CSS properties on "@page" directive or "<body>" element.
                    Section section = document.Sections[0];
                    PageSetup pageSetup = section.PageSetup;
                    PageMargins pageMargins = pageSetup.PageMargins;
                    pageSetup.Orientation = Orientation.Landscape;
                    pageSetup.PaperType = GemBox.Document.PaperType.A4;
                    pageMargins.Top = pageMargins.Bottom = pageMargins.Left = pageMargins.Right = 10;
                    dataBytes = AddDocToList(document);
                    DocParts.Add(dataBytes);
                }
                //File.Delete(Fname);
                #endregion
                
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 90, Message = "Compile content selesai..." });
                await Task.Delay(1);

                dataBytes = PdfHelper.MergePdf(DocParts);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 100, Message = "Merge report selesai..." });
                await Task.Delay(1);
                //concatenate doc part..

                return await Task.FromResult(dataBytes);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 0, Message = "Terjadi kesalahan:" + ex.Message });
                Console.WriteLine(ex);
                return null;
            }
            void ChangeCellValue(DocumentModel document, GemBox.Document.Tables.Table table, int row, int col, object valueCell)
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
                    paragraph.ParagraphFormat.Style.CharacterFormat = new CharacterFormat() { Bold = true, Size = 9 };
                    cell.Blocks.Add(paragraph);
                }
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
            string ReadHtmlAsText(string FilePath)
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
                var text = System.IO.File.ReadAllText(_temp);
                return text;
            }
        }

        public async Task<byte[]> GenerateReportPerKP(List<PembagianPerKP> DataPembagianKP, bool IsLinux = false)
        {
            try
            {

                List<byte[]> DocParts = new List<byte[]>();
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "Mulai bikin laporan..." });
                await Task.Delay(1);
                var content_template = ReadHtmlAsText(AppConstants.Qurban_Content);
                var content = new System.Text.StringBuilder();
                
                #region pembagian
                var data_hewan = @"<h3>Data Pembagian Hewan Qurban</h3>
   
    <div class=""row"">

        <div class=""table-responsive"">


            <table id=""gridData"" name=""gridData"" class=""table table-striped w-auto"">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>Nama</th>
                        <th>Golongan</th>
                        <th>Status</th>
                        <th>KP</th>
                        <th>Kantong BL</th>
                        <th>Pembagian (kg)</th>
                        <th>BL</th>
                        <th>Kaki</th>
                        <th>Kepala</th>
                        <th>Tulang</th>
                        <th>Jerohan</th>
                        <th>Apresiasi (kg)</th>
                    </tr>
                </thead>
                <tbody>";
                    if (DataPembagianKP != null)
                    {
                        int counter = 1;

                        foreach (var item in DataPembagianKP)
                        {
                        data_hewan+=@$"
                            <tr>
                                <td colspan=""13""><b>BERAT {item.Berat.ToString("n0")} Kg / KANTONG {item.KP}</b></td>
                            </tr>";
                            foreach (var row in item.Pembagian)
                            {
                            data_hewan += @$"
                                <tr>
                                    <td>{(counter++)}</td>
                                    <td>{row.Nama}</td>
                                    <td>{row.Golongan}</td>
                                    <td>{row.Status}</td>
                                    <td>{row.KP}</td>
                                    <td>{row.Kantong}</td>
                                    <td>{row.Pembagian}</td>
                                    <td>{row.BL}</td>
                                    <td>{row.KAKI}</td>
                                    <td>{row.KEPALA}</td>
                                    <td>{row.TULANG}</td>
                                    <td>{row.JEROHAN}</td>
                                    <td>{row.APRESIASI}</td>
                                </tr>";
                            }
                        data_hewan += $@"
                            <tr>
                                <td colspan=""6"">Total</td>
                                <td><b>{ item.Pembagian?.Sum(x => x.Pembagian).ToString("n2")} kg</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.BL).ToString("n2")} kg</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.KAKI).ToString("n2")}</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.KEPALA).ToString("n2")}</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.TULANG).ToString("n2")}</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.JEROHAN).ToString("n2")}</b></td>
                                <td><b>{ item.Pembagian?.Sum(x => x.APRESIASI).ToString("n2")} kg</b></td>
                            </tr>";
                        }

                    }
                    else
                    {
                    data_hewan+=@"
                        <tr>
                            <td colspan=""13"">
                                Loading
                            </td>
                        </tr>";
                    }
                    data_hewan+=@"
                </tbody>
            </table>
        </div>
    </div>";
               
                content.AppendLine(data_hewan);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 80, Message = "Data Pembagian selesai..." });
                await Task.Delay(1);
                #endregion
                byte[] dataBytes;

                content_template = content_template.Replace("[CONTENT]", content.ToString());
                var htmlLoadOptions = new GemBox.Document.HtmlLoadOptions() { };
                using (var htmlStream = new MemoryStream(htmlLoadOptions.Encoding.GetBytes(content_template)))
                {
                    // Load input HTML text as stream.
                    var document = DocumentModel.Load(htmlStream, htmlLoadOptions);
                    // When reading any HTML content a single Section element is created,
                    // which can be used to specify various Word document's page options.
                    // The same can also be achieved with HTML document itself,
                    // by using CSS properties on "@page" directive or "<body>" element.
                    Section section = document.Sections[0];
                    PageSetup pageSetup = section.PageSetup;
                    PageMargins pageMargins = pageSetup.PageMargins;
                    pageSetup.Orientation = Orientation.Landscape;
                    pageSetup.PaperType = GemBox.Document.PaperType.A4;
                    pageMargins.Top = pageMargins.Bottom = pageMargins.Left = pageMargins.Right = 10;
                    dataBytes = AddDocToList(document);
                    DocParts.Add(dataBytes);
                }
                //File.Delete(Fname);
               

                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 90, Message = "Compile content selesai..." });
                await Task.Delay(1);

                dataBytes = PdfHelper.MergePdf(DocParts);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 100, Message = "Membuat report selesai..." });
                await Task.Delay(1);
                //concatenate doc part..

                return await Task.FromResult(dataBytes);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 0, Message = "Terjadi kesalahan:" + ex.Message });
                Console.WriteLine(ex);
                return null;
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
            string ReadHtmlAsText(string FilePath)
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
                var text = System.IO.File.ReadAllText(_temp);
                return text;
            }
        }

    }
}
