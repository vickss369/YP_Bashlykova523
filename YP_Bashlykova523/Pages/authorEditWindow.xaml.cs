using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
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
    /// Логика взаимодействия для authorEditWindow.xaml
    /// </summary>
    public partial class authorEditWindow : Window
    {
        private string mode;
        private int id;

        private List<int> selectedGenreIds = new List<int>();

        public bool IsChanged { get; private set; } = false;

        public authorEditWindow(string mode, int id)
        {
            InitializeComponent();

            this.id = id;
            this.mode = mode;

            LoadData();
        }

        private void LoadData()
        {
            var allGenres = Core.Context.Genre.ToList();
            genresList.ItemsSource = allGenres;
            selectedGenreIds.Clear();

            if (mode == "редактирование")
            {
                titleTBl.Text = "Редактирование книги";

                var book = Core.Context.Book.FirstOrDefault(b => b.ID == id);
                if (book == null) return;

                bookTitleTB.Text = book.Title;
                imagePathTB.Text = book.ImagePath;
                annotationTB.Text = book.Annotation;
                fullTextTB.Text = book.FullText;

                selectedGenreIds = book.BookGenre.Select(bg => bg.GenreID).ToList();
                UpdateSelectedGenresText();
            }

            else if (mode == "добавление")
            {
                titleTBl.Text = "Добавление книги";

                UpdateSelectedGenresText();
                selectedGenreIds.Clear();
            }
        }

        private void genreChB_Loaded(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var genre = cb.DataContext as Genre;
            if (genre == null) return;

            cb.IsChecked = selectedGenreIds.Contains(genre.ID);
        }

        private void genreChB_Checked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            var genre = cb.DataContext as Genre;
            if (genre == null) return;

            if (cb.IsChecked == true)
            {
                if (!selectedGenreIds.Contains(genre.ID))
                    selectedGenreIds.Add(genre.ID);
            }

            else
                selectedGenreIds.Remove(genre.ID);

            UpdateSelectedGenresText();
        }

        private void UpdateSelectedGenresText()
        {
            var names = Core.Context.Genre.Where(g => selectedGenreIds.Contains(g.ID)).Select(g => g.Name).ToList();
            selectedGenresTBl.Text = names.Count > 0 ? string.Join(", ", names) : "не выбрано";
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bookTitleTB.Text) || string.IsNullOrWhiteSpace(imagePathTB.Text)
                || string.IsNullOrWhiteSpace(annotationTB.Text) || string.IsNullOrWhiteSpace(fullTextTB.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (mode == "редактирование")
            {
                var book = Core.Context.Book.FirstOrDefault(b => b.ID == id);
                if (book == null) return;

                book.Title = bookTitleTB.Text;
                book.ImagePath = imagePathTB.Text;
                book.Annotation = annotationTB.Text;
                book.FullText = fullTextTB.Text;

                var oldGenres = book.BookGenre.ToList();
                Core.Context.BookGenre.RemoveRange(oldGenres);

                foreach (var genreId in selectedGenreIds)
                {
                    Core.Context.BookGenre.Add(new BookGenre
                    {
                        BookID = book.ID,
                        GenreID = genreId
                    });
                }
            }

            else if (mode == "добавление")
            {
                var b = new Book
                {
                    Title = bookTitleTB.Text,
                    ImagePath = imagePathTB.Text,
                    Annotation = annotationTB.Text,
                    FullText = fullTextTB.Text,
                    AuthorID = User.currentUser.ID,
                    IsFreeze = false
                };

                Core.Context.Book.Add(b);
                Core.Context.SaveChanges();

                foreach (var genreID in selectedGenreIds)
                {
                    Core.Context.BookGenre.Add(new BookGenre
                    {
                        BookID = b.ID,
                        GenreID = genreID
                    });
                }
            }
            Core.Context.SaveChanges();

            IsChanged = true;
            Close();
        }
    }
}
