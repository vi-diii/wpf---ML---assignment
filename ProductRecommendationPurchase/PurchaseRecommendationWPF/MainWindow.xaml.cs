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
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using static Microsoft.ML.DataOperationsCatalog;
using SalesDataAccess;
using SalesDataModel;
using Microsoft.Win32;


namespace PurchaseRecommendationWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
         private MLContext MLContext;
        private TrainTestData trainTestData;
        private MatrixFactorizationTrainer.Options options;
        private MatrixFactorizationTrainer trainer;
        private ITransformer trainedModel;
        private void btnStep1_Click(object sender, RoutedEventArgs e)
        {
            MLContext = new MLContext();
            lblStep1Status.Content = "Mlcontext is created successul";
        }

        private void btnStep2_Click(object sender, RoutedEventArgs e)
        {
            List<DataEntry> dataEntryList = new DataEntryConnector().GetDataEntries();
            IDataView dataview = MLContext.Data.LoadFromEnumerable(dataEntryList);
            double trainRate = double.Parse(txtTrainRate.Text)/100;
            double testRate = 1- trainRate;
            trainTestData = MLContext.Data.TrainTestSplit(dataview, testFraction: testRate);
            lblStep2Status.Content = "Created Train test data successful!";
        }

        private void btnStep3_Click(object sender, RoutedEventArgs e)
        {
            options = new MatrixFactorizationTrainer.Options();
            options.MatrixColumnIndexColumnName = nameof(DataEntry.CustomerId);
            options.MatrixRowIndexColumnName = nameof(DataEntry.ProductID);
            options.LabelColumnName = "Label";
            options.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            options.Alpha = 0.01;
            options.Lambda = 0.025;
            options.C = 0.00001;
            lblStep3Status.Content = "Configuration Matrix Factorization Trainer successful!";

        }

        private void btnStep4_Click(object sender, RoutedEventArgs e)
        {
            trainer = MLContext.Recommendation().Trainers.MatrixFactorization(options);
            lblStep4Status.Content = "Create MatrixFactorization trainer successful!";
        }

        private void btnStep5_Click(object sender, RoutedEventArgs e)
        {
            trainedModel = trainer.Fit(trainTestData.TrainSet);
            lblStep5Status.Content = "Trained model successful!";
        }

        private void btnStep6_Click(object sender, RoutedEventArgs e)
        {
            IDataView prediction = trainedModel.Transform(trainTestData.TestSet);
            RegressionMetrics metrics = MLContext.Regression.Evaluate(prediction
                , labelColumnName:"Label", scoreColumnName:"Score");
            txtLossFunction.Text = metrics.LossFunction.ToString();
            txtMAE.Text = metrics.MeanAbsoluteError.ToString();
            txtMSE.Text = metrics.MeanSquaredError.ToString();
            txtRMSE.Text = metrics.RootMeanSquaredError.ToString();
        }

        private void btnStep7_Click(object sender, RoutedEventArgs e)
        {
            var predictionengine = MLContext.Model.CreatePredictionEngine<DataEntry, DataPrediction>(trainedModel);
            uint customerId = uint.Parse(txtUserId.Text);
            uint productId = uint.Parse(txtMovieId.Text);
            var prediction = predictionengine.Predict(
                new DataEntry()
                {
                    CustomerId = customerId,
                    ProductID = productId,
                }
                );
            FlowDocument mc = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run("For Customer Id = "));
            para.Inlines.Add(new Bold(new Run(customerId + " ")));
            para.Inlines.Add(new Run(" and product Id= "));
            para.Inlines.Add(new Bold(new Run(productId + " ")));
            para.Inlines.Add(new Run(" the predicted score = "));
            para.Inlines.Add(new Bold(new Run(Math.Round(prediction.Score, 1) + "")));
            mc.Blocks.Add(para);
            richTxtResult.Document = mc;

        }

        private void btnStep81_Click(object sender, RoutedEventArgs e)
        {
            // save model
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File .zip|*.zip";
            if (saveFileDialog.ShowDialog() == true)
            {
                MLContext.Model.Save(trainedModel, trainTestData.TrainSet.Schema, saveFileDialog.FileName);
                MessageBox.Show("Save trained  model successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }

        private void btnStep82_Click(object sender, RoutedEventArgs e)
        {
            // step 8.2: load model
            DataViewSchema modelSchema;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "File .zip|*.zip";
            if (openFileDialog.ShowDialog() == true)
            {
                if (MLContext == null)
                    MLContext = new MLContext();
                trainedModel = MLContext.Model.Load(openFileDialog.FileName, out modelSchema);
                MessageBox.Show("Load trained model successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }

        private void btnPredictionForAll_Click(object sender, RoutedEventArgs e)
        {
            ProductRecomendationWindow pwd = new ProductRecomendationWindow();
            pwd.Show();
        }
    }
}
