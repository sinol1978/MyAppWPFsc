using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
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
using System.Windows.Shapes;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для NewOrderWindow.xaml
    /// </summary>
    public partial class NewOrderWindow : Window
    {
        private Model1 _entities;
        private Product addedProduct = new Product();
        private ObservableCollection<OrderLine> olines;
        private Order order;
        private User curUser;
        private double sumS;
        private List<Product> clientProducts;
        public NewOrderWindow()
        {
            InitializeComponent();
        }
        public NewOrderWindow(User user)
        {
            InitializeComponent();
            this.curUser = user;
            this._entities = new Model1();
            this.dPicker.SelectedDate = DateTime.Now.Date;
            olines = new ObservableCollection<OrderLine>();
            this.dgridOrderlines.ItemsSource = olines;
            this.cboxClients.DisplayMemberPath = "Name";
            this.cboxClients.SelectedValuePath = "Id";
            this.cboxClients.SelectedIndex = 0;
            this.cboxClients.ItemsSource = _entities.Clients.OrderBy(c => c.Name).ToList();
            //this.lblManager.Content = "Менеджер: " + curUser.Name + ", Контактный телефон: " + curUser.Phone1;
            this.txtPayment.Text = "0,00";
            this.order = new Order();
            this.Add.IsEnabled = false;
            cboxProducts.Text = "Выберите продукт из списка";
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (olines.Count > 0)
            {
                this.AddOrder();
                this.Close();
            }
            else MessageBox.Show("Нельзя добавить пустой заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void AddOrder()
        {
            this.order.ClientId = (int)this.cboxClients.SelectedValue;
            this.order.OrderDate = this.dPicker.SelectedDate.Value;
            foreach (OrderLine ol in olines)
            {
                OrderLine tempol = new OrderLine();
                tempol.ProductId = ol.ProductId;
                tempol.Quantity = ol.Quantity;
                tempol.Price = ol.Price;
                tempol.Total = (Convert.ToDouble(tempol.Quantity) * Convert.ToDouble(tempol.Price)).ToString();
                sumS += Convert.ToDouble(tempol.Total);
                sumS = Math.Round(sumS, 2);
                this.order.OrderLines.Add(tempol);
            }
            this.order.TotalS = String.Format("{0:0.00}", sumS);
            this.order.ManagerId = curUser.Id;
            this.order.PaymentS = txtPayment.Text;
            order.BalanceS = String.Format("{0:0.00}", (Math.Round(Convert.ToDouble(order.TotalS) - Convert.ToDouble(order.PaymentS), 2)));
            _entities.Orders.Add(order);
            _entities.SaveChanges();
            double balS = 0;
            foreach (Order o in _entities.Orders.Where(o => o.ClientId == order.ClientId))
            {
                balS += Convert.ToDouble(o.BalanceS);
                balS = Math.Round(balS, 2);
            }
            _entities.Clients.Find(order.ClientId).DebtS = String.Format("{0:0.00}", balS);

            _entities.SaveChanges();
            Order orderForSaving = _entities.Orders.FirstOrDefault(x => x.Id == order.Id);
            FileSaver.SaveExcelFile(orderForSaving);
            //FileSaver.SaveExcelFile(order);
            MessageBox.Show("Заказ добавлен и сохранен в файл", "Добавление заказа", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddOrderLine()
        {
            this.cboxClients.IsEnabled = false;
            this.dPicker.IsEnabled = false;
            if (this.txtQuantity.Text != String.Empty && this.txtPrice.Text != String.Empty)
            {
                OrderLine ol = new OrderLine();
                ol.ProductId = addedProduct.Id;
                ol.Products = addedProduct;
                ol.Quantity = this.txtQuantity.Text;
                ol.Price = String.Format("{0:0.000}", Convert.ToDouble(this.txtPrice.Text)).ToString();
                ol.Total = String.Format("{0:0.00}",(Convert.ToDouble(ol.Quantity) * Convert.ToDouble(ol.Price))).ToString();
                ol.Orders = this.order;
                try
                {
                    if (olines.Where(o => o.Products.Name == ol.Products.Name).Count() == 0)
                    {
                        olines.Add(ol);
                        dgridOrderlines.ScrollIntoView(ol);//прокрутка скроллинга на добавляемую строку
                    }
                    else if (olines.Where(o => o.Products.Name == ol.Products.Name).Count() > 0)
                    {
                        MessageBox.Show("Товар уже добавлен в список.\nДля редактирования строки заказа,\nкликните два раза по ней.", "Добавление товара", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch
                {
                    return;
                }
                double sumS = 0;
                if (olines.Count() > 0)
                {
                    sumS = olines.Sum(o => Convert.ToDouble(o.Total));
                }
                sumS = Math.Round(sumS, 2);
                lblTotal.Content = String.Format("Итого: {0:0.00}", sumS);
                cboxProducts.SelectedIndex = -1;
                txtQuantity.Text = String.Empty;
                txtPrice.Text = String.Empty;
                cboxProducts.Focus();
            }
            else
            {
                MessageBox.Show("Поля 'Количество' и 'Цена'\nдолжны содержать значения", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddOrderLine();
        }
        private void cboxProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                addedProduct = (Product)e.AddedItems[0];
                txtPrice.Text = addedProduct.PriceEnter;
                lblUnit.Content = addedProduct.Unit;
                Add.IsEnabled = true;
            }
            catch
            {
                this.Add.IsEnabled = false;
                return;
            }
            this.txtQuantity.Clear();
            this.txtQuantity.Focus();
        }

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"^([\d,]+)$");
            if (!reg.IsMatch(e.Text)) e.Handled = true;
        }

        private void dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.lblDate.Content = "Заказ от " + this.dPicker.SelectedDate.Value.ToLongDateString();
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
                        var sumS = olines.Sum(o => Convert.ToDouble(o.Total));
                        sumS = Math.Round(sumS, 2);
                        lblTotal.Content = String.Format("Итого: {0:0.00}", sumS.ToString());
                }
            }
            catch
            {
                MessageBox.Show("Не удалось отредактировать строку заказа", "Ошибка формирования заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить строку?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (this.olines != null)
                {
                    olines.Remove(this.dgridOrderlines.SelectedItem as OrderLine);
                }
                var sumS = olines.Sum(o => Convert.ToDouble(o.Total));
                sumS = Math.Round(sumS, 2);
                lblTotal.Content = String.Format("Итого: {0:0.00}", sumS);
            }
            else return;
        }

        private void cboxClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Client cl = cboxClients.SelectedItem as Client;
                //this.lblClient.Content = "Клиент: " + cl.Name;
                clientProducts = new List<Product>();
                foreach (Product prod in _entities.Products.Where(p => p.ClientId == cl.Id).OrderBy(c => c.Name))
                {
                    clientProducts.Add(prod);
                }
                this.cboxProducts.DisplayMemberPath = "Name";
                this.cboxProducts.SelectedValuePath = "Id";
                this.cboxProducts.ItemsSource = clientProducts;


            }
            catch
            {
                //this.lblClient.Content = String.Empty;
                return;
            }
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

        private void dgridOrderlines_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key.Equals(Key.Enter))
            {
                AddOrderLine();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}