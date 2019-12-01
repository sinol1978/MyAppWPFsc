using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyAppWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model1 _entities;
        private User currentUser;
        public MainWindow()
        {
            InitializeComponent();
            _entities = new Model1();
            this.txtLogin.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (this.Login() == true)
            {
                this.txtLogin.Clear();
                this.passPass.Clear();
                this.txtLogin.Focus();
                WorkWindow work = new WorkWindow(currentUser);
                Hide();
                work.ShowDialog();
                Show();
            }
            else
            {
                MessageBox.Show("Пожалуйста, авторизируйтесь!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtLogin.Clear();
                this.passPass.Clear();
                this.txtLogin.Focus();
                return;
            }
        }
        public bool Login()
        {
            try
            {
                foreach (User u in _entities.Users)
                {
                    if (u.UserLogin == this.txtLogin.Text && u.LogPass == this.passPass.Password)
                    {
                        currentUser = _entities.Users.Where(user => user.UserLogin == u.UserLogin).FirstOrDefault();
                        return true;
                    }
                }
                return false;

            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных. Приложение будет закрыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"D:\Projects\Tumbochki XP\BackupScript\backup.bat");
            this.Close();
        }

        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return)) Enter();
        }
        private void Enter()
        {
            if (this.Login() == true)
            {
                this.txtLogin.Clear();
                this.passPass.Clear();
                this.txtLogin.Focus();
                WorkWindow work = new WorkWindow(currentUser);
                Hide();
                work.ShowDialog();
                ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, авторизируйтесь!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                this.txtLogin.Clear();
                this.passPass.Clear();
                this.txtLogin.Focus();
                return;
            }

        }

        private void passPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return)) Enter();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            //Registration rw = new Registration();
            //Hide();
            //rw.ShowDialog();
            //ShowDialog();
            MessageBox.Show("Регистрация недоступна!", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
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
