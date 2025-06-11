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
        //private void LoadRequests()
        //{
        //    var requests = 
        //    RequestsLv.ItemsSource = requests;  
        //}
    }
}
