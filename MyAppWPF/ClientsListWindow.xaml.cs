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
    /// Логика взаимодействия для ClientsListWindow.xaml
    /// </summary>
    public partial class ClientsListWindow : Window
    {
        private Model1 _entities = new Model1();
        public ObservableCollection<Client> clients;
        
        public ClientsListWindow()
        {
            InitializeComponent();
            clients = new ObservableCollection<Client>();
            foreach(Client client in _entities.Clients.OrderBy(c=>c.Name))
            {
                clients.Add(client);
            }
            
            this.dgridClients.ItemsSource = clients;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgridClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Client currentClient = (Client)dgridClients.SelectedItem;
            EditClientWindow ecw = new EditClientWindow(currentClient);
            ecw.Closing += delegate(object fsender, System.ComponentModel.CancelEventArgs fe)
            {
                if (ecw.DialogResult == true)
                {
                    using (Model1 _entities = new Model1())
                    {
                        try
                        {
                            Client editClient = _entities.Clients.Find(currentClient.Id);
                            editClient.Name = currentClient.Name;
                            editClient.Phone1 = currentClient.Phone1;
                            editClient.Phone2 = currentClient.Phone2;
                            editClient.DebtS = currentClient.DebtS;
                            editClient.Mail = currentClient.Mail;
                            editClient.RegionInfo = currentClient.RegionInfo;
                            editClient.Descr = currentClient.Descr;
                            _entities.Entry(editClient).State = System.Data.Entity.EntityState.Modified;
                            _entities.SaveChanges();
                        }
                        catch (Exception ex) { }
                    }
                }
            };
            this.Effect = new System.Windows.Media.Effects.BlurEffect();
            ecw.ShowDialog();
            Effect = null;
        }

        private void dgridClients_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dgr = e.Row as DataGridRow;
            Client cl = (Client)e.Row.DataContext;
            //if (Convert.ToDouble(cl.DebtS) > 0)
            //{
            //    dgr.Foreground = Brushes.Red;
            //}
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            NewClientWindow ncw = new NewClientWindow();
            Hide();
            ncw.ShowDialog();
            ShowDialog();
            this.Close();
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
