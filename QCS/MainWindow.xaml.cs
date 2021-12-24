using System.Windows;
using System.Windows.Controls;
using Windows.System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QCS
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Product currentProduct;
        public Batch currentBatch;
        public List<Requirement> currentRequirements = new List<Requirement>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                var products = db.Products;
                var batchs = db.Batchs;
                var requirements = db.Requirements;
                foreach (Product product in products)
                {
                    ProductGrid.Items.Add(product);
                }
                foreach (Batch batch in batchs)
                {
                    BatchGrid.Items.Add(batch);
                }
                foreach (Requirement requirement in requirements)
                {
                    RequirementGrid.Items.Add(requirement);
                }
            }
        }

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {
            if (ProductGrid.SelectedItem != null && BatchGrid.SelectedItem != null && RequirementGrid.SelectedItem != null)
                new TestWindow(currentProduct, currentBatch, currentRequirements).Show();
            else
                PopUpApi.ShowPopUp("Выберите продукт, партию и требования для проверки.");
        }

        private void HistoryButtonClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow().Show();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            ProductGrid.SelectedItem = currentProduct = null;
            BatchGrid.SelectedItem = currentBatch = null;
            RequirementGrid.SelectedItem = null;
            RefreshGrid();

        }

        private void ProductGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((currentProduct = (Product)ProductGrid.SelectedItem) != null)
                using (DataBase.UserContext db = new DataBase.UserContext())
                {
                    BatchGrid.Items.Clear();
                    RequirementGrid.Items.Clear();

                    var norms = (from norm in db.Norms
                                 where currentProduct.ProductID == norm.ProductID
                                 select norm).ToList();

                    var batchs = (from batch in db.Batchs
                                  where currentProduct.ProductID == batch.ProductID
                                  select batch).ToList();

                    foreach (Batch batch in batchs)
                    {
                        BatchGrid.Items.Add(batch);
                    }

                    foreach (Norm norm in norms)
                    {
                        foreach (Requirement requirement in db.Requirements)
                        {
                            if (norm.RequirementID == requirement.RequirementID)
                            {
                                RequirementGrid.Items.Add(requirement);
                                break;
                            }
                        }
                    }
                }
        }

        private void BatchGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            currentBatch = (Batch)BatchGrid.SelectedItem;
        }

        private void RequirementGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentRequirements.Clear();
            foreach (Requirement requirement in RequirementGrid.SelectedItems)
            {
                currentRequirements.Add(requirement);
            }
        }

        void RefreshGrid()
        {
            using (DataBase.UserContext db = new DataBase.UserContext())
            {
                ProductGrid.Items.Clear();
                BatchGrid.Items.Clear();
                RequirementGrid.Items.Clear();

                foreach (Product product in db.Products)
                {
                    ProductGrid.Items.Add(product);
                }

                foreach (Batch batch in db.Batchs)
                {
                    BatchGrid.Items.Add(batch);
                }

                foreach (Requirement requirement in db.Requirements)
                {
                    RequirementGrid.Items.Add(requirement);
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ProductGrid.SelectedIndex = -1;
                BatchGrid.SelectedIndex = -1;
                RequirementGrid.SelectedIndex = -1;
                RefreshGrid();
            }
        }

        private void AddNorm_Click(object sender, RoutedEventArgs e)
        {
            RequirementWindow requirementWindow = new RequirementWindow();
            requirementWindow.Show();
        }
    }
}
