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

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для RequirementWindow.xaml
    /// </summary>
    public partial class RequirementWindow : Window
    {
        List<NewNorm> norms = new List<NewNorm>();
        int currentNorm = -1;
        bool isRefresh;

        public RequirementWindow()
        {
            InitializeComponent();
            RefreshComboBoxes();
            InitializeGrid();
        }

        void RefreshGrid()
        {
            NormGrid.Items.Clear();

            foreach (NewNorm norm in norms)
            {
                NormGrid.Items.Add(norm);
            }
        }

        void InitializeGrid()
        {
            norms.Clear();

            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Norm norm in db.Norms)
                {
                    norms.Add(new NewNorm(norm));
                }
            }

            RefreshGrid();
        }

        void RefreshComboBoxes()
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                foreach (Product product in db.Products)
                {
                    ProductsComboBox.Items.Add(product.PName);
                }

                foreach (Requirement requirement in db.Requirements)
                {
                    RequirementComboBox.Items.Add(requirement.RName);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NormGrid.SelectedItem != null && !isRefresh)
            {
                norms[currentNorm].NValue = NValueTextBox.Text;
                RefreshGrid();
                NormGrid.SelectedIndex = currentNorm;
            }
        }

        private void TValueTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NValueTextBox.Text == "-")
                NValueTextBox.Text = "";
            NValueTextBox.SelectAll();
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NormGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NormGrid.Items.Count != 0)
            {
                if (NormGrid.SelectedIndex != -1)
                {
                    currentNorm = NormGrid.SelectedIndex;
                }
                isRefresh = true;
                NValueTextBox.Text = norms[currentNorm].NValue;
                ProductsComboBox.SelectedItem = norms[currentNorm].Product.PName;
                RequirementComboBox.SelectedItem = norms[currentNorm].Requirement.RName;
                isRefresh = false;
                NormGrid.SelectedIndex = currentNorm;
            }
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            SaveNorm();
            PopUpApi.ShowPopUp("Изменено успешно.");
            RefreshGrid();
            NormGrid.SelectedIndex = currentNorm;
        }

        private void SaveNorm()
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {

                foreach (NewNorm newNorm in norms)
                {
                    foreach (Norm norm in db.Norms)
                    {
                        if (newNorm.NormID == norm.NormID)
                        {
                            norm.NValue = newNorm.NValue;
                            norm.ProductID = newNorm.ProductID;
                            norm.RequirementID = newNorm.RequirementID;
                        }
                    }
                }

                db.SaveChanges();
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsComboBox.SelectedItem != null && RequirementComboBox.SelectedItem != null && !string.IsNullOrEmpty(NValueTextBox.Text))
            {
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    foreach (Requirement requirement in db.Requirements)
                    {
                        foreach (Product product in db.Products)
                        {
                            if (ProductsComboBox.SelectedItem.ToString() == product.PName && RequirementComboBox.SelectedItem.ToString() == requirement.RName)
                            {
                                db.Norms.Add(new Norm(new NewNorm(NValueTextBox.Text, product.ProductID, requirement.RequirementID)));
                                break;
                            }
                        }
                    }
                    db.SaveChanges();
                }

            }
            else
            {
                PopUpApi.ShowPopUp($"Для добавления записи необходимо заполнить все поля.");
            }
            InitializeGrid();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                RefreshGrid();
                NormGrid.SelectedIndex = -1;
                ProductsComboBox.SelectedIndex = -1;
                RequirementComboBox.SelectedIndex = -1;
                NValueTextBox.Text = "";
                currentNorm = -1;
            }
        }

        private void ProductsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NormGrid.SelectedItem != null && !isRefresh)
            {
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    foreach (Product product in db.Products)
                    {
                        if (product.PName == ProductsComboBox.SelectedItem.ToString())
                            ((NewNorm)NormGrid.SelectedItem).Product = product;
                    }
                }
                RefreshGrid();
            }
        }

        private void RequirementComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NormGrid.SelectedItem != null && !isRefresh)
            {
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    foreach (Requirement requirement in db.Requirements)
                    {
                        if (requirement.RName == RequirementComboBox.SelectedItem.ToString())
                            ((NewNorm)NormGrid.SelectedItem).Requirement = requirement;
                    }
                }
                RefreshGrid();
            }
        }

        private void NormGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                Norm delNorm;
                foreach (Norm norm in db.Norms)
                {
                    {
                    foreach (NewNorm newNorm in NormGrid.SelectedItems)
                        if (norm.NormID == newNorm.NormID)
                        {
                            delNorm = norm;
                            db.Norms.Remove(delNorm);
                        }
                    }
                }
                db.SaveChanges();
            }
            PopUpApi.ShowPopUp($"Удалено {NormGrid.SelectedItems.Count} записей.");
            InitializeGrid();
        }
    }
}
