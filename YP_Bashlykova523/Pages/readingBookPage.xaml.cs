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
    /// Логика взаимодействия для readingBookPage.xaml
    /// </summary>
    public partial class readingBookPage : Page
    {
        private Book currentBook;

        public readingBookPage(Book b)
        {
            InitializeComponent();
            currentBook = b;

            LoadText();
        }

        private void LoadText()
        {
            FlowDocument doc = new FlowDocument();

            Paragraph p = new Paragraph();
            p.Inlines.Add(new Run(currentBook.FullText));

            doc.Blocks.Add(p);

            bookFullTextRTB.Document = doc;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
