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
using YP_Bashlykova523.Classes;

namespace YP_Bashlykova523.Pages
{
    /// <summary>
    /// Логика взаимодействия для adminEditWindow.xaml
    /// </summary>
    public partial class adminEditWindow : Window
    {

        private string mode;
        private int id;

        public bool IsChanged { get; private set; } = false;

        public adminEditWindow(string mode, int id)
        {
            InitializeComponent();

            this.mode = mode;
            this.id = id;

            LoadData();
            roleCB.ItemsSource = Core.Context.Role.ToList();
        }

        private void LoadData()
        {
            if (mode == "редактирование")
            {
                titleTBl.Text = "Редактирование пользователя";

                var user = Core.Context.User.FirstOrDefault(u => u.ID == id);
                if (user != null) 
                {
                    usernameTB.Text = user.FullName;
                    roleCB.SelectedValue = user.RoleID;
                    emailTB.Text = user.Email;
                    isFreezeChB.IsChecked = user.IsFreeze;
                    loginTB.Text = user.Login;  
                    passwordTB.Text = user.Password;
                }
            }

            else if (mode == "добавление")
            {
                titleTBl.Text = "Добавление пользователя";
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTB.Text) || roleCB.SelectedValue == null
                || string.IsNullOrWhiteSpace(emailTB.Text) || string.IsNullOrWhiteSpace(loginTB.Text) || string.IsNullOrWhiteSpace(passwordTB.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (mode == "редактирование")
            {
                var user = Core.Context.User.FirstOrDefault(u => u.ID == id);
                if (user != null)
                {
                    user.FullName = usernameTB.Text;
                    user.RoleID = Convert.ToInt32(roleCB.SelectedValue);
                    user.Email = emailTB.Text;
                    user.IsFreeze = isFreezeChB.IsChecked == true;
                    user.Login = loginTB.Text;
                    user.Password = passwordTB.Text;
                }
            }

            else if (mode == "добавление")
            {
                User u = new User()
                {
                    FullName = usernameTB.Text,
                    RoleID = Convert.ToInt32(roleCB.SelectedValue),
                    Email = emailTB.Text,
                    IsFreeze = isFreezeChB.IsChecked == true,
                    Login = loginTB.Text,
                    Password = passwordTB.Text
                };
                Core.Context.User.Add(u);
            }

            Core.Context.SaveChanges();

            IsChanged = true;
            Close();
        }
    }
}
