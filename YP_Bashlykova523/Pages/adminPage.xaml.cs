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
using static System.Net.Mime.MediaTypeNames;

namespace YP_Bashlykova523.Pages
{
    /// <summary>
    /// Логика взаимодействия для adminPage.xaml
    /// </summary>
    public partial class adminPage : Page
    {
        private string currentMode;
        public adminPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void HideAll()
        {
            usersList.Visibility = Visibility.Collapsed;
            complaintsList.Visibility = Visibility.Collapsed;
            applicationsList.Visibility = Visibility.Collapsed;
            frozenBooksList.Visibility = Visibility.Collapsed;

            complaintFilterCB.Visibility = Visibility.Collapsed;
            applicationFilterCB.Visibility = Visibility.Collapsed;

            addUserBtn.Visibility = Visibility.Collapsed;
        }

        private void LoadUsers()
        {
            HideAll();
            currentMode = "пользователи";

            usersList.Visibility = Visibility.Visible;
            addUserBtn.Visibility= Visibility.Visible;

            var allUsers = Core.Context.User.ToList();
            var displayUsers = allUsers.Select(u => new
            {
                u.ID,
                u.FullName,
                u.Role,
                u.Email,
                u.Login,
                u.Password,
                CardColor = u.IsFreeze ? "#A3A3A3" : "#CFC4E4"
            }).ToList();

            usersList.ItemsSource = displayUsers;

            titleTBl.Text = "Пользователи";
        }

        private void LoadComplaints()
        {
            currentMode = "жалобы";

            HideAll();
            complaintsList.Visibility = Visibility.Visible;

            var complaints = Core.Context.Complaint.ToList();
            var displayCmpl = complaints.Select(c => new
            {
                c.ID,
                User = c.User,
                c.Reason,
                c.CreateDate,

                TargetText =
                    c.ReviewID != null ? $"Отзыв №{c.ReviewID}" :
                    c.BookID != null && c.Reason.StartsWith("[Жалоба на автора]") ? $"Автор: {c.User?.FullName}" :
                    c.BookID != null ? $"Книга: {c.Book?.Title}" :
                    "Неизвестно"
            }).ToList();
            complaintsList.ItemsSource = displayCmpl;

            titleTBl.Text = "Жалобы";

            LoadComboBoxes(currentMode);
        }

        private void LoadApplications()
        {
            currentMode = "заявки";

            HideAll();
            applicationsList.Visibility = Visibility.Visible;

            applicationsList.ItemsSource = Core.Context.Application.ToList();
            titleTBl.Text = "Заявки";

            LoadComboBoxes(currentMode);
        }

        private void LoadFrozenBooks()
        {
            HideAll();
            frozenBooksList.Visibility = Visibility.Visible;

            frozenBooksList.ItemsSource = Core.Context.Book.Where(b => b.IsFreeze).ToList();
            titleTBl.Text = "Замороженные книги";
        }

        private void usersListBtn_Click(object sender, RoutedEventArgs e) 
        {
            LoadUsers();
        }

        private void complaintsBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadComplaints();
        }

