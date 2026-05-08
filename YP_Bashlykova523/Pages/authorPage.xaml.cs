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
    /// Логика взаимодействия для authorPage.xaml
    /// </summary>
    public partial class authorPage : Page
    {
        private List<Book> allAuthorBooks;
        private string currentMode;
        public authorPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            allAuthorBooks = Core.Context.Book.Where(b => b.AuthorID == User.currentUser.ID).ToList();
            var displayAllAuthorBooks = allAuthorBooks.Select(b => new
            {
                b.ID,
                b.Title,
                b.Annotation,
                b.ImagePath,
                b.IsFreeze,

                CardColor = b.IsFreeze ? "#A3A3A3" : "#F5D4BD",
                EditVisible = b.IsFreeze ? Visibility.Collapsed : Visibility.Visible,
                RequestVisible = b.IsFreeze ? Visibility.Visible : Visibility.Collapsed
            }).ToList();
            authorBooksList.ItemsSource = displayAllAuthorBooks;

            frozenBooksCB.SelectedIndex = 0;
            addBookBtn.Visibility = Visibility.Visible;
        }

        private void frozenBooksCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allAuthorBooks == null) return;

            var displayAllAuthorBooks = allAuthorBooks.Select(b => new
            {
                b.ID,
                b.Title,
                b.Annotation,
                b.ImagePath,
                b.IsFreeze,

                CardColor = b.IsFreeze ? "#A3A3A3" : "#F5D4BD",
                EditVisible = b.IsFreeze ? Visibility.Collapsed : Visibility.Visible,
                RequestVisible = b.IsFreeze ? Visibility.Visible : Visibility.Collapsed
            }).ToList();

            if (frozenBooksCB.SelectedIndex == 0)
            {
                authorBooksList.ItemsSource = displayAllAuthorBooks;
                addBookBtn.Visibility = Visibility.Visible;
            }
            else if (frozenBooksCB.SelectedIndex == 1)
            {
                authorBooksList.ItemsSource = displayAllAuthorBooks.Where(b => b.IsFreeze).ToList();
                addBookBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void addBookBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "добавление";

            var win = new authorEditWindow(currentMode, 0);
            win.ShowDialog();

            if (win.IsChanged) LoadData();
        }

        private void editBookBtn_Click(object sender, RoutedEventArgs e)
        {
            currentMode = "редактирование";

            var btn = sender as Button;
            if (btn == null) return;

            dynamic book = btn.DataContext;
            if (book == null) return;
            int id = book.ID;

            var win = new authorEditWindow(currentMode, id);
            win.ShowDialog();

            if (win.IsChanged) LoadData();
        }
        
        private void delBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic book = btn.DataContext;
            if (book == null) return;
            int id = book.ID;

            var realBook = Core.Context.Book.FirstOrDefault(b => b.ID == id);
            if (realBook != null)
            {
                var result = MessageBox.Show($"Удалить книгу {realBook.Title}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Core.Context.Book.Remove(realBook);
                    Core.Context.SaveChanges();

                    LoadData();
                }
            }
        }

        private void requestUnfreezeBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic book = btn.DataContext;
            if (book == null) return;

            var win = new applicationWindow("Разморозка книги", book.ID);
            win.ShowDialog();
        }
    }
}
