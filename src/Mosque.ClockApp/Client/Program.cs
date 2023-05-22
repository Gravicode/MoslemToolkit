using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Mosque.ClockApp.Client;
using Blazored.LocalStorage;
using Blazored.Toast;
using Mosque.ClockApp.Client.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOptions();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

//builder.Services.AddMvcCore()
//    .AddJsonOptions(option =>
//    {
//        option.UseCamelCasing(false);
//        option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
//    });

// read configuration
AppConstant.ApiUrl = builder.Configuration["AppSettings:ApiUrl"];
AppConstant.AlarmApiUrl = builder.Configuration["AppSettings:AlarmApiUrl"];
AppConstant.ApiKey = builder.Configuration["AppSettings:ApiKey"];


await builder.Build().RunAsync();
