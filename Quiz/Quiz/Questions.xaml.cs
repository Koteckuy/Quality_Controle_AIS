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

namespace Quiz
{
    /// <summary>
    /// Логика взаимодействия для Questions.xaml
    /// </summary>
    public partial class Questions : Window
    {
        DataBase dataBase;
        List<string> answers = new List<string>();
        TextBox[] textBoxes = new TextBox[4];
        CheckBox[] checkBoxes = new CheckBox[4];
        List<string> answersCorrectness = new List<string>();

        public Questions()
        {
            InitializeComponent();
            dataBase = new DataBase();
        }

        public Questions(DataBase dataBase)
        {
            InitializeComponent();
            this.dataBase = dataBase;
            textBoxes[0] = answerText1;
            textBoxes[1] = answerText2;
            textBoxes[2] = answerText3;
            textBoxes[3] = answerText4;
            checkBoxes[0] = answerCorrectness1;
            checkBoxes[1] = answerCorrectness2;
            checkBoxes[2] = answerCorrectness3;
            checkBoxes[3] = answerCorrectness4;
            questionText.Text = dataBase.LoadData($"SELECT QuestionText FROM Question WHERE TestCode={dataBase.mainTest}")[0];
            answers = dataBase.LoadData($"SELECT AnswerText FROM Question INNER JOIN Answer ON Question.Code=Answer.QuestionCode WHERE QuestionCode={dataBase.mainQuestion}");
            answersCorrectness = dataBase.LoadData($"SELECT Correctness FROM Question INNER JOIN Answer ON Question.Code=Answer.QuestionCode WHERE QuestionCode={dataBase.mainQuestion}");
            for (int i = 0; i < answers.Count; i++)
            {
                textBoxes[i].Text = answers[i];
                checkBoxes[i].IsChecked = answersCorrectness[i] == "1" ? true : false;
            }
        }

        private void saveQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            dataBase.EditQuestionData(dataBase.mainQuestion, questionText.Text);
        }
    }
}
