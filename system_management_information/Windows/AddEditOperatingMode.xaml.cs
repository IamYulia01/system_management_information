using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
using static system_management_information.Pages.AddEditSightPage;

namespace system_management_information.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditOperatingMode.xaml
    /// </summary>
    public partial class AddEditOperatingMode : Window
    {
        public VisitCenterContext context { get; set; }
        public SightOperatingMode operatingMode { get; set; }
        public bool isAdd { get; set; }

        public bool specialDay { get; set; }
        public AddEditSightPage pagePerent { get; set; }
        public AddEditOperatingMode(int? idOperatingMode, bool isSpecialDay, AddEditSightPage page)
        {
            InitializeComponent();
            context = new VisitCenterContext();
            pagePerent = page;
            specialDay = isSpecialDay;
            operatingMode = new SightOperatingMode();
            if (!specialDay)
            {
                dayWeek.Items.Add("Понедельник");
                dayWeek.Items.Add("Вторник");
                dayWeek.Items.Add("Среда");
                dayWeek.Items.Add("Четверг");
                dayWeek.Items.Add("Пятница");
                dayWeek.Items.Add("Суббота");
                dayWeek.Items.Add("Воскресенье");
                dayWeek.Items.Add("Не выбрано");
                statusDay.Visibility = Visibility.Collapsed;
                specialDayText.Visibility = Visibility.Collapsed;
            }
            else daysWeekGrid.Visibility = Visibility.Collapsed;
            
            if (idOperatingMode != null)
            {
                operatingMode = context.SightOperatingModes.Include(s => s.IdOperatingModeNavigation).Include(s => s.IdSpecialDaySightNavigation).Where(s => s.IdSightOperatingMode == idOperatingMode).FirstOrDefault();
                isAdd = false;
                titlePage.Text = "Редактирование графика дня";
                LoadOperatingMode();
            }
            else
            {
                operatingMode = new SightOperatingMode();
                isAdd = true;
                titlePage.Text = "Добавление графика дня";

                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
        }

        public void LoadOperatingMode()
        {
            if (operatingMode.IdOperatingModeNavigation != null)
            {
                startTime.Text = operatingMode.IdOperatingModeNavigation?.StartTime.ToShortTimeString();
                endTime.Text = operatingMode.IdOperatingModeNavigation?.EndTime.ToShortTimeString();
            }
            if (specialDay)
            {
                if (operatingMode.IdSpecialDaySightNavigation?.SpecialDayDate != null)
                {
                    dateSpecialDay.Text = operatingMode.IdSpecialDaySightNavigation.SpecialDayDate.ToString();
                }
                else
                {
                    dateSpecialDay.Text = "";
                }
            }
            else
            {
                switch (operatingMode.WorkingDayWeek)
                {
                    case 1:
                        {
                            dayWeek.SelectedItem = "Понедельник";
                            break;
                        }
                    case 2:
                        {
                            dayWeek.SelectedItem = "Вторник";
                            break;
                        }
                    case 3:
                        {
                            dayWeek.SelectedItem = "Среда";
                            break;
                        }
                    case 4:
                        {
                            dayWeek.SelectedItem = "Четверг";
                            break;
                        }
                    case 5:
                        {
                            dayWeek.SelectedItem = "Пятница";
                            break;
                        }
                    case 6:
                        {
                            dayWeek.SelectedItem = "Суббота";
                            break;
                        }
                    case 7:
                        {
                            dayWeek.SelectedItem = "Воскресенье";
                            break;
                        }
                    default:
                        {
                            dayWeek.SelectedItem = "Не выбрано";
                            break;
                        }
                }
            }
        }

        private void DeleteOperatingMode(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить график?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                deleteOperatingModes.Add(operatingMode);
                if (pagePerent != null)
                {
                    var operatingModeToRemove = pagePerent.operatingModes.FirstOrDefault(t => t.IdSightOperatingMode == operatingMode.IdSightOperatingMode);
                    if (operatingModeToRemove != null)
                    {
                        pagePerent.operatingModes.Remove(operatingModeToRemove);
                        
                    }
                    if(addOperatingModes.Contains(operatingMode))
                        addOperatingModes.Remove(operatingMode);
                    if(editOperatingModes.Contains(operatingMode))
                        editOperatingModes.Remove(operatingMode);
                    pagePerent.RefreshSight();
                }
                this.DialogResult = true;
                this.Close();
            }
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveOperatingMode(object sender, RoutedEventArgs e)
        {
            TimeOnly? newStartTime = new TimeOnly();
            TimeOnly? newEndTime = new TimeOnly();
            DateOnly? newSpecialDayDate = new DateOnly();
            string newStatusDay = "";
            if (!((string.IsNullOrEmpty(startTime.Text)
                || startTime.Text.Trim() == "00:00"
                || startTime.Text.Trim() == "0:00"
                || startTime.Text.Trim() == "__:__"
                || startTime.Text.Trim() == ":")
                && (string.IsNullOrEmpty(endTime.Text)
                || endTime.Text.Trim() == "00:00"
                || startTime.Text.Trim() == "0:00"
                || endTime.Text.Trim() == "__:__"
                || endTime.Text.Trim() == ":")))
            {
                if (TimeOnly.TryParse(startTime.Text, out TimeOnly timeStart))
                    newStartTime = timeStart;
                else
                {
                    MessageBox.Show("Проверьте время начала рабочего дня! Оно введено неправильно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (TimeOnly.TryParse(endTime.Text, out TimeOnly timeEnd))
                    newEndTime = timeEnd;
                else
                {
                    MessageBox.Show("Проверьте время конца рабочего дня! Оно введено неправильно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (dayWeek.SelectedItem != null && !specialDay)
            {
                switch (dayWeek.SelectedItem.ToString())
                {
                    case "Понедельник":
                        {
                            operatingMode.WorkingDayWeek = 1;
                            break;
                        }
                    case "Вторник":
                        {
                            operatingMode.WorkingDayWeek = 2;
                            break;
                        }
                    case "Среда":
                        {
                            operatingMode.WorkingDayWeek = 3;
                            break;
                        }
                    case "Четверг":
                        {
                            operatingMode.WorkingDayWeek = 4;
                            break;
                        }
                    case "Пятница":
                        {
                            operatingMode.WorkingDayWeek = 5;
                            break;
                        }
                    case "Суббота":
                        {
                            operatingMode.WorkingDayWeek = 6;
                            break;
                        }
                    case "Воскресенье":
                        {
                            operatingMode.WorkingDayWeek = 7;
                            break;
                        }
                    default:
                        {
                            operatingMode.WorkingDayWeek = null;
                            break;
                        }
                }
            }
            else
                operatingMode.WorkingDayWeek = null;
            if (specialDay)
            {
                if (DateOnly.TryParse(dateSpecialDay.Text, out DateOnly date) && date.Year >= 2025)
                    newSpecialDayDate = date;
                else
                {
                    MessageBox.Show("Проверьте дату! Дата введена неправильно (год проведения мероприятия должен быть не меньше 2025!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                operatingMode.IdSpecialDaySightNavigation = new SpecialDaySight
                {
                    SpecialDayDate = date,
                    SpecialDayStatus = status.Text
                };
                operatingMode.IdSpecialDaySight = null;
            }
            if (newEndTime != new TimeOnly(0, 0) && newStartTime != new TimeOnly(0, 0))
            {
                operatingMode.IdOperatingModeNavigation = new OperatingMode
                {
                    StartTime = newStartTime.Value,
                    EndTime = newEndTime.Value
                };

                operatingMode.IdOperatingMode = null;
            }
            else
            {
                operatingMode.IdOperatingModeNavigation = null;
                operatingMode.IdOperatingMode = null;

            }


            if (isAdd)
            {
                if (!addOperatingModes.Contains(operatingMode))
                    addOperatingModes.Add(operatingMode);
                if(pagePerent != null && !pagePerent.operatingModes.Contains(operatingMode))
                    pagePerent.operatingModes.Add(operatingMode);
            }
            else
            {
                if (pagePerent != null)
                {
                    var existingMode = pagePerent.operatingModes.FirstOrDefault(t => t.IdSightOperatingMode == operatingMode.IdSightOperatingMode);
                    if (existingMode != null)
                    {
                        existingMode.IdOperatingMode = null;
                        existingMode.IdSpecialDaySight = null;

                        existingMode.IdOperatingModeNavigation = operatingMode.IdOperatingModeNavigation;
                        existingMode.IdSpecialDaySightNavigation = operatingMode.IdSpecialDaySightNavigation;
                        existingMode.WorkingDayWeek = operatingMode.WorkingDayWeek;
                        if (!editOperatingModes.Contains(operatingMode))
                        {
                            editOperatingModes.Add(existingMode);
                        }
                    }

                }
            }
                MessageBox.Show("График Сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                if (pagePerent != null)
                {
                    pagePerent.RefreshSight();
                }
                else
                {
                    MessageBox.Show("Ошибка: ссылка на родительскую страницу потеряна!");
                }

                this.DialogResult = true;
                this.Close();
            }
    }
}
