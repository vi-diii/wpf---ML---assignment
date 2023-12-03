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
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.Recommender;
using Microsoft.ML.Transforms;
using Microsoft.ML;
using MovieDataAccess;
using MovieModel;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.Win32;
using Microsoft.ML.Trainers;

namespace WPFRecommendationClient
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
        private MLContext mlContext;
        private TrainTestData trainTestData;
        private EstimatorChain<ValueToKeyMappingTransformer> dataProcessingPipeline;
        private MatrixFactorizationTrainer.Options options;
        private EstimatorChain<MatrixFactorizationPredictionTransformer> trainingPipeLine;
        private ITransformer trainedModel;

        private void btnStep1_Click(object sender, RoutedEventArgs e)
        {
            // Step 1: create ml context to be shared across the model
            // creation workflow objects
            mlContext = new MLContext();
            lblStep1Status.Content = "MlContext is created successful";

        }

        private void btnStep2_Click(object sender, RoutedEventArgs e)
        {
            // Step 2: Read the training data which will be used to train the movie recommendation model.
            List<MovieRating> ratingList = new MovieRatingConnector().GetAllMovieRating();
            IDataView dataView = mlContext.Data.LoadFromEnumerable(ratingList);
            double trainRate = double.Parse(txtTrainRate.Text)/100;
            double testRate = 1- trainRate;
            trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: testRate);
            lblStep2Status.Content = "Created Train test data sucessful!";
        }

        private void btnStep3_Click(object sender, RoutedEventArgs e)
        {
            // step 3: Tranform your data by encoding the two features userID and movie ID.
            // these encođe features will be provided as input
            // to our MatrixFactorizationTrainer
            dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                outputColumnName: "userIdEncoded", inputColumnName: nameof(MovieRating.UserId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:"movieIdEncoded",
                inputColumnName:nameof(MovieRating.MovieID)));
            // Specify the options for MatrixFactorization Trainer
            options = new MatrixFactorizationTrainer.Options();
            options.MatrixColumnIndexColumnName = "userIdEncoded";
            options.MatrixRowIndexColumnName = "movieIdEncoded";
            options.LabelColumnName = "Label";
            options.NumberOfIterations = 20;
            options.ApproximationRank = 100;
            lblStep3Status.Content = "Transform data was sucessful";

        }

        private void btnStep4_Click(object sender, RoutedEventArgs e)
        {
            // step 4: create the training pipeline
            trainingPipeLine = dataProcessingPipeline.Append(mlContext.Recommendation().Trainers
                .MatrixFactorization(options));
            lblStep4Status.Content = "Created the training pipeline successful!";
        }

        private void btnStep5_Click(object sender, RoutedEventArgs e)
        {
            // step 5: train the model fitting to the dataset
            trainedModel = trainingPipeLine.Fit(trainTestData.TrainSet);
            lblStep5Status.Content = "Trained model successful!";
        }

        private void btnStep6_Click(object sender, RoutedEventArgs e)
        {
             IDataView      prediction =   trainedModel.Transform(trainTestData.TestSet);
            RegressionMetrics metrics =   mlContext.Regression.Evaluate(prediction,labelColumnName:"Label",scoreColumnName:"Score");
            txtLossFunction.Text = metrics.LossFunction.ToString();
            txtMAE.Text = metrics.MeanAbsoluteError.ToString();
            txtMSE.Text = metrics.MeanSquaredError.ToString();
            txtRMSE.Text = metrics.RSquared.ToString();
        }

        private void btnStep7_Click(object sender, RoutedEventArgs e)
        {
            // step 7: Test a single prediction by predicting a single movie rating for a specific user
            PredictionEngine<MovieRating, MovieRatingPrediction> predictionEngine =
               mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(trainedModel);
            int predictionuserId = int.Parse(txtUserId.Text);
            int predictionmiveId = int.Parse(txtMovieId.Text);
            MovieRatingPrediction movieRatingPrediction = predictionEngine.Predict(new MovieRating()
            {
                UserId = predictionuserId,      
                MovieID = predictionmiveId,

            });
            FlowDocument flowDocument = new FlowDocument();
            // create a paragraph with text
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run("For userId: "));
            paragraph.Inlines.Add(new Bold(new Run(predictionuserId + " ")));
            paragraph.Inlines.Add(new Run("movie rating prediction (1-5 stars) for movie: "));
            Movie mv = new MovieConnector().GetMovie(predictionmiveId);
            paragraph.Inlines.Add(new Bold(new Run(mv.Title)));
            paragraph.Inlines.Add(new Run(" is: "));
            paragraph.Inlines.Add(new Bold(new Run(Math.Round(movieRatingPrediction.Score,1) +"")));
            // add the paragraph to blocks of paragraph
            flowDocument.Blocks.Add(paragraph);
            richTxtResult.Document = flowDocument;
        }

        private void btnStep81_Click(object sender, RoutedEventArgs e)
        {
            // save model
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File .zip|*.zip";
            if(saveFileDialog.ShowDialog() == true)
            {
                mlContext.Model.Save(trainedModel, trainTestData.TrainSet.Schema, saveFileDialog.FileName);
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
                if (mlContext == null)
                    mlContext = new MLContext();
                trainedModel = mlContext.Model.Load(openFileDialog.FileName, out modelSchema);
                MessageBox.Show("Load trained model successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
