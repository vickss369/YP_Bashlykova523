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
    /// Логика взаимодействия для readingListPage.xaml
    /// </summary>
    public partial class readingListPage : Page
    {
        private List<ReadingList> allRecords;
        private List<Book> allBooks = new List<Book>();

        private int currentSectionID = 0;
        private Button activeSectionButton = null;

        public readingListPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (User.currentUser == null) return;

            allRecords = Core.Context.ReadingList.Where(r => r.UserID == User.currentUser.ID).ToList();

            allBooks = allRecords.Select(r => r.Book).ToList();
            booksList.ItemsSource = allBooks;

            var genres = Core.Context.Genre.ToList();
            genres.Insert(0, new Genre { ID = 0, Name = "Все жанры" });
            genreCB.ItemsSource = genres;
            genreCB.SelectedIndex = 0;

            titleCB.SelectedIndex = 0;
            ratingCB.SelectedIndex = 0;

            sectionList.ItemsSource = Core.Context.Section.ToList();

            activeSectionButton = allSectionsBtn;
        }

        private void sectionBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (activeSectionButton != null)
            {
                if (activeSectionButton == allSectionsBtn)
                    activeSectionButton.Background = (Brush)new BrushConverter().ConvertFrom("#A8CCA3");
                else
                    activeSectionButton.Background = (Brush)new BrushConverter().ConvertFrom("#8A9AAD");
            }

            btn.Background = (Brush)new BrushConverter().ConvertFrom("#A2676C");
            activeSectionButton = btn;

            if (btn == allSectionsBtn)
            {
                currentSectionID = 0;
            }
            else
            {
                Section section = btn.DataContext as Section;
                if (section != null)
                    currentSectionID = section.ID;
            }

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var records = allRecords;

            if (currentSectionID != 0)
                records = records.Where(r => r.SectionID == currentSectionID).ToList();

            var books = records.Select(r => r.Book).ToList();

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
            var book = btn.DataContext as Book;
            if (book == null) return;

            NavigationService.Navigate(new chosenBookPage(book));
        }
    }
}
