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

namespace YP_Bashlykova523.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = User.currentUser;

            UpdateUI();
            ContentFrame.Navigate(new bookCatalogPage());
        }

        private void UpdateUI()
        {
            adminBtn.Visibility = Visibility.Collapsed;
            authorBtn.Visibility = Visibility.Collapsed;
            profileBtn.Visibility = Visibility.Collapsed;
            bookListsBtn.Visibility = Visibility.Collapsed;

            logoutBtn.Visibility = Visibility.Collapsed;
            entrBtn.Visibility = Visibility.Visible;

            freezePanel.Visibility = Visibility.Collapsed;

            if (User.currentUser == null) return;

            entrBtn.Visibility = Visibility.Collapsed;
            logoutBtn.Visibility = Visibility.Visible;
            profileBtn.Visibility = Visibility.Visible;

            if (User.currentUser.RoleID == 1) 
            {
                bookListsBtn.Visibility = Visibility.Visible;
            }
            else if (User.currentUser.RoleID == 2)
            {
                authorBtn.Visibility = Visibility.Visible;
                bookListsBtn.Visibility = Visibility.Visible;
            }
            else
            {
                adminBtn.Visibility = Visibility.Visible;
                bookListsBtn.Visibility = Visibility.Visible;
            }

            if (User.currentUser.IsFreeze)
            {
                freezePanel.Visibility = Visibility.Visible;
            }
        }

        private void bookCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new bookCatalogPage());
        }

        private void bookListsBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new readingListPage());
        }

        private void adminBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new adminPage());
        }

        private void authorBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new authorPage());
        }

        private void profileBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new accountPage());
        }

        private void entrBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new enterPage());
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            User.currentUser = null;

            UpdateUI();
            ContentFrame.Navigate(new bookCatalogPage());
        }

        private void applicationBtn_Click(object sender, RoutedEventArgs e)
        {
            new applicationWindow("Разморозка аккаунта").ShowDialog();
        }
    }
}
