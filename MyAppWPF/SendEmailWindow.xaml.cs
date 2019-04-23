using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Interaction logic for SendEmailWindow.xaml
    /// </summary>
    public partial class SendEmailWindow : Window
    {
        public string MailAddress;
        public StringBuilder body;
        public String Filename;
        private bool choiceMail;
        public Order Order { get; set; }
        public SendEmailWindow()
        {
            InitializeComponent();
        }
        public SendEmailWindow(Order order, String filename)
        {
            InitializeComponent();
            Order = order;
            DataContext = Order;
            txtAnotherMail.IsEnabled = false;
            Filename = filename;
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        { 
            txtAnotherMail.IsEnabled = true;
            txtAnotherMail.Focus();
            txtAnotherMail.Clear();
            choiceMail = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(choiceMail == false)
            {
                MailAddress = Order.Clients.Mail;
            }
            else
            {
                MailAddress = txtAnotherMail.Text;
                if (MailAddress.Length == 0)
                {
                    MessageBox.Show("Введите адрес почты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAnotherMail.Focus();
                    return;
                }
                else if (!Regex.IsMatch(MailAddress, @"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
                {
                    MessageBox.Show("Неправильно написан адрес почты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAnotherMail.Select(0, txtAnotherMail.Text.Length);
                    txtAnotherMail.Focus();
                    return;
                }
            }
            //GetBody(Order);
            try
            {
                //EmailSender.SendMail(MailAddress, body.ToString());
                FileSaver.SaveExcelFile(Order, Filename);
                EmailSender.SendMailWithAttachment(MailAddress, Filename);
                FileSaver.DeleteExcelFile(Order, Filename);
                MessageBox.Show("Заказ отправлен.", "Отправка", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch
            {
                MessageBox.Show("Заказ не отправлен.", "Отправка", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        //private void GetBody(Order order)
        //{
        //    body = new StringBuilder()
        //        .AppendLine("Заказ № " + order.Id)
        //        .AppendLine("Дата: " + order.OrderDate.ToShortDateString())
        //        //.AppendLine("Менеджер: " + order.Users.Name + " тел. " + order.Users.Phone1)
        //        .AppendLine("\n");
        //    int i = 0;
        //    if(Convert.ToDouble(Order.TotalS) != 0 || Order.TotalS != String.Empty)
        //    {
        //        foreach (OrderLine ol in order.OrderLines.OrderBy(o=>o.Products.Name))
        //        {
        //            body.AppendFormat("{0,4}. {1,-50}{2,-5}{3,5} * {4,8} = {5,8}\n", ++i, ol.Products.Name, ol.Products.Unit, ol.Quantity, (Convert.ToDouble(ol.Price)).ToString("N3"), (Convert.ToDouble(ol.Total)).ToString("F"));
        //        }
        //        body.AppendLine("----------------------------------------\n");
        //        body.AppendFormat("Итого: {0}   Оплачено: {1}", (Convert.ToDouble(Order.TotalS)).ToString("F"),
        //            (Convert.ToDouble(Order.PaymentS)).ToString("F"));
        //    }
        //    body.AppendLine("\n");
        //    body.AppendLine("----------------------------------------\n");
        //    body.AppendFormat("Общий долг: {0:0.00}", Convert.ToDouble(Order.Clients.DebtS));
        //}

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txtAnotherMail.IsEnabled = false;
            choiceMail = false;
        }

        private static bool IsEmailAllowed(string text)
        {
            bool blnValidEmail = false;
            Regex regEMail = new Regex(@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$");
            if (text.Length > 0)
            {
                blnValidEmail = regEMail.IsMatch(text);
            }

            return blnValidEmail;
        }

        private void txtAnotherMail_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsEmailAllowed(txtAnotherMail.Text.Trim()) == false)
            {
                e.Handled = true;
                MessageBox.Show("Неправильно написан адрес почты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAnotherMail.Focus();
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
