using System.Drawing;
using CCTVMasjid.Helpers;
using CCTVMasjid;
using CCTVMasjid.DataModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace YoloV4MlNet
{
    class Program
    {
        static int DelayTime = 3000;
        static string ApiUrl = "http://localhost:/api/cctv/sendimage";
        static List<(string name, string url)>? CCTVUrls;

        //""
        static string Username = "admin";
        static string Password = "123qweasd!";//"cctv1234";

        private static readonly string _modelPath = $"{FileHeper.GetAppPath()}\\Model\\yolov4.onnx";
        private static readonly string _imageOutputFolder = $@"{FileHeper.GetAppPath()}\Output";
        private static readonly string[] _classesNames = new string[] {
            "person", "bicycle", "car", "motorbike", "aeroplane", "bus", "train", "truck", "boat", "traffic light", "fire hydrant", "stop sign", "parking meter",
            "bench", "bird", "cat", "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase",
            "frisbee", "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket", "bottle", "wine glass",
            "cup", "fork", "knife", "spoon", "bowl", "banana", "apple", "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "sofa",
            "pottedplant", "bed", "diningtable", "toilet", "tvmonitor", "laptop", "mouse", "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink",
            "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush" };

        static void Main()
        {
            ReadConfig();
            RunService();

            Console.WriteLine($"Check images in the output folder {_imageOutputFolder}...");
            Console.ReadLine();
        }

        static async Task RunService()
        {
            var trainer = new Trainer();

            Console.WriteLine("Build and train YOLO V4 model...");
            var trainedModel = trainer.BuildAndTrain(_modelPath);

            Console.WriteLine("Create predictor...");
            var predictor = new Predictor(trainedModel);

            Console.WriteLine("Run predictions on images...");
            if (!Directory.Exists(_imageOutputFolder)) Directory.CreateDirectory(_imageOutputFolder);
            var credentials = new NetworkCredential(Username, Password);
            var handler = new HttpClientHandler { Credentials = credentials };

            HttpClient client = new HttpClient(handler);
            while (true)
            {
                foreach (var item in CCTVUrls)
                {
                    byte[] res=null;
                    try
                    {
                        res = await client.GetByteArrayAsync(item.url);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    if (res != null)
                    {
                        using (var image = new Bitmap(Image.FromStream(new MemoryStream(res))))
                        {
                            var predict = predictor.Predict(image);
                            var results = predict.GetResults(_classesNames);
                            var filter = new string[] { "person" };
                            var bytes = DrawResults.DrawAndGet(_imageOutputFolder, $"{GetRandomName()}.jpg", results, image, filter);
                             DrawResults.DrawAndStore(_imageOutputFolder, $"{GetRandomName()}.jpg", results, image, filter);
                            //push to cloud
                            var info = new CCTVImage() { CctvName = item.name, Image64 = Convert.ToBase64String(bytes) };
                            var json = JsonConvert.SerializeObject(info);
                            var hasil = await client.PostAsync(ApiUrl, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                            if (hasil.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"{DateTime.Now} => push image dari {item.name} sukses");
                            }
                            else
                            {
                                Console.WriteLine($"{DateTime.Now} => push image dari {item.name} gagal, {await hasil.Content.ReadAsStringAsync()}");
                            }
                        }
                    }
                }
                Thread.Sleep(DelayTime);
            }

        }


        static string GetRandomName()
        {
            string name = "detection-" + Guid.NewGuid().ToString();
            return name;
        }
        static void ReadConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("config.json", optional: false);

                IConfiguration Configuration = builder.Build();


                DelayTime = int.Parse(Configuration["App:Delay"]);
                ApiUrl = (Configuration["App:ApiUrl"]);
                var items = Configuration["App:CCTVUrls"].Split(';');
                CCTVUrls = new List<(string name, string url)>();
                int count = 0;
                foreach (var item in items)
                {
                    count++;
                    CCTVUrls.Add(($"cctv-{count}", item));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("read config failed:" + ex);
            }
            Console.WriteLine("Read config successfully.");
        }
    }
}
