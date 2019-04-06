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
    /// Логика взаимодействия для AllOrdersList.xaml
    /// </summary>
    public partial class AllOrdersList : Window
    {
        private Model1 _entities;
        public ObservableCollection<Order> orders;
        public AllOrdersList()
        {
            InitializeComponent();
            _entities = new Model1();
            orders = new ObservableCollection<Order>();
            try
            {
                GetOrders();
                dgrOrders.ItemsSource = orders;
                string total = orders.Sum(o => Convert.ToDouble(o.TotalS)).ToString();
                string pay = orders.Sum(o => Convert.ToDouble(o.PaymentS)).ToString();
                string debt = orders.Sum(o => Convert.ToDouble(o.BalanceS)).ToString();
                lblAllTotal.Content = String.Format("Сумма всех заказов: {0:0.00}", Convert.ToDouble(total));
                lblAllPay.Content = String.Format("Сумма всех оплат: {0:0.00}", Convert.ToDouble(pay));
                lblAllDebt.Content = String.Format("Сумма долгов: {0:0.00}", Convert.ToDouble(debt));
            }
            catch
            {
                MessageBox.Show("Ошибка. Окно будет закрыто.", "Список заказов", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        private void GetOrders()
        {
            var sortOrders = _entities.Orders.OrderByDescending(o => o.OrderDate).ToList();
            if (_entities.Orders != null && _entities.Orders.Count() > 0)
            {
                foreach(Order o in sortOrders)
                {
                    orders.Add(o);
                }
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void dgrOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            e.Row.Background = Brushes.Wheat;
        }
        private void dgrOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Order order = (Order)dgrOrders.SelectedItem;
                PrintDelOrder pd = new PrintDelOrder(order);
                pd.Closing += delegate (object fsender, System.ComponentModel.CancelEventArgs fe)
                {
                    using (Model1 _entities = new Model1())
                    {
                        try
                        {
                            Order editOrder = _entities.Orders.Find(order.Id);
                            editOrder.PaymentS = order.PaymentS;
                            editOrder.BalanceS = (Convert.ToDouble(order.TotalS) - Convert.ToDouble(order.PaymentS)).ToString();
                            _entities.Entry(editOrder).State = System.Data.Entity.EntityState.Modified;
                            _entities.SaveChanges();
                        }
                        catch (Exception ex) { }
                    }
                };
                Hide();
                pd.ShowDialog();
                ShowDialog();
            }
            catch { return; }
        }
        private void dgrOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Order order = (Order)dgrOrders.SelectedItem;
                string content1 = "Заказ оплачен";
                string content2 = String.Format("Долг по заказу: {0:0.00}", Convert.ToDouble(order.BalanceS));
                if (Convert.ToDouble(order.BalanceS) != 0)
                {
                    lblInfo.Content = content2;
                }
                else
                {
                    lblInfo.Content = content1;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка. Окно будет закрыто.", "Список заказов", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
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
    }
}
