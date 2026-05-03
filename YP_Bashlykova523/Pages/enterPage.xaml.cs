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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YP_Bashlykova523.Classes;

namespace YP_Bashlykova523.Pages
{
    /// <summary>
    /// Логика взаимодействия для enterPage.xaml
    /// </summary>
    public partial class enterPage : Page
    {
        private bool isLoginMode = true;
        private string specSymbols = "!?<>^*#№$%&~";

        public enterPage()
        {
            InitializeComponent();
            UpdateUIForMode();
        }

        private bool IsInputValid()
        {
            return InputError() == null;
        }

        private string InputError()
        {
            if (isLoginMode)
            {
                if (string.IsNullOrWhiteSpace(loginTB.Text)) return "Введите логин!";

                if (string.IsNullOrWhiteSpace(passwordPB.Password)) return "Введите пароль!";

                return null;
            }

            else
            {
                if (string.IsNullOrWhiteSpace(fullNameTB.Text) || fullNameTB.Text.Length < 5 || fullNameTB.Text.Any(char.IsDigit))
                    return "Введите корректное имя!";

                if (string.IsNullOrWhiteSpace(loginTB.Text) || loginTB.Text.Length < 3)
                    return "Введите корректный логин!";

                if (string.IsNullOrWhiteSpace(passwordPB.Password) || passwordPB.Password.Length < 5 || !passwordPB.Password.Any(char.IsDigit) || !passwordPB.Password.Any(c => specSymbols.Contains(c)))
                    return "Пароль должен быть ≥5 символов, содержать цифру и спецсимвол.";

                if (string.IsNullOrWhiteSpace(emailTB.Text) || !emailTB.Text.Contains("@") || !emailTB.Text.Contains("."))
                    return "Введите корректный email!";

                return null;
            }
        }

        private void UpdateUIForMode()
        {
            if (isLoginMode)
            {
                modeTitle.Text = "ВХОД";
                entrBtn.Content = "Войти";
                switchModeTBl.Text = "Ещё нет аккаунта? Зарегистрироваться";

                fullNameTB.Visibility = Visibility.Collapsed;
                fullNameTBl.Visibility = Visibility.Collapsed;
                emailTB.Visibility = Visibility.Collapsed;
                emailTBl.Visibility = Visibility.Collapsed;
            }

            else
            {
                modeTitle.Text = "РЕГИСТРАЦИЯ";
                entrBtn.Content = "Зарегистрироваться";
                switchModeTBl.Text = "Уже есть аккаунт? Войти";

                fullNameTB.Visibility = Visibility.Visible;
                fullNameTBl.Visibility = Visibility.Visible;
                emailTB.Visibility = Visibility.Visible;
                emailTBl.Visibility = Visibility.Visible;
            }
        }

        private void entrBtn_Click(object sender, RoutedEventArgs e)
        {
            string error = InputError();
            if (error != null)
            {
                MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string login = loginTB.Text.Trim();
            string pass = passwordPB.Password.Trim();

            if (isLoginMode)
            {
                var user = Core.Context.User.FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    loginTB.Text = "";
                    loginTB.Focus();
                    passwordPB.Password = "";
                    return;
                }

                if (user.Password != pass)
                {
                    MessageBox.Show("Неверный пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordPB.Password = "";
                    passwordPB.Focus();
                    return;
                }

                if (user.Password.Length < 5 || !user.Password.Any(char.IsDigit) || !user.Password.Any(c => specSymbols.Contains(c)))
                {
                    MessageBox.Show("Ваш пароль не соответствует новым требованиям.\nПожалуйста, обновите его.","ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NavigationService.Navigate(new MainPage());
                }

                else
                {
                    User.currentUser = user;
                    if (user.IsFreeze)
                    {
                        MessageBox.Show("Вы заморожены!", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    NavigationService.Navigate(new MainPage());
                }
            }

            else
            {
                if (Core.Context.User.Any(u => u.Login == login))
                {
                    MessageBox.Show("Этот логин уже занят.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    loginTB.Text = "";
                    loginTB.Focus();
                    passwordPB.Password = "";
                    return;
                }

                User newUser = new User
                {
                    FullName = fullNameTB.Text.Trim(),
                    Login = login,
                    Password = pass,
                    Email = emailTB.Text.Trim(),
                    RoleID = 1,
                    IsFreeze = false
                };
                Core.Context.User.Add(newUser);
                Core.Context.SaveChanges();

                User.currentUser = newUser;
                MessageBox.Show("Регистрация прошла успешно!\nДобро пожаловать!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new MainPage());
            }
        }

        private void SwitchMode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isLoginMode = !isLoginMode;
            UpdateUIForMode();

            loginTB.Text = "";
            passwordPB.Password = "";
            fullNameTB.Text = "";
            emailTB.Text = "";
        }

        private void SwitchMode_Hover(object sender, MouseEventArgs e)
        {
            if (e.RoutedEvent == MouseEnterEvent) switchModeTBl.Foreground = Brushes.Blue;
            else switchModeTBl.Foreground = Brushes.Brown;
        }
    }
}
