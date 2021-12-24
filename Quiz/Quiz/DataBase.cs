using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz
{
    public class DataBase
    {
        OleDbConnection oleDbConnection;
        OleDbCommand command;
        OleDbDataReader oleDbDataReader;
        public int mainTest;
        public int mainQuestion;

        void InitializeDB()
        {
            oleDbConnection = new OleDbConnection();
            oleDbConnection.ConnectionString = ConfigurationManager.ConnectionStrings["Connection"].ToString();
            oleDbConnection.Open();
        }

        public List<string> LoadData(string selector)
        {
            List<string> lines = new List<string>();

            InitializeDB();

            command = new OleDbCommand();
            command.CommandText = selector;
            command.Connection = oleDbConnection;
            oleDbDataReader = command.ExecuteReader();

            while (oleDbDataReader.Read())
            {
                lines.Add(oleDbDataReader[0].ToString());
            }
            return lines;
        }

        public void SaveTestData(string testName, string testDescription, string testType, string testImage)
        {
            using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString()))
            {
                string query = $"INSERT INTO [Test] ([TestName],[Description],[Type],[Image]) VALUES (@testName,@testDescription,@testType,@testImage)";

                using (command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@testName", testName);
                    command.Parameters.AddWithValue("@testDescription", testDescription);
                    command.Parameters.AddWithValue("@testType", testType);
                    command.Parameters.AddWithValue("@testImage", testImage);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveQuestionData(int testCode, string questionText)
        {
            using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString()))
            {
                string query = $"INSERT INTO [Question] ([TestCode],[QuestionText]) VALUES (@testCode,@questionText)";

                using (command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@testCode", testCode);
                    command.Parameters.AddWithValue("@questionText", questionText);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EditQuestionData(int testCode, string questionText)
        {
            using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString()))
            {
                string query = $"UPDATE [Question] SET [TestCode] = @testCode, [QuestionText] = @questionText WHERE [Code]=@currentQuestion";

                using (command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@testCode", testCode);
                    command.Parameters.AddWithValue("@questionText", questionText);
                    command.Parameters.AddWithValue("@currentQuestion", mainQuestion);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveAnswerData(string testName, string testDescription, string testType, string testImage)
        {
            using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["Connection"].ToString()))
            {
                string query = $"INSERT INTO [Test] ([TestName],[Description],[Type],[Image]) VALUES (@testName,@testDescription,@testType,@testImage)";

                using (command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@testName'", testName);
                    command.Parameters.AddWithValue("@testDescription'", testDescription);
                    command.Parameters.AddWithValue("@testType'", testType);
                    command.Parameters.AddWithValue("@testImage'", testImage);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
