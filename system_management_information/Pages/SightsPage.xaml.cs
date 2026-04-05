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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using system_management_information.Services;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для SightsPage.xaml
    /// </summary>
    public partial class SightsPage : Page
    {
        public class SightShow
        {
            public int idSight { get; set; }
            public string typeSight { get; set; } = null!;
            public string nameSight { get; set; } = null!;
            public string addressSight { get; set; } = null!;
            public string? description { get; set; }
            public string? contactNumber { get; set; }
            public string? email { get; set; }
            public string? urlSight { get; set; }
            public ImageSource photoSight { get; set; }
            public string? hyperlinkSight { get; set; }
            public string? operationMode { get; set; }
        }
        public VisitCenterContext context { get; set; }
        public List<Sight> sights { get; set; }
        public List<SightShow> listSights { get; set; }
        public string foundSigt { get; set; } = null!;
        public SightsPage()
        {
            InitializeComponent();
            context = new VisitCenterContext();
            sights = new List<Sight>();
            listSights = new List<SightShow>();
            foundSigt = "";
            LoadSights();
            DataContext = this;
        }
        public void RefreshSights()
        {
            context.ChangeTracker.Clear();
            ListSights.ItemsSource = null;
            LoadSights();
        }
        public void LoadSights()
        {
            
            sights = context.Sights.ToList();
            if(!string.IsNullOrEmpty(foundSigt))
            {
                sights = sights.Where(s => s.ContactNumber != null && s.ContactNumber.Contains(foundSigt)
                || s.Description != null && s.Description.ToLower().Contains(foundSigt)
                || s.TypeSight.ToLower().Contains(foundSigt)
                || s.NameSight.ToLower().Contains(foundSigt)
                || s.LocationStreet.ToLower().Contains(foundSigt)
                || s.LocationHouse != null && s.LocationHouse != null && s.LocationHouse.ToLower().Contains(foundSigt)
                || s.Email != null && s.Email.ToLower().Contains(foundSigt)
                || s.SightUrl != null && s.SightUrl.ToLower().Contains(foundSigt))
                    .ToList();
            }
            ShowSights();
        }
        public void ShowSights()
        {
            listSights.Clear();
            var mediaService = MediaService.Instance;
            foreach (var sight in sights)
            {
                var sightShow = new SightShow();
                sightShow.idSight = sight.IdSight;
                sightShow.typeSight = sight.TypeSight;
                sightShow.nameSight = sight.NameSight;
                sightShow.addressSight = $"ул. {sight.LocationStreet}";
                if (!string.IsNullOrEmpty(sight.LocationHouse))
                    sightShow.addressSight += $", д. {sight.LocationHouse}";

                if (!string.IsNullOrEmpty(sight.Email))
                    sightShow.email = sight.Email;
                else sightShow.email = "Не указан";
                if (!string.IsNullOrEmpty(sight.SightUrl))
                {
                    sightShow.hyperlinkSight = sight.SightUrl;
                    sightShow.urlSight = "";
                }
                else
                {
                    sightShow.urlSight = "Не указан";
                    sightShow.hyperlinkSight = "";
                }
                if (!string.IsNullOrEmpty(sight.ContactNumber))
                    sightShow.contactNumber = sight.ContactNumber;
                else sightShow.contactNumber = "Не указан";
                sightShow.description = sight.Description;
                var modeOperations = context.SightOperatingModes.Include(s => s.IdOperatingModeNavigation)
                    .Include(s => s.IdSpecialDaySightNavigation)
                            .Where(s => s.IdSight == sight.IdSight)
                            .OrderBy(s => s.WorkingDayWeek)
                            .ToList();

                if (modeOperations != null && modeOperations.Count != 0)
                {
                    string specialDays = "";
                    if ((modeOperations.Last().WorkingDayWeek == null || modeOperations.Last().WorkingDayWeek > 7 || modeOperations.Last().WorkingDayWeek <= 0) && modeOperations.Last().IdSpecialDaySight == null)
                        sightShow.operationMode += $" {modeOperations.Last().IdOperatingModeNavigation.StartTime.ToShortTimeString()} " +
                            $"- {modeOperations.Last().IdOperatingModeNavigation.EndTime.ToShortTimeString()}\n";
                    else
                    {
                        //группировка дней недели с одинаковым графиком работы
                        var modes = new List<List<SightOperatingMode>>();
                        List<SightOperatingMode> help = new List<SightOperatingMode>();
                        foreach (var modeOperation in modeOperations)
                        {
                            if (modeOperation.WorkingDayWeek != null && modeOperation.IdSpecialDaySight == null)
                            {
                                if (help.Count == 0)
                                    help.Add(modeOperation);
                                else if (modeOperation.WorkingDayWeek == help.Last().WorkingDayWeek + 1 && modeOperation.IdOperatingMode == help.First().IdOperatingMode)
                                {
                                    help.Add(modeOperation);
                                }
                                else
                                {
                                    if (help.Count != 0)
                                        modes.Add(new List<SightOperatingMode>(help));
                                    help.Clear();
                                    help.Add(modeOperation);
                                }
                            }
                        }
                        if (help.Count != 0)
                            modes.Add(new List<SightOperatingMode>(help));
                        foreach (var modeOperation in modes)
                        {
                            int count = modeOperation.Count;
                            string[] week = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
                            var first = modeOperation.First();
                            if (count == 1)
                            {
                                if (first.IdOperatingModeNavigation != null)
                                {
                                    sightShow.operationMode += $"    {week[(int)first.WorkingDayWeek - 1]}: " +
                                        $"{first.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                        $" - {first.IdOperatingModeNavigation.EndTime.ToShortTimeString()} \n";
                                }
                                else sightShow.operationMode += $"    {week[(int)first.WorkingDayWeek - 1]}: Целый день\n";
                            }
                            else if (count > 1)
                            {
                                var last = modeOperation.Last();
                                if (last.IdOperatingModeNavigation != null)
                                {
                                    sightShow.operationMode += $"    {week[(int)first.WorkingDayWeek - 1]}-{week[(int)last.WorkingDayWeek - 1]}: " +
                                    $"{first.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                    $" - {first.IdOperatingModeNavigation.EndTime.ToShortTimeString()} \n";
                                }
                                else sightShow.operationMode += $"    {week[(int)first.WorkingDayWeek - 1]}-{week[(int)last.WorkingDayWeek - 1]}: Целый день\n";
                            }
                        }
                    }
                    modeOperations = modeOperations
                            .Where(s => s.IdSpecialDaySight != null
                            && s.IdSpecialDaySightNavigation != null
                            && s.IdSpecialDaySightNavigation.SpecialDayDate >= DateOnly.FromDateTime(DateTime.Now)
                            && s.IdSpecialDaySightNavigation.SpecialDayDate <= s.IdSpecialDaySightNavigation.SpecialDayDate.AddDays(14))
                            .ToList();
                    Console.WriteLine(modeOperations.Count.ToString());
                    if (modeOperations.Count != 0)
                    {
                        foreach (var modeOperation in modeOperations)
                        {
                            specialDays += $"{modeOperation.IdSpecialDaySightNavigation.SpecialDayDate}: ";
                            if (modeOperation.IdOperatingMode == null)
                            {
                                specialDays += $"Закрыто\n";
                            }
                            else
                            {
                                specialDays += $"{modeOperation.IdOperatingModeNavigation.StartTime.ToShortTimeString()}" +
                                    $" - {modeOperation.IdOperatingModeNavigation.EndTime.ToShortTimeString()}\n";
                            }
                        }
                        sightShow.operationMode += specialDays;
                    }
                }
                var photoSight = context.PhotoSights.Where(p => p.IdSight == sight.IdSight).FirstOrDefault();
                if (photoSight != null && !string.IsNullOrEmpty(photoSight.LinkPhoto))
                    sightShow.photoSight = mediaService.GetImage(photoSight.LinkPhoto.Trim());
                else
                    sightShow.photoSight = mediaService.GetImage("pictureSight.jpg");
                listSights.Add(sightShow);
            }
            ListSights.ItemsSource = null;
            ListSights.ItemsSource = listSights.OrderBy(s => s.idSight);
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void FoundSight(object sender, TextChangedEventArgs e)
        {
            foundSigt = foundSightText.Text.ToLower();
            listSights.Clear();
            LoadSights();
            ListSights.ItemsSource = null;
            ListSights.ItemsSource = listSights;
        }

        private void ToEditSight(object sender, SelectionChangedEventArgs e)
        {
            if (ListSights.SelectedItem != null)
            {
                SightShow sightShow = ListSights.SelectedItem as SightShow;
                ListSights.SelectedItem = null;
                NavigationService.Navigate(new AddEditSightPage(sightShow.idSight, RefreshSights));
            }
        }

        private void ToSiteSight(object sender, RequestNavigateEventArgs e)
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
                MessageBox.Show("Не удалось открыть ссылку!","Ошибка!",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToAddSight(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditSightPage(null, RefreshSights));
        }
    }
}
