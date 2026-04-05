using Microsoft.EntityFrameworkCore;
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
using static system_management_information.Pages.SightsPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для CateringPage.xaml
    /// </summary>
    public partial class CateringPage : Page
    {
        public class CateringShow
        {
            public int idCatering { get; set; }

            public string establishmentCategory { get; set; } = null!;
            public string establishmentName { get; set; } = null!;
            public string addressCatering { get; set; } = null!;
            public string? establishmentPhone { get; set; }
            public string? urlCatering { get; set; }
            public string? hyperlinkCatering { get; set; }
            public string? operationModeCatering { get; set; }
            public string typesKitchen { get; set; }
        }
        public VisitCenterContext context { get; set; }
        public List<Catering> caterings { get; set; }
        public List<CateringShow> listCaterings { get; set; }
        public string foundCatering { get; set; } = null!;
        public CateringPage()
        {
            InitializeComponent();
            context = new VisitCenterContext();
            caterings = new List<Catering>();
            listCaterings = new List<CateringShow>();
            foundCatering = "";
            LoadCaterings();
            DataContext = this;
        }
        public void LoadCaterings()
        {
            caterings = context.Caterings.ToList();
            if(!string.IsNullOrEmpty(foundCatering))
            {
                caterings = caterings.Where(c => c.EstablishmentCategory.ToLower().Contains(foundCatering)
                || c.EstablishmentName.ToLower().Contains(foundCatering)
                || c.EstablishmentStreet.ToLower().Contains(foundCatering)
                || c.EstablishmentHouse.ToLower().Contains(foundCatering)
                || c.EstablishmentPhone.ToLower().Contains(foundCatering)
                || c.CateringUrl.ToLower().Contains(foundCatering)).ToList();
            }
            ShowCaterings();
        }

        public void RefreshCatering()
        {
            context.ChangeTracker.Clear();
            ListCaterings.ItemsSource = null;
            LoadCaterings();
        }
        public void ShowCaterings()
        {
            listCaterings.Clear();
            foreach(var catering in caterings)
            {
                var cateringShow = new CateringShow();
                cateringShow.idCatering = catering.IdCatering;
                cateringShow.establishmentCategory = catering.EstablishmentCategory;
                cateringShow.establishmentName = catering.EstablishmentName;
                if (!string.IsNullOrEmpty(catering.CateringUrl))
                {
                    cateringShow.urlCatering = "";
                    cateringShow.hyperlinkCatering = catering.CateringUrl;
                }
                else
                {
                    cateringShow.urlCatering = "Не указан";
                    cateringShow.hyperlinkCatering = "";
                }
                cateringShow.addressCatering = $"ул. {catering.EstablishmentStreet}, д. {catering.EstablishmentHouse}";
                cateringShow.establishmentPhone = catering.EstablishmentPhone;
                var modeOperations = context.CateringModeOperationCaterings.Include(s => s.IdModeOperationCateringNavigation)
                    .Include(s => s.IdSpecialDayCateringNavigation)
                            .Where(s => s.IdCatering == catering.IdCatering)
                            .OrderBy(s => s.DayWeek)
                            .ToList();
                
                if (modeOperations != null && modeOperations.Count != 0)
                {
                    string specialDays = "";
                     
                    if ((modeOperations.Last().DayWeek == null || modeOperations.Last().DayWeek > 7 || modeOperations.Last().DayWeek <= 0) && modeOperations.Last().IdSpecialDayCatering == null)
                        cateringShow.operationModeCatering += $" {modeOperations.Last().IdModeOperationCateringNavigation.Beginning.ToShortTimeString()} " +
                            $"- {modeOperations.Last().IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}\n";
                    else
                    {
                        //группировка дней недели с одинаковым графиком работы
                        var modes = new List<List<CateringModeOperationCatering>>();
                        List<CateringModeOperationCatering> help = new List<CateringModeOperationCatering>();
                        foreach (var modeOperation in modeOperations)
                        {
                            if (modeOperation.DayWeek != null && modeOperation.IdSpecialDayCatering == null)
                            {
                                if (help.Count == 0)
                                    help.Add(modeOperation);
                                else if (modeOperation.DayWeek == help.Last().DayWeek + 1 && modeOperation.IdModeOperationCatering == help.First().IdModeOperationCatering)
                                {
                                    help.Add(modeOperation);
                                }
                                else
                                {
                                    if (help.Count != 0)
                                        modes.Add(new List<CateringModeOperationCatering>(help));
                                    help.Clear();
                                    help.Add(modeOperation);
                                }
                            }
                        }
                        if (help.Count != 0)
                            modes.Add(new List<CateringModeOperationCatering>(help));
                        foreach (var modeOperation in modes)
                        {
                            int count = modeOperation.Count;
                            string[] week = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
                            var first = modeOperation.First();
                            if (count == 1)
                                cateringShow.operationModeCatering += $"    {week[(int)first.DayWeek - 1]}: " +
                                    $"{first.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                    $" - {first.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()} \n";
                            else if (count > 1)
                            {
                                var last = modeOperation.Last();
                                cateringShow.operationModeCatering += $"    {week[(int)first.DayWeek - 1]}-{week[(int)last.DayWeek - 1]}: " +
                                    $"{first.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                    $" - {first.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()} \n";
                            }
                        }
                    }
                    modeOperations = modeOperations
                        .Where(s => s.IdSpecialDayCatering != null
                            && s.IdSpecialDayCateringNavigation != null
                            && s.IdSpecialDayCateringNavigation.Date >= DateOnly.FromDateTime(DateTime.Now)
                            && s.IdSpecialDayCateringNavigation.Date <= s.IdSpecialDayCateringNavigation.Date.AddDays(14))
                            .ToList();
                    if (modeOperations.Count != 0)
                    {
                        foreach (var modeOperation in modeOperations)
                        {
                            specialDays += $"{modeOperation.IdSpecialDayCateringNavigation.Date}: ";
                            if (modeOperation.IdModeOperationCatering == null)
                            {
                                specialDays += $"Закрыто\n";
                            }
                            else
                            {
                                specialDays += $"{modeOperation.IdModeOperationCateringNavigation.Beginning.ToShortTimeString()}" +
                                    $" - {modeOperation.IdModeOperationCateringNavigation.EndDay.ToShortTimeString()}\n";
                            }
                        }
                        cateringShow.operationModeCatering += specialDays;
                    }
                }
                var kitchens = context.CateringTypeKitchens.Include(t => t.IdTypeKitchenNavigation).Where(t => t.IdCatering == catering.IdCatering).ToList();
                if (kitchens.Count > 0)
                {
                    foreach (var kitchen in kitchens)
                    {
                        cateringShow.typesKitchen += $"{kitchen.IdTypeKitchenNavigation.NameTypeKitchen}\n";
                    }
                }
                else cateringShow.typesKitchen = "Не указано";
                listCaterings.Add(cateringShow);

            }
            ListCaterings.ItemsSource = null;
            ListCaterings.ItemsSource = listCaterings.OrderBy(s => s.idCatering);
        }

        private void FoundCatering(object sender, TextChangedEventArgs e)
        {
            foundCatering = foundCateringText.Text.ToLower();
            listCaterings.Clear();
            LoadCaterings();
            ListCaterings.ItemsSource = null;
            ListCaterings.ItemsSource = listCaterings;
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ToAddEvent(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditCateringPage(null, RefreshCatering));
        }

        private void ToEditCatering(object sender, SelectionChangedEventArgs e)
        {
            if (ListCaterings.SelectedItem != null)
            {
                CateringShow cateringShow = ListCaterings.SelectedItem as CateringShow;
                ListCaterings.SelectedItem = null;
                NavigationService.Navigate(new AddEditCateringPage(cateringShow.idCatering, RefreshCatering));
            }
        }
        private void ToSiteCatering(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось открыть ссылку!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
