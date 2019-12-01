using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для PrintDelOrder.xaml
    /// </summary>
    public partial class PrintDelOrder : Window
    {

        public Order CurrentOrder { get; set; }
        public int ClientId { get; set; }
        public StringBuilder body;
        //private Excel.Application excelApp;
        public String FileName;
        public String FileNameForAttachment;
        public PrintDelOrder()
        {
            InitializeComponent();
        }
        public PrintDelOrder(Order order)
        {
            InitializeComponent();
            CurrentOrder = order;
            ClientId = order.ClientId;
            DataContext = CurrentOrder;
            lblDate.Content = CurrentOrder.OrderDate.ToLongDateString();
            dgOrderLines.ItemsSource = CurrentOrder.OrderLines.ToList().OrderBy(o=>o.Products.Name);
            FileName = @"D:\Baraban Orders\" + (order.Clients.Name + "_" + order.Id + "_" + order.OrderDate.Year + "-" + order.OrderDate.Month + "-" + order.OrderDate.Day).ToString();
            FileNameForAttachment = @"D:\Baraban Orders\" + (order.Clients.Name + "_" + order.OrderDate.Year + "-" + order.OrderDate.Month + "-" + order.OrderDate.Day).ToString();
        }
        private void btnDelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDouble(CurrentOrder.BalanceS) != 0)
            {
                MessageBox.Show("Заказ не оплачен.", "Удаление заказа", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                if (MessageBox.Show("Вы действительно хотите удалить этот заказ?", "Удаление заказа", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    using (Model1 _entities = new Model1())
                    {
                        try
                        {
                            var olines = CurrentOrder.OrderLines;
                            if (olines != null)
                            {
                                foreach (OrderLine ol in olines)
                                {
                                    _entities.OrderLines.Remove(_entities.OrderLines.Where(o => o.Id == ol.Id).FirstOrDefault());
                                }
                            }
                            _entities.Orders.Remove(_entities.Orders.Where(o => o.Id == CurrentOrder.Id).FirstOrDefault());
                            _entities.SaveChanges();

                            double balS = 0;
                            foreach (Order o in _entities.Orders)
                            {
                                if (o.ClientId == ClientId)
                                {
                                    balS += Convert.ToDouble(o.BalanceS);
                                }
                            }
                            _entities.Clients.Find(ClientId).DebtS = balS.ToString();
                            _entities.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка. Окно будет закрыто.", "Заказ", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                    MessageBox.Show("Заказ удален", "Удаление заказа", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    this.Close();
                }
                else return;
            }
        }

        private void btnPrintOrder_Click(object sender, RoutedEventArgs e)
        {
            SendEmailWindow sew = new SendEmailWindow(CurrentOrder, FileNameForAttachment);
            Effect = new System.Windows.Media.Effects.BlurEffect();
            sew.ShowDialog();
            Effect = null;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            EditPaymentWindow ep = new EditPaymentWindow(CurrentOrder);
            Effect = new System.Windows.Media.Effects.BlurEffect();
            ep.ShowDialog();
            Effect = null;
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

        private void dgOrderLines_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            Effect = new System.Windows.Media.Effects.BlurEffect();
            //GetBody(CurrentOrder);
            //AutoSave(body, CurrentOrder);
            //SaveToFile(body);
            try
            {
                FileSaver.SaveExcelFile(CurrentOrder);
                MessageBox.Show("Заказ успешно сохранен в файл\n" + FileName, "Сохранение заказа", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить заказ в файл\n", "Ошибка записи", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Effect = null;
        }
        private void GetBody(Order order)
        {

            body = new StringBuilder();
            if (Convert.ToDouble(order.TotalS) < 0)
            {
                body.AppendLine("Возврат №" + order.Id);
            }
            else
            {
                body.AppendLine("Заказ № " + order.Id);
            }
            body.AppendLine("Дата: " + order.OrderDate.ToShortDateString())
            //.AppendLine("Клиент: " + order.Clients.Name + "\t" + order.Clients.Phone1 + "\t(" + order.Clients.RegionInfo + ")")
            .AppendLine("-------------------------------------------------------------------------------");
            int i = 0;
            if (Convert.ToDouble(order.TotalS) != 0 || order.TotalS != String.Empty)
            {
                foreach (OrderLine ol in order.OrderLines.OrderBy(o => o.Products.Name))
                {
                    body.AppendFormat("{0,4}. {1,-39}{2,-5}{3,5} * {4,8} = {5,8}\n", ++i, ol.Products.Name, ol.Products.Unit, ol.Quantity, (Convert.ToDouble(ol.Price)).ToString("N3"), (Convert.ToDouble(ol.Total)).ToString("F"));
                }
                body.AppendLine("-------------------------------------------------------------------------------\n");
                body.AppendFormat("Итого: {0:0.00}", (Convert.ToDouble(order.TotalS)).ToString("F"));
                body.AppendLine("\n");
                body.AppendFormat("Оплачено: {0:0.00}", (Convert.ToDouble(order.PaymentS)).ToString("F"));
            }
            body.AppendLine("\n");
            body.AppendFormat("Общий долг: {0:0.00}", Convert.ToDouble(order.Clients.DebtS));
        }
        private void SaveToFile(StringBuilder sb)
        {

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.txt)|*.txt";
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.OpenFile(), System.Text.Encoding.Unicode))
                    {
                        sw.Write(sb);
                        sw.Close();
                        MessageBox.Show("Заказ успешно сохранен в файл\n" + sfd.FileName, "Сохранение заказа", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch
                {
                    MessageBox.Show("Во время сохранения возникла ошибка.", "Сохранение заказа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void AutoSave(StringBuilder sb, Order order)
        {
            string fileName = (order.Clients.Name + "_" + order.Id + "_" + order.OrderDate.Year + "-" + order.OrderDate.Month + "-" + order.OrderDate.Day).ToString();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"D:\Baraban Orders\" + fileName + ".txt"))
            {
                sw.Write(sb);
                sw.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
