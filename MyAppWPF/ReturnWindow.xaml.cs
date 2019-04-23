using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Interaction logic for ReturnWindow.xaml
    /// </summary>
    public partial class ReturnWindow : Window
    {
        private Model1 _entities;
        public User User { get; set; }
        public Product Product { get; set; }
        public ObservableCollection<OrderLine> Olines { get; set; }
        public Order Order { get; set; }
        private Client Client { get; set; }


        public ReturnWindow()
        {
            InitializeComponent();
        }
        public ReturnWindow(User user)
        {
            InitializeComponent();
            User = user;
            _entities = new Model1();
            Olines = new ObservableCollection<OrderLine>();
            btnAdd.IsEnabled = false;
            Product = new Product();
            Client = new Client();

            cboxProducts.DisplayMemberPath = "Name";
            cboxProducts.SelectedValuePath = "Id";

            dgridOrderlines.ItemsSource = Olines;

            cboxClients.ItemsSource = _entities.Clients.OrderBy(c => c.Name).ToList();
            cboxClients.DisplayMemberPath = "Name";
            cboxClients.SelectedValuePath = "Id";
            cboxClients.SelectedIndex = 0;

            this.dtPicker.SelectedDate = DateTime.Now.Date;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Olines.Count > 0)
                {
                    AddOrder();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Не удалось оформить возврат", "Возврат", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void AddOrder()
        {
            Order = new Order();
            double SumS = 0;
            Order.ClientId = (int)cboxClients.SelectedValue;
            Order.OrderDate = dtPicker.SelectedDate.Value;
            foreach (OrderLine ol in Olines)
            {
                OrderLine tempol = new OrderLine();
                tempol.ProductId = ol.ProductId;
                tempol.Quantity = Convert.ToDouble(ol.Quantity).ToString();
                tempol.Price = String.Format("{0:0.000}", ol.Price);
                tempol.Total = (Convert.ToDouble(tempol.Quantity) * Convert.ToDouble(tempol.Price)).ToString();
                SumS += Convert.ToDouble(tempol.Total);
                Order.OrderLines.Add(tempol);
            }
            SumS *= -1;
            Order.TotalS = String.Format("{0:0.00}", SumS);
            Order.ManagerId = User.Id;
            Order.PaymentS = "0,00";
            Order.BalanceS = Order.TotalS;
            _entities.Orders.Add(Order);
            _entities.SaveChanges();

            double balS = 0;
            foreach (Order o in _entities.Orders.Where(c=>c.ClientId == Order.ClientId))
            {
                //if (o.ClientId == Order.ClientId)
                //{
                    balS += Convert.ToDouble(o.BalanceS);
                //}
            }

            _entities.Clients.Find(Order.ClientId).DebtS = String.Format("{0:0.00}", balS);
            _entities.SaveChanges();
            MessageBox.Show("Возврат оформлен", "Возврат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgridOrderlines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OrderLine ol = (OrderLine)dgridOrderlines.SelectedItem;
                EditOrderWindow eow = new EditOrderWindow(ol);
                Effect = new System.Windows.Media.Effects.BlurEffect();
                eow.ShowDialog();
                Effect = null;
                if (eow.DialogResult == true)
                {
                        var sumS = Olines.Sum(o => Convert.ToDouble(o.Total));
                        lblTotalS.Content = String.Format("Итого: {0:0.00}", sumS);
                }
            }
            catch { return; }
        }

        private void Border_MouseMove_1(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(2, 2, 2, 2);
        }

        private void Border_MouseLeave_1(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddOrderLine();
        }

        private void btnAdd_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.LightGray;
        }

        private void btnAdd_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.Wheat;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить строку?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (Olines != null && Olines.Count > 0)
                {
                    Olines.Remove(this.dgridOrderlines.SelectedItem as OrderLine);
                }
                var sumS = Olines.Sum(o => Convert.ToDouble(o.Total));
                lblTotalS.Content = String.Format("Итого: {0:0.00}", sumS);
            }
            else return;
        }

        private void cboxProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Product = (Product)e.AddedItems[0];
                txtPrice.Text = Product.PriceEnter;
                lblUnit.Content = Product.Unit;
                btnAdd.IsEnabled = true;
            }
            catch
            {
                btnAdd.IsEnabled = false;
                return;
            }
            txtQuantity.Clear();
            txtQuantity.Focus();
        }

        private void dgridOrderlines_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void cboxClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Client = cboxClients.SelectedItem as Client;
                cboxProducts.ItemsSource = _entities.Products.Where(c=>c.ClientId == Client.Id).OrderBy(p => p.Name).ToList();
            }
            catch
            {
                return;
            }
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AddOrderLine();
            }
        }

        private void AddOrderLine()
        {
            cboxClients.IsEnabled = false;
            dtPicker.IsEnabled = false;

            if (txtQuantity.Text != String.Empty && txtPrice.Text != String.Empty)
            {
                OrderLine ol = new OrderLine();
                ol.ProductId = Product.Id;
                ol.Products = Product;
                ol.Quantity = (Convert.ToDouble(txtQuantity.Text)).ToString();
                ol.Price = txtPrice.Text;
                ol.Total = String.Format("{0:0.00}",(Convert.ToDouble(ol.Quantity) * Convert.ToDouble(ol.Price)));
                ol.Orders = Order;


                if (Olines.Where(o => o.Products.Name == ol.Products.Name).Count() == 0)
                {
                    Olines.Add(ol);
                }
                else if (Olines.Where(o => o.Products.Name == ol.Products.Name).Count() > 0)
                {
                    MessageBox.Show("Товар уже добавлен в список.\nДля редактирования строки заказа,\nкликните два раза по ней.", "Добавление товара", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                double sumS = Olines.Sum(o => Convert.ToDouble(o.Total)) * (-1);
                lblTotalS.Content = String.Format("Итого: {0:0.00}", sumS);
                cboxProducts.SelectedIndex = -1;
                txtQuantity.Text = String.Empty;
                txtPrice.Text = String.Empty;
            }
            else
            {
                MessageBox.Show("Поля 'Количество' и 'Цена'\nдолжны содержать значения", "Ошибка формирования возврата", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
