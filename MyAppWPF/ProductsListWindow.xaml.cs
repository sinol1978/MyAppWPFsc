using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для ProductsListWindow.xaml
    /// </summary>
    public partial class ProductsListWindow : Window
    {
        private Model1 _entities;
        private ObservableCollection<Product> prods;
        private Client Client { get; set; }
        private StringBuilder body;
        public ProductsListWindow()
        {
            InitializeComponent();
            _entities = new Model1();
            cmbClients.ItemsSource = _entities.Clients.OrderBy(c=>c.Name).ToList();
            cmbClients.DisplayMemberPath = "Name";
            cmbClients.SelectedValuePath = "Id";
            cmbClients.SelectedIndex = 0;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgridProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Product currentProduct = (Product)dgridProducts.SelectedItem;
                EditProductWindow editProductForm = new EditProductWindow(currentProduct);
                editProductForm.ShowDialog();
                if (editProductForm.DialogResult == true)
                {
                    MessageBox.Show("Товар успешно отредактирован.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    //поставить действие на неизменение товара
                }

            }
            catch
            {
                return;
            }
        }

        private void btnAddNewProd_Click(object sender, RoutedEventArgs e)
        {
            NewProductWindow np = new NewProductWindow(Client);
            Hide();
            np.ShowDialog();
            ShowDialog();
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

        private void dgridProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void cmbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            prods = new ObservableCollection<Product>();
            Client = cmbClients.SelectedItem as Client;
            foreach (Product p in _entities.Products.Where(c=>c.ClientId == Client.Id).OrderBy(o => o.Name))
            {
                prods.Add(p);
            }
            dgridProducts.ItemsSource = prods.OrderBy(p => p.Name).ToList();
        }

        private void btnCreatePrice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileSaver.SaveExcelFile(Client);
            }
            catch
            {
                MessageBox.Show("Во время сохранения возникла ошибка.", "Сохранение прайслиста", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveToFile(StringBuilder body)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.txt)|*.txt";
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.OpenFile(), System.Text.Encoding.Default))
                    {
                        sw.Write(body);
                        sw.Close();
                        MessageBox.Show("Прайслист успешно сохранен в файл\n" + sfd.FileName, "Сохранение прайслиста", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch
                {
                    MessageBox.Show("Во время сохранения возникла ошибка.", "Сохранение прайслиста", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

 //       private void Window_MouseDown(object sender, MouseButtonEventArgs e)
 //       {
 //           this.DragMove();
 //       }

        private void btnCreateAllPrices_Click(object sender, RoutedEventArgs e)
        {
            var clients = _entities.Clients.ToList();
            clients.ForEach(FileSaver.SaveExcelFile);
        }
    }
}
