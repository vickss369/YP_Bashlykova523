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
            ContentFrame.Navigate(new bookCatalogPage());
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
    }
}
