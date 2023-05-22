using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MoslemToolkit;
using MoslemToolkit.Shared;
using MoslemToolkit.Shared.Usercontrols;
using MoslemToolkit.Data;
using BlazorInputFile;
using Blazored.Toast;
using Blazored.Toast.Services;
using Blazored.Typeahead;
using ChartJs.Blazor;

namespace MoslemToolkit.Shared.Usercontrols
{
    public class DateChangedEventArgs : EventArgs
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
    public partial class EventCalendar
    {
        public event DateChangedEventHandler DateChanged;
        public delegate void DateChangedEventHandler(object sender, DateChangedEventArgs e);


        [Parameter]
        public DateTime CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                StartDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                EndDate = StartDate.AddMonths(1).AddDays(-1);
            }
        }
        DateTime currentDate;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        Dictionary<int, DayEvent> Days;
        public EventCalendar()
        {
            CurrentDate = DateHelper.GetLocalTimeNow();
            if (Days == null) Days = new Dictionary<int, DayEvent>();
        }
        public bool HasEvent(DateTime date)
        {
            if (Days.ContainsKey(date.Day))
            {
                return true;
            }
            return false;
        }

        public void Refresh()
        {
            StateHasChanged();
        }
        public void AddEvent(DateTime date, string Title)
        {
            if(date.Month == currentDate.Month)
            {
                Days.Add(date.Day, new DayEvent() { EventDate = date, Title = Title });
            }
        }
        public void ClearEvent()
        {
            Days.Clear();
        }
        void Next()
        {
            CurrentDate = CurrentDate.AddMonths(1);
            DateChanged?.Invoke(this, new DateChangedEventArgs() { Start = StartDate, End = EndDate });
        }
        void Prev()
        {
            CurrentDate = CurrentDate.AddMonths(-1);
            DateChanged?.Invoke(this, new DateChangedEventArgs() { Start = StartDate, End = EndDate });
        }

    }

    public class DayEvent
    {
        public DateTime EventDate { get; set; }
        public string Title { get; set; }
    }
}