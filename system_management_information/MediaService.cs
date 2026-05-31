using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace system_management_information.Services
{
    public class MediaService
    {
        private static MediaService _instance;
        private static readonly object _lock = new object();

        public string MediaPath { get; private set; }

        private MediaService()
        {
            MediaPath = GetMediaPathFromConfig();
            // Проверяем доступность сетевой папки при запуске
            CheckNetworkAccess();
            DebugMediaPath();
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
                            _instance = new MediaService();
                        }
                    }
                }
                return _instance;
            }
        }

        private void CheckNetworkAccess()
        {
            try
            {
                if (MediaPath.StartsWith(@"\\"))
                {
                    // Проверяем доступность сетевой папки
                    if (!Directory.Exists(MediaPath))
                    {
                        string message = $"Сетевая папка недоступна:\n{MediaPath}\n\n" +
                                         "Проверьте:\n" +
                                         "1. Подключение к сети\n" +
                                         "2. Доступность сервера\n" +
                                         "3. Правильность пути в appsettings.json";
                        MessageBox.Show(message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки сетевой папки: {ex.Message}");
            }
        }

        private string GetMediaPathFromConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (!File.Exists(configPath))
                {
                    WriteLog($"Файл конфигурации не найден: {configPath}");
                    return FindMediaFolder();
                }

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string mediaPathFromConfig = configuration["MediaSettings:MediaFolderPath"];

                if (string.IsNullOrEmpty(mediaPathFromConfig))
                {
                    WriteLog("Путь к медиафайлам не найден в конфигурации");
                    return FindMediaFolder();
                }

                WriteLog($"Путь из конфигурации: {mediaPathFromConfig}");

                // Если это сетевой путь - используем ТОЛЬКО его
                if (mediaPathFromConfig.StartsWith(@"\\"))
                {
                    WriteLog($"Сетевой путь: {mediaPathFromConfig}");

                    // Проверяем доступность
                    if (Directory.Exists(mediaPathFromConfig))
                    {
                        WriteLog($"Сетевая папка доступна: {mediaPathFromConfig}");
                        return mediaPathFromConfig;
                    }
                    else
                    {
                        WriteLog($"Сетевая папка НЕ ДОСТУПНА: {mediaPathFromConfig}");

                        // НЕ ВОЗВРАЩАЕМ локальную папку!
                        // Показываем ошибку и возвращаем сетевой путь (приложение будет работать через заглушки)
                        MessageBox.Show(
                            $"Сетевая папка недоступна!\n{mediaPathFromConfig}\n\n" +
                            "Проверьте:\n" +
                            "1. Доступен ли сервер 192.168.10.12\n" +
                            "2. Существует ли папка data_media_visit_center\n" +
                            "3. Открыт ли общий доступ к папке\n\n" +
                            "Фотографии не будут отображаться до устранения проблемы.",
                            "Внимание",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        // Возвращаем сетевой путь, даже если он недоступен
                        return mediaPathFromConfig;
                    }
                }

                // Обработка локального пути
                string fullMediaPath = mediaPathFromConfig;
                if (!Path.IsPathRooted(mediaPathFromConfig))
                {
                    fullMediaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mediaPathFromConfig);
                    fullMediaPath = Path.GetFullPath(fullMediaPath);
                }

                if (!Directory.Exists(fullMediaPath))
                {
                    WriteLog($"Папка не существует, создаю: {fullMediaPath}");
                    Directory.CreateDirectory(fullMediaPath);
                }

                return fullMediaPath;
            }
            catch (Exception ex)
            {
                WriteLog($"Ошибка: {ex.Message}");
                return FindMediaFolder();
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media_debug.txt");
                File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss} - {message}\n");
            }
            catch { }
        }

        public string FindMediaFolder()
        {
            
            string[] possibleFolderNames = { "data_media_visit_center", "Media", "Data", "Images" };

            // Сначала проверяем сетевой путь
            string[] networkPaths = {
                @"\\192.168.10.12\data_media_visit_center",
                @"\\192.168.10.12\VisitCenterPhotos",
                @"\\192.168.10.12\SharedPhotos"
            };

            foreach (string networkPath in networkPaths)
            {
                try
                {
                    if (Directory.Exists(networkPath))
                    {
                        WriteLog($"Найдена сетевая папка: {networkPath}");
                        return networkPath;
                    }
                }
                catch { }
            }

            string curDir = AppDomain.CurrentDomain.BaseDirectory;
            for (int i = 0; i < 5; i++)
            {
                foreach (string folderName in possibleFolderNames)
                {
                    string mediaPath = Path.Combine(curDir, folderName);
                    if (Directory.Exists(mediaPath))
                    {
                        WriteLog($"Папка найдена: {mediaPath}");
                        return mediaPath;
                    }
                }
                DirectoryInfo parent = Directory.GetParent(curDir);
                if (parent == null) break;
                curDir = parent.FullName;
            }

            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data_media_visit_center");
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                WriteLog($"Создана папка по умолчанию: {defaultPath}");
            }
            return defaultPath;
        }

        public string GetFullPath(string relativePath)
        {
            return Path.Combine(MediaPath, relativePath);
        }

        public BitmapImage GetImage(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return GetDefaultImage();

                string fullPath = GetFullPath(relativePath);

                WriteLog($"Загрузка изображения: {fullPath}");
                WriteLog($"Файл существует: {File.Exists(fullPath)}");

                if (!File.Exists(fullPath))
                {
                    WriteLog($"Файл не найден: {fullPath}");
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
                WriteLog($"Ошибка загрузки изображения: {ex.Message}");
                return GetDefaultImage();
            }
        }

        private BitmapImage GetDefaultImage()
        {
            try
            {
                string[] possibleDefaults = { "pictureSight.jpg", "default.jpg", "noimage.jpg" };

                foreach (string defaultName in possibleDefaults)
                {
                    string defaultPath = GetFullPath(defaultName);
                    if (File.Exists(defaultPath))
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(defaultPath, UriKind.Absolute);
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                WriteLog($"Ошибка загрузки default изображения: {ex.Message}");
                return null;
            }
        }

        public string CopyFileToMedia(string filePath)
        {
            try
            {
                // Проверяем доступность сетевой папки
                if (MediaPath.StartsWith(@"\\") && !Directory.Exists(MediaPath))
                {
                    MessageBox.Show($"Сетевая папка недоступна!\n{MediaPath}\n\nПроверьте подключение к серверу.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if (!Directory.Exists(MediaPath))
                {
                    Directory.CreateDirectory(MediaPath);
                }

                string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
                string newFullPath = GetFullPath(uniqueName);

                File.Copy(filePath, newFullPath, true);
                WriteLog($"Файл скопирован: {newFullPath}");
                return uniqueName;
            }
            catch (Exception ex)
            {
                WriteLog($"Ошибка копирования: {ex.Message}");
                MessageBox.Show($"Ошибка копирования: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        public void DebugMediaPath()
        {
            string debugInfo = $"=== ДИАГНОСТИКА MEDIA SERVICE ===\n" +
                               $"MediaPath: {MediaPath}\n" +
                               $"Путь существует: {Directory.Exists(MediaPath)}\n" +
                               $"Тип пути: {(MediaPath.StartsWith(@"\\") ? "Сетевой" : "Локальный")}\n" +
                               $"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}\n\n" +
                               $"Проверка доступа к папке:\n";

            try
            {
                if (Directory.Exists(MediaPath))
                {
                    var files = Directory.GetFiles(MediaPath, "*.*", SearchOption.TopDirectoryOnly);
                    debugInfo += $"Найдено файлов: {files.Length}\n";
                    foreach (var file in files.Take(5))
                    {
                        debugInfo += $"  - {Path.GetFileName(file)}\n";
                    }
                }
                else
                {
                    debugInfo += "Папка НЕ ДОСТУПНА!\n";
                    debugInfo += $"Полный путь: {Path.GetFullPath(MediaPath)}\n";
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"ОШИБКА: {ex.Message}\n";
            }

            debugInfo += $"\n=== appsettings.json ===\n";
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                debugInfo += File.ReadAllText(configPath);
            }
            else
            {
                debugInfo += "Файл не найден!";
            }

            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media_debug.txt");
            File.WriteAllText(logPath, debugInfo);
            MessageBox.Show($"Диагностика сохранена в:\n{logPath}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void DeleteFileFromMedia(string fileName)
        {
            try
            {
                string fullPath = GetFullPath(fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    WriteLog($"Файл удалён: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Ошибка удаления: {ex.Message}");
            }
        }
    }
}