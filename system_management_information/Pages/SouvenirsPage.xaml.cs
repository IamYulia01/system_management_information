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
using static system_management_information.Pages.SightsPage;
using static system_management_information.Pages.SouvenirsPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для SouvenirsPage.xaml
    /// </summary>
    public partial class SouvenirsPage : Page
    {
        public class SouvenirShow
        {
            public int idSouvenir { get; set; }
            public string product { get; set; } = null!;
            public string nameSouvenir { get; set; } = null!;
            public string? tastes { get; set; }
            public string? weight { get; set; }
        }
        public VisitCenterContext context {  get; set; }
        public string foundSouvenir { get; set; }
        public List<Souvenir> souvenirs { get; set; }
        public List<SouvenirShow> listSouvenirs { get; set; }

        public SouvenirsPage()
        {
            InitializeComponent();
            context = new VisitCenterContext();
            foundSouvenir = "";
            souvenirs = new List<Souvenir>();
            listSouvenirs = new List<SouvenirShow>();
            LoadSouvenirs();
            DataContext = this;
        }
        public void RefreshSouvenirs()
        {
            context.ChangeTracker.Clear();
            ListSouvenirs.ItemsSource = null;
            LoadSouvenirs();
        }
        private void LoadSouvenirs()
        {
            souvenirs = context.Souvenirs.ToList();
            if (!string.IsNullOrEmpty(foundSouvenir))
            {
                souvenirs = souvenirs.Where(s => s.NameSouvenir.ToLower().Contains(foundSouvenir)
                || s.Product.ToLower().Contains(foundSouvenir)
                || s.Tastes != null && s.Tastes.ToLower().Contains(foundSouvenir)
                || s.Weight != null && s.Weight.ToLower().Contains(foundSouvenir))
                    .ToList();
            }
            ShowSouvenirs();
        }

        public void ShowSouvenirs()
        {
            listSouvenirs.Clear();
            foreach(var souvenir in souvenirs)
            {
                var souvenirShow = new SouvenirShow();
                souvenirShow.idSouvenir = souvenir.IdSouvenir;
                souvenirShow.nameSouvenir = souvenir.NameSouvenir;
                souvenirShow.product = souvenir.Product;
                if (!string.IsNullOrEmpty(souvenir.Weight))
                    souvenirShow.weight = souvenir.Weight;
                else souvenirShow.weight = "Не указан";
                if (!string.IsNullOrEmpty(souvenir.Tastes))
                    souvenirShow.tastes = souvenir.Tastes;
                else souvenirShow.tastes = "Не указан";
                listSouvenirs.Add(souvenirShow);
            }
            ListSouvenirs.ItemsSource = null;
            ListSouvenirs.ItemsSource = listSouvenirs;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ToAddSouvenir(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditSouvenirPage(null, RefreshSouvenirs));
        }

        private void ToEditSouvenir(object sender, SelectionChangedEventArgs e)
        {
            if (ListSouvenirs.SelectedItem != null)
            {
                SouvenirShow souvenirShow = ListSouvenirs.SelectedItem as SouvenirShow;
                ListSouvenirs.SelectedItem = null;
                NavigationService.Navigate(new AddEditSouvenirPage(souvenirShow.idSouvenir, RefreshSouvenirs));
            }
        }

        private void FoundSouvenir(object sender, TextChangedEventArgs e)
        {
            foundSouvenir = foundSouvenirText.Text.ToLower();
            listSouvenirs.Clear();
            LoadSouvenirs();
            ListSouvenirs.ItemsSource = null;
            ListSouvenirs.ItemsSource = listSouvenirs;
        }
    }
}
