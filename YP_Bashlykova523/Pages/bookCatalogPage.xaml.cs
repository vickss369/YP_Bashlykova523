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
    /// Логика взаимодействия для bookCatalogPage.xaml
    /// </summary>
    public partial class bookCatalogPage : Page
    {
        private List<Book> allBooks = new List<Book>();
        private Book selectedBook;

        public bookCatalogPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            allBooks = Core.Context.Book.ToList();
            booksList.ItemsSource = allBooks;

            var genres = Core.Context.Genre.ToList();
            genres.Insert(0, new Genre { ID = 0, Name = "Все жанры" });
            genreCB.ItemsSource = genres;
            genreCB.SelectedIndex = 0;

            titleCB.SelectedIndex = 0;
            ratingCB.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var books = allBooks;

            if (!string.IsNullOrWhiteSpace(searchTB.Text))
            {
                string text = searchTB.Text.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(text) || b.User.FullName.ToLower().Contains(text)).ToList();
            }

            if (genreCB.SelectedItem is Genre genre && genre.ID != 0)
            {
                var bookIds = Core.Context.BookGenre.Where(bg => bg.GenreID == genre.ID).Select(bg => bg.BookID).ToList();
                books = books.Where(b => bookIds.Contains(b.ID)).ToList();
            }

            if (titleCB.SelectedIndex == 1)
                books = books.OrderBy(b => b.Title).ToList();

            if (titleCB.SelectedIndex == 2)
                books = books.OrderByDescending(b => b.Title).ToList();

            if (ratingCB.SelectedIndex == 1)
            {
                books = books.OrderBy(b => Core.Context.Review.Where(r => r.BookID == b.ID).Select(r => (int?)r.Rating).Average() ?? 0).ToList();
            }

            if (ratingCB.SelectedIndex == 2)
            {
                books = books.OrderByDescending(b => Core.Context.Review.Where(r => r.BookID == b.ID).Select(r => (int?)r.Rating).Average() ?? 0).ToList();
            }

            booksList.ItemsSource = books;
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void genreCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void titleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ratingCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void toBookDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            dynamic item = btn.DataContext;
            var book = allBooks.FirstOrDefault(b => b.ID == item.ID);
            if (book == null) return;

            selectedBook = book;
            NavigationService.Navigate(new chosenBookPage(selectedBook));
        }

        /*private void toBookDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var book = btn.DataContext as Book;
            if (book == null) return;

            selectedBook = book;

            var main = Window.GetWindow(this) as MainWindow;
            main.MainFrame.Content = new chosenBookPage(selectedBook);
        }*/
    }
}
