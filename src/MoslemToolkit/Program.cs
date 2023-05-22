using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using MoslemToolkit.Data;
using MoslemToolkit.Tools;
using MoslemToolkit.Data;
using ServiceStack.Text;
using System.Net;
using System.Text;
using Blazored.Toast;

namespace MoslemToolkit
{
    public class Program
    {
        static IConfiguration Configuration { get; set; }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Configuration = builder.Configuration;
            // Add services to the container.
            var services = builder.Services;

          

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS"));
            });
           
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<TempStorageService>();
            services.AddSingleton<NgajiService>();
            services.AddSingleton<AbsensiService>();
            services.AddSingleton<FaceService>();
            services.AddTransient<AzureBlobHelper>();
            services.AddTransient<CashflowQurbanService>();
            services.AddTransient<LaporanBulananService>();
            services.AddTransient<SiswaPerKelasService>();
            services.AddTransient<TargetPerKelasService>();
            services.AddTransient<MateriPerKelasService>();



            AppConstants.SQLConn = Configuration["ConnectionStrings:SqlConn"];
            AppConstants.RedisCon = Configuration["RedisCon"];
            AppConstants.JamMasjidUrl = Configuration["JamMasjidUrl"];
            AppConstants.JamMasjidImagePrefix = Configuration["JamMasjidImagePrefix"];
            AppConstants.UploadUrlPrefix = Configuration["UploadUrlPrefix"];
            AppConstants.BlobConn = Configuration["BlobConn"];
            AppConstants.KehadiranReportUrl = (Configuration["App:KehadiranReportUrl"]);
            AppConstants.Evaluasi4sReportUrl = (Configuration["App:Evaluasi4sReportUrl"]);
            AppConstants.KUReportUrl = (Configuration["App:KUReportUrl"]);
            AppConstants.KUInfaqBulananUrl = (Configuration["App:KUInfaqBulananUrl"]);
            AppConstants.ReportQurbanUrl = (Configuration["App:ReportQurbanUrl"]);
            AppConstants.LaporanPencapaianPJKBM = (Configuration["App:LaporanPencapaianPJKBM"]);

            AppConstants.ReportPJKBM = (Configuration["App:ReportPJKBM"]);
            AppConstants.ReportKPI = (Configuration["App:ReportKPI"]);
            AppConstants.LaporanZakatFitrah = (Configuration["App:LaporanZakatFitrah"]);
            AppConstants.HargaJualBeras = int.Parse(Configuration["App:HargaJualBeras"]);
            services.AddBlazoredLocalStorage();
            services.AddBlazoredToast();

            MailService.MailUser = Configuration["MailSettings:MailUser"];
            MailService.MailPassword = Configuration["MailSettings:MailPassword"];
            MailService.MailServer = Configuration["MailSettings:MailServer"];
            MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
            MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
            MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
            MailService.UseSendGrid = true;


            SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
            SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];
            SmsService.TokenKey = Configuration["SmsSettings:TokenKey"];

            //report
            AppConstants.Report_Anekdot = Configuration["Reports:Anekdot"];
            AppConstants.Report_Cover = Configuration["Reports:Cover"];
            AppConstants.Report_Evaluasi = Configuration["Reports:Evaluasi"];
            AppConstants.Report_Jurnal = Configuration["Reports:Jurnal"];
            AppConstants.Report_Kalender = Configuration["Reports:Kalender"];
            AppConstants.Report_Presensi = Configuration["Reports:Presensi"];
            AppConstants.Report_Prosem = Configuration["Reports:Prosem"];
            AppConstants.Report_Raport = Configuration["Reports:Raport"];

            AppConstants.NAMA_DAERAH = Configuration["Info:NAMA_DAERAH"];
            AppConstants.NAMA_DESA = Configuration["Info:NAMA_DESA"];
            AppConstants.NAMA_KELOMPOK = Configuration["Info:NAMA_KELOMPOK"];
            AppConstants.SCORE_DATA = Configuration["Info:SCORE_DATA"];

            //zakat fitrah
            AppConstants.Zakat_Fitrah_Cover = Configuration["Zakat:Cover"];
            AppConstants.Zakat_Fitrah_Content = Configuration["Zakat:Content"];

            //qurban
            AppConstants.Qurban_Cover = Configuration["Qurban:Cover"];
            AppConstants.Qurban_Content = Configuration["Qurban:Content"];

            /*
            //ML
            services.AddPredictionEnginePool<ImageInputData, TinyYoloPrediction>().
                FromFile(_mlnetModelFilePath);

            //services.AddTransient<IImageFileWriter, ImageFileWriter>();
            services.AddTransient<IObjectDetectionService, ObjectDetectionService>();
            */

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("103.189.234.4"));
            });

            services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 128 * 1024; // 1MB
            });
            /*
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Ngaji Data API",
                    Version = "v1",
                    Description = "Rest API for accessing data",

                    Contact = new Microsoft.OpenApi.Models.OpenApiContact { Name = "Ngaji Online Team", Email = "cimanggu.online@gmail.com", Url = new Uri("https://cimanggu.online/") },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense { Name = "For developers only", Url = new Uri("https://cimanggu.online/license") }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });
            //end swagger
            */
            NgajiDB db = new NgajiDB();
            db.Database.EnsureCreated();
            /*
            if (!db.SiswaPerkelass.Any())
            {
                var siswas = db.Jamaahs.Where(x => x.KelasId > 0).ToList();
                foreach(var item in siswas)
                {
                    var newItem = new SiswaPerKelas() { JamaahId = item.Id, KelasId = item.KelasId, Tahun = DateTime.Now.Year };
                    db.SiswaPerkelass.Add(newItem);
                }
                db.SaveChanges();
            }*/
            if (!db.Asnabs.Any())
            {
                foreach (var item in AppConstants.AsnabDefault)
                {
                    db.Asnabs.Add(item.Value);
                }
            }
            if (db.ProsentaseZakats.Count() <= 0)
            {
                foreach (var item in AppConstants.PembagianDefault)
                {
                    db.ProsentaseZakats.Add(item);
                }
            }
            db.SaveChanges();

            var app = builder.Build();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                //app.UseHttpsRedirection();

            }
            StaticWebAssetsLoader.UseStaticWebAssets(
                      app.Environment,
                      Configuration);

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(x => x
           .AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(origin => true) // allow any origin  
           .AllowCredentials());               // allow credentials 
            /*
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ngaji Online Data API V1");
            });
            //end swagger
            */
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapGet("/docs/{filename}", async c => {
                    var blob = app.Services.GetService<AzureBlobHelper>();
                    var fileName = c.Request.RouteValues["filename"].ToString();
                   
                    var fileinfo = new FileInfo(blob.GetFPath(fileName));
                    c.Response.Clear();
                    c.Response.Headers.Add("Content-Disposition", "attachment;filename=" + fileinfo.Name);
                    c.Response.Headers.Add("Content-Length", fileinfo.Length.ToString());
                    c.Response.Headers.Add("Content-Transfer-Encoding", "binary");
                    new FileExtensionContentTypeProvider().Mappings.TryGetValue(fileinfo.Extension, out var contenttype);
                    c.Response.ContentType = contenttype ?? "application/octet-stream";
                    await c.Response.SendFileAsync(fileinfo.FullName);

                });
                // BLAZOR COOKIE Auth Code (begin)
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");

            });
            app.Run();
            /*
          var builder = WebApplication.CreateBuilder(args);
          Configuration = builder.Configuration;
          // Add services to the container.
          builder.Services.AddRazorPages();
          builder.Services.AddServerSideBlazor();
          builder.Services.AddSingleton<WeatherForecastService>();

          var app = builder.Build();

          // Configure the HTTP request pipeline.
          if (!app.Environment.IsDevelopment())
          {
              app.UseExceptionHandler("/Error");
              // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
              app.UseHsts();
          }

          app.UseHttpsRedirection();

          app.UseStaticFiles();

          app.UseRouting();

          app.MapBlazorHub();
          app.MapFallbackToPage("/_Host");

          app.Run();
          */
        }

        
    }
}