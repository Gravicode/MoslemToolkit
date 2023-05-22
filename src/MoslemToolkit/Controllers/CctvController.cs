﻿using Microsoft.AspNetCore.Mvc;
using MoslemToolkit.Data;
using MoslemToolkit.Helpers;
using MoslemToolkit.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoslemToolkit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CctvController : Controller
    {
        TempStorageService tempStorageService;
        public CctvController(TempStorageService tempStorageService)
        {
            this.tempStorageService = tempStorageService;

        }
        // /api/Sms/GetData
        [HttpPost("[action]")]
        public IActionResult SendImage(CCTVImage info)
        {
            try
            {
                var bytes = Convert.FromBase64String(info.Image64);
                tempStorageService.SetItem(info.CctvName, bytes);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        public IActionResult GetImage(string Key)
        {
            var img = tempStorageService.GetItem(Key);
            if (img == null)
            {
                var stream = new MemoryStream(ImageNotAvailable());
                return File(stream, "image/jpeg");
            }
            else
            {
                var stream = new MemoryStream(img);
                return File(stream, "image/jpeg");
            }
        }

        byte[] ImageNotAvailable()
        {
            System.Drawing.Image upImage = new Bitmap(320, 240);
            using (Graphics g = Graphics.FromImage(upImage))
            {
                var brush = new SolidBrush(Color.White);
                g.FillRectangle(brush, 0, 0, upImage.Width, upImage.Height);
                // For Transparent Watermark Text
                int opacity = 200; // range from 0 to 255

                //SolidBrush brush = new SolidBrush(Color.Red);
                brush = new SolidBrush(Color.FromArgb(opacity, Color.Red));
                Font font = new Font("Arial", 20);
                g.DrawString("NO CCTV IMAGE", font, brush, new PointF((upImage.Width/2) - 50, upImage.Height/2)); //txtWatermarkText.Text.Trim()
                var bytes = ImageHelper.ImageToByte(upImage);
                return bytes;
            }
        }
    }

}
