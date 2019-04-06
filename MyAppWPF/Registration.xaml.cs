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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
            txtName.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return)) RegistrNewUser();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            RegistrNewUser();
        }

        private void RegistrNewUser()
        {
            if(txtName.Text != String.Empty && txtPhone.Text != String.Empty 
                && txtLogin.Text != String.Empty && passPass.Text != String.Empty)
            {
                User user = new User();
                user.Name = txtName.Text;
                user.Phone1 = txtPhone.Text;
                user.UserLogin = txtLogin.Text;
                user.LogPass = passPass.Text;
                user.Descr = txtDescr.Text;
                using (Model1 _entities = new Model1())
                {
                    try
                    {
                        bool reg = false;
                        foreach (User u in _entities.Users)
                        {
                            if (u.LogPass == user.LogPass || u.Name == user.Name || u.Phone1 == user.Phone1)
                            {
                                reg = false;
                            }
                            else reg = true;
                        }
                        if(reg == false)
                        {
                            MessageBox.Show("Такой пользователь уже существует.", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            _entities.Users.Add(user);
                            _entities.SaveChanges();
                            MessageBox.Show("Вы успешно зарегистрировались.", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                    }
                    catch (Exception ex) { }
                };
            }
            else
            {
                MessageBox.Show("Для регистрации необходимо заполнить поля,\nпомеченные звездочкой.", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
