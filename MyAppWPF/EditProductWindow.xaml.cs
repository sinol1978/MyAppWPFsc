using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        private Product curProduct;

        public Product CurProduct
        {
            get { return curProduct; }
            set { curProduct = value; }
        }
        public EditProductWindow()
        {
            InitializeComponent();
        }
        public EditProductWindow(Product currentProduct)
        {
            InitializeComponent();
            CurProduct = currentProduct;
            DataContext = CurProduct;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(this.DialogResult == true)
            {
                using(Model1 _entities = new Model1())
                {
                    try
                    {
                        Product editProduct = _entities.Products.Find(CurProduct.Id);
                        editProduct.Name = txtName.Text;
                        editProduct.PriceEnter = txtPrice.Text;
                        editProduct.Descr = txtDescr.Text;
                        editProduct.Unit = txtUnit.Text;
                        _entities.Entry(editProduct).State = System.Data.Entity.EntityState.Modified;
                        _entities.SaveChanges();
                    }
                    catch
                    {
                        MessageBox.Show("Не удалось отредактировать товар.", "Редактирование товара", MessageBoxButton.OK, MessageBoxImage.Hand);

                    }
                }
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"^([\d,]+)$");
            if (!reg.IsMatch(e.Text))
                e.Handled = true;
        }

        #region ButtonCloseEvents
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

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Red;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Wheat;
        }
        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
