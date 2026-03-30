using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using system_management_information.Pages;
using static system_management_information.Pages.AddEditEventPage;

namespace system_management_information.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditTicket.xaml
    /// </summary>
    public partial class AddEditTicket : Window
    {
        public VisitCenterContext context { get; set; }
        public Ticket ticket { get; set; }
        public bool isAdd { get; set; }
        public AddEditEventPage pagePerent { get; set; }

        public AddEditTicket(int? idTicket, AddEditEventPage page)
        {
            InitializeComponent();
            context = new VisitCenterContext();
            pagePerent = page;
            if (idTicket != null)
            {
                ticket = context.Tickets.Find(idTicket);
                isAdd = false;
                titlePage.Text = "Редактирование билета";
                LoadTicket();
            }
            else
            {
                ticket = new Ticket();
                isAdd = true;
                titlePage.Text = "Добавление билета";

                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
        }
        public void LoadTicket()
        {
            if(ticket.MinimumAge != null)
                minAge.Text = ticket.MinimumAge.ToString();
            if (ticket.MaximumAge != null) 
                maxAge.Text = ticket.MaximumAge.ToString();
            if (ticket.Price == null)
                price.Text = "0,00";
            else price.Text = ticket.Price.Value.ToString("F2");
            if (ticket.CountPeople != null) 
                countPeople.Text = ticket.CountPeople.ToString();
        }

        private void DeleteTicket(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить билет?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                deleteTickets.Add(ticket);
                if (pagePerent != null)
                {
                    var ticketToRemove = pagePerent.tickets.FirstOrDefault(t => t.IdTicket == ticket.IdTicket);
                    if (ticketToRemove != null)
                    {
                        pagePerent.tickets.Remove(ticketToRemove);
                        pagePerent.RefreshTickets();
                    }
                }
                this.DialogResult = true;
                this.Close();
            }
        }

        private void SaveTicket(object sender, RoutedEventArgs e)
        {
            int min = 0;
            if(string.IsNullOrEmpty(minAge.Text))
            {
                ticket.MinimumAge = null;
            }
            else if(int.TryParse(minAge.Text, out min) && min >= 0 && min <= 100)
                ticket.MinimumAge = min;
            else
            {
                MessageBox.Show("Минимальный возраст должен быть числом в диапазоне от 0 до 100","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(string.IsNullOrEmpty(maxAge.Text))
                ticket.MaximumAge = null;
            else if(int.TryParse(maxAge.Text, out int max) && max > min && max <= 100)
                ticket.MaximumAge = max;
            else
            {
                MessageBox.Show("Максимальный возраст должен быть больше минимального возраста и меньше 100", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if(string.IsNullOrEmpty(price.Text))
                ticket.Price = null;
            else if(decimal.TryParse(price.Text, out decimal priceTicket) && priceTicket >= 0)
                ticket.Price = priceTicket;
            else
            {
                MessageBox.Show("Цена должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(countPeople.Text))
                ticket.CountPeople = null;
            else if (int.TryParse(countPeople.Text, out int count) && count > 0 && count <= 50)
                ticket.CountPeople = count;
            else
            {
                MessageBox.Show("Количество человек в билете должно быть в диапазоне от 1 до 50 включительно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(isAdd)
                addTickets.Add(ticket);
            else
                editTickets.Add(ticket);
            MessageBox.Show("Билет Сохранён!","Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            if (pagePerent != null)
            {
                if (isAdd)
                    pagePerent.tickets.Add(ticket);
                else
                {
                    var existingTicket = pagePerent.tickets.FirstOrDefault(t => t.IdTicket == ticket.IdTicket);
                    if (existingTicket != null)
                    {
                        existingTicket.Price = ticket.Price;
                        existingTicket.MinimumAge = ticket.MinimumAge;
                        existingTicket.MaximumAge = ticket.MaximumAge;
                        existingTicket.CountPeople = ticket.CountPeople;
                    }
                }
                pagePerent.RefreshTickets();
            }
            else
            {
                MessageBox.Show("Ошибка: ссылка на родительскую страницу потеряна!");
            }

            this.DialogResult = true;
            this.Close();


        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
