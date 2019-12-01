using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для NewProductWindow.xaml
    /// </summary>
    public partial class NewProductWindow : Window
    {
        private Product newProduct;
        private Client Client { get; set; }
        private Model1 _entities;
        public NewProductWindow()
        {
            InitializeComponent();

        }

        public NewProductWindow(Client client)
        {
            InitializeComponent();
            Client = client;
            _entities = new Model1();

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == String.Empty)
            {
                MessageBox.Show("Название товара\nне может быть пустым", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.txtName.Focus();
            }
            else
            {
                using (Model1 _entities = new Model1())
                {
                    foreach (Product p in _entities.Products.Where(c=>c.ClientId == Client.Id))
                    {
                        if (p.Name == this.txtName.Text)
                        {
                            MessageBox.Show("Товар с таким названием\nуже существует", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                            this.txtName.Text = String.Empty;
                            this.txtPrice.Text = String.Empty;
                            this.txtDescr.Text = String.Empty;
                            txtUnit.Clear();
                            this.txtName.Focus();
                            return;
                        }
                    }
                    try
                    {
                        newProduct = new Product();
                        newProduct.Name = this.txtName.Text;
                        newProduct.PriceEnter = txtPrice.Text;
                        newProduct.Unit = txtUnit.Text;
                        newProduct.Descr = txtDescr.Text;
                        newProduct.ClientId = Client.Id;
                        _entities.Products.Add(newProduct);
                        _entities.SaveChanges();
                        MessageBox.Show("Товар с именем\n'" + newProduct.Name + "'\nуспешно помещен в базу данных.", "Добавление товара в базу данных", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        this.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось добавить товар в базу", "Добавление нового товара", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"^([\d,]+)$");
            if (!reg.IsMatch(e.Text)) e.Handled = true;
        }
        #region ButtonCloseEvents
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Red;
        }
        private void btnClose_MouseMove(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Foreground = Brushes.White;
        }
        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Foreground = Brushes.Black;
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Transparent;
        }
        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
