using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        public WorkWindow()
        {
            InitializeComponent();
        }
        private User currentUser;
        public WorkWindow(User user)
        {
            InitializeComponent();
            this.currentUser = user;
            this.tblockUser.Text = currentUser.Name;
            lblDate.Content = DateTime.Now.ToShortDateString();
            lblDate.HorizontalContentAlignment = HorizontalAlignment.Right;
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void tblockUser_MouseMove(object sender, MouseEventArgs e)
        {
            ToolTip qwe = new ToolTip();
            qwe.Content = currentUser.Name + "\nТел.: " + currentUser.Phone1 + "\nДолжность: " + currentUser.Descr;
            this.tblockUser.ToolTip = qwe;
        }
        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            ClientsListWindow clw = new ClientsListWindow();
            Hide();
            clw.ShowDialog();
            ShowDialog();
        }
        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsListWindow plw = new ProductsListWindow();
            Hide();
            plw.ShowDialog();
            ShowDialog();
        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            NewProductWindow npw = new NewProductWindow();
            Hide();
            npw.ShowDialog();
            ShowDialog();            
        }
        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            NewClientWindow ncw = new NewClientWindow();
            Hide();
            ncw.ShowDialog();
            ShowDialog();
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrderWindow now = new NewOrderWindow(currentUser);
            Hide();
            now.ShowDialog();
            ShowDialog();
        }

        private void btnStat_Click(object sender, RoutedEventArgs e)
        {
            StatWindow sw = new StatWindow();
            Hide();
            sw.ShowDialog();
            ShowDialog();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            //запуск окна Сотрудники
            UsersWindow uw = new UsersWindow();
            Hide();
            uw.ShowDialog();
            ShowDialog();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(2, 2, 2, 2);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void btnNewOrder_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.LightGray;
        }

        private void btnNewOrder_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.Wheat;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            ReturnWindow rw = new ReturnWindow(currentUser);
            Hide();
            rw.ShowDialog();
            ShowDialog();
        }
    }
}
