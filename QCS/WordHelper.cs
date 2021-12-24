using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace QCS
{
    class WordHelper
    {
        private FileInfo fileInfo;
        private bool doc;

        public WordHelper(bool test)
        {
            doc = test;
            if (doc)
            {
                if (File.Exists(@"C:\Users\MSI\source\repos\QCS\QCS\Certificate.docx"))
                {
                    fileInfo = new FileInfo(@"C:\Users\MSI\source\repos\QCS\QCS\Certificate.docx");
                }
                else
                {
                    throw new ArgumentException("Экземпляр сертификата не найден");
                }
            }
            else
            {
                if (File.Exists(@"C:\Users\MSI\source\repos\QCS\QCS\Act.docx"))
                {
                    fileInfo = new FileInfo(@"C:\Users\MSI\source\repos\QCS\QCS\Act.docx");
                }
                else
                {
                    throw new ArgumentException("Экземпляр акта не найден");
                }
            }
        }

        internal bool Process(Dictionary<string, string> items)
        {
            Word.Application app = null;

            try
            {
                app = new Word.Application();
                Object file = fileInfo.FullName;

                Object missing = Type.Missing;

                app.Documents.Open(file);

                foreach (var item in items)
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = item.Key;
                    find.Replacement.Text = item.Value;

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replace = Word.WdReplace.wdReplaceAll;

                    find.Execute(FindText: Type.Missing,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: missing,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: wrap,
                        Format: false,
                        ReplaceWith: missing, Replace: replace); 
                }


                Object newFileName;

                using (FolderBrowserDialog FBD = new FolderBrowserDialog())
                {
                    FBD.Description = "Выберите папку для сохранения";
                    DialogResult result = FBD.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(FBD.SelectedPath))
                    {
                        if (doc)
                        {
                            newFileName = FBD.SelectedPath + "\\Сертификат соответствия за " + DateTime.Now.ToString("dd.MM.yyyy") + ".docx";
                        }
                        else
                        {
                            newFileName = FBD.SelectedPath + "\\Акт выбраковки за " + DateTime.Now.ToString("dd.MM.yyyy HH/mm") + ".docx";
                        }

                        try
                        {
                            app.ActiveDocument.SaveAs2(newFileName);
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show(e.Message);
                        }

                    }
                }

                app.ActiveDocument.Close();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                if (app != null)
                    app.Quit();
            }

            return false;
        }
    }
}
