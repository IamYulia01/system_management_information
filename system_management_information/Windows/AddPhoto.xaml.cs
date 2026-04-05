using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using static system_management_information.Pages.AddEditHotelPage;
using static system_management_information.Pages.AddEditSightPage;

namespace system_management_information.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddPhoto.xaml
    /// </summary>
    public partial class AddPhoto : Window
    {
        private string _tempFilePath = "";
        public FileInfo _tempFile;
        public VisitCenterContext context { get; set; }

        public AddEditHotelPage pagePerentHotel { get; set; }
        public AddEditSightPage pagePerentSight { get; set; }
        public ImageSource imageSource { get; set; }
        public bool isHotel { get; set; }
        public AddPhoto(AddEditHotelPage pageHotel, AddEditSightPage pageSight)
        {
            InitializeComponent();
            if(pageHotel == null)
            {
                pagePerentSight = pageSight;
                pagePerentHotel = null;
                isHotel = false;
            }
            else
            {
                pagePerentSight = null;
                pagePerentHotel = pageHotel;
                isHotel = true;
            }
            imageSource = null;
            context = new VisitCenterContext();
            _tempFile = null;
            
            btnDelete.Visibility = Visibility.Collapsed;
            DataContext = this;
        }

        private void DropPhoto(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                    LoadImage(files[0]);
            }
        }

        private void DownloadPhoto(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog.Title = "Выберите фотографию";
            if(openFileDialog.ShowDialog() == true)
                LoadImage(openFileDialog.FileName);
        }

        private void LoadImage(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                _tempFile = fileInfo;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".bmp" && extension != ".gif")
                {
                    MessageBox.Show("Поддерживаются только форматы jpg, jpeg, png, bmp, gif!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string tempDir = System.IO.Path.GetTempPath();
                _tempFilePath = System.IO.Path.Combine(tempDir, Guid.NewGuid().ToString() + extension);
                File.Copy(filePath, _tempFilePath, true);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_tempFilePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                image.Source = bitmap;
                imageSource = bitmap;
                btnDelete.Visibility = Visibility.Visible;
                NoImage.Visibility = Visibility.Collapsed;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePhoto(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(_tempFilePath))
            {
                try
                {
                    File.Delete(_tempFilePath);
                }
                catch { }
            }
            _tempFilePath = "";
            image.Source = null;
            imageSource = null;
            _tempFile = null;
            NoImage.Visibility= Visibility.Visible;
            btnDelete.Visibility= Visibility.Collapsed;
        }

        private void SavePhoto(object sender, RoutedEventArgs e)
        {
            
            PhotoShow photoShow = new PhotoShow();
            if(imageSource != null && _tempFile != null)
            {
                photoShow.filePhoto = _tempFile;
                photoShow.photo = imageSource;
                if(!string.IsNullOrEmpty(description.Text))
                    photoShow.shortDescription = description.Text;
                if(isHotel)
                    addPhoto.Add(photoShow);
                else 
                    addPhotoSight.Add(photoShow);

                if (pagePerentHotel != null || pagePerentSight != null)
                {
                    if (pagePerentHotel != null)
                    {
                        pagePerentHotel.listPhotos.Add(photoShow);
                        pagePerentHotel.RefreshHotels();
                    }
                    else
                    {
                        pagePerentSight.listPhotosSight.Add(photoShow);
                        pagePerentSight.RefreshSight();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка: ссылка на родительскую страницу потеряна!");
                }

                this.DialogResult = true;
                this.Close();
                return;
            }
            else
                MessageBox.Show($"Вы не добавили фотографию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_tempFilePath))
            {
                try
                {
                    File.Delete(_tempFilePath);
                }
                catch { }
            }
            _tempFilePath = "";
            image.Source = null;
            imageSource = null;
            _tempFile = null;
            this.Close();
        }
    }
}
