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
using system_management_information.Services;
using static system_management_information.Pages.EventsPage;
using static system_management_information.Pages.SightsPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для HotelsPage.xaml
    /// </summary>
    public partial class HotelsPage : Page
    {
        public class HotelShow
        {
            public int idHotel { get; set; }
            public string hotelName { get; set; } = null!;
            public string addressHotel { get; set; } = null!;
            public string contactNumberHotel { get; set; } = null!;
            public string? hotelUrl { get; set; }
            public string? hyperlinkHotel { get; set; }
            public string? typeHotel { get; set; }
            public ImageSource photoHotel { get; set; }

        }
        public VisitCenterContext context {  get; set; }
        public string foundHotel { get; set; }
        public List<Hotel> hotels { get; set; }
        public List<HotelShow> listHotels { get; set; }
        public HotelsPage()
        {
            InitializeComponent();
            context = new VisitCenterContext();
            foundHotel = "";
            hotels = new List<Hotel>();
            listHotels = new List<HotelShow>();
            LoadHotels();
            DataContext = this;
        }

        public void RefreshHotel()
        {
            context.ChangeTracker.Clear();
            ListHotels.ItemsSource = null;
            LoadHotels();
        }

        public void LoadHotels()
        {
            hotels = context.Hotels.ToList();
            if (!string.IsNullOrEmpty(foundHotel))
            {
                hotels = hotels.Where(h => h.TypeHotel.ToLower().Contains(foundHotel)
                || h.HotelName.ToLower().Contains(foundHotel)
                || h.HotelStreet.ToLower().Contains(foundHotel)
                || h.HotelHouse.ToLower().Contains(foundHotel)
                || h.HotelUrl != null && h.HotelUrl.ToLower().Contains(foundHotel)
                || h.ContactNumberHotel.ToLower().Contains(foundHotel)).ToList();
            }
            ShowHotels();
        }

        private void ShowHotels()
        {
            listHotels.Clear();
            var mediaService = MediaService.Instance;
            foreach (var hotel in hotels)
            {
                var hotelShow = new HotelShow();
                hotelShow.idHotel = hotel.IdHotel;
                hotelShow.typeHotel = hotel.TypeHotel;
                hotelShow.hotelName = hotel.HotelName;
                if (!string.IsNullOrEmpty(hotel.ContactNumberHotel))
                    hotelShow.contactNumberHotel = hotel.ContactNumberHotel;
                else hotelShow.contactNumberHotel = "Не указан";
                if (!string.IsNullOrEmpty(hotel.HotelUrl))
                {
                    hotelShow.hyperlinkHotel = hotel.HotelUrl;
                    hotelShow.hotelUrl = "";
                }
                else
                {
                    hotelShow.hotelUrl = "Не указан";
                    hotelShow.hyperlinkHotel = "";
                }
                hotelShow.addressHotel = $"ул. {hotel.HotelStreet}, д. {hotel.HotelHouse}";

                var photo = context.PhotoHotels.Where(p => p.IdHotel == hotel.IdHotel).FirstOrDefault();
                if (photo != null && !string.IsNullOrEmpty(photo.NameFile))
                    hotelShow.photoHotel = mediaService.GetImage(photo.NameFile.Trim());
                else
                    hotelShow.photoHotel = mediaService.GetImage("pictureSight.jpg");

                listHotels.Add(hotelShow);
            }
            ListHotels.ItemsSource = null;
            ListHotels.ItemsSource = listHotels.OrderBy(h => h.idHotel);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void ToSiteHotel(object sender, RequestNavigateEventArgs e)
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

        private void FoundHotel(object sender, TextChangedEventArgs e)
        {
            foundHotel = foundHotelText.Text.ToLower();
            listHotels.Clear();
            LoadHotels();
            ListHotels.ItemsSource = null;
            ListHotels.ItemsSource = listHotels;
        }

        private void ToEditHotel(object sender, SelectionChangedEventArgs e)
        {
            if (ListHotels.SelectedItem != null)
            {
                HotelShow hotelShow = ListHotels.SelectedItem as HotelShow;
                ListHotels.SelectedItem = null;
                NavigationService.Navigate(new AddEditHotelPage(hotelShow.idHotel, RefreshHotel));
            }
        }

        private void ToAddHotel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditHotelPage(null, RefreshHotel));
        }
    }
}
