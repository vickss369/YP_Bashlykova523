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
    /// Логика взаимодействия для accountPage.xaml
    /// </summary>
    public partial class accountPage : Page
    {
        public accountPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (User.currentUser == null) return;

            var user = User.currentUser;

            userFullNameTB.Text = user.FullName;
            loginTB.Text = user.Login;
            emailTB.Text = user.Email;

            if (user.RoleID == 1) roleTB.Text = "Читатель";
            else if (user.RoleID == 2) roleTB.Text = "Автор";
            else roleTB.Text = "Администратор";

            reviewsList.ItemsSource = Core.Context.Review.Where(r => r.UserID == user.ID).ToList();

            if (user.IsFreeze) toBeAuthorPanel.Visibility = Visibility.Collapsed;

            if (user.RoleID == 1)
                toBeAuthorPanel.Visibility = Visibility.Visible;
            else
                toBeAuthorPanel.Visibility = Visibility.Collapsed;
        }

        private void applicationAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            new applicationWindow("Становление автором").ShowDialog();
        }
    }
}
