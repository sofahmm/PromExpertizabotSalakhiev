using ExpertizaWPF.DataBase;
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

namespace ExpertizaWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        private DataBaseService dbService = new DataBaseService();
        public static List<DataBase.Application> applist {  get; set; }
        public RequestPage()
        {
            InitializeComponent();
            applist = dbService.GetAllAplications();
            this.DataContext = this;
            //LoadRequests();
        }

        private void StatusReqCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (StatusReqCmb.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (t == "Новые")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Status == "Новая").ToList();
            else if (t == "Закрытые")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Status == "Закрыта").ToList();
            else if (t == "Показать всё")
                RequestsLv.ItemsSource = dbService.GetAllAplications();
        }

        private void ObjectCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (ObjectCmb.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (t == "Один")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "один").ToList();
            else if (t == "Два")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "два").ToList();
            else if (t == "Три")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "три").ToList();
            else if (t == "Четыре")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "четыре").ToList();
            else if (t == "Пять")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "пять").ToList();
            else if (t == "Два")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Object_Id == "шесть").ToList();
            else if (t == "Показать всё")
                RequestsLv.ItemsSource = dbService.GetAllAplications();
        }

        private void SortDateCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = (SortDateCmb.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (t == "По дате(по возрастанию)")
                RequestsLv.ItemsSource = dbService.GetAllAplications().OrderBy(i => i.Date_create);
            else if (t == "По дате(по убыванию)")
                RequestsLv.ItemsSource = dbService.GetAllAplications().OrderByDescending(i => i.Date_create);
            else if (t == "Показать всё")
                RequestsLv.ItemsSource = dbService.GetAllAplications();
        }

        private void SearchExecuterTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(SearchExecuterTb.Text != "")
                RequestsLv.ItemsSource = dbService.GetAllAplications().Where(i => i.Description != null && i.Description.ToLower()
                .Contains(SearchExecuterTb.Text.Trim().ToLower())).ToList();
            else
                RequestsLv.ItemsSource = dbService.GetAllAplications();
        }

        private void RequestsLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = RequestsLv.SelectedItem as DataBase.Application;
            Windows.AboutRequestWindow aboutRequestWindow = new Windows.AboutRequestWindow(t);
            aboutRequestWindow.Show();

        }
    }
}
