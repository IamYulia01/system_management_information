using Microsoft.EntityFrameworkCore;
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
using system_management_information.Windows;
using static system_management_information.Pages.SightsPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditSightPage.xaml
    /// </summary>
    public partial class AddEditSightPage : Page
    {
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
            isAdd = true;
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
        }
        public void LoadSight()
        {
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
                    var modeEdit = context.SightOperatingModes.Find(mode.IdSightOperatingMode);
                    modeEdit.IdOperatingMode = mode.IdOperatingMode;
                    modeEdit.IdSpecialDaySight = mode.IdSpecialDaySight;
                    modeEdit.WorkingDayWeek = mode.WorkingDayWeek;
                }
            }
            typeSight.Text = sight.TypeSight;
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
                else if (operatingMode.IdSpecialDaySight != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDaySight != null)
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

        public void RefreshOperatingMode()
        {
            listOperatingModes.Clear();
            
            foreach (var operatingMode in operatingModes)
            {
                var operatingModeShow = new OperatingModeShow();
                operatingModeShow.idSightOperatingMode = operatingMode.IdSightOperatingMode;
                if (operatingMode.IdOperatingModeNavigation != null)
                    operatingModeShow.time = $"{operatingMode.IdOperatingModeNavigation?.StartTime.ToShortTimeString()} - {operatingMode.IdOperatingModeNavigation?.EndTime.ToShortTimeString()}";
                else if (operatingMode.IdSpecialDaySight != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDaySight != null)
                {
                    operatingModeShow.day = $"{operatingMode.IdSpecialDaySightNavigation?.SpecialDayDate.ToString()}";
                    if (!string.IsNullOrEmpty(operatingMode.IdSpecialDaySightNavigation?.SpecialDayStatus))
                        operatingModeShow.day += $"    ({operatingMode.IdSpecialDaySightNavigation?.SpecialDayStatus})";
                }
                else
                {
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
            if (!string.IsNullOrEmpty(typeSight.Text))
                sight.TypeSight = typeSight.Text;
            else
            {
                MessageBox.Show("Введите тип достопримечательности!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if(isAdd)
                context.Sights.Add(sight);
            context.SaveChanges();

            foreach (var addOperatingMode in addOperatingModes)
            {
                addOperatingMode.IdSight = sight.IdSight;
                context.SightOperatingModes.Add(addOperatingMode);
            }
            foreach (var deleteOperatingMode in deleteOperatingModes)
            {
                var localOperatingMode = context.SightOperatingModes.Local.FirstOrDefault(t => t.IdSightOperatingMode == deleteOperatingMode.IdSightOperatingMode);
                if (localOperatingMode != null)
                    context.SightOperatingModes.Remove(localOperatingMode);
                else
                {
                    context.SightOperatingModes.Attach(deleteOperatingMode);
                    context.SightOperatingModes.Remove(deleteOperatingMode);
                }
            }
            foreach (var editOperatingMode in editOperatingModes)
            {
                var mode = context.Tickets.Find(editOperatingMode.IdSightOperatingMode);
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
        }

        private void EditOperatingMode(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                OperatingModeShow operatingModeShow = e.AddedItems[0] as OperatingModeShow;

                if (operatingModeShow != null)
                {

                    if (operatingModeShow.isSpecialDay)
                    {
                        AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(operatingModeShow.idSightOperatingMode, true, this);
                        addEditOperatingMode.Owner = Window.GetWindow(this);
                        addEditOperatingMode.ShowDialog();

                        //ListOperatingModes.SelectedItem = null;
                    }
                    else
                    {
                        AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(operatingModeShow.idSightOperatingMode, false, this);
                        addEditOperatingMode.Owner = Window.GetWindow(this);
                        addEditOperatingMode.ShowDialog();

                        //ListOperatingModes.SelectedItem = null;
                    }

                    
                }
            }
        }

        private void AddSpecialDay(object sender, RoutedEventArgs e)
        {
            AddEditOperatingMode addEditOperatingMode = new AddEditOperatingMode(null, true, this);
            var parentWindow = Window.GetWindow(this);
            addEditOperatingMode.ShowDialog();
        }
    }
}
