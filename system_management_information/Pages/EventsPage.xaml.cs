using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для EventsPage.xaml
    /// </summary>
    public partial class EventsPage : Page
    {
        public class EventShow
        {
            public int idEvent { get; set; }
            public string typeEvent { get; set; } = null!;
            public string nameEvent { get; set; } = null!;
            public string addressEvent { get; set; } = null!;
            public string? dateEvent { get; set; }
            public string? timeBeginningEvent { get; set; }
            public string? ageLimit { get; set; }
            public string? tickets { get; set; } = "";
            public string colorItem { get; set; }
            public string foreground { get; set; }
            public string visibilityTickets { get; set; }
        }
        public VisitCenterContext context { get; set; }
        public List<Event> events { get; set; }
        public List<EventShow> listEvents { get; set; }
        public string foundEvent { get; set; } = null!;

        public EventsPage()
        {
            InitializeComponent();
            context = new VisitCenterContext();

            events = new List<Event>();
            listEvents = new List<EventShow>();
            foundEvent = "";
            LoadEvents();
            DataContext = this;
        }
        public void RefreshEvents()
        {
            context.ChangeTracker.Clear();
            ListEvents.ItemsSource = null;
            LoadEvents();
        }
        public void LoadEvents()
        {
            events = context.Events.ToList();
            events = events.OrderBy(e => e.DateEvent).ToList();
            if(!string.IsNullOrEmpty(foundEvent))
                events = events.Where(e => e.AgeLimit != null && e.AgeLimit.ToLower().Contains(foundEvent)
                    || e.TypeEvent.ToLower().Contains(foundEvent)
                    || e.NameEvent.ToLower().Contains(foundEvent)
                    || e.StreetEvent.ToLower().Contains(foundEvent)
                    || e.HouseEvent != null && e.HouseEvent != null && e.HouseEvent.ToLower().Contains(foundEvent))
                    .ToList();
            
            ShowInfo();
        }
        public void ShowInfo()
        {
            listEvents.Clear();
            foreach(var _event in events)
            {
                var tickets = context.Tickets.Where(e => e.IdEvent == _event.IdEvent).ToList();
                EventShow eventInfo = new EventShow();
                eventInfo.idEvent = _event.IdEvent;
                eventInfo.typeEvent = _event.TypeEvent;
                eventInfo.nameEvent = _event.NameEvent;
                if (_event.DateEvent != null)
                    eventInfo.dateEvent = _event.DateEvent.ToString();
                else eventInfo.dateEvent = "  Не указана";
                eventInfo.addressEvent = $"ул. {_event.StreetEvent}";
                if(!string.IsNullOrEmpty(_event.HouseEvent))
                    eventInfo.addressEvent += $", д. {_event.HouseEvent}";
                if (_event.TimeBeginningEvent != null)
                    eventInfo.timeBeginningEvent = _event.TimeBeginningEvent.Value.ToShortTimeString();
                else eventInfo.timeBeginningEvent = "  Не указано";
                    eventInfo.ageLimit = _event.AgeLimit;
                if(_event.DateEvent <= DateOnly.FromDateTime(DateTime.Now) && _event.DateEvent != null)
                {
                    eventInfo.foreground = "White";
                    eventInfo.colorItem = "#95a5a6";
                    eventInfo.visibilityTickets = "Hidden";
                }
                else
                {
                    eventInfo.foreground = "Black";
                    eventInfo.colorItem = "White";
                    eventInfo.visibilityTickets = "Visible";
                    if (tickets.Count > 0)
                    {
                        foreach (var ticket in tickets)
                        {
                            if ((ticket.MinimumAge != null && ticket.MinimumAge >= 0) || (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100))
                            {
                                if (ticket.MinimumAge != null && ticket.MinimumAge >= 0)
                                    eventInfo.tickets += $"От {ticket.MinimumAge}  ";
                                if (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100)
                                    eventInfo.tickets += $"До {ticket.MaximumAge}  ";
                                eventInfo.tickets += $"лет:  ";
                            }
                            if (ticket.Price != null && ticket.Price > 0)
                                eventInfo.tickets += $"{ticket.Price.Value.ToString("F2")} руб. ";
                            else eventInfo.tickets += $"Бесплатно ";
                            if (ticket.CountPeople > 1)
                                eventInfo.tickets += $"  (на {ticket.CountPeople} чел.)";
                            eventInfo.tickets += "\n";
                        }
                    }
                    else eventInfo.visibilityTickets = "Hidden";
                }
                listEvents.Add(eventInfo);
            }
            ListEvents.ItemsSource = listEvents
                    .OrderByDescending(e =>
                    {
                        if (e.dateEvent == "  Не указана")
                            return DateTime.MaxValue;
                        if (DateTime.TryParse(e.dateEvent, out DateTime date))
                            return date;
                        return DateTime.MaxValue;
                    })
                    .ToList();
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void FoundEvent(object sender, TextChangedEventArgs e)
        {
            foundEvent = foundEventText.Text.ToLower();
            listEvents.Clear();
            LoadEvents();
            ListEvents.ItemsSource = null;
            ListEvents.ItemsSource = listEvents;
        }

        private void ToAddEvent(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditEventPage(null, RefreshEvents));
        }
        private void ToEditEvent(object sender, SelectionChangedEventArgs e)
        {
            if (ListEvents.SelectedItem != null)
            {
                EventShow eventShow = ListEvents.SelectedItem as EventShow;
                ListEvents.SelectedItem = null;
                NavigationService.Navigate(new AddEditEventPage(eventShow.idEvent, RefreshEvents));
            }
        }
    }
}
