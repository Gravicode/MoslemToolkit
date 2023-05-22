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
    public class ReportZakatService
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
        public ReportZakatService()
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

        public async Task<byte[]> GenerateReport(ZakatFitrahParameter param, bool IsLinux = false)
        {
            try
            {

                List<byte[]> DocParts = new List<byte[]>();
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "Mulai bikin laporan..." });
                await Task.Delay(1);
                #region cover
                //1. Cover

                var bytes = ReadDocAsBytes(AppConstants.Zakat_Fitrah_Cover);
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
                var content_template = ReadHtmlAsText(AppConstants.Zakat_Fitrah_Content);
                var content = new System.Text.StringBuilder();
                #region muzaki
                var muzaki_detail = $@"<div class='row'>
        <h4>Muzaki Titip Beras:</h4>
        <div class='table-responsive'>
            <table class='table table-bordered table-striped'>
                <tr>
                    <td>No</td>
                    <td>KK</td>
                    <td>Nama</td>
                    <td>Sudah Tercatat</td>
                   
                  
                </tr>";


                var counter = 1;

                foreach (var orang in param.MuzakiBeras.OrderBy(x=>x.KK).ThenBy(x=>x.NoUrut))
                {
                    muzaki_detail += $@"
                    <tr>
                        <td>{(counter++)}</td>
                        <td>{orang.KK}</td>
                        <td>{(orang.Posisi == PosisiKeluarga.KK?$"<b>{orang.Nama}</b>": orang.Nama)}</td>
                        <td>{(orang.SudahTercatat ? "Sudah" : "Belum")}</td>
                    
                        

                    </tr>";
                }
                muzaki_detail += $@"
            </table>
        </div>
        <br/>
        <h4>Muzaki Titip Uang:</h4>
        <div class='table-responsive'>
            <table class='table table-bordered table-striped'>
                <tr>
                    <td>No</td>
                    <td>KK</td>
                    <td>Nama</td>
                    <td>Sudah Tercatat</td>
                   
                    
                </tr>";

                counter = 1;

                foreach (var orang in param.MuzakiUang.OrderBy(x => x.KK).ThenBy(x => x.NoUrut))
                {
                    muzaki_detail += $@"
                    <tr>
                        <td>{(counter++)}</td>
                        <td>{orang.KK}</td>
                        <td>{(orang.Posisi == PosisiKeluarga.KK ? $"<b>{orang.Nama}</b>" : orang.Nama)}</td>
                        <td>{(orang.SudahTercatat ? "Sudah" : "Belum")}</td>
                    
                        
                    </tr>";
                }
                muzaki_detail += $@"
            </table>
        </div>
        <br/>
        <h4>Muzaki Belum Setor Zakat:</h4>
        <div class='table-responsive'>
            <table class='table table-bordered table-striped'>
                <tr>
                    <td>No</td>
                    <td>KK</td>
                    <td>Nama</td>
                    <td>Sudah Tercatat</td>
                </tr>";

                counter = 1;

                foreach (var orang in param.MuzakiBelum.OrderBy(x => x.KK).ThenBy(x => x.NoUrut))
                {
                    muzaki_detail += $@"
                    <tr>
                        <td>{(counter++)}</td>
                        <td>{orang.KK}</td>
                        <td>{(orang.Posisi == PosisiKeluarga.KK ? $"<b>{orang.Nama}</b>" : orang.Nama)}</td>
                        <td>{(orang.SudahTercatat ? "Sudah" : "Belum")}</td>
                        
                    </tr>";
                }
                muzaki_detail += $@"
            </table>
        </div>        
    </div>";
                content.AppendLine(muzaki_detail);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 20, Message = "Muzaki detail selesai..." });
                await Task.Delay(1);

                #endregion
                #region iterasi pembagian
                var iterasi_pembagian = $@"<hr />

    <h3>Iterasi Pembagian</h3>
    <div class='row'>";
                if (param.PembagianDetail != null)
                {
                    foreach (var item in param.PembagianDetail)
                    {
                        iterasi_pembagian += $@"
                <div class='table-responsive'>
                    <table class='table table-bordered table-striped'>
                        <tr>
                            <td colspan='7'>Pembagian {item.No}</td>
                        </tr>
                        <tr>
                            <td colspan='7'>{item.TotalPembagian.ToString("n0")} sok</td>
                        </tr>
                        <tr>
                            <td>Penerima</td>
                            <td colspan='4'>Beras (sok)</td>
                            <td>Uang (Rp)</td>
                            <td>Pembeli</td>

                        </tr>
                        <tr>
                            <td>Nama</td>
                            <td>Sisa Sebelumnya</td>
                            <td>Diterima</td>
                            <td>Dijual</td>
                            <td>Sisa</td>
                            <td>Hasil Dijual (Rp)</td>
                            <td>Nama</td>
                        </tr>";
                        foreach (var detail in item.Detail)
                        {
                            iterasi_pembagian += $@"
                            <tr>
                                <td>{detail.Nama}</td>
                                <td>{detail.SisaSebelumnya.ToString("n2")}</td>
                                <td>{detail.Terima.ToString("n2")}</td>
                                <td>{detail.Dijual}</td>
                                <td>{detail.Sisa.ToString("n2")}</td>
                                <td>{detail.Uang.ToString("n2")}</td>
                                <td>";

                            var names = detail.DibeliOleh.Count > 0 ? String.Join(", ", detail.DibeliOleh.Select(x => x.Nama).ToArray()) : "-";
                            iterasi_pembagian += $@"
                                    {names}
                                </td>
                            </tr>";
                        }
                        iterasi_pembagian += $@"
                        <tr>
                            <td><b>Total</b></td>
                            <td><b>{item.TotalBerasSisaSebelumnya.ToString("n2")}</b></td>
                            <td><b>{item.TotalPembagian.ToString("n2")}</b></td>
                            <td><b>{item.TotalBerasDijual.ToString("n2")}</b></td>
                            <td><b>{item.TotalBerasSisa.ToString("n2")}</b></td>
                            <td><b>Rp. {item.TotalUang.ToString("n2")}</b></td>
                            <td></td>

                        </tr>
                    </table>
                </div>";

                    }

                }
                iterasi_pembagian += "</div>";
                content.AppendLine(iterasi_pembagian);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 30, Message = "Iterasi pembagian selesai..." });
                await Task.Delay(1);

                #endregion
                #region pembagian uang
                var pembagian_uang = $@"<br/><hr />
    <h3>Pembagian Khusus (Dalam Bentuk Uang)</h3>
    <div class='row'>
        <div class='table-responsive'>
            <table class='table table-bordered'>
                <tr>
                    <td>Nama</td>
                    <td>Beras (sok)</td>
                    <td>Harga Jual Beras (Rp)/Sok</td>
                    <td>Total Jual Beras (Rp)</td>
                    <td>Uang</td>
                    <td>Total Jual Beras + Uang</td>
                </tr>";
                if (param.Pembagian != null)
                {
                    var SubTotal = 0d;
                    var Total = 0d;
                    foreach (var item in param.Pembagian)
                    {
                        if (item.Jenis != "Aznab" && item.Nama != "Kelompok")
                        {
                            SubTotal += (item.Beras * param.HargaJualBeras);
                            Total += (item.Beras * param.HargaJualBeras) + item.Uang;
                            pembagian_uang += $@"
                            <tr>
                                <td>{item.Nama}</td>
                                <td>{item.Beras.ToString("n2")} sok</td>
                                <td>{ param.HargaJualBeras}</td>
                                <td>Rp. {((item.Beras * param.HargaJualBeras).ToString("n0"))}</td>
                                <td>Rp. {item.Uang.ToString("n0")}</td>
                                <td>Rp. {(((item.Beras * param.HargaJualBeras) + item.Uang).ToString("n0"))}</td>
                            </tr>";
                        }
                    }
                    pembagian_uang += $@"
                    <tr>
                        <td colspan='3'>Sub Total</td>
                        <td><b>Rp. {SubTotal.ToString("n0")}</b></td>
                        <td>Total</td>
                        <td><b>Rp. {Total.ToString("n0")}</b></td>
                    </tr>";
                }
                pembagian_uang += $@"
            </table>
        </div>
    </div>";
                content.AppendLine(pembagian_uang);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 40, Message = "Pembagian uang selesai..." });
                await Task.Delay(1);

                #endregion
                #region summary


                //fill content
                var summary_str = $@"<br/><h3>Summary</h3>
    <div class='row'>
        <table class='table'>
            <tr>
                <td>Keterangan</td>
                <td>Nilai</td>
            </tr>
            <tr>
                <td>Tahun</td>
                <td>{param.Tahun}</td>
            </tr>
            <tr>
                <td>Total Beras (sok)</td>
                <td>{param.TotalBeras.ToString("n0")}</td>
            </tr>
            <tr>
                <td>Total Uang Titipan Zakat (Rp)</td>
                <td>Rp. {param.TotalUang.ToString("n0")}</td>
            </tr>
            <tr>
                <td>Total dalam Sok</td>
                <td>{param.TotalDalamBeras.ToString("n2")} sok</td>
            </tr>
            <tr>
                <td>Harga Beras</td>
                <td>{param.HargaBeras.ToString("n0")}</td>
            </tr>

        </table>
    </div>";

                content.AppendLine(summary_str);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 50, Message = "Summary total selesai..." });
                await Task.Delay(1);

                #endregion
                #region muzaki summary
                var muzaki_summary = $@"<br/><hr/>
