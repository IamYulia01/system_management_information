using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace system_management_information.Services
{
    public class MediaService
    {
        private static MediaService _instance;
        private static readonly object _lock = new object();

        public string MediaPath {  get; private set; }

        private MediaService() 
        {
            MediaPath = GetMediaPathFromConfig();

        }

        public static MediaService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            {
                                _instance = new MediaService();
                            }
                        }
                    }
                }
                return _instance;
            }
        }
        private string GetMediaPathFromConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!System.IO.File.Exists(configPath))
                {
                    Console.WriteLine($"Файл конфигурации не найден: {configPath}");
                    return FindMediaFolder();
                }

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string mediaPathFromConfig = configuration["MediaSettings:MediaFolderPath"];

                if (string.IsNullOrEmpty(mediaPathFromConfig))
                {
                    Console.WriteLine("Путь к медиафайлам не найден в конфигурации");
                    return FindMediaFolder();
                }
                Console.WriteLine($"Путь из конфигурации: {mediaPathFromConfig}");

                if (Directory.Exists(mediaPathFromConfig))
                {
                    Console.WriteLine($"Папка существует: {mediaPathFromConfig}");
                    return mediaPathFromConfig;
                }
                else
                {
                    Console.WriteLine($"Папка не существует: {mediaPathFromConfig}");
                    Console.WriteLine("Проверьте правильность пути в appsettings.json");
                    return FindMediaFolder();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении конфигурации: {ex.Message}");
                return FindMediaFolder();
            }
        }
        public string FindMediaFolder()
        {
            string curDir = Directory.GetCurrentDirectory();
            for (int i = 0; i < 5; i++)
            {
                string mediaPath = Path.Combine(curDir, "Media");
                if (Directory.Exists(mediaPath))
                    return mediaPath;
                DirectoryInfo parent = Directory.GetParent(curDir);
                if (parent == null) break;
                curDir = parent.FullName;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media");
        }

        public string GetFullPath(string relativePath)
        {
            return Path.Combine(MediaPath, relativePath);
        }

        public BitmapImage GetImage(string relativePath)
        {
            try
            {
                string fullPath = GetFullPath(relativePath);

                if (!File.Exists(fullPath))
                {
                    return GetDefaultImage();
                }

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(fullPath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                return image;
            }
            catch (Exception ex)
            {
                return GetDefaultImage();
            }
        }
        private BitmapImage GetDefaultImage()
        {
            try
            {
                string defaultPath = Path.Combine(MediaPath, "pictureSight.jpg");
                if (!File.Exists(defaultPath))
                {
                    return null;
                }
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(defaultPath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();

                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string CopyFileToMedia(string filePath)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);
                string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);

                string newFullPath = GetFullPath(uniqueName);

                File.Copy(filePath, newFullPath, true);
                return uniqueName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка копирования: {ex.Message}");
                return null;
            }
        }
        public void DeleteFileFromMedia(string fileName)
        {
            try
            {
                string fullPath = GetFullPath(fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return;
                }
            }
            catch { }
        }
    }
}
