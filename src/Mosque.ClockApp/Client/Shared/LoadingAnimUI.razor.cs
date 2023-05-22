using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Mosque.ClockApp.Client;
using Mosque.ClockApp.Client.Shared;
using Blazored.LocalStorage;
using Blazored.Toast;
using Blazored.Toast.Services;

namespace Mosque.ClockApp.Client.Shared
{
    public interface ILoadingAnim
    {
        void Show();
        void Hide();
    }
    public class LoadingAnim : ComponentBase, ILoadingAnim
    {
        private IJSRuntime _jSRuntime;
        public LoadingAnim(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async void Hide()
        {
            await _jSRuntime.InvokeAsync<object>("loadingComponent", false);
        }

        public async void Show()
        {
            await _jSRuntime.InvokeAsync<object>("loadingComponent", true);
        }
    }
    public partial class LoadingAnimUI
    {
       
    }
}