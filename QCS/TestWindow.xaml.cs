using ModernWpf.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow
    {
        List<NewTest> tests = new List<NewTest>();
        int currentTest = -1;
        bool isEdit;

        public TestWindow(Product product, Batch batch, List<Requirement> requirements)
        {
            InitializeComponent();

            foreach (Requirement requirement in requirements)
            {
                tests.Add(new NewTest(product, batch, requirement));
            }
        }

        public TestWindow(IList tests)
        {
            InitializeComponent();

            foreach (NewTest test in tests)
            {
                this.tests.Add(test);
            }

            isEdit = true;

            RefreshGrid();
        }

        private void TestGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TestGrid.SelectedIndex != -1)
            {
                tests[currentTest].TValue = TValueTextBox.Text;
                RefreshGrid();
                TestGrid.SelectedIndex = currentTest;
                CheckResult();
            }
        }

        void RefreshGrid()
        {
            TestGrid.Items.Clear();
            foreach (NewTest test in tests)
            {
                TestGrid.Items.Add(test);
            }
            if (currentTest != -1)
            {
                TestGrid.SelectedIndex = currentTest;
            }
        }

        private void TestGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestGrid.Items.Count != 0)
            {
                if (TestGrid.SelectedIndex != -1)
                {
                    currentTest = TestGrid.SelectedIndex;
                }
                CheckResult();
                TValueTextBox.Text = tests[currentTest].TValue;
                TestSubjectLabel.Content = tests[currentTest].Product.PName;
                TestRequirementLabel.Content = tests[currentTest].Requirement.RName;
            }
        }

        void CheckResult()
        {
            if ((TestGrid.SelectedItem as NewTest).Norm.NValue == ((NewTest)TestGrid.SelectedItem).TValue)
            {
                ResultLabel.Content = "Соответствует";
                ResultLabel.Background = Brushes.Green;
                ResultLabel.Foreground = Brushes.Black;
                (TestGrid.SelectedItem as NewTest).Result = true;
            }
            else
            {
                ResultLabel.Content = "Не соответствует";
                ResultLabel.Background = Brushes.Red;
                ResultLabel.Foreground = Brushes.White;
                (TestGrid.SelectedItem as NewTest).Result = false;
            }

        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            ChangeDate();
            RefreshGrid();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ChangeDate();
            RefreshGrid();
        }

        void ChangeDate()
        {
            if (TestDatePicker.SelectedDate != null)
                if ((bool)DateDefaultCheckbox.IsChecked)
                    foreach (NewTest test in TestGrid.Items)
                        test.TestDate = (DateTime)TestDatePicker.SelectedDate;
                else if (TestGrid.SelectedItem != null)
                    ((NewTest)TestGrid.SelectedItem).TestDate = (DateTime)TestDatePicker.SelectedDate;
        }

        void SaveTest()
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (NewTest test in tests)
                {
                    db.Tests.Add(new Test(test));
                }
                
                db.SaveChanges();
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            bool allowSave = true;
            foreach (NewTest test in tests)
            {
                if (test.TValue == "-" || string.IsNullOrEmpty(test.TValue))
                {
                    allowSave = false;
                    PopUpApi.ShowPopUp("Проведены не все тесты.");
                    break;
                }
            }

            if (allowSave)
            {
                if (isEdit)
                    EditTest();
                else
                    SaveTest();
                PopUpApi.ShowPopUp("Сохранено успешно.");
                Close();
            }
        }

        private void EditTest()
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (NewTest newTest in tests)
                {
                    foreach (Test test in db.Tests)
                    {
                        if (test.TestID == newTest.TestID)
                        {
                            test.TValue = newTest.TValue;
                            test.TestDate = newTest.TestDate;
                            test.Result = newTest.Result;
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TValueTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TValueTextBox.Text == "-")
                TValueTextBox.Text = "";
            TValueTextBox.SelectAll();
        }

        private void AddNorm_Click(object sender, RoutedEventArgs e)
        {
            RequirementWindow requirementWindow = new RequirementWindow();
            requirementWindow.Show();
        }
    }
}
    