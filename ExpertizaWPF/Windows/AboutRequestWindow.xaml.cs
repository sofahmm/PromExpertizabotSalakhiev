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
using System.Windows.Shapes;

namespace ExpertizaWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для AboutRequestWindow.xaml
    /// </summary>
    public partial class AboutRequestWindow : Window
    {
        public static DataBase.Application application1 = new DataBase.Application();
        public AboutRequestWindow(DataBase.Application application)
        {
            InitializeComponent();
            application1 = application;

            UserNameTbl.Text = application1.User;
            if(application1.Status == "Закрыта")
            {
                StatusTbl.Foreground = Brushes.Green;
            }
            else
            {
                StatusTbl.Foreground = Brushes.Red;
            }
            StatusTbl.Text = application1.Status;
            IdRequestTbl.Text = application1.Id.ToString();
            ExecutorNameTbl.Text = application1.Executor;
            DescriptionTbl.Text = application1.Description; 
            DateCreateTbl.Text = application1.Date_create;
            DateCloseTbl.Text = application1.Date_close;
        }
    }
}
