using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для NewClientWindow.xaml
    /// </summary>
    public partial class NewClientWindow : Window
    {
        private Client newClient;
        public NewClientWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text == String.Empty)
            {
                MessageBox.Show("Имя клиента\nне может быть пустым", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.txtName.Focus();
            }
            else
            {
                using (Model1 _entities = new Model1())
                {
                    if(_entities.Clients.Count()>0)
                    {
                        foreach (Client c in _entities.Clients)
                        {
                            if (c.Name == this.txtName.Text
                                || (c.Phone1 == this.txtPhone1.Text && this.txtPhone1.Text != String.Empty)
                                || (c.Phone2 == this.txtPhone2.Text && this.txtPhone2.Text != String.Empty)
                                || (c.Mail == this.txtMail.Text && this.txtMail.Text != String.Empty))
                            {
                                MessageBox.Show("Такой клиент\nуже существует", "Ошибка заполнения", MessageBoxButton.OK, MessageBoxImage.Warning);
                                this.txtName.Clear();
                                this.txtDescr.Clear();
                                this.txtMail.Clear();
                                this.txtPhone1.Clear();
                                this.txtPhone2.Clear();
                                this.txtRegion.Clear();
                                this.txtName.Focus();
                                return;
                            }
                        }
                    }
                    try
                    {
                        newClient = new Client();
                        newClient.Name = this.txtName.Text;
                        newClient.Descr = this.txtDescr.Text;
                        newClient.Mail = this.txtMail.Text;
                        newClient.Phone1 = this.txtPhone1.Text;
                        newClient.Phone2 = this.txtPhone2.Text;
                        newClient.RegionInfo = this.txtRegion.Text;
                        _entities.Clients.Add(newClient);
                        _entities.SaveChanges();
                        MessageBox.Show(newClient.Name + "\nуспешно добавлен в базу данных.", "Добавление нового клиента", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        this.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка. Окно будет закрыто.", "Добавление клиента", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                    }
                }
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
