using Microsoft.AspNetCore.Mvc;
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
    public class SmsController : Controller
    {
        // /api/Sms/GetData
        [HttpGet("[action]")]
        public List<SmsBroadcastPrimitive> GetData()
        {
            var svc = new SmsBroadcastService();
            var data = svc.GetAllData2();

            return data;
        }
    }

}
