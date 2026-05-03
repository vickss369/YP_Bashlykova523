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
    /// Логика взаимодействия для complaintWindow.xaml
    /// </summary>
    public partial class complaintWindow : Window
    {
        private string mode;
        private int targetID;

        public complaintWindow(string m, int tID)
        {
            InitializeComponent();

            mode = m;
            targetID = tID;

            LoadData();
        }

        private void LoadData()
        {
            userFullNameTBl.Text = User.currentUser.FullName;

            if (mode == "Book")
            {
                complaintTitleTBl.Text += " книгу";
                complaintTBl.Text = "Книга:";

                var book = Core.Context.Book.First(b => b.ID == targetID);
                complaintObjectTBl.Text = book.Title;
            }
            else if (mode == "Author")
            {
                complaintTitleTBl.Text += " автора";
                complaintTBl.Text = "Автор:";

                var author = Core.Context.User.First(a => a.ID == targetID);
                complaintObjectTBl.Text = author.FullName;
            }
            else
            {
                complaintTitleTBl.Text += " отзыв";
                complaintTBl.Text = "№ отзыва:";

                var review = Core.Context.Review.First(r => r.ID == targetID);
                complaintObjectTBl.Text = review.ID.ToString();
            }
        }

        private void sendComplaintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(complaintReasonTB.Text))
            {
                MessageBox.Show("Не все поля жалобы заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (complaintReasonTB.Text.Length > 300)
            {
                MessageBox.Show("Описание причины не должно превышать 300 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool alreadyExists = false;

            if (mode == "Review")
            {
                alreadyExists = Core.Context.Complaint.Any(c => c.UserID == User.currentUser.ID && c.ReviewID == targetID);
            }
            else
            {
                alreadyExists = Core.Context.Complaint.Any(c => c.UserID == User.currentUser.ID &&
                                                           c.BookID == targetID && c.ReviewID == null);
            }

            if (alreadyExists)
            {
                MessageBox.Show("Вы уже отправляли жалобу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Complaint cmpl = new Complaint() {
                UserID = User.currentUser.ID,
                Reason = complaintReasonTB.Text,
                StatusID = 1,
                CreateDate = DateTime.Now
            };

            if (mode == "Book")
            {
                cmpl.BookID = targetID;
                cmpl.ReviewID = null;
            }
            else if (mode == "Author")
            {
                cmpl.BookID = targetID;
                cmpl.ReviewID = null;

                cmpl.Reason = "[Жалоба на автора]\n" + cmpl.Reason;
            }
            else
            {
                cmpl.BookID = null;
                cmpl.ReviewID = targetID;
            }

            Core.Context.Complaint.Add(cmpl);
            Core.Context.SaveChanges();

            MessageBox.Show("Жалоба успешно отправлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
