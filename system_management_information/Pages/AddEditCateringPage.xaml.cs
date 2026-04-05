using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static system_management_information.Pages.AddEditCateringPage;
using static system_management_information.Pages.AddEditSightPage;

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditCateringPage.xaml
    /// </summary>
    public partial class AddEditCateringPage : Page
    {
        public class OperatingModeShow
        {
            public int idOperatingMode { get; set; }
            public string time { get; set; }
            public string day { get; set; }
            public bool isSpecialDay { get; set; }
        }
        public class TypeKitchenShow
        {
            public int idType { get; set; }
            public string type { get; set; }
            public bool isHave {  get; set; }
        }
        public VisitCenterContext context { get; set; }
        public bool isAdd { get; set; }
        private Action _onCateringSaved;
        public Catering catering { get; set; }
        public List<TypeKitchen> typesKitchen { get; set; }
        public List<TypeKitchenShow> listTypesKitchen { get; set; }
        public List<CateringModeOperationCatering> operatingModes { get; set; }
        public static List<TypeKitchenShow> addTypesKitchen { get; set; } = new List<TypeKitchenShow>();
        public static List<TypeKitchenShow> deleteTypesKitchen { get; set; } = new List<TypeKitchenShow>();
        public static List<CateringModeOperationCatering> editOperatingModes { get; set; } = new List<CateringModeOperationCatering>();
        public static List<CateringModeOperationCatering> deleteOperatingModes { get; set; } = new List<CateringModeOperationCatering>();
        public static List<CateringModeOperationCatering> addOperatingModes { get; set; } = new List<CateringModeOperationCatering>();
        public List<OperatingModeShow> listOperatingModes { get; set; }
        public AddEditCateringPage(int? idCatering, Action onCateringSaved = null)
        {
            InitializeComponent();
            _onCateringSaved = onCateringSaved;
            context = new VisitCenterContext();
            this.InvalidateVisual();
            isAdd = true;
            editOperatingModes.Clear();
            deleteOperatingModes.Clear();
            addOperatingModes.Clear();
            addTypesKitchen.Clear();
            deleteTypesKitchen.Clear();

            typesKitchen = new List<TypeKitchen>();
            listTypesKitchen = new List<TypeKitchenShow>();
            typesKitchen = context.TypeKitchens.ToList();

            otherView.Visibility = Visibility.Collapsed;
            var categories = context.Caterings.Select(c => c.EstablishmentCategory).ToList();
            foreach (var category in categories)
            {
                if(!categoryCatering.Items.Contains(category))
                    categoryCatering.Items.Add(category);
            }
            categoryCatering.Items.Add("Другое");

            operatingModes = new List<CateringModeOperationCatering>();
            listOperatingModes = new List<OperatingModeShow>();

            if (idCatering != null)
            {
                isAdd = false;
                titlePage.Text = "Редактирование общепита";
                catering = context.Caterings.Find(idCatering);
                operatingModes = context.CateringModeOperationCaterings
                    .Include(c => c.IdModeOperationCateringNavigation)
                    .Include(c => c.IdSpecialDayCateringNavigation)
                    .Where(c => c.IdCatering == idCatering)
                    .ToList();
                LoadCatering();
            }
            else
            {
                isAdd = true;
                titlePage.Text = "Добавление общепита";
                catering = new Catering();
                operatingModes = new List<CateringModeOperationCatering>();
                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
        }
        public void LoadCatering()
        {
            foreach(var type in typesKitchen)
            {
                var typeShow = new TypeKitchenShow();
                typeShow.idType = type.IdTypeKitchen;
                typeShow.type = type.NameTypeKitchen;
                var cateringTypesKitchen = context.CateringTypeKitchens.Where(t => t.IdCatering == catering.IdCatering && t.IdTypeKitchen == type.IdTypeKitchen).ToList();
                if (cateringTypesKitchen.Count == 0)
                    typeShow.isHave = false;
                else typeShow.isHave = true;
                listTypesKitchen.Add(typeShow);
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
                    var modeEdit = context.CateringModeOperationCaterings.Find(mode.IdCateringModeOperationCatering);
                    modeEdit.IdModeOperationCatering = mode.IdModeOperationCatering;
                    modeEdit.IdSpecialDayCatering = mode.IdSpecialDayCatering;
                    modeEdit.DayWeek = mode.DayWeek;
                }
            }
            categoryCatering.SelectedItem = catering.EstablishmentCategory;
            nameCatering.Text = catering.EstablishmentName;
            streetCatering.Text = catering.EstablishmentStreet;
            houseCatering.Text = catering.EstablishmentHouse;
            if (!string.IsNullOrEmpty(catering.EstablishmentPhone))
                contactNumberCatering.Text = catering.EstablishmentPhone;
            if (!string.IsNullOrEmpty(catering.CateringUrl))
                urlCatering.Text = catering.CateringUrl;
            operatingModes = operatingModes.OrderBy(s => s.DayWeek).ToList();
            foreach (var operatingMode in operatingModes)
            {
                var operatingModeShow = new OperatingModeShow();
                operatingModeShow.idOperatingMode = operatingMode.IdCateringModeOperationCatering;
                if (operatingMode.IdModeOperationCatering != null)
                    operatingModeShow.time = $"{operatingMode.IdModeOperationCateringNavigation?.Beginning.ToShortTimeString()} - {operatingMode.IdModeOperationCateringNavigation?.EndDay.ToShortTimeString()}";
                else if (operatingMode.IdSpecialDayCatering != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDayCatering != null)
                {
                    operatingModeShow.isSpecialDay = true;
                    operatingModeShow.day = $"{operatingMode.IdSpecialDayCateringNavigation?.Date.ToString()}";
                    if (!string.IsNullOrEmpty(operatingMode.IdSpecialDayCateringNavigation?.StatusDay))
                        operatingModeShow.day += $"    ({operatingMode.IdSpecialDayCateringNavigation?.StatusDay})";
                }
                else
                {
                    operatingModeShow.isSpecialDay = false;
                    switch (operatingMode.DayWeek)
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

        public void RefreshTypesKitchen()
        {
            listTypesKitchen.Clear();

            foreach (var type in typesKitchen)
            {
                var typeShow = new TypeKitchenShow();
                typeShow.idType = type.IdTypeKitchen;
                typeShow.type = type.NameTypeKitchen;
                var cateringTypesKitchen = context.CateringTypeKitchens.Where(t => t.IdCatering == catering.IdCatering && t.IdTypeKitchen == type.IdTypeKitchen).ToList();
                if (cateringTypesKitchen.Count == 0)
                    typeShow.isHave = false;
                else typeShow.isHave = true;
                listTypesKitchen.Add(typeShow);
            }
            foreach (var add in addTypesKitchen)
            {
                listTypesKitchen.Add(add);
            }
            foreach (var del in deleteTypesKitchen)
            {
                listTypesKitchen.Remove(del);
            }

            ListTypesKitchen.ItemsSource = null;
            ListTypesKitchen.ItemsSource = listTypesKitchen;
        }

        public void RefreshOperatingMode()
        {
            listOperatingModes.Clear();

            foreach (var operatingMode in operatingModes)
            {
                if (operatingMode.IdModeOperationCatering != null && operatingMode.IdModeOperationCateringNavigation == null)
                {
                    context.Entry(operatingMode)
                        .Reference(m => m.IdModeOperationCateringNavigation)
                        .Load();
                }
                if (operatingMode.IdSpecialDayCatering != null && operatingMode.IdSpecialDayCateringNavigation == null)
                {
                    context.Entry(operatingMode)
                        .Reference(m => m.IdSpecialDayCateringNavigation)
                        .Load();
                }
                var operatingModeShow = new OperatingModeShow();
                operatingModeShow.idOperatingMode = operatingMode.IdCateringModeOperationCatering;
                if (operatingMode.IdModeOperationCateringNavigation != null)
                    operatingModeShow.time = $"{operatingMode.IdModeOperationCateringNavigation?.Beginning.ToShortTimeString()} - {operatingMode.IdModeOperationCateringNavigation?.EndDay.ToShortTimeString()}";
                else if (operatingMode.IdSpecialDayCateringNavigation != null)
                    operatingModeShow.time = "Закрыто";
                else operatingModeShow.time = "24 часа";

                if (operatingMode.IdSpecialDayCateringNavigation != null)
                {
                    operatingModeShow.day = $"{operatingMode.IdSpecialDayCateringNavigation?.Date.ToString()}";
                    if (!string.IsNullOrEmpty(operatingMode.IdSpecialDayCateringNavigation?.StatusDay))
                        operatingModeShow.day += $"    ({operatingMode.IdSpecialDayCateringNavigation?.StatusDay})";
                }
                else
                {
                    switch (operatingMode.DayWeek)
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
        private void DeleteCatering(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить место общепита из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var operatingModes = context.CateringModeOperationCaterings.Where(o => o.IdCatering == catering.IdCatering).ToList();
                foreach (var operatingMode in operatingModes)
                {
                    context.CateringModeOperationCaterings.Remove(operatingMode);
                    context.SaveChanges();
                }
                context.Caterings.Remove(catering);
                context.SaveChanges();
                _onCateringSaved?.Invoke();
                NavigationService.GoBack();
            }
        }

        private void SaveCatering(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(categoryCatering.SelectedItem.ToString()))
            {
                if(categoryCatering.SelectedItem.ToString() == "Другое")
                {
                    if (!string.IsNullOrEmpty(otherTypeCatering.Text))
                    {
                        catering.EstablishmentCategory = otherTypeCatering.Text;
                    }
                    else
                    {
                        MessageBox.Show("Введите новую категорию общепита или выберите из списка!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                    catering.EstablishmentCategory = categoryCatering.SelectedItem.ToString();
            }
            else
            {
                MessageBox.Show("Выберите категорию общепита!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(nameCatering.Text))
                catering.EstablishmentName = nameCatering.Text;
            else
            {
                MessageBox.Show("Введите название заведения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(streetCatering.Text))
                catering.EstablishmentStreet = streetCatering.Text;
            else
            {
                MessageBox.Show("Введите улицу расположения заведения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(houseCatering.Text))
                catering.EstablishmentHouse = houseCatering.Text;
            else
            {
                MessageBox.Show("Введите дом расположения заведения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string contactNumberString = contactNumberCatering.Text.Trim().Replace(" ", "").Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (string.IsNullOrEmpty(contactNumberCatering.Text))
                catering.EstablishmentPhone = null;
            else if (Int64.TryParse(contactNumberString, out Int64 result) && contactNumberString.Length >= 11)
                catering.EstablishmentPhone = contactNumberString;
            else
            {
                MessageBox.Show("Проверьте правильность ввода номера телефона! Он должен состоять не меньше чем из 11 цифр!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            catering.CateringUrl = urlCatering.Text;

            if (isAdd)
                context.Caterings.Add(catering);
            context.SaveChanges();

            foreach (var addOperatingMode in addOperatingModes)
            {
                addOperatingMode.IdCatering = catering.IdCatering;

                if (addOperatingMode.IdSpecialDayCateringNavigation != null)
                {
                    var existingSpecialDay = context.SpecialDayCaterings.FirstOrDefault(s => s.Date == addOperatingMode.IdSpecialDayCateringNavigation.Date
                    && s.StatusDay == addOperatingMode.IdSpecialDayCateringNavigation.StatusDay);

                    if (existingSpecialDay != null)
                    {
                        addOperatingMode.IdSpecialDayCatering = existingSpecialDay.IdSpecialDayCatering;
                        addOperatingMode.IdSpecialDayCateringNavigation = null;
                    }
                    else
                    {
                        context.SpecialDayCaterings.Add(addOperatingMode.IdSpecialDayCateringNavigation);
                        context.SaveChanges();
                        addOperatingMode.IdSpecialDayCatering = addOperatingMode.IdSpecialDayCateringNavigation.IdSpecialDayCatering;
                        addOperatingMode.IdSpecialDayCateringNavigation = null;
                    }
                }

                if (addOperatingMode.IdModeOperationCateringNavigation != null)
                {
                    var existingOperatingMode = context.ModeOperationCaterings.FirstOrDefault(s => s.Beginning == addOperatingMode.IdModeOperationCateringNavigation.Beginning
                    && s.EndDay == addOperatingMode.IdModeOperationCateringNavigation.EndDay);

                    if (existingOperatingMode != null)
                    {
                        addOperatingMode.IdModeOperationCatering = existingOperatingMode.IdModeOperationCatering;
                        addOperatingMode.IdModeOperationCateringNavigation = null;
                    }
                    else
                    {
                        context.ModeOperationCaterings.Add(addOperatingMode.IdModeOperationCateringNavigation);
                        context.SaveChanges();
                        addOperatingMode.IdModeOperationCatering = addOperatingMode.IdModeOperationCateringNavigation.IdModeOperationCatering;
                        addOperatingMode.IdModeOperationCateringNavigation = null;
                    }
                }
                context.CateringModeOperationCaterings.Add(addOperatingMode);
            }

            foreach (var editOperatingMode in editOperatingModes)
            {

                var existingMode = context.CateringModeOperationCaterings
                    .Include(s => s.IdModeOperationCateringNavigation)
                    .Include(s => s.IdSpecialDayCateringNavigation)
                    .FirstOrDefault(s => s.IdCateringModeOperationCatering == editOperatingMode.IdCateringModeOperationCatering);
                if (existingMode != null)
                {
                    existingMode.DayWeek = editOperatingMode.DayWeek;
                    if (editOperatingMode.IdSpecialDayCateringNavigation != null)
                    {
                        var existingSpecialDay = context.SpecialDayCaterings.FirstOrDefault(s => s.Date == editOperatingMode.IdSpecialDayCateringNavigation.Date
                    && s.StatusDay == editOperatingMode.IdSpecialDayCateringNavigation.StatusDay);

                        if (existingSpecialDay != null)
                        {
                            existingMode.IdSpecialDayCatering = existingSpecialDay.IdSpecialDayCatering;
                        }
                        else
                        {
                            var newSpecialDay = new SpecialDayCatering
                            {
                                Date = editOperatingMode.IdSpecialDayCateringNavigation.Date,
                                StatusDay = editOperatingMode.IdSpecialDayCateringNavigation.StatusDay
                            };
                            context.SpecialDayCaterings.Add(newSpecialDay);
                            context.SaveChanges();
                            existingMode.IdSpecialDayCatering = newSpecialDay.IdSpecialDayCatering;
                        }
                    }
                    else
                    {
                        existingMode.IdSpecialDayCatering = null;
                    }

                    if (editOperatingMode.IdModeOperationCateringNavigation != null)
                    {
                        var existingOperatingMode = context.ModeOperationCaterings.FirstOrDefault(s => s.Beginning == editOperatingMode.IdModeOperationCateringNavigation.Beginning
                    && s.EndDay == editOperatingMode.IdModeOperationCateringNavigation.EndDay);

                        if (existingOperatingMode != null)
                        {
                            existingMode.IdModeOperationCatering = existingOperatingMode.IdModeOperationCatering;
                        }
                        else
                        {
                            context.ModeOperationCaterings.Add(editOperatingMode.IdModeOperationCateringNavigation);
                            context.SaveChanges();
                            existingMode.IdModeOperationCatering = editOperatingMode.IdModeOperationCateringNavigation.IdModeOperationCatering;
                        }
                    }
                    else
                    {
                        existingMode.IdModeOperationCatering = null;
                    }

                }
            }

            foreach (var deleteOperatingMode in deleteOperatingModes)
            {
                var localOperatingMode = context.CateringModeOperationCaterings.Local.FirstOrDefault(t => t.IdCateringModeOperationCatering == deleteOperatingMode.IdCateringModeOperationCatering);

                if (localOperatingMode != null)
                    context.CateringModeOperationCaterings.Remove(localOperatingMode);
                else
                {
                    context.CateringModeOperationCaterings.Attach(deleteOperatingMode);
                    context.CateringModeOperationCaterings.Remove(deleteOperatingMode);
                }
            }

            foreach (var add in addTypesKitchen)
            {
                if (deleteTypesKitchen.Contains(add))
                {
                    addTypesKitchen.Remove(add);
                    deleteTypesKitchen.Remove(add);
                }
                else
                {
                    var type = new TypeKitchen();
                    type.NameTypeKitchen = add.type;
                    context.TypeKitchens.Add(type);
                }
            }
            foreach (var del in deleteTypesKitchen)
            {
                var type = context.TypeKitchens.Find(del.idType);
                if (type != null)
                {
                    var typesCaterings = context.CateringTypeKitchens.Where(t => t.IdTypeKitchen == type.IdTypeKitchen);
                    foreach(var t in typesCaterings)
                        context.CateringTypeKitchens.Remove(t);
                    context.SaveChanges();
                    context.TypeKitchens.Remove(type);
                }
            }
            foreach(var type in listTypesKitchen)
            {
                var typeKitchen = context.TypeKitchens.Where(t => t.NameTypeKitchen == type.type).FirstOrDefault();
                var cateringTypesKitchen = context.CateringTypeKitchens.Where(t => t.IdCatering == catering.IdCatering && t.IdTypeKitchen == type.idType).ToList();

                if (type.isHave)
                {
                    if( cateringTypesKitchen.Count == 0 && typeKitchen != null)
                    {
                        CateringTypeKitchen cateringTypeKitchen = new CateringTypeKitchen();
                        cateringTypeKitchen.IdTypeKitchen = typeKitchen.IdTypeKitchen;
                        cateringTypeKitchen.IdCatering = catering.IdCatering;
                        context.CateringTypeKitchens.Add(cateringTypeKitchen);
                    }
                }
                else
                {
                    if (cateringTypesKitchen.Count != 0 && typeKitchen != null)
                    {
                        foreach(var cateringType in cateringTypesKitchen)
                            context.CateringTypeKitchens.Remove(cateringType);
                    }
                }
            }

            context.SaveChanges();

            addOperatingModes.Clear();
            deleteOperatingModes.Clear();
            editOperatingModes.Clear();
            addTypesKitchen.Clear();
            deleteTypesKitchen.Clear();

            _onCateringSaved?.Invoke();
            NavigationService.GoBack();
        }
        private void AddDay(object sender, RoutedEventArgs e)
        {
            AddEditOperatingModeCatering addEditOperatingMode = new AddEditOperatingModeCatering(null, false, this);
            var parentWindow = Window.GetWindow(this);
            addEditOperatingMode.ShowDialog();
            RefreshOperatingMode();
        }

        private void EditOperatingMode(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                OperatingModeShow operatingModeShow = e.AddedItems[0] as OperatingModeShow;

                if (operatingModeShow != null)
                {
                    var operatingMode = context.CateringModeOperationCaterings.Find(operatingModeShow.idOperatingMode);
                    if (operatingMode != null)
                    {
                        bool isSpecialDay = operatingMode.IdSpecialDayCateringNavigation != null;
                        AddEditOperatingModeCatering addEditOperatingMode = new AddEditOperatingModeCatering(operatingModeShow.idOperatingMode, isSpecialDay, this);
                        addEditOperatingMode.Owner = Window.GetWindow(this);
                        addEditOperatingMode.ShowDialog();
                        RefreshOperatingMode();
                    }
                    else
                        MessageBox.Show("Вы не можете редактировать только что добавленный график! Сначала сохраните данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        private void AddSpecialDay(object sender, RoutedEventArgs e)
        {
            AddEditOperatingModeCatering addEditOperatingMode = new AddEditOperatingModeCatering(null, true, this);
            var parentWindow = Window.GetWindow(this);
            addEditOperatingMode.ShowDialog();
            RefreshOperatingMode();
        }

        private void SelectCategoryCatering(object sender, SelectionChangedEventArgs e)
        {
            if(categoryCatering.SelectedItem == "Другое")
            {
                otherView.Visibility = Visibility.Visible;
            }
            else otherView.Visibility = Visibility.Collapsed;
        }

        private void AddTypeKitchen(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(newTypeKitchen.Text))
            {
                if (!context.TypeKitchens.Select(t => t.NameTypeKitchen.ToLower()).Contains(newTypeKitchen.Text.ToLower()))
                {
                    listTypesKitchen.Clear();
                    var newType = new TypeKitchenShow();
                    newType.type = newTypeKitchen.Text;
                    newType.isHave = false;
                    addTypesKitchen.Add(newType);
                    RefreshTypesKitchen();
                    ListTypesKitchen.ItemsSource = null;
                    ListTypesKitchen.ItemsSource = listTypesKitchen;
                    newTypeKitchen.Text = "";
                }
                else
                {
                    MessageBox.Show("Такой тип кухни уже есть в списке!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Название нового типа кухни не может пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void DeleteTypeKitchen(object sender, RoutedEventArgs e)
        {
            if (ListTypesKitchen.SelectedItem != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить выбранный тип кухни из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var type = ListTypesKitchen.SelectedItem as TypeKitchenShow;
                    var delType = context.TypeKitchens.Find(type.idType);
                    if (addTypesKitchen.Contains(type))
                    {
                        addTypesKitchen.Remove(type);
                    }
                    else
                    {
                        deleteTypesKitchen.Add(type);
                        if (delType != null)
                            typesKitchen.Remove(delType);
                    }

                    listTypesKitchen.Clear();
                    RefreshTypesKitchen();
                    ListTypesKitchen.ItemsSource = null;
                    ListTypesKitchen.ItemsSource = listTypesKitchen;
                }
            }
            else MessageBox.Show("Вы не выбрали фото для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AddCateringTypeKitchen(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            TypeKitchenShow typeKitchen = checkBox?.DataContext as TypeKitchenShow;
            if (typeKitchen != null)
                typeKitchen.isHave = true;
        }

        private void DeleteCateringTypeKitchen(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            TypeKitchenShow typeKitchen = checkBox?.DataContext as TypeKitchenShow;
            if (typeKitchen != null)
                typeKitchen.isHave = false;
        }
    }
}
