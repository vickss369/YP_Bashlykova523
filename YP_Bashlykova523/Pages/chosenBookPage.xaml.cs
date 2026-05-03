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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YP_Bashlykova523.Classes;
using static System.Collections.Specialized.BitVector32;

namespace YP_Bashlykova523.Pages
{
    /// <summary>
    /// Логика взаимодействия для chosenBookPage.xaml
    /// </summary>
    public partial class chosenBookPage : Page
    {
        private Book currentBook;

        public chosenBookPage(Book book)
        {
            InitializeComponent();
            currentBook = book;
            DataContext = currentBook;

            LoadReviews();
            LoadSections();
            LoadExtraBookInfo();
        }

        private void LoadReviews()
        {
            reviewsList.ItemsSource = Core.Context.Review.Where(r => r.BookID == currentBook.ID).ToList();
        }

        private void LoadSections()
        {
            sectionCB.ItemsSource = Core.Context.Section.ToList();

            var bookInRL = Core.Context.ReadingList.First(rl => rl.UserID == User.currentUser.ID && rl.BookID == currentBook.ID);
            sectionCB.SelectedItem = bookInRL.Section;
        }

        private void LoadExtraBookInfo()
        {
            genresTB.Text = "Жанры: " + string.Join(", ", Core.Context.BookGenre.Where(bg => bg.BookID == currentBook.ID).Select(bg => bg.Genre.Name).ToList());

            var ratings = Core.Context.Review.Where(r => r.BookID == currentBook.ID).Select(r => r.Rating).ToList();
            double avg = 0;

            if (ratings.Count > 0)
                avg = ratings.Sum() / (double)ratings.Count;

            ratingTB.Text = "Рейтинг: " + avg.ToString("0.0") + " / 10";
        }

        private bool CheckUser()
        {
            if (User.currentUser == null)
            {
                MessageBox.Show("Вы не зарегистрированы.", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);

                var main = Window.GetWindow(this) as MainWindow;
                main.MainFrame.Content = new enterPage();

                return false;
            }

            return true;
        }

        private void readBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            if (User.currentUser.IsFreeze)
            {
                MessageBox.Show("Вы заморожены.", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var main = Window.GetWindow(this) as MainWindow;
            main.MainFrame.Content = new readingBookPage(currentBook);
        }

        private void addToListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            if (User.currentUser.IsFreeze)
            {
                MessageBox.Show("Вы заморожены.", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Section section = sectionCB.SelectedItem as Section;
            if (section == null) return;

            bool exists = Core.Context.ReadingList.Any(rl => rl.UserID == User.currentUser.ID && rl.BookID == currentBook.ID);
            if (exists)
            {
                MessageBox.Show("Книга уже есть в вашем списке.", "ВНИМАНИЕ", MessageBoxButton.OK, MessageBoxImage.Warning);
                sectionCB.SelectedItem = null;
                return;
            }

            ReadingList rl1 = new ReadingList() {
                BookID = currentBook.ID,
                UserID = User.currentUser.ID,
                SectionID = section.ID
            };
            Core.Context.ReadingList.Add(rl1);
            Core.Context.SaveChanges();

            MessageBox.Show("Добавлено в список");
        }

        private void complaintBookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            bool hasBook = Core.Context.ReadingList.Any(rl => rl.UserID == User.currentUser.ID && rl.BookID == currentBook.ID);
            if (!hasBook)
            {
                MessageBox.Show("Вы не можете пожаловаться на книгу, которой нет в вашем списке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new complaintWindow("Book", currentBook.ID).ShowDialog();
        }

        private void complaintAuthorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            bool hasBook = Core.Context.ReadingList.Any(rl => rl.UserID == User.currentUser.ID && rl.BookID == currentBook.ID);
            if (!hasBook)
            {
                MessageBox.Show("Вы не можете пожаловаться на автора, пока книга не добавлена в ваш список.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new complaintWindow("Author", currentBook.User.ID).ShowDialog();
        }

        private void addReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            bool hasBook = Core.Context.ReadingList.Any(rl => rl.UserID == User.currentUser.ID && rl.BookID == currentBook.ID);
            if (!hasBook)
            {
                MessageBox.Show("Вы не можете оставить отзыв на книгу, которой нет в вашем списке.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool alreadyExists = Core.Context.Review.Any(r => r.UserID == User.currentUser.ID && r.BookID == currentBook.ID);
            if (alreadyExists)
            {
                MessageBox.Show("Вы уже оставляли отзыв на эту книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new reviewWindow(currentBook.ID).ShowDialog();
            LoadReviews();
            LoadExtraBookInfo();
        }

        private void complaintReviewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckUser()) return;

            Button btn = sender as Button;
            Review review = btn.DataContext as Review;

            new complaintWindow("Review", review.ID).ShowDialog();
        }
    }
}
