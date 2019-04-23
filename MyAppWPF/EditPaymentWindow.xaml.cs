using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditPaymentWindow.xaml
    /// </summary>
    public partial class EditPaymentWindow : Window
    {
        public Order Order { get; set; }
        public EditPaymentWindow()
        {
            InitializeComponent();
        }
        public EditPaymentWindow(Order currentOrder)
        {
            InitializeComponent();
            this.Order = currentOrder;
            DataContext = currentOrder;
            txtPaymentS.Focus();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == true)
            {
                using (Model1 _entities = new Model1())
                {
                    try
                    {
                        Order order = _entities.Orders.Find(Order.Id);
                        order.PaymentS = txtPaymentS.Text;
                        _entities.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        _entities.SaveChanges();
                        order = _entities.Orders.Find(Order.Id);
                        order.BalanceS = String.Format("{0:0.00}", Convert.ToDouble(order.TotalS) - Convert.ToDouble(order.PaymentS)).ToString();
                        _entities.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        _entities.SaveChanges();
                        double balS = 0;
                        foreach (Order o in _entities.Orders.Where(c=>c.ClientId == order.ClientId))
                        {
                            balS += Convert.ToDouble(o.BalanceS);
                            balS = Math.Round(balS, 2);
                        }
                        _entities.Clients.Find(order.ClientId).DebtS = balS.ToString();
                        _entities.SaveChanges();
                    }
                    catch (Exception ex) { }
                }
            }
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

        private void txtPaymentS_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"^([-\d,]+)$");
            if (!reg.IsMatch(e.Text)) e.Handled = true;

        }

        private void txtPaymentS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
