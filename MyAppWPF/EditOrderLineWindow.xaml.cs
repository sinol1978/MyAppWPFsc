using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        private OrderLine curOL;
        private Model1 _entities = new Model1();
        public OrderLine CurOL
        {
            get { return curOL; }
            set { curOL = value; }
        }

        public EditOrderWindow()
        {
            InitializeComponent();
        }
        public EditOrderWindow(OrderLine ol)
        {
            InitializeComponent();
            this.CurOL = ol;
            DataContext = CurOL;
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

        private void txtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex reg = new Regex(@"^([\d,]+)$");
            if (!reg.IsMatch(e.Text))
                e.Handled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == true)
            {
                this.CurOL.Quantity = this.txtQuantity.Text;
                this.CurOL.Price = this.txtPrice.Text;
                this.CurOL.Total = (Convert.ToDouble(this.CurOL.Quantity) * Convert.ToDouble(this.CurOL.Price)).ToString();
                lblTotal.Content = CurOL.Total;
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

        private void cboxProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product product = (Product)e.AddedItems[0];
            this.lblUnit.Content = product.Unit;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}