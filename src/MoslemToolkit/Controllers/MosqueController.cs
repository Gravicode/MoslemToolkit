using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MoslemToolkit.Data;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoslemToolkit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MosqueController : Controller
    {
        InfoMasjidService svc;

        public MosqueController()
        {
            svc = new InfoMasjidService();
        }
        // /api/Sms/GetData
        [HttpGet("[action]")]
        public InfoDetailMasjid GetData(string Kode)
        {
            InfoDetailMasjid item = null;
            var data = svc.GetData(Kode);

            //get kegiatan
            var tanggal = DateHelper.GetLocalTimeNow();
            KalenderKelompokService KegiatanSvc = new KalenderKelompokService();
            var kegiatan = KegiatanSvc.GetAllData(tanggal,tanggal.AddMonths(1));
            if (data != null)
            {
                item = JsonConvert.DeserializeObject<InfoDetailMasjid>(data.DataMasjid);
                item.Kegiatan.Clear();
                if(!string.IsNullOrEmpty(AppConstants.JamMasjidImagePrefix) && item.ImageUrl!=null)
                for(var i=0;i<item.ImageUrl.Count;i++)
                {
                        item.ImageUrl[i].Url = AppConstants.JamMasjidImagePrefix + item.ImageUrl[i].Url;
                }
                foreach(var keg in kegiatan)
                {
                    item.Kegiatan.Add(new KegiatanMasjid() { Nama = keg.Kegiatan, Tanggal = keg.TanggalMulai });
                }
            }
            return item;
        }

        // /api/Sms/GetAlarmData
        [HttpGet("[action]")]
        public List<MosqueAlarmPrimitive> GetAlarmData()
        {
            var svc = new MosqueAlarmService();
            var data = svc.GetAllData2();

            return data;
        }
    }

}
