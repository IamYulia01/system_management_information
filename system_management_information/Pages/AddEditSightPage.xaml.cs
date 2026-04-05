using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
using static system_management_information.Pages.AddEditSightPage;
using static system_management_information.Pages.SightsPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditSightPage.xaml
    /// </summary>
    public partial class AddEditSightPage : Page
    {
        public class PhotoShow
        {
            public int idPhoto { get; set; }

            public ImageSource photo { get; set; } = null!;
            public FileInfo? filePhoto { get; set; }
            public string? shortDescription { get; set; }
        }
        public class OperatingModeShow
        {
            public int idSightOperatingMode { get; set; }
            public string time { get; set; }
            public string day { get; set; }
            public bool isSpecialDay { get; set; }
        }
        private Action _onSightSaved;
        public VisitCenterContext context {  get; set; }
        public bool isAdd {  get; set; }
        public Sight sight { get; set; }
        public List<PhotoShow> listPhotosSight { get; set; }
        public static List<PhotoShow> deletePhotosSight { get; set; } = new List<PhotoShow>();
        public static List<PhotoShow> addPhotoSight { get; set; } = new List<PhotoShow>();
        public List<PhotoSight> photosSight { get; set; }
        private ScrollViewer scrollViewer { get; set; }
        public List<SightOperatingMode> operatingModes { get; set; }
        public static List<SightOperatingMode> editOperatingModes { get; set; } = new List<SightOperatingMode>();
        public static List<SightOperatingMode> deleteOperatingModes { get; set; } = new List<SightOperatingMode>();
        public static List<SightOperatingMode> addOperatingModes { get; set; } = new List<SightOperatingMode>();
        public List<OperatingModeShow> listOperatingModes { get; set; }
        public AddEditSightPage(int? idSight, Action onSightSaved = null)
        {
            InitializeComponent();
            _onSightSaved = onSightSaved;
            context = new VisitCenterContext();
            this.InvalidateVisual();
            photosSight = new List<PhotoSight>();
            listPhotosSight = new List<PhotoShow>();
            isAdd = true;
            editOperatingModes.Clear();
            deleteOperatingModes.Clear();
            addOperatingModes.Clear();

            var typesSight = context.Sights.Select(s => s.TypeSight).ToList();
            foreach (var type in typesSight)
            {
                if (!typeSight.Items.Contains(type))
                    typeSight.Items.Add(type);
            }
            typeSight.Items.Add("Другое");
            otherView.Visibility = Visibility.Collapsed;

            operatingModes = new List<SightOperatingMode>();
            listOperatingModes = new List<OperatingModeShow>();
            if (idSight != null)
            {
                isAdd = false;
                titlePage.Text = "Редактирование достопримечательности";
                sight = context.Sights.Find(idSight);
                operatingModes = context.SightOperatingModes
                    .Include(s => s.IdOperatingModeNavigation)
                    .Include(s => s.IdSpecialDaySightNavigation)
                    .Where(s => s.IdSight == idSight)
                    .ToList();
                photosSight = context.PhotoSights.Where(s => s.IdSight == idSight).ToList();
                LoadSight();
            }
            else
            {
                isAdd = true;
                titlePage.Text = "Добавление достопримечательности";
                sight = new Sight();
                operatingModes = new List<SightOperatingMode>();

                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
            ListPhotos.Loaded += ListPhotosLoaded;
        }

        private void ListPhotosLoaded(object sender, RoutedEventArgs e)
        {
            scrollViewer = FindVisualChild<ScrollViewer>(ListPhotos);
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
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

        public void LoadSight()
        {
            var mediaService = MediaService.Instance;
            foreach (var photoSight in photosSight)
            {
                var photoShow = new PhotoShow();
                photoShow.idPhoto = photoSight.IdPhotoSight;
                if (photoSight != null && !string.IsNullOrEmpty(photoSight.LinkPhoto))
                    photoShow.photo = mediaService.GetImage(photoSight.LinkPhoto.Trim());
                else
                    photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                photoShow.shortDescription = photoSight?.ShortDescription;
                listPhotosSight.Add(photoShow);
            }
            if (photosSight.Count == 0)
            {
                var photoShow = new PhotoShow();
                photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                listPhotosSight.Add(photoShow);
            }
            foreach (var deletePhoto in deletePhotosSight)
            {
                listPhotosSight.Remove(deletePhoto);
            }
            foreach (var photo in addPhotoSight)
            {
                listPhotosSight.Add(photo);
            }

            if (addOperatingModes.Count > 0)
            {
                foreach (var mode in addOperatingModes)
                {
                    operatingModes.Add(mode);
                }
            }
            if (deleteOperatingModes.Count > 0)
            {
                foreach (var mode in deleteOperatingModes)
                    operatingModes.Remove(mode);
            }
            if (editOperatingModes.Count > 0)
            {
                foreach (var mode in editOperatingModes)
                {
                    var modeEdit = operatingModes.Where(s => s.IdSightOperatingMode == mode.IdSightOperatingMode).FirstOrDefault();
                    modeEdit.IdOperatingModeNavigation = mode.IdOperatingModeNavigation;
                    modeEdit.IdSpecialDaySightNavigation = mode.IdSpecialDaySightNavigation;
                    modeEdit.WorkingDayWeek = mode.WorkingDayWeek;
                }
            }
            typeSight.SelectedItem = sight.TypeSight;
            nameSight.Text = sight.NameSight;
            contactNumberSight.Text = sight.ContactNumber;
            streetSight.Text = sight.LocationStreet;
            houseSight.Text = sight.LocationHouse;
            emailSight.Text = sight.Email;
            urlSight.Text = sight.SightUrl;
            descriptionSight.Text = sight.Description;
            operatingModes = operatingModes.OrderBy(s => s.WorkingDayWeek).ToList();
            foreach (var operatingMode in operatingModes)
            {
                var operatingModeShow = new OperatingModeShow();
                operatingModeShow.idSightOperatingMode = operatingMode.IdSightOperatingMode;
                if (operatingMode.IdOperatingMode != null)
                    operatingModeShow.time = $"{operatingMode.IdOperatingModeNavigation.StartTime.ToShortTimeString()} - {operatingMode.IdOperatingModeNavigation.EndTime.ToShortTimeString()}";
                else if (operatingMode.IdSpecialDaySightNavigation != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDaySightNavigation != null)
                {
                    operatingModeShow.isSpecialDay = true;
                    operatingModeShow.day = $"{operatingMode.IdSpecialDaySightNavigation.SpecialDayDate.ToString()}";
                    if (!string.IsNullOrEmpty(operatingMode.IdSpecialDaySightNavigation.SpecialDayStatus))
                        operatingModeShow.day += $"    ({operatingMode.IdSpecialDaySightNavigation.SpecialDayStatus})";
                }
                else
                {
                    operatingModeShow.isSpecialDay = false;
                    switch(operatingMode.WorkingDayWeek)
                    {
                        case 1:
                            {
                                operatingModeShow.day = "Понедельник";
                                break;
                            }
                        case 2:
                            {
                                operatingModeShow.day = "Вторник";
                                break;
                            }
                        case 3:
                            {
                                operatingModeShow.day = "Среда";
                                break;
                            }
                        case 4:
                            {
                                operatingModeShow.day = "Четверг";
                                break;
                            }
                        case 5:
                            {
                                operatingModeShow.day = "Пятница";
                                break;
                            }
                        case 6:
                            {
                                operatingModeShow.day = "Суббота";
                                break;
                            }
                        case 7:
                            {
                                operatingModeShow.day = "Воскресенье";
                                break;
                            }
                        default:
                            {
                                operatingModeShow.day = "Каждый день";
                                break;
                            }
                    }
                }
                listOperatingModes.Add(operatingModeShow);
            }
        }

        public void RefreshSight()
        {
            listOperatingModes.Clear();
            listPhotosSight.Clear();
            var mediaService = MediaService.Instance;

            foreach (var photoSight in photosSight)
            {
                var photoShow = new PhotoShow();
                photoShow.idPhoto = photoSight.IdPhotoSight;
                if (photoSight != null && !string.IsNullOrEmpty(photoSight.LinkPhoto))
                    photoShow.photo = mediaService.GetImage(photoSight.LinkPhoto.Trim());
                else
                    photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                photoShow.shortDescription = photoSight?.ShortDescription;
                listPhotosSight.Add(photoShow);
            }
            if (photosSight.Count == 0)
            {
                var photoShow = new PhotoShow();
                photoShow.photo = mediaService.GetImage("pictureSight.jpg");
                listPhotosSight.Add(photoShow);
            }
            foreach (var deletePhoto in deletePhotosSight)
            {
                listPhotosSight.Remove(deletePhoto);
            }
            foreach (var photo in addPhotoSight)
            {
                listPhotosSight.Add(photo);
            }
            

            foreach (var operatingMode in operatingModes)
            {
                var operatingModeShow = new OperatingModeShow();
                operatingModeShow.idSightOperatingMode = operatingMode.IdSightOperatingMode;
                
                if (operatingMode.IdOperatingModeNavigation != null)
                    operatingModeShow.time = $"{operatingMode.IdOperatingModeNavigation.StartTime.ToShortTimeString()} - {operatingMode.IdOperatingModeNavigation.EndTime.ToShortTimeString()}";
                else if (operatingMode.IdSpecialDaySightNavigation != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDaySightNavigation != null)
                {
                    operatingModeShow.isSpecialDay = true;
                    operatingModeShow.day = $"{operatingMode.IdSpecialDaySightNavigation.SpecialDayDate.ToString()}";
                    if (!string.IsNullOrEmpty(operatingMode.IdSpecialDaySightNavigation.SpecialDayStatus))
                        operatingModeShow.day += $"    ({operatingMode.IdSpecialDaySightNavigation.SpecialDayStatus})";
                }
                else
                {
                    operatingModeShow.isSpecialDay = false;
                    switch (operatingMode.WorkingDayWeek)
                    {
                        case 1:
                            {
                                operatingModeShow.day = "Понедельник";
                                break;
                            }
                        case 2:
                            {
                                operatingModeShow.day = "Вторник";
                                break;
                            }
                        case 3:
                            {
                                operatingModeShow.day = "Среда";
                                break;
                            }
                        case 4:
                            {
                                operatingModeShow.day = "Четверг";
                                break;
                            }
                        case 5:
                            {
                                operatingModeShow.day = "Пятница";
                                break;
                            }
                        case 6:
                            {
                                operatingModeShow.day = "Суббота";
                                break;
                            }
                        case 7:
                            {
                                operatingModeShow.day = "Воскресенье";
                                break;
                            }
                        default:
                            {
                                operatingModeShow.day = "Каждый день";
                                break;
                            }
                    }
                }
                listOperatingModes.Add(operatingModeShow);
            }
            ListPhotos.ItemsSource = null;
            ListPhotos.ItemsSource = listPhotosSight;
            ListOperatingModes.ItemsSource = null;
            ListOperatingModes.ItemsSource = listOperatingModes;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeleteSight(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить достопримечательность из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var operatingModes = context.SightOperatingModes.Where(o => o.IdSight == sight.IdSight).ToList();
                foreach (var operatingMode in operatingModes)
                {
                    context.SightOperatingModes.Remove(operatingMode);
                    context.SaveChanges();
                }
                var photos = context.PhotoSights.Where(p => p.IdSight == sight.IdSight).ToList();
                foreach (var photo in photos)
                {
                    context.PhotoSights.Remove(photo);
                    context.SaveChanges();
                }
                context.Sights.Remove(sight);
                context.SaveChanges();
                _onSightSaved?.Invoke();
                NavigationService.GoBack();
            }
        }

        private void SaveSight(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(typeSight.SelectedItem.ToString()))
            {
                if (typeSight.SelectedItem.ToString() == "Другое")
                {
                    if (!string.IsNullOrEmpty(otherTypeCatering.Text))
                    {
                        sight.TypeSight = otherTypeCatering.Text;
                    }
                    else
                    {
                        MessageBox.Show("Введите новый тип достопримечательности или выберите из списка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                    sight.TypeSight = typeSight.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Выберите тип достопримечательности!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(nameSight.Text))
                sight.NameSight = nameSight.Text;
            else
            {
                MessageBox.Show("Введите название достопримечательности!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            sight.Description = descriptionSight.Text;

            var emailRegex = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
            if (!string.IsNullOrEmpty(streetSight.Text))
                sight.LocationStreet = streetSight.Text;
            else
            {
                MessageBox.Show("Введите улицу расположения достопримечательности!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            sight.LocationHouse = houseSight.Text;

            if (string.IsNullOrEmpty(emailSight.Text))
                sight.Email = null;
            else if (Regex.Match(emailSight.Text, emailRegex).Success)
                sight.Email = emailSight.Text;
            else
            {
                MessageBox.Show("Проверьте правильность ввода электронной почты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string contactNumberString = contactNumberSight.Text.Trim().Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (string.IsNullOrEmpty(contactNumberSight.Text))
                sight.ContactNumber = null;
            else if (Int64.TryParse(contactNumberString, out Int64 result) && contactNumberString.Length >= 11)
                sight.ContactNumber = contactNumberString;
            else
            {
                MessageBox.Show("Проверьте правильность ввода номера телефона! Он должен состоять не меньше чем из 11 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            sight.SightUrl = urlSight.Text;

            if (isAdd)
                context.Sights.Add(sight);
            context.SaveChanges();

            foreach (var addOperatingMode in addOperatingModes)
            {
                addOperatingMode.IdSight = sight.IdSight;

                if (addOperatingMode.IdSpecialDaySightNavigation != null)
                {
                    var existingSpecialDay = context.SpecialDaySights.FirstOrDefault(s => s.SpecialDayDate == addOperatingMode.IdSpecialDaySightNavigation.SpecialDayDate
                    && s.SpecialDayStatus == addOperatingMode.IdSpecialDaySightNavigation.SpecialDayStatus);

                    if (existingSpecialDay != null)
                    {
                        addOperatingMode.IdSpecialDaySight = existingSpecialDay.IdSpecialDaySight;
                        addOperatingMode.IdSpecialDaySightNavigation = null;
                    }
                    else
                    {
                        context.SpecialDaySights.Add(addOperatingMode.IdSpecialDaySightNavigation);
                        context.SaveChanges();
                        addOperatingMode.IdSpecialDaySight = addOperatingMode.IdSpecialDaySightNavigation.IdSpecialDaySight;
                        addOperatingMode.IdSpecialDaySightNavigation = null;
                    }
                }

                if (addOperatingMode.IdOperatingModeNavigation != null)
                {
                    var existingOperatingMode = context.OperatingModes.FirstOrDefault(s => s.StartTime == addOperatingMode.IdOperatingModeNavigation.StartTime
                    && s.EndTime == addOperatingMode.IdOperatingModeNavigation.EndTime);

                    if (existingOperatingMode != null)
                    {
                        addOperatingMode.IdOperatingMode = existingOperatingMode.IdOperatingMode;
                        addOperatingMode.IdOperatingModeNavigation = null;
                    }
                    else
                    {
                        context.OperatingModes.Add(addOperatingMode.IdOperatingModeNavigation);
                        context.SaveChanges();
                        addOperatingMode.IdOperatingMode = addOperatingMode.IdOperatingModeNavigation.IdOperatingMode;
                        addOperatingMode.IdOperatingModeNavigation = null;
                    }
                }
                context.SightOperatingModes.Add(addOperatingMode);
            }

            foreach(var editOperatingMode in editOperatingModes)
            {
                
                var existingMode = context.SightOperatingModes
                    .Include(s => s.IdOperatingModeNavigation)
                    .Include(s => s.IdSpecialDaySightNavigation)
                    .FirstOrDefault(s => s.IdSightOperatingMode == editOperatingMode.IdSightOperatingMode);
                if (existingMode != null)
                {
                    existingMode.WorkingDayWeek = editOperatingMode.WorkingDayWeek;
                    if (editOperatingMode.IdSpecialDaySightNavigation != null)
                    {
                        var existingSpecialDay = context.SpecialDaySights.FirstOrDefault(s => s.SpecialDayDate == editOperatingMode.IdSpecialDaySightNavigation.SpecialDayDate
                    && s.SpecialDayStatus == editOperatingMode.IdSpecialDaySightNavigation.SpecialDayStatus);

                        if (existingSpecialDay != null)
                        {
                            existingMode.IdSpecialDaySight = existingSpecialDay.IdSpecialDaySight;
                        }
                        else
                        {
                            var newSpecialDay = new SpecialDaySight
                            {
                                SpecialDayDate = editOperatingMode.IdSpecialDaySightNavigation.SpecialDayDate,
                                SpecialDayStatus = editOperatingMode.IdSpecialDaySightNavigation.SpecialDayStatus
                            };
                            context.SpecialDaySights.Add(newSpecialDay);
                            context.SaveChanges();
                            existingMode.IdSpecialDaySight = newSpecialDay.IdSpecialDaySight;
                        }
                    }
                    else
                    {
                        existingMode.IdSpecialDaySight = null;
                    }

                    if (editOperatingMode.IdOperatingModeNavigation != null)
                    {
                        var existingOperatingMode = context.OperatingModes.FirstOrDefault(s => s.StartTime == editOperatingMode.IdOperatingModeNavigation.StartTime
                    && s.EndTime == editOperatingMode.IdOperatingModeNavigation.EndTime);

                        if (existingOperatingMode != null)
                        {
                            existingMode.IdOperatingMode = existingOperatingMode.IdOperatingMode;
                        }
                        else
                        {
                            context.OperatingModes.Add(editOperatingMode.IdOperatingModeNavigation);
                            context.SaveChanges();
                            existingMode.IdOperatingMode = editOperatingMode.IdOperatingModeNavigation.IdOperatingMode;
                        }
                    }
                    else
                    {
                        existingMode.IdOperatingMode = null;
                    }

                }
            }
            
            foreach( var deleteOperatingMode in deleteOperatingModes)
            {
                var localOperatingMode = context.SightOperatingModes.Local.FirstOrDefault(t => t.IdSightOperatingMode == deleteOperatingMode.IdSightOperatingMode);

                if(localOperatingMode != null) 
                    context.SightOperatingModes.Remove(localOperatingMode);
                else
                {
                    context.SightOperatingModes.Attach(deleteOperatingMode);
                    context.SightOperatingModes.Remove(deleteOperatingMode);
                }
            }

            context.SaveChanges();

            addOperatingModes.Clear();
            deleteOperatingModes.Clear();
            editOperatingModes.Clear();

            _onSightSaved?.Invoke();
            NavigationService.GoBack();
        }

        private void AddDay(object sender, RoutedEventArgs e)
        {
            AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(null, false, this);
            var parentWindow = Window.GetWindow(this);
            addEditOperatingMode.ShowDialog();
            RefreshSight();
        }

        private void EditOperatingMode(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                OperatingModeShow operatingModeShow = e.AddedItems[0] as OperatingModeShow;

                if (operatingModeShow != null)
                {
                    var operatingMode = context.SightOperatingModes.Find(operatingModeShow.idSightOperatingMode);
                    if (operatingMode != null)
                    {
                        bool isSpecialDay = operatingMode.IdSpecialDaySightNavigation != null;
                        AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(operatingModeShow.idSightOperatingMode, isSpecialDay, this);
                        addEditOperatingMode.Owner = Window.GetWindow(this);
                        addEditOperatingMode.ShowDialog();
                        RefreshSight();
                    }
                    else
                        MessageBox.Show("Вы не можете редактировать только что добавленный график! Сначала сохраните данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        private void AddSpecialDay(object sender, RoutedEventArgs e)
        {
            AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(null, true, this);
            var parentWindow = Window.GetWindow(this);
            addEditOperatingMode.ShowDialog();
            RefreshSight();
        }

        private void AddPhoto(object sender, RoutedEventArgs e)
        {
            AddPhoto addPhotoWindow = new AddPhoto(null, this);
            var parentWindow = Window.GetWindow(this);
            addPhotoWindow.ShowDialog();
        }

        private void DeletePhoto(object sender, RoutedEventArgs e)
        {
            if (ListPhotos.SelectedItem != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить выбранную фотографию достопримечательности из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var photo = ListPhotos.SelectedItem as PhotoShow;
                    var delPhoto = context.PhotoSights.Find(photo.idPhoto);
                    if (addPhotoSight.Contains(photo))
                    {
                        addPhotoSight.Remove(photo);
                    }
                    else
                    {
                        deletePhotosSight.Add(photo);
                        if (delPhoto != null)
                            photosSight.Remove(delPhoto);
                    }

                    listPhotosSight.Clear();
                    RefreshSight();
                    ListPhotos.ItemsSource = null;
                    ListPhotos.ItemsSource = listPhotosSight;
                }
            }
            else MessageBox.Show("Вы не выбрали фото для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void SeclectionTypeSight(object sender, SelectionChangedEventArgs e)
        {
            if (typeSight.SelectedItem == "Другое")
            {
                otherView.Visibility = Visibility.Visible;
            }
            else otherView.Visibility = Visibility.Collapsed;
        }
    }
}
