using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для StatWindow.xaml
    /// </summary>
    public partial class StatWindow : Window
    {
        public StatWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOrdersByDates_Click(object sender, RoutedEventArgs e)
        {
            OrdersByDateWindow od = new OrdersByDateWindow();
            Hide();
            od.ShowDialog();
            ShowDialog();
        }

        private void btnOrdersByClients_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrdersByClientWindow oc = new OrdersByClientWindow();
                Hide();
                oc.ShowDialog();
                ShowDialog();
            }
            catch
            {
                return;
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(2, 2, 2, 2);
        }

        private void btnAllOrders_Click(object sender, RoutedEventArgs e)
        {
            AllOrdersList aol = new AllOrdersList();
            Hide();
            aol.ShowDialog();
            ShowDialog();
        }

        private void btnAllOrders_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.LightGray;
        }

        private void btnAllOrders_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.SkyBlue;
        }

        #region ButtonCloseEvents
        private void Border_MouseMove_1(object sender, MouseEventArgs e)
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
        private void Border_MouseLeave_1(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Transparent;
        }
        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
