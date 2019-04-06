using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для OrdersByDateWindow.xaml
    /// </summary>
    public partial class OrdersByDateWindow : Window
    {
        private Model1 _entities;
        private ObservableCollection<Order> orders = new ObservableCollection<Order>();
        public OrdersByDateWindow()
        {
            InitializeComponent();
            _entities = new Model1();
            List<DateTime> dates = new List<DateTime>();
            dPicker.SelectedDate = DateTime.Now;
            dPicker.Focus();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            orders.Clear();
            lblAllTotal.Content = String.Empty;
            lblAllDebt.Content = String.Empty;
            foreach (DateTime curDate in e.AddedItems)
            {
                if (curDate != null)
                {
                    var ord = _entities.Orders.Where(o => o.OrderDate == curDate);
                    foreach(Order o in ord)
                    {
                        orders.Add(o);
                    }
                    if(orders.Count() > 0)
                    {
                        this.dgridOrders.ItemsSource = orders.ToList<Order>();
                        this.dgridOrders.SelectedIndex = 0;
                        Order curOrder = (Order)dgridOrders.SelectedItem;
                        var olines = _entities.Orders.Find(curOrder.Id).OrderLines.ToList<OrderLine>();
                        this.dgrodOlines.ItemsSource = olines;
                        string total = orders.Sum(o => Convert.ToDouble(o.TotalS)).ToString();
                        string debt = orders.Sum(o => Convert.ToDouble(o.BalanceS)).ToString();
                        lblAllTotal.Content = String.Format("Сумма заказов: {0:0.00}", Convert.ToDouble(total));
                        lblAllDebt.Content = String.Format("Сумма долгов: {0:0.00}", Convert.ToDouble(debt));
                    }
                    else
                    {
                        this.dgridOrders.ItemsSource = null;
                        this.dgrodOlines.ItemsSource = null;
                    }
                }
            }
        }

        private void dgridOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(Order curOrder in e.AddedItems)
            {
                var olines = _entities.Orders.Find(curOrder.Id).OrderLines.ToList<OrderLine>();
                if (olines.Count > 0)
                {
                    this.dgrodOlines.ItemsSource = olines;
                }
                else this.dgrodOlines.ItemsSource = null;
            }
        }

        private void dgridOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dgr = e.Row as DataGridRow;
            Order order = (Order)e.Row.DataContext;
            //if (Convert.ToDouble(order.TotalS) != Convert.ToDouble(order.PaymentS))
            //{
            //    dgr.Background = Brushes.SkyBlue;
            //    dgr.Foreground = Brushes.DarkRed;
            //}
            //else dgr.Background = Brushes.Wheat;
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dgridOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Order order = (Order)dgridOrders.SelectedItem;
            PrintDelOrder pd = new PrintDelOrder(order);
            Hide();
            pd.ShowDialog();
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

        private void dgrodOlines_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }

}
