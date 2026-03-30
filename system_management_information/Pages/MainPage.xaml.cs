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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void GoToSights(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SightsPage());
        }

        private void GoToCatering(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CateringPage());
        }

        private void GoToEvents(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EventsPage());
        }

        private void GoToHotels(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HotelsPage());
        }

        private void GoToSouvenirs(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SouvenirsPage());
        }
    }
}
