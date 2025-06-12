using ClosedXML.Excel;
using ExpertizaWPF.DataBase;
using ExpertizaWPF.DataBase;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpertizaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            NavFrame.NavigationService.Navigate(new Pages.RequestPage());
        }


        private void requestBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void reportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = new DataBaseService();
                var allApps = db.GetAllAplications();

                // Отфильтруем только незавершённые заявки
                var openApps = allApps
                    .Where(a => a.Status != null &&
                                (a.Status == "Новая"))
                    .ToList();

                if (openApps.Count == 0)
                {
                    MessageBox.Show("Нет незавершённых заявок.");
                    return;
                }

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Незавершённые заявки");

                // Заголовки
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Пользователь";
                ws.Cell(1, 3).Value = "Исполнитель";
                ws.Cell(1, 4).Value = "Тип";
                ws.Cell(1, 5).Value = "Объект";
                ws.Cell(1, 6).Value = "Описание";
                ws.Cell(1, 7).Value = "Статус";
                ws.Cell(1, 8).Value = "Дата создания";

                // Данные
                for (int i = 0; i < openApps.Count; i++)
                {
                    var app = openApps[i];
                    ws.Cell(i + 2, 1).Value = app.Id;
                    ws.Cell(i + 2, 2).Value = app.User;
                    ws.Cell(i + 2, 3).Value = app.Executor;
                    ws.Cell(i + 2, 4).Value = app.Type;
                    ws.Cell(i + 2, 5).Value = app.Object_Id;
                    ws.Cell(i + 2, 6).Value = app.Description;
                    ws.Cell(i + 2, 7).Value = app.Status;
                    ws.Cell(i + 2, 8).Value = app.Date_create;
                }

                ws.Columns().AdjustToContents();

                // Сохраняем
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Сохранить отчёт",
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Незавершённые_заявки_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    wb.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show($"Отчёт сохранён:\n{saveFileDialog.FileName}");
                }
                else
                {
                    MessageBox.Show("Сохранение отменено.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте: " + ex.Message);
            }

        }

        private void report1Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = new DataBaseService();
                var allApps = db.GetAllAplications();

                // Фильтрация по сегодняшнему дню
                var today = DateTime.Today;
                var todayApps = allApps
                    .Where(a => DateTime.TryParse(a.Date_create, out var date)
                                && date.Date == today)
                    .ToList();

                if (todayApps.Count == 0)
                {
                    MessageBox.Show("Сегодня не было заявок.");
                    return;
                }

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Заявки за сегодня");

                // Заголовки
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Пользователь";
                ws.Cell(1, 3).Value = "Исполнитель";
                ws.Cell(1, 4).Value = "Тип";
                ws.Cell(1, 5).Value = "Объект";
                ws.Cell(1, 6).Value = "Описание";
                ws.Cell(1, 7).Value = "Статус";
                ws.Cell(1, 8).Value = "Дата создания";
                ws.Cell(1, 9).Value = "Дата закрытия";

                // Данные
                for (int i = 0; i < todayApps.Count; i++)
                {
                    var app = todayApps[i];
                    ws.Cell(i + 2, 1).Value = app.Id;
                    ws.Cell(i + 2, 2).Value = app.User;
                    ws.Cell(i + 2, 3).Value = app.Executor;
                    ws.Cell(i + 2, 4).Value = app.Type;
                    ws.Cell(i + 2, 5).Value = app.Object_Id;
                    ws.Cell(i + 2, 6).Value = app.Description;
                    ws.Cell(i + 2, 7).Value = app.Status;
                    ws.Cell(i + 2, 8).Value = app.Date_create;
                    ws.Cell(i + 2, 9).Value = app.Date_close;
                }

                ws.Columns().AdjustToContents();

                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Сохранить отчёт за сегодня",
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    FileName = $"Заявки_за_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    wb.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show($"Отчёт успешно сохранён:\n{saveFileDialog.FileName}");
                }
                else
                {
                    MessageBox.Show("Сохранение отменено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте: " + ex.Message);

            }
        }
    }
}