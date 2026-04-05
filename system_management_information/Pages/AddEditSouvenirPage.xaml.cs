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

namespace system_management_information.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditSouvenirPage.xaml
    /// </summary>
    public partial class AddEditSouvenirPage : Page
    {
        public VisitCenterContext context { get; set; }
        public bool isAdd { get; set; }
        private Action _onSouvenirSaved;
        public Souvenir souvenir { get; set; }
        public AddEditSouvenirPage(int? idSouvenir, Action onSouvenirSaved = null)
        {
            InitializeComponent();
            _onSouvenirSaved = onSouvenirSaved;
            context = new VisitCenterContext();
            this.InvalidateVisual();
            isAdd = true;

            if (idSouvenir != null)
            {
                isAdd = false;
                titlePage.Text = "Редактирование сувенира";
                souvenir = context.Souvenirs.Find(idSouvenir);

                LoadSouvenir();
            }
            else
            {
                isAdd = true;
                titlePage.Text = "Добавление сувенира";
                souvenir = new Souvenir();
                btnDelete.Visibility = Visibility.Collapsed;
            }
            DataContext = this;
        }

        public void LoadSouvenir()
        {
            product.Text = souvenir.Product;
            nameSouvenir.Text = souvenir.NameSouvenir;
            
            if (!string.IsNullOrEmpty(souvenir.Weight))
                weight.Text = souvenir.Weight;
            if (!string.IsNullOrEmpty(souvenir.Tastes))
                tastes.Text = souvenir.Tastes;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveSouvenir(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(product.Text))
                souvenir.Product = product.Text;
            else
            {
                MessageBox.Show("Введите продукт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(nameSouvenir.Text))
                souvenir.NameSouvenir = nameSouvenir.Text;
            else
            {
                MessageBox.Show("Введите название сувенира!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(weight.Text))
                souvenir.Weight = null;
            else if (weight.Text.Any(char.IsDigit))
                souvenir.Weight = weight.Text;
            else
            {
                MessageBox.Show("Проверьте вес! Он должно содержать цифры!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            souvenir.Tastes = tastes.Text;

            if (isAdd)
                context.Souvenirs.Add(souvenir);
            context.SaveChanges();

            _onSouvenirSaved?.Invoke();
            NavigationService.GoBack();
        }

        private void DeleteSouvenir(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить сувенир из базы данных?", "Подтверждение!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                context.Souvenirs.Remove(souvenir);
                context.SaveChanges();
                _onSouvenirSaved?.Invoke();
                NavigationService.GoBack();
            }
        }
    }
}
