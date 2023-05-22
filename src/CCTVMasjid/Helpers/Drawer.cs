using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CCTVMasjid.DataModel;

namespace CCTVMasjid.Helpers
{
    public static class DrawResults
    {
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        public static void DrawAndStore(string imageOutputFolder, string imageName,
                    IReadOnlyList<Result> results, Bitmap image,string[] filters)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                foreach (var result in results)
                {
                    if(filters != null)
                    {
                        if (!filters.Any(x=>x == result.Label)) continue;
                    }
                    var x1 = result.BoundingBox[0];
                    var y1 = result.BoundingBox[1];
                    var x2 = result.BoundingBox[2];
                    var y2 = result.BoundingBox[3];

                    graphics.DrawRectangle(Pens.DarkGreen, x1, y1, x2 - x1, y2 - y1);

                    using (var brushes = new SolidBrush(Color.FromArgb(40, Color.LightGreen)))
                    {
                        graphics.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                    }

                    graphics.DrawString(result.Label + " " + result.Confidence.ToString("0.00"),
                                 new Font("Arial", 12), Brushes.Blue, new PointF(x1, y1));
                }

                image.Save(Path.Combine(imageOutputFolder, Path.ChangeExtension(imageName, "_yoloed"
                                        + Path.GetExtension(imageName))));
            }
        }

        public static byte[] DrawAndGet(string imageOutputFolder, string imageName,
                   IReadOnlyList<Result> results, Bitmap image, string[] filters)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                foreach (var result in results)
                {
                    if (filters != null)
                    {
                        if (!filters.Any(x => x == result.Label)) continue;
                    }
                    var x1 = result.BoundingBox[0];
                    var y1 = result.BoundingBox[1];
                    var x2 = result.BoundingBox[2];
                    var y2 = result.BoundingBox[3];

                    graphics.DrawRectangle(Pens.DarkGreen, x1, y1, x2 - x1, y2 - y1);

                    using (var brushes = new SolidBrush(Color.FromArgb(40, Color.LightGreen)))
                    {
                        graphics.FillRectangle(brushes, x1, y1, x2 - x1, y2 - y1);
                    }

                    graphics.DrawString(result.Label + " " + result.Confidence.ToString("0.00"),
                                 new Font("Arial", 12), Brushes.Blue, new PointF(x1, y1));
                }

                return ImageToByte(image);
            }
        }
    }
}
