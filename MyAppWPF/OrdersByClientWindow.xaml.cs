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
    /// Логика взаимодействия для OrdersByClientWindow.xaml
    /// </summary>
    public partial class OrdersByClientWindow : Window
    {
        private Model1 _entities;
        private ObservableCollection<Order> orders = new ObservableCollection<Order>();

        public OrdersByClientWindow()
        {
            InitializeComponent();
            _entities = new Model1();
            cboxClients.DisplayMemberPath = "Name";
            cboxClients.SelectedValuePath = "Id";
            cboxClients.ItemsSource = _entities.Clients.OrderBy(c=>c.Name).ToList();

        }

        private void cboxClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgressBar pBar = new ProgressBar();
            pBar.IsIndeterminate = true;
            spanPrBar.Children.Add(pBar);
            orders.Clear();
            lblAllTotal.Content = String.Empty;
            lblAllDebt.Content = String.Empty;
            foreach (Client curClient in e.AddedItems)
            {
                if(curClient != null)
                {
                    var ord = _entities.Orders.Where(o => o.ClientId == curClient.Id).ToList<Order>();
                    foreach (Order o in ord)
                    {
                        orders.Add(o);
                    }
                    if (orders.Count() > 0)
                    {
                        
                        dgridOrders.ItemsSource = orders.OrderByDescending(o=>o.OrderDate).ToList();
                        dgridOrders.SelectedIndex = 0;
                        Order curOrder = (Order)dgridOrders.SelectedItem;

                        var olines = _entities.Orders.Find(curOrder.Id).OrderLines.ToList<OrderLine>().OrderBy(o=>o.Products.Name);
                        dgrodOlines.ItemsSource = olines;
                        string total = orders.Sum(o => Convert.ToDouble(o.TotalS)).ToString("F");
                        string debt = orders.Sum(o => Convert.ToDouble(o.BalanceS)).ToString("F");
                        lblAllTotal.Content = String.Format("Сумма всех заказов: {0}", total);
                        lblAllDebt.Content = String.Format("Общий долг: {0}", debt);
                    }
                    else
                    {
                        dgridOrders.ItemsSource = null;
                        dgrodOlines.ItemsSource = null;
                    }
                }
            }
            spanPrBar.Children.Remove(pBar);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgridOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Order or in e.AddedItems)
            {
                if(or != null)
                {
                    var olines = _entities.Orders.Find(or.Id).OrderLines.ToList<OrderLine>().OrderBy(o=>o.Products.Name);
                    this.dgrodOlines.ItemsSource = olines;
                }
            }
        }

        private void dgridOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dgr = e.Row as DataGridRow;
            Order order = (Order)e.Row.DataContext;
            if (Convert.ToDouble(order.TotalS)!=Convert.ToDouble(order.PaymentS))
            {
                dgr.Foreground = Brushes.Red;
                dgr.FontWeight = FontWeights.Bold;
            }
            e.Row.Header = e.Row.GetIndex() + 1;
            dgr.Background = Brushes.Wheat;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgridOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Order order = (Order)dgridOrders.SelectedItem;
                PrintDelOrder pd = new PrintDelOrder(order);
                Hide();
                pd.ShowDialog();
                ShowDialog();
            }
            catch { return; }
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

        private void dgrodOlines_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            e.Row.Background = Brushes.Wheat;
        }

        private void dgrodOlines_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            return;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
