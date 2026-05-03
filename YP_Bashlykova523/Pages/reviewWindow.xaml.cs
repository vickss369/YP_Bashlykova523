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
    /// Логика взаимодействия для reviewWindow.xaml
    /// </summary>
    public partial class reviewWindow : Window
    {
        private int bookID;

        public reviewWindow(int bID)
        {
            InitializeComponent();
            bookID = bID;

            LoadData();
        }

        private void LoadData()
        {
            userFullNameTBl.Text = User.currentUser.FullName;

            var book = Core.Context.Book.First(b => b.ID == bookID);
            bookTitleTBl.Text = book.Title;

            var num = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ratingCB.ItemsSource = num;
        }

        private void saveReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ratingCB.SelectedItem == null || string.IsNullOrWhiteSpace(reviewCommentTB.Text))
            {
                MessageBox.Show("Не все поля отзыва заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (reviewCommentTB.Text.Length > 1000)
            {
                MessageBox.Show("Комментарий не должен превышать 1000 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Review rev = new Review()
            {
                UserID = User.currentUser.ID,
                BookID = bookID,
                Comment = reviewCommentTB.Text,
                Rating = Convert.ToInt32(ratingCB.SelectedItem),
                CreateDate = DateTime.Now,
            };
            Core.Context.Review.Add(rev);
            Core.Context.SaveChanges();

            MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
