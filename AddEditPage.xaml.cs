using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace GanievLanguage
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentClient = new Client();
        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();
            if (SelectedClient != null)
            {
                this._currentClient = SelectedClient;
                if (_currentClient.GenderCode == "м")
                    RBtnMal.IsChecked = true;
                else
                    RBtnFem.IsChecked = true;

            }
            else
            {
                TBID.Text = (GanievLanguageEntities.GetContext().Client.OrderByDescending(x => x.ID).First().ID + 1).ToString();
            }
        

            DataContext = _currentClient;

            //RBtnFem.IsChecked = cli == "ж";
            //RBtnMal.IsChecked = cli == "м";


        }

        private void BtnEditPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {

                FileInfo fileInfo = new FileInfo(myOpenFileDialog.FileName);


                if (fileInfo.Length < 2 * 1024 * 1024)
                {
                    _currentClient.PhotoPath = myOpenFileDialog.FileName;
                    Photo.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
                }
                else
                {

                    MessageBox.Show("Размер файла превышает 2 мегабайта. Выберите другой файл.");
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(_currentClient.LastName) ||
                !_currentClient.LastName.All(c => char.IsLetter(c) || c == ' ' || c == '-') ||
                _currentClient.LastName.Length > 50)
            {
                errors.AppendLine("Фамилия может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов.");
            }

            // Проверка имени
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName) ||
                !_currentClient.FirstName.All(c => char.IsLetter(c) || c == ' ' || c == '-') ||
                _currentClient.FirstName.Length > 50)
            {
                errors.AppendLine("Имя может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов.");
            }

            // Проверка отчества
            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic) ||
                !_currentClient.Patronymic.All(c => char.IsLetter(c) || c == ' ' || c == '-') ||
                _currentClient.Patronymic.Length > 50)
            {
                errors.AppendLine("Отчество может содержать только буквы, пробелы и дефисы, и не может быть длиннее 50 символов.");
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(_currentClient.Email))
            {
                errors.AppendLine("Email не может быть пустым.");
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(_currentClient.Email);
                    // Проверка, что доменная часть содержит минимум две буквы после точки
                    if (addr.Address != _currentClient.Email ||
                        addr.Host.Split('.').Length < 2 ||
                        addr.Host.Split('.').Last().Length < 2)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    errors.AppendLine("Укажите правильный email агента.");
                }
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
            {
                errors.AppendLine("Телефон не может быть пустым.");
            }
            else
            {
                string phonePattern = @"^[0-9() ]+$"; // разрешаем цифры, скобки, пробелы и дефисы
                if (!Regex.IsMatch(_currentClient.Phone, phonePattern))
                {
                    errors.AppendLine("Телефон может содержать только цифры, минусы, скобки и пробелы.");
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClient.GenderCode = RBtnMal.IsChecked == true ? "м" : "ж";

            if (_currentClient.ID == 0)
            {
                _currentClient.ID = Convert.ToInt32(TBID.Text);
                _currentClient.LastName = TBLastName.Text;
                _currentClient.FirstName = TBLastName.Text;
                _currentClient.Patronymic = TBPatron.Text;
                _currentClient.Email = TBEmail.Text;
                _currentClient.Phone = TBNumber.Text;
                DateTime var = DateTime.Now;
                DateTime.TryParse(TBBirthday.Text, out var);
                _currentClient.Birthday = var;
                _currentClient.RegistrationDate = DateTime.Now;
                if (RBtnFem.IsChecked == true)
                {
                    _currentClient.GenderCode = "ж";
                }
                else
                {
                    _currentClient.GenderCode = "м";
                }
                _currentClient.PhotoPath = "/Resources/Клиенты/m99.png";
                GanievLanguageEntities.GetContext().Client.Add(_currentClient);
            }
            try
            {
                GanievLanguageEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
