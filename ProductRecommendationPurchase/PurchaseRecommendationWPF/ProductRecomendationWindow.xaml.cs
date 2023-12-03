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
using Microsoft.ML;
using Microsoft.Win32;
using SalesDataAccess;
using SalesDataModel;

namespace PurchaseRecommendationWPF
{
    /// <summary>
    /// Interaction logic for ProductRecomendationWindow.xaml
    /// </summary>
    public partial class ProductRecomendationWindow : Window
    {
        public ProductRecomendationWindow()
        {
            InitializeComponent();
        }
        private MLContext mlContext;
        private ITransformer trainedModel;

        private void btnPickModel_Click(object sender, RoutedEventArgs e)
        {
            // step 8.2: load model
            DataViewSchema modelSchema;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "File .zip|*.zip";
            if (openFileDialog.ShowDialog() == true)
            {
                if (mlContext == null)
                    mlContext = new MLContext();
                trainedModel = mlContext.Model.Load(openFileDialog.FileName, out modelSchema);
                MessageBox.Show("Load trained model successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSelectedModel.Text = openFileDialog.FileName;
                ProcessListPrediciton();
            }
        }

        private void ProcessListPrediciton()
        {
           var predictionengine = mlContext.Model.CreatePredictionEngine<DataEntry, DataPrediction>(trainedModel);
            List<Customer> customers = new CustomerConnector().GetAllCustomer();
            List<Product> products = new ProductConnector().GetAllProducts();
            lvProductRecommendation.Items.Clear();
            foreach ( Customer customer in customers)
            {
                foreach (Product product in products)
                {
                    var ret = predictionengine.Predict(new DataEntry()
                    {
                        CustomerId = (uint)customer.CusTomerId,
                        ProductID = product.ProductId
                    });
                    ResultInfor infor = new ResultInfor();
                    infor.CustomerID = (uint)customer.CusTomerId;
                    infor.ContactName = customer.ContactName;
                    infor.ProductName = product.ProductName;
                    infor.Score = Math.Min(1, Math.Round(ret.Score, 1));
                    if (infor.Score >= 0.5)
                    {
                        infor.Decision = "recommendation";
                    }
                    lvProductRecommendation.Items.Add(infor);
                }
            }
        }
    }
}
