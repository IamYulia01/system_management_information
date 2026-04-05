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
using static system_management_information.Pages.EventsPage;
using system_management_information.Windows;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditEventPage.xaml
    /// </summary>
    public partial class AddEditEventPage : Page
    {
        public class TicketShow
        {
            public int idTicket { get; set; }
            public string? ages { get; set; }
            public string price { get; set; }
            public string countPeople { get; set; }
        }
        public VisitCenterContext context { get; set; }
        public Event eventEdit { get; set; }
        public List<Ticket> tickets { get; set; }
        public static List<Ticket> deleteTickets { get; set; } = new List<Ticket>();
        public static List<Ticket> addTickets { get; set; } = new List<Ticket>();
        public static List<Ticket> editTickets { get; set; } = new List<Ticket>();
        public List<TicketShow> listTickets { get; set; }
        public bool isAdd { get; set; }
        private Action _onEventSaved;

        public AddEditEventPage(int? idEvent, Action onEventSaved = null)
        {
            InitializeComponent();
            _onEventSaved = onEventSaved;
            context = new VisitCenterContext();
            this.InvalidateVisual();
            isAdd = true;

            var typesEvent = context.Events.Select(s => s.TypeEvent).ToList();
            foreach (var type in typesEvent)
            {
                if (!typeEvent.Items.Contains(type))
                    typeEvent.Items.Add(type);
            }
            typeEvent.Items.Add("Другое");
            otherView.Visibility = Visibility.Collapsed;

            listTickets = new List<TicketShow>();
            if (idEvent != null )
            {
                isAdd = false;
                titlePage.Text = "Редактирование мероприятия";
                eventEdit = context.Events.Find(idEvent);
                tickets = context.Tickets.Where(t => t.IdEvent == idEvent).ToList();
                
                LoadEvent();
            }
            else
            {
                isAdd = true;
                titlePage.Text = "Добавление мероприятия";
                eventEdit = new Event();
                tickets = new List<Ticket>();
                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
        }
        public void LoadEvent()
        {
            if (addTickets.Count > 0)
            {
                foreach (var ticket in addTickets)
                {
                    tickets.Add(ticket);
                }
            }
            if (deleteTickets.Count > 0)
            {
                foreach (var ticket in deleteTickets)
                    tickets.Remove(ticket);
            }
            if (editTickets.Count > 0)
            {
                foreach (var ticket in editTickets)
                {
                    var ticketEdit = context.Tickets.Find(ticket.IdTicket);
                    ticketEdit.Price = ticket.Price;
                    ticketEdit.MinimumAge = ticket.MinimumAge;
                    ticketEdit.MaximumAge = ticket.MaximumAge;
                    ticketEdit.CountPeople = ticket.CountPeople;
                }
            }
            typeEvent.SelectedItem = eventEdit.TypeEvent;
            nameEvent.Text = eventEdit.NameEvent;
            streetEvent.Text = eventEdit.StreetEvent;
            houseEvent.Text = eventEdit.HouseEvent;
            ageLimit.Text = eventEdit.AgeLimit;
            dateEvent.Text = eventEdit.DateEvent.ToString();
            if(eventEdit.TimeBeginningEvent != null)
                timeEvent.Text = eventEdit.TimeBeginningEvent.Value.ToShortTimeString();
            foreach(var ticket in tickets)
            {
                TicketShow ticketShow = new TicketShow();
                ticketShow.idTicket = ticket.IdTicket;
                if ((ticket.MinimumAge != null && ticket.MinimumAge >= 0) || (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100))
                {
                    if (ticket.MinimumAge != null && ticket.MinimumAge >= 0)
                        ticketShow.ages += $"От {ticket.MinimumAge}  ";
                    if (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100)
                        ticketShow.ages += $"До {ticket.MaximumAge}  ";
                    ticketShow.ages += $"лет:";
                }
                else ticketShow.ages = "Для всех";
                if (ticket.Price != null && ticket.Price > 0)
                    ticketShow.price += $"Цена:  {ticket.Price.Value.ToString("F2")} руб.";
                else ticketShow.price += $"Бесплатно";
                if (ticket.CountPeople > 1)
                    ticketShow.countPeople += $"билет на {ticket.CountPeople} чел.";
                else ticketShow.countPeople += $"билет на 1 чел.";
                listTickets.Add(ticketShow);
            }
        }
        public void RefreshTickets()
        {
            listTickets.Clear();

            foreach (var ticket in tickets)
            {
                TicketShow ticketShow = new TicketShow();
                ticketShow.idTicket = ticket.IdTicket;
                if ((ticket.MinimumAge != null && ticket.MinimumAge >= 0) || (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100))
                {
                    if (ticket.MinimumAge != null && ticket.MinimumAge >= 0)
                        ticketShow.ages += $"От {ticket.MinimumAge}  ";
                    if (ticket.MaximumAge != null && ticket.MaximumAge != 0 && ticket.MaximumAge <= 100)
                        ticketShow.ages += $"До {ticket.MaximumAge}  ";
                    ticketShow.ages += $"лет:";
                }
                else ticketShow.ages = "Для всех";
                if (ticket.Price != null && ticket.Price > 0)
                    ticketShow.price += $"Цена:  {ticket.Price.Value.ToString("F2")} руб.";
                else ticketShow.price += $"Бесплатно";
                if (ticket.CountPeople > 1)
                    ticketShow.countPeople += $"билет на {ticket.CountPeople} чел.";
                else ticketShow.countPeople += $"билет на 1 чел.";
                listTickets.Add(ticketShow);
            }

            ListTickets.ItemsSource = null;
            ListTickets.ItemsSource = listTickets;
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить мероприятие из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var tickets = context.Tickets.Where(e => e.IdEvent == eventEdit.IdEvent).ToList();
                foreach (var ticket in tickets)
                {
                    context.Tickets.Remove(ticket);
                    context.SaveChanges();
                }
                context.Events.Remove(eventEdit);
                context.SaveChanges();
                _onEventSaved?.Invoke();
                NavigationService.GoBack();
            }
        }

        private void AddTicket(object sender, RoutedEventArgs e)
        {
            AddEditTicket addEditTicket = new AddEditTicket(null, this);
            var parentWindow = Window.GetWindow(this);
            addEditTicket.ShowDialog();
        }

        private void EditTicket(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                TicketShow ticketShow = e.AddedItems[0] as TicketShow;
                var ticket = context.Tickets.Find(ticketShow.idTicket);
                if (ticket != null)
                {
                    AddEditTicket addEditTicket = new AddEditTicket(ticket.IdTicket, this);
                    addEditTicket.Owner = Window.GetWindow(this);
                    addEditTicket.ShowDialog();

                    ListTickets.SelectedItem = null;
                }
                else
                    MessageBox.Show("Вы не можете редактировать только что добавленный билет! Сначала сохраните данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void SaveEvent(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(typeEvent.SelectedItem.ToString()))
            {
                if (typeEvent.SelectedItem.ToString() == "Другое")
                {
                    if (!string.IsNullOrEmpty(otherTypeCatering.Text))
                    {
                        eventEdit.TypeEvent = otherTypeCatering.Text;
                    }
                    else
                    {
                        MessageBox.Show("Введите новый тип мероприятия или выберите из списка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                    eventEdit.TypeEvent = typeEvent.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Выберите тип мероприятия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(nameEvent.Text))
                eventEdit.NameEvent = nameEvent.Text;
            else
            {
                MessageBox.Show("Введите название мероприятия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(streetEvent.Text))
                eventEdit.StreetEvent = streetEvent.Text;
            else
            {
                MessageBox.Show("Введите улицу проведения мероприятия!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            eventEdit.HouseEvent = houseEvent.Text;

            if (string.IsNullOrEmpty(ageLimit.Text))
                eventEdit.AgeLimit = null;
            else if(ageLimit.Text.Any(char.IsDigit))
                eventEdit.AgeLimit = ageLimit.Text;
            else
            {
                MessageBox.Show("Проверьте возрастное ограничение! Оно должно содержать возраст!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(dateEvent.Text))
                eventEdit.DateEvent = null;
            else if(DateOnly.TryParse(dateEvent.Text, out DateOnly date) && date.Year >= 2025)
                eventEdit.DateEvent = date;
            else
            {
                MessageBox.Show("Проверьте дату! Дата введена неправильно (год проведения мероприятия должен быть не меньше 2025!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(timeEvent.Text) 
                || timeEvent.Text.Trim() == "00:00"
                || timeEvent.Text.Trim() == "__:__"
                || timeEvent.Text.Trim() == ":")
                eventEdit.TimeBeginningEvent = null;
            else if (TimeOnly.TryParse(timeEvent.Text, out TimeOnly time))
                eventEdit.TimeBeginningEvent = time;
            else
            {
                MessageBox.Show("Проверьте время проведения мероприятия! Оно введено неправильно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(isAdd)
                context.Events.Add(eventEdit);
            context.SaveChanges();

            foreach(var addTicket in addTickets)
            {
                addTicket.IdEvent = eventEdit.IdEvent;
                context.Tickets.Add(addTicket);
            }
            foreach(var deleteTicket in deleteTickets)
            {
                var localTicket = context.Tickets.Local.FirstOrDefault(t => t.IdTicket == deleteTicket.IdTicket);
                if (localTicket != null)
                    context.Tickets.Remove(localTicket);
                else
                {
                    context.Tickets.Attach(deleteTicket);
                    context.Tickets.Remove(deleteTicket);
                }
            }
            foreach(var editTicket in editTickets)
            {
                var ticket = context.Tickets.Find(editTicket.IdTicket);
            }
            context.SaveChanges();
            addTickets.Clear();
            deleteTickets.Clear();
            editTickets.Clear();
            _onEventSaved?.Invoke();

            NavigationService.GoBack();
        }

        private void SelectionTypeEvent(object sender, SelectionChangedEventArgs e)
        {
            if (typeEvent.SelectedItem.ToString() == "Другое")
            {
                otherView.Visibility = Visibility.Visible;
            }
            else otherView.Visibility = Visibility.Collapsed;
        }
    }
}
