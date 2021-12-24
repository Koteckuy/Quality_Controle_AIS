using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Quiz
{
    /// <summary>
    /// Логика взаимодействия для AddTest.xaml
    /// </summary>
    public partial class AddTest : Window
    {
        string type;
        List<string> questions = new List<string>();
        List<string> test = new List<string>();
        DataBase dataBase;


        public AddTest()
        {
            InitializeComponent();
            dataBase = new DataBase();
        }
        public AddTest(DataBase dataBase) 
        {
            InitializeComponent();
            this.dataBase = dataBase;
            questions = dataBase.LoadData($"SELECT QuestionText FROM Test INNER JOIN Question ON Test.Code=Question.TestCode WHERE TestCode={dataBase.mainTest}");
            foreach (string question in questions)
                questionList.Items.Add(question);
            testName.Text = dataBase.LoadData($"SELECT TestName FROM Test WHERE Code={dataBase.mainTest}")[0];
            testDescription.Text = dataBase.LoadData($"SELECT Description FROM Test WHERE Code={dataBase.mainTest}")[0];
            if (dataBase.LoadData($"SELECT Type FROM Test WHERE Code={dataBase.mainTest}")[0] == "2")
                testTypeBlitz.IsChecked = true;
            else
                testTypeSimple.IsChecked = true;
        }

        void saveTestButton_Click(object sender, RoutedEventArgs e)
        {
            type = testTypeSimple.IsChecked == true ? "0" : "1";

            DataBase dataBase = new DataBase();
            dataBase.SaveTestData(testName.Text, testDescription.Text, type, "123");
        }

        void EditQuestion()
        {
            DataBase dataBase = new DataBase();
            testName.Text = dataBase.LoadData("SELECT TestName FROM Test")[0];
        }

        private void editQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            dataBase.mainQuestion = Int32.Parse(dataBase.LoadData($"SELECT Code FROM Question WHERE QuestionText = '{questionList.SelectedItem.ToString()}'")[0]);
            Questions questionForm = new Questions(dataBase);
            questionForm.Show();
        }

        private void addTestButton_Click(object sender, RoutedEventArgs e)
        {
            Questions questionForm = new Questions();
            questionForm.Show();
        }

        private void deleteQuestionButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
