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
    /// Логика взаимодействия для EditClientWindow.xaml
    /// </summary>
    public partial class EditClientWindow : Window
    {
        private Client curClient;

        public Client CurClient
        {
            get { return curClient; }
            set { curClient = value; }
        }
        public EditClientWindow()
        {
            InitializeComponent();
        }
        public EditClientWindow(Client currentClient)
        {
            InitializeComponent();
            this.CurClient = currentClient;
            this.DataContext = this.CurClient;
            lblDebtS.Content = String.Format("{0:0.00}", Convert.ToDouble(CurClient.DebtS)).ToString();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == true)
            {
                using (Model1 _entities = new Model1())
                {
                    try
                    {
                        Client editClient = _entities.Clients.Find(CurClient.Id);
                        editClient.Name = CurClient.Name;
                        editClient.Phone1 = CurClient.Phone1;
                        editClient.Phone2 = CurClient.Phone2;
                        editClient.DebtS = CurClient.DebtS;
                        editClient.Mail = CurClient.Mail;
                        editClient.RegionInfo = CurClient.RegionInfo;
                        editClient.Descr = CurClient.Descr;
                        _entities.Entry(editClient).State = System.Data.Entity.EntityState.Modified;
                        _entities.SaveChanges();
                    }
                    catch (Exception ex) { }
                }
            }
        }
        private void btnDelClient_Click(object sender, RoutedEventArgs e)
        {

            using (Model1 _entities = new Model1())
            {
                try
                {
                    Client editClient = _entities.Clients.Find(CurClient.Id);
                    if (editClient != null)
                    {
                        if (Convert.ToDouble(CurClient.DebtS) == 0 && CurClient.Orders.Count == 0)
                        {
                            if (MessageBox.Show(String.Format("Вы действительно хотите удалить клиента \n{0}", CurClient.Name), "Удаление клиента", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                _entities.Clients.Remove(_entities.Clients.Find(CurClient.Id));
                                _entities.SaveChanges();
                                MessageBox.Show("Клиент успешно удален.", "Удаление клиента", MessageBoxButton.OK, MessageBoxImage.Information);
                                this.Close();
                                return;
                            }
                            else return;
                        }
                        MessageBox.Show("Этого клиента нельзя удалить!\n\nОн либо не погасил долг,\nлибо является активным.", "Удаление клиента", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    return;
                }
                catch (Exception ex) { }
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
