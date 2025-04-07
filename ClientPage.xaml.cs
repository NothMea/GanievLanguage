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

namespace GanievLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int itemsPerPage = 10; // Количество клиентов на одной странице
        int currentPage = 1; // Текущая страница (можно менять в зависимости от запроса)
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        private int recordsPerPage = 10;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;


        private void ChangePage(int direction, int? selectedPage)
        {
            var CurrentClient = GanievLanguageEntities.GetContext().Client.ToList();
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % recordsPerPage > 0)
            {
                CountPage = CountRecords / recordsPerPage + 1;
            }
            else
            {
                CountPage = CountRecords / recordsPerPage;
            }

            Boolean Ifupdate = true;
            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                    for (int i = CurrentPage * recordsPerPage; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }

                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                            for (int i = CurrentPage * recordsPerPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;


                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                            for (int i = CurrentPage * recordsPerPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;
                int totalClients = CurrentClient.Count;
                min = CurrentPage * recordsPerPage + recordsPerPage < CountRecords ? CurrentPage * recordsPerPage + recordsPerPage : CountRecords;
                TBCount.Text = CountRecords.ToString();
                TBAllRecords.Text = " из " + totalClients.ToString();

                ClientListView.ItemsSource = CurrentPageList;
                ClientListView.Items.Refresh();
            }
        }

        private void UpdateClient()
        {
            var CurrentClient = GanievLanguageEntities.GetContext().Client.ToList();
            string searchText = TBoxSearch.Text.ToLower().Replace(" ", "");

            // Фильтрация списка клиентов
            CurrentClient = CurrentClient.Where(p =>
                p.FirstName.Replace(" ", "").ToLower().Contains(searchText) ||
                p.LastName.Replace(" ", "").ToLower().Contains(searchText) ||
                p.Patronymic.Replace(" ", "").ToLower().Contains(searchText) ||
                (p.Phone != null && p.Phone.Replace(" ", "").Replace("(","").Replace("-","").Replace(")","").ToLower().Contains(searchText)) ||
                (p.Email != null && p.Email.Replace(" ", "").ToLower().Contains(searchText))
            ).ToList();

            if (ComboType.SelectedIndex == 0)
            {
                CurrentClient = CurrentClient.Where(p =>
                    p.Gender.Name.Contains("мужской") ||
                    p.Gender.Name.Contains("женский")
                ).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                CurrentClient = CurrentClient.Where(p => (p.Gender.Name.Contains("женский"))).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                CurrentClient = CurrentClient.Where(p => (p.Gender.Name.Contains("мужской"))).ToList();
            }
            if (ComboType2.SelectedIndex == 1)
            {
                CurrentClient = CurrentClient.OrderBy(p => p.FirstName).ToList();
            }

            if (ComboType2.SelectedIndex == 2)
            {
                CurrentClient = CurrentClient.OrderByDescending(p => p.LastDateTimeV).ToList();
            }
            if (ComboType2.SelectedIndex == 3)
            {
                CurrentClient = CurrentClient.OrderByDescending(p => p.VisitCount).ToList();
            }


            ClientListView.ItemsSource = CurrentClient.ToList();
            ClientListView.ItemsSource = CurrentClient;
            TableList = CurrentClient;
            ChangePage(0, 0);

        }

        public ClientPage()
        {
            InitializeComponent();
            var currentClients = GanievLanguageEntities.GetContext().Client.ToList();
            var currentClientServices = GanievLanguageEntities.GetContext().ClientService.ToList();
            for (int i = 0; i < currentClientServices.Count; i++)
            {

            }
            ClientListView.ItemsSource = currentClients;
            UpdateClient();
            RecordsPerPageComboBox.SelectedIndex = 0;
        }

        private void BTNDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            var currentClientServices = GanievLanguageEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно удалить клиента с посещениями");
            else
            {

                if (MessageBox.Show("Вы точно хотите удалить клиента?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        GanievLanguageEntities.GetContext().Client.Remove(currentClient);
                        GanievLanguageEntities.GetContext().SaveChanges();
                        ClientListView.ItemsSource = GanievLanguageEntities.GetContext().Client.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }

            }
            UpdateClient();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void RecordsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранного значения из ComboBox
            ComboBoxItem selectedItem = (ComboBoxItem)RecordsPerPageComboBox.SelectedItem;
            string selectedValue = selectedItem.Content.ToString();


            switch (selectedValue)
            {
                case "10":
                    recordsPerPage = 10;
                    break;
                case "50":
                    recordsPerPage = 50;
                    break;
                case "200":
                    recordsPerPage = 200;
                    break;
                case "Все":
                    recordsPerPage = CountRecords; 
                    break;
            }


            ChangePage(0, 0);
            UpdateClient();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClient();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void ComboType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClient();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void BTNEditClient_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Client));
            //NavigationService.Navigate(new AddEditPage());
        }
    }
}
