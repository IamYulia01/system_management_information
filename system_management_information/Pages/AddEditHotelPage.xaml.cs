using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using system_management_information.Windows;
using static system_management_information.Pages.AddEditEventPage;
using static system_management_information.Pages.AddEditSightPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditHotelPage.xaml
    /// </summary>
    public partial class AddEditHotelPage : Page
    {
        public VisitCenterContext context {  get; set; }
        public bool isAdd { get; set; }
        private Action _onHotelSaved;
        public Hotel hotel { get; set; }
        public List<PhotoShow> listPhotos { get; set; }
        public static List<PhotoShow> deletePhotos { get; set; } = new List<PhotoShow>();
        public static List<PhotoShow> addPhoto { get; set; } = new List<PhotoShow>();
        public List<PhotoHotel> photos { get; set; }
        private ScrollViewer scrollViewer { get; set; }
        public AddEditHotelPage(int? idHotel, Action onHotelSaved = null)
        {
            InitializeComponent();
            _onHotelSaved = onHotelSaved;
            context = new VisitCenterContext();
            listPhotos = new List<PhotoShow>();
            photos = new List<PhotoHotel>();
            this.InvalidateVisual();
            isAdd = true;
            deletePhotos.Clear();
            addPhoto.Clear();

            var typesHotel = context.Hotels.Select(s => s.TypeHotel).ToList();
            foreach (var type in typesHotel)
            {
                if (!typeHotel.Items.Contains(type))
                    typeHotel.Items.Add(type);
            }
            typeHotel.Items.Add("Другое");
            otherView.Visibility = Visibility.Collapsed;

            if (idHotel != null)
            {
                isAdd = false;
                titlePage.Text = "Редактирование гостиницы";
                hotel = context.Hotels.Find(idHotel);
                photos = context.PhotoHotels.Where(h => h.IdHotel == idHotel).ToList();

                LoadHotel();
            }
            else
            {
                isAdd = true;
                titlePage.Text = "Добавление гостиницы";
                hotel = new Hotel();
                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
            
            //Подписка на событие полной загрузки ListView
            ListPhotos.Loaded += ListPhotosLoaded;
        }

        private void ListPhotosLoaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = FindVisualChild<ScrollViewer>(ListPhotos);
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            //Цикл проходит во всем дочерним элементам ListView и проверяет, не является ли он ScrollView
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T) return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null) return childOfChild;
                }
            }
            return null;
        }

        public void RefreshHotels()
        {
            listPhotos.Clear();

            var mediaService = MediaService.Instance;
            
            foreach (var photoHotel in photos)
            {
                var photoShow = new PhotoShow();
                photoShow.idPhoto = photoHotel.IdPhotoHotel;
                if (photoHotel != null && !string.IsNullOrEmpty(photoHotel.NameFile))
                    photoShow.photo = mediaService.GetImage(photoHotel.NameFile.Trim());
                else
                    photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                photoShow.shortDescription = photoHotel?.DescriptionPhoto;
                listPhotos.Add(photoShow);
            }
            if (photos.Count == 0)
            {
                var photoShow = new PhotoShow();
                photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                listPhotos.Add(photoShow);
            }
            foreach (var deletePhoto in deletePhotos)
            {
                listPhotos.Remove(deletePhoto);
            }
            foreach (var photo in addPhoto)
            {
                listPhotos.Add(photo);
            }
            ListPhotos.ItemsSource = null;
            ListPhotos.ItemsSource = listPhotos;
        }
        public void LoadHotel()
        {
            var mediaService = MediaService.Instance;

            foreach (var photoHotel in photos)
            {
                var photoShow = new PhotoShow();
                photoShow.idPhoto = photoHotel.IdPhotoHotel;
                if (photoHotel != null && !string.IsNullOrEmpty(photoHotel.NameFile))
                    photoShow.photo = mediaService.GetImage(photoHotel.NameFile.Trim());
                else
                    photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                photoShow.shortDescription = photoHotel?.DescriptionPhoto;
                listPhotos.Add(photoShow);
            }
            if (photos.Count == 0)
            {
                var photoShow = new PhotoShow();
                photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                listPhotos.Add(photoShow);
            }
            foreach (var deletePhoto in deletePhotos)
            {
                listPhotos.Remove(deletePhoto);
            }
            foreach (var photo in addPhoto)
            {
                listPhotos.Add(photo);
            }
            typeHotel.SelectedItem = hotel.TypeHotel;
            nameHotel.Text = hotel.HotelName;
            streetHotel.Text = hotel.HotelStreet;
            houseHotel.Text = hotel.HotelHouse;
            if(!string.IsNullOrEmpty(hotel.ContactNumberHotel))
                contactNumberHotel.Text = hotel.ContactNumberHotel;
            if (!string.IsNullOrEmpty(hotel.HotelUrl))
                urlHotel.Text = hotel.HotelUrl;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveHotel(object sender, RoutedEventArgs e)
        {
            var mediaService = MediaService.Instance;
            if (!string.IsNullOrEmpty(typeHotel.SelectedItem.ToString()))
            {
                if (typeHotel.SelectedItem.ToString() == "Другое")
                {
                    if (!string.IsNullOrEmpty(otherTypeCatering.Text))
                    {
                        hotel.TypeHotel = otherTypeCatering.Text;
                    }
                    else
                    {
                        MessageBox.Show("Введите новый тип гостиницы или выберите из списка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                    hotel.TypeHotel = typeHotel.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Выберите тип гостиницы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(nameHotel.Text))
                hotel.HotelName = nameHotel.Text;
            else
            {
                MessageBox.Show("Введите название гостиницы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(streetHotel.Text))
                hotel.HotelStreet = streetHotel.Text;
            else
            {
                MessageBox.Show("Введите улицу расположения гостиницы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(houseHotel.Text))
                hotel.HotelHouse = houseHotel.Text;
            else
            {
                MessageBox.Show("Введите дом расположения гостиницы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string contactNumberString = contactNumberHotel.Text.Trim().Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (string.IsNullOrEmpty(contactNumberHotel.Text))
                hotel.ContactNumberHotel = null;
            else if (Int64.TryParse(contactNumberString, out Int64 result) && contactNumberString.Length >= 11)
                hotel.ContactNumberHotel = contactNumberString;
            else
            {
                MessageBox.Show("Проверьте правильность ввода номера телефона! Он должен состоять не менее чем из 11 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            hotel.HotelUrl = urlHotel.Text;

            
            if (isAdd)
                context.Hotels.Add(hotel);
            context.SaveChanges();

            foreach (var add in addPhoto)
            {
                if (deletePhotos.Contains(add))
                {
                    addPhoto.Remove(add);
                    deletePhotos.Remove(add);
                }
                else
                {
                    var photo = new PhotoHotel();
                    photo.NameFile = mediaService.CopyFileToMedia(add.filePhoto.FullName);
                    if (!string.IsNullOrEmpty(add.shortDescription))
                        photo.DescriptionPhoto = add.shortDescription;
                    
                    photo.IdHotelNavigation = hotel;
                    context.PhotoHotels.Add(photo);
                }
            }
            foreach (var del in deletePhotos)
            {
                var photo = context.PhotoHotels.Find(del.idPhoto);
                if (photo != null)
                {
                    mediaService.DeleteFileFromMedia(photo.NameFile);
                    context.PhotoHotels.Remove(photo);
                }
            }

            context.SaveChanges();

            
            _onHotelSaved?.Invoke();
            NavigationService.GoBack();
        }

        private void DeleteHotel(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить гостиницу из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var photos = context.PhotoHotels.Where(e => e.IdHotel == hotel.IdHotel).ToList();
                foreach (var photo in photos)
                {
                    context.PhotoHotels.Remove(photo);
                    context.SaveChanges();
                }
                context.Hotels.Remove(hotel);
                context.SaveChanges();
                _onHotelSaved?.Invoke();
                NavigationService.GoBack();
            }
        }

        private void ListToLeft(object sender, RoutedEventArgs e)
        {
            if (scrollViewer != null)
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - 300);
        }
        private void ListToRight(object sender, RoutedEventArgs e)
        {
            if (scrollViewer != null)
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + 300);
        }

        private void AddPhoto(object sender, RoutedEventArgs e)
        {
            AddPhoto addPhotoWindow = new AddPhoto(this, null);
            var parentWindow = Window.GetWindow(this);
            addPhotoWindow.ShowDialog();
        }

        private void DeletePhoto(object sender, RoutedEventArgs e)
        {
            if (ListPhotos.SelectedItem != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить выбранную фотографию гостиницы из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var photo = ListPhotos.SelectedItem as PhotoShow;
                    var delPhoto = context.PhotoHotels.Find(photo.idPhoto);
                    if(addPhoto.Contains(photo))
                    {
                        addPhoto.Remove(photo);
                    }
                    else
                    {
                        deletePhotos.Add(photo);
                        if(delPhoto != null)
                            photos.Remove(delPhoto);
                    }
                        
                    listPhotos.Clear();
                    RefreshHotels();
                    ListPhotos.ItemsSource = null;
                    ListPhotos.ItemsSource = listPhotos;
                }
            }
            else MessageBox.Show("Вы не выбрали фото для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SelectionTypeHotel(object sender, SelectionChangedEventArgs e)
        {
            if (typeHotel.SelectedItem == "Другое")
            {
                otherView.Visibility = Visibility.Visible;
            }
            else otherView.Visibility = Visibility.Collapsed;
        }
    }
}