<h3>Muzaki</h3>
    <div class='row'>
        <div class='col-6'>
            <table class='table'>
                <tr>
                    <td>Keterangan</td>
                    <td>Jumlah</td>
                    <td>Prosentase</td>
                </tr>";
                if (param.MuzakiStat != null)
                {
                    foreach (var row in param.MuzakiStat)
                    {
                        muzaki_summary += $@"<tr>
<td>{row.Nama}</td>
<td>{row.Total.ToString("n2")}</td>
<td>{((row.Prosentase * 100).ToString("n2"))}%</td>

</tr>";
                    }
                }
                muzaki_summary += $@"
                <tr>
                    <td>Total</td>
                    <td><b>{(param.MuzakiStat.Sum(z => z.Total).ToString("n0"))}</b></td>
                    <td><b>{((param.MuzakiStat.Sum(z => z.Prosentase) * 100).ToString("n2"))} %</b></td>
                </tr>
            </table>
            <hr />
            <table class='table'>
                <tr>
                    <td>Keterangan</td>
                    <td>Jumlah</td>
                    <td>Prosentase</td>
                </tr>";
                if (param.Stats != null)
                {
                    foreach (var row in param.Stats)
                    {
                        muzaki_summary += $@"
                       <tr>
<td>{row.Nama}</td>
<td>{row.Jumlah.ToString("n0")}</td>
<td>{((row.Persen).ToString("n2"))}%</td>

</tr>";
                    }
                }
                muzaki_summary += $@"
                <tr>
                    <td>Total</td>
                    <td><b>{(param.Stats.Sum(z => z.Jumlah).ToString("n0"))}</b></td>
                    <td><b>{((param.Stats.Sum(z => z.Persen)).ToString("n2"))} %</b></td>
                </tr>
            </table>
        </div>
        
    </div>";
                content.AppendLine(muzaki_summary);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 60, Message = "summary muzaki selesai..." });
                await Task.Delay(1);

                #endregion
                #region mustahik summary
                var mustahik_summary = $@"<br/><h3>Mustahik</h3>
    <div class='row'>
        <div class='col-6'>
            <table class='table'>
                <tr>
                    <td>Keterangan</td>
                    <td>Jumlah</td>
                    <td>Prosentase</td>
                </tr>";
                if (param.MustahikStat != null)
                {
                    foreach (var row in param.MustahikStat)
                    {
                        mustahik_summary += $@"
                        <tr>
                            <td>{row.Nama}</td>
                            <td>{row.Total.ToString("n2")}</td>
                            <td>{((row.Prosentase * 100).ToString("n2"))} %</td>
                        </tr>";
                    }
                }
                mustahik_summary += $@"
                <tr>
                    <td>Total</td>
                    <td><b>{(param.MustahikStat.Sum(z => z.Total).ToString("n0"))}</b></td>
                    <td><b>{((param.MustahikStat.Sum(z => z.Prosentase) * 100).ToString("n2"))} %</b></td>
                </tr>
            </table>
        </div>        
    </div>";
                content.AppendLine(mustahik_summary);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 70, Message = "summary mustahik selesai..." });
                await Task.Delay(1);

                #endregion
                #region pembagian zakat
                var pembagian_summary = $@"<br/><hr />
    <h3>Pembagian Zakat</h3>
    <div class='row'>
        <div class='table-responsive'>
            <table class='table table-bordered'>
                <tr>
                    <td>Penerima</td>
                    <td>Nama</td>
                    <td>Persen</td>
                    <td>Beras</td>
                    <td>Uang</td>

                </tr>";
                if (param.Pembagian != null)
                {
                    foreach (var item in param.Pembagian)
                    {
                        pembagian_summary += $@"
                        <tr>
                            <td>{item.Jenis}</td>
                            <td>{item.Nama}</td>
                            <td>{((item.Persen * 100).ToString("n2"))} %</td>
                            <td>{item.Beras.ToString("n2")} sok</td>
                            <td>Rp. {item.Uang.ToString("n2")}</td>

                        </tr>";
                    }
                    pembagian_summary += $@"
                    <tr>
                        <td colspan='2'><b>Total</b></td>
                        <td><b>{ ((param.Pembagian.Sum(x => x.Persen) * 100).ToString("n2"))} %</b></td>
                        <td><b>{ (param.Pembagian.Sum(x => x.Beras).ToString("n2"))} sok</b></td>
                        <td><b>Rp. { (param.Pembagian.Sum(x => x.Uang).ToString("n2"))}</b></td>

                    </tr>";
                }
                pembagian_summary += $@"
            </table>
        </div>
    </div>";

                content.AppendLine(pembagian_summary);
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 80, Message = "pembagian akhir selesai..." });
                await Task.Delay(1);

                #endregion
                content_template = content_template.Replace("[CONTENT]", content.ToString());
                //string Fname = Path.GetTempFileName() + ".html";
                //File.WriteAllText(Fname, content_template);
                //document = DocumentModel.Load(Fname);
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

    }
}
