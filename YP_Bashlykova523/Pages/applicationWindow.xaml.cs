using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для applicationWindow.xaml
    /// </summary>
    public partial class applicationWindow : Window
    {
        private string applicationPurpose;
        private int? bookID = null;
        public applicationWindow(string ap) //если на пользователя
        {
            InitializeComponent();
            applicationPurpose = ap;

            LoadData();
        }

        public applicationWindow(string ap, int bookid) //если заявка на книгу
        {
            InitializeComponent();
            applicationPurpose = ap;
            bookID = bookid;

            LoadData();
        }

        private void LoadData()
        {
            userFullNameTBl.Text = User.currentUser.FullName;

            var purposes = Core.Context.ApplicationPurpose.ToList();
            purposeCB.ItemsSource = purposes;

            if (!string.IsNullOrEmpty(applicationPurpose))
                purposeCB.SelectedItem = purposes.FirstOrDefault(p => p.Name == applicationPurpose);

            if (bookID != null)
            {
                var book = Core.Context.Book.FirstOrDefault(b => b.ID == bookID);
                if (book != null)
                {
                    bookPanel.Visibility = Visibility.Visible;
                    bookTBl.Text = book.Title;
                }
            }
        }

        private void sendApplicationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (purposeCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите цель заявки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(messageTB.Text))
            {
                MessageBox.Show("Введите комментарий.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (messageTB.Text.Length > 300)
            {
                MessageBox.Show("Комментарий не должен превышать 300 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedPurpose = purposeCB.SelectedItem as ApplicationPurpose;

            bool alreadyExists = Core.Context.Application.Any(a => a.UserID == User.currentUser.ID 
                                 && a.PurposeID == selectedPurpose.ID && a.StatusID == 1 
                                 && a.BookID == bookID);

            if (alreadyExists)
            {
                MessageBox.Show("У вас уже есть заявка на рассмотрении.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Application app = new Application()
            {
                UserID = User.currentUser.ID,
                PurposeID = selectedPurpose.ID,
                Message = messageTB.Text,
                CreateDate = DateTime.Now,
                StatusID = 1,
                BookID = bookID
            };
            Core.Context.Application.Add(app);
            Core.Context.SaveChanges();

            MessageBox.Show("Заявка успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
