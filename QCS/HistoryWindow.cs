using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.IO;
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
using System.Windows.Forms;

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        NewTest currentNewTest = null;
        List<NewTest> tests = new List<NewTest>();
        bool search;

        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void HistoryGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        void RefreshGrid()
        {
            HistoryGrid.Items.Clear();
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Test test in db.Tests)
                {
                    HistoryGrid.Items.Add(new NewTest(test));
                }
            }
            foreach (NewTest test in HistoryGrid.Items)
            {
                tests.Add(test);
            }
        }

        private void CanselButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (HistoryGrid.SelectedItems != null)
            {
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    Test delTest;
                    foreach (NewTest newTest in HistoryGrid.SelectedItems)
                    {
                        foreach (Test test in db.Tests)
                        {
                            if (test.TestID == newTest.TestID)
                            {
                                delTest = test;
                                db.Tests.Remove(delTest);
                            }
                        }

                    }
                    db.SaveChanges();
                }
                PopUpApi.ShowPopUp($"Удалено {HistoryGrid.SelectedItems.Count} записей.");
                RefreshGrid();
            }
            else
            {
                PopUpApi.ShowPopUp("Записи для удаления не выбраны.");
            }

        }

        private void EditButtonClick(object sender, RoutedEventArgs e) 
        {
            if (HistoryGrid.SelectedItems != null) 
            {
                TestWindow testWindow = new TestWindow(HistoryGrid.SelectedItems);
                testWindow.Show();
                Close();
            }
            else
            {
                PopUpApi.ShowPopUp("Записи для изменения не выбраны.");
            }
    
        }

        private void ExcelButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ExcelHelper helper = new ExcelHelper())
                {
                    using (FolderBrowserDialog FBD = new FolderBrowserDialog())
                    {
                        FBD.Description = "Выберите папку для сохранения";
                        DialogResult result = FBD.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(FBD.SelectedPath))
                        {
                            if (helper.Open(filePath: System.IO.Path.Combine(FBD.SelectedPath, "Тесты " + DateTime.Now.ToString("dd.MM.yyyy") + ".xlsx")))
                            {
                                for (int i = 2; i < tests.Count + 2; i++)
                                {
                                    NewTest test = tests[i - 2];
                                    helper.Set(column: "A", row: 1, data: "Продукция");
                                    helper.Set(column: "B", row: 1, data: "Требование");
                                    helper.Set(column: "C", row: 1, data: "Норма");
                                    helper.Set(column: "D", row: 1, data: "Замер");
                                    helper.Set(column: "E", row: 1, data: "Дата");
                                    helper.Set(column: "F", row: 1, data: "Результат");
                                    helper.Set(column: "A", row: i, data: test.Product.PName);
                                    helper.Set(column: "B", row: i, data: test.Requirement.RName);
                                    helper.Set(column: "C", row: i, data: test.Norm.NValue);
                                    helper.Set(column: "D", row: i, data: test.TValue);
                                    helper.Set(column: "E", row: i, data: test.TestDate.ToString("dd MM yyyy"));
                                    helper.Set(column: "F", row: i, data: test.Result);
                                }

                                helper.Save();
                            }
                        }
                    }
                }
            }
            catch
            {
                PopUpApi.ShowPopUp("Ошибка при формировании документа.");
            }
        }

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            List<Test> tests = new List<Test>();
            bool result = true;

            if (HistoryGrid.SelectedItems.Count == 1)
            {
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    foreach (Test test in db.Tests)
                    {
                        if (test.BatchID == new Test(currentNewTest).BatchID)
                        {
                            tests.Add(test);
                        }
                    }

                    foreach (Test test in tests)
                    {
                        if (!test.Result)
                        {
                            result = false;
                            break;
                        }
                    }


                    if (result)
                    {
                        Certificate certivicate = db.Certificates.Add(new Certificate(new Test(currentNewTest)));
                        db.SaveChanges();

                        var helper = new WordHelper(true);

                        var items = new Dictionary<string, string>
                        {
                            { "<СertificateID>",  certivicate.СertificateID.ToString()},
                            { "<СertificateStartDate>",  certivicate.RegistrationDate.ToString("dd.MM.yyyy")},
                            { "<СertificateEndDate>",  certivicate.RegistrationDate.AddYears(1).ToString("dd.MM.yyyy")},
                            { "<ProductionDate>",  currentNewTest.Batch.ProductionDate.ToString("dd.MM.yyyy")},
                            { "<PName>",  currentNewTest.Product.PName},
                            { "<ProductID>",  currentNewTest.Product.ProductID.ToString()},
                            { "<BatchID>",  certivicate.BatchID.ToString()},
                        };

                        helper.Process(items);

                        PopUpApi.ShowPopUp("Сертификат сформирован успешно.");
                    }
                    else
                    {
                        Act act = db.Acts.Add(new Act(new Test(currentNewTest)));
                        db.SaveChanges();

                        var helper = new WordHelper(false);

                        var items = new Dictionary<string, string>
                        {
                            { "<ActID>",  act.ActID.ToString()},
                            { "<ActDate>",  act.RegistrationDate.ToString("dd.MM.yyyy")},
                            { "<ProductionDate>",  currentNewTest.Batch.ProductionDate.ToString("dd.MM.yyyy")},
                            { "<PName>",  currentNewTest.Product.PName},
                            { "<ProductID>",  currentNewTest.Product.ProductID.ToString()},
                            { "<BatchID>",  act.BatchID.ToString()},
                        };

                        helper.Process(items);

                        PopUpApi.ShowPopUp("Акт сформирован успешно.");
                    }
                }
            }
            else
            {
                PopUpApi.ShowPopUp("Для формирования документа необходимо выбрать только 1 тест.");
            }
        }

        private void HistoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentNewTest = (NewTest)HistoryGrid.SelectedItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChartWindow _ = new ChartWindow();
            _.Show();
        }

        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
                
            /*searched.Clear();
            searched = tests.Where(test => test.Product.PName.ToLower().Contains(SearchBox.Text.ToLower())).ToList();


            if (searched.Count == 0)
                HistoryGrid.SelectedItems.Clear();
            else
            {
                HistoryGrid.SelectedItems.Clear();
                foreach (NewTest item in searched)
                {
                    if (!string.IsNullOrEmpty(SearchBox.Text))
                        HistoryGrid.SelectedItems.Add(item);
                    else
                        HistoryGrid.SelectedItems.Clear();
                }
            }*/
        }

        private void FilterBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            List<NewTest> filtered = tests.Where(test => test.Product.PName.ToLower().Contains(FilterBox.Text.ToLower())).ToList();

            if (!search)
                HistoryGrid.Items.Clear();
            HistoryGrid.ItemsSource = filtered;
            HistoryGrid.AutoGenerateColumns = false;
            search = true;
        }

        private void AddNorm_Click(object sender, RoutedEventArgs e)
        {
            RequirementWindow requirementWindow = new RequirementWindow();
            requirementWindow.Show();
        }
    }
}