        private void applicationsBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadApplications();
        }

        private void frozenBooksBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadFrozenBooks();
        }

        private void unfreezeBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var book = btn.DataContext as Book;
            book.IsFreeze = false;
            Core.Context.SaveChanges();

            LoadFrozenBooks();
        }

        private void LoadComboBoxes(string cm)
        {
            if (cm == "жалобы")
            {
                complaintFilterCB.Visibility = Visibility.Visible;
                applicationFilterCB.Visibility = Visibility.Collapsed;

                complaintFilterCB.SelectedIndex = 0;
            }

            if (cm == "заявки")
            {
                applicationFilterCB.Visibility = Visibility.Visible;
                complaintFilterCB.Visibility = Visibility.Collapsed;

                var ap = Core.Context.ApplicationPurpose.ToList();
                ap.Insert(0, new ApplicationPurpose { ID = 0, Name = "Все" });

                applicationFilterCB.ItemsSource = ap;
                applicationFilterCB.SelectedIndex = 0;
            }
        }

        private void applicationFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var applications = Core.Context.Application.ToList();

            if (applicationFilterCB.SelectedIndex == 0)
                applicationsList.ItemsSource = applications;

            else
            {
                var applPurpose = applicationFilterCB.SelectedItem as ApplicationPurpose;
                if (applPurpose == null) return;

                applications = applications.Where(ap => ap.ApplicationPurpose != null && ap.ApplicationPurpose.Name == applPurpose.Name).ToList();
                applicationsList.ItemsSource = applications;
            }
        }

        private void complaintFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var complaints = Core.Context.Complaint.ToList();

            if (complaintFilterCB.SelectedIndex == 1)
                complaints = complaints.Where(c => c.BookID != null && c.ReviewID == null).ToList();

            if (complaintFilterCB.SelectedIndex == 2)
                complaints = complaints.Where(c => c.BookID != null && c.ReviewID == null && c.Reason.StartsWith("[Жалоба на автора]")).ToList();

            if (complaintFilterCB.SelectedIndex == 3)
                complaints = complaints.Where(c => c.BookID == null && c.ReviewID != null).ToList();

            var displayCmpl = complaints.Select(c => new
            {
                c.ID,
                User = c.User,
                c.Reason,
                c.CreateDate,

                TargetText =
                    c.ReviewID != null ? $"Отзыв №{c.ReviewID}" :
                    c.BookID != null && c.Reason.StartsWith("[Жалоба на автора]") ? $"Автор: {c.User?.FullName}" :
                    c.BookID != null ? $"Книга: {c.Book?.Title}" :
                    "Неизвестно"
            }).ToList();

            complaintsList.ItemsSource = displayCmpl;
        }

        private void actionAcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic item = btn.DataContext;
            int id = item.ID;

            if (currentMode == "жалобы")
            {
                var complaint = Core.Context.Complaint.FirstOrDefault(c => c.ID == id);

                if (complaint.ReviewID != null)
                {
                    var review = Core.Context.Review.FirstOrDefault(r => r.ID == complaint.ReviewID);
                    if (review != null)
                        Core.Context.Review.Remove(review);
                }
                else if (complaint.BookID != null && complaint.Reason.StartsWith("[Жалоба на автора]"))
                {
                    var user = Core.Context.User.FirstOrDefault(u => u.ID == complaint.UserID);
                    if (user != null)
                        user.IsFreeze = true;
                }
                else if (complaint.BookID != null)
                {
                    var book = Core.Context.Book.FirstOrDefault(b => b.ID == complaint.BookID);
                    if (book != null)
                        book.IsFreeze = true;
                }

                Core.Context.Complaint.Remove(complaint);
                Core.Context.SaveChanges();

                LoadComplaints();
            }

            if (currentMode == "заявки")
            {
                var app = Core.Context.Application.FirstOrDefault(a => a.ID == id);

                if (app.ApplicationPurpose?.Name == "Становление автором")
                {
                    var user = Core.Context.User.FirstOrDefault(u => u.ID == app.UserID);
                    if (user != null)
                        user.RoleID = 2;
                }

                if (app.ApplicationPurpose?.Name == "Разморозка книги")
                {
                    var book = Core.Context.Book.FirstOrDefault(b => b.ID == app.BookID);
                    if (book != null)
                        book.IsFreeze = false;
                }

                if (app.ApplicationPurpose?.Name == "Разморозка аккаунта")
                {
                    var user = Core.Context.User.FirstOrDefault(u => u.ID == app.UserID);
                    if (user != null)
                        user.IsFreeze = false;
                }

                Core.Context.Application.Remove(app);
                Core.Context.SaveChanges();

                LoadApplications();
            }
        }

        private void actionRejectBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic item = btn.DataContext;
            int id = item.ID;

            if (currentMode == "жалобы")
            {
                var complaint = Core.Context.Complaint.FirstOrDefault(c => c.ID == id);
                if (complaint != null)
                {
                    Core.Context.Complaint.Remove(complaint);
                }
                Core.Context.SaveChanges();

                LoadComplaints();
            }

            if (currentMode == "заявки")
            {
                var application = Core.Context.Application.FirstOrDefault(a => a.ID == id);
                if (application != null)
                {
                    Core.Context.Application.Remove(application);
                }
                Core.Context.SaveChanges();

                LoadApplications();
            }
        }

        private void addUserBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "добавление";

            var win = new adminEditWindow(currentMode, 0);
            win.ShowDialog();

            if (win.IsChanged) LoadUsers();
        }

        private void editUserBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "редактирование";

            var btn = sender as Button;
            if (btn == null) return;

            dynamic user = btn.DataContext;
            int id = user.ID;

            var win = new adminEditWindow(currentMode, id);
            win.ShowDialog();

            if (win.IsChanged) LoadUsers();
        }

        private void delUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic user = btn.DataContext;
            int id = user.ID;

            var realUser = Core.Context.User.FirstOrDefault(u => u.ID == id);
            if (realUser != null)
            {
                var result = MessageBox.Show($"Удалить пользователя {realUser.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Core.Context.User.Remove(realUser);
                    Core.Context.SaveChanges();
                    LoadUsers();
                }
            }
        }
    }
}
