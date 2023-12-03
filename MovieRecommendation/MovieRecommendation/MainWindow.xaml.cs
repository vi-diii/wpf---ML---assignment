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
using Microsoft.ML.Trainers.Recommender;
using Microsoft.ML.Transforms;
using static Microsoft.ML.DataOperationsCatalog ;
using Microsoft.Win32;
using MovieRecommendation.Model;
using System.IO;
using System.Windows.Markup;

namespace MovieRecommendation
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
        private static string DatasetRelativePath = @"Data/ml-latest-small";
        private static string RatingRelativePath = $"{DatasetRelativePath}/rating.csv";
        private MLContext mlContext;
        private TrainTestData trainTestData;
        private EstimatorChain<ValueToKeyMappingTransformer> dataProcessingPipeline;
        private MatrixFactorizationTrainer.Options options;
        private EstimatorChain<MatrixFactorizationPredictionTransformer> trainingPipeLine;
        private ITransformer trainedModel;

        private void step1_Click(object sender, RoutedEventArgs e)
        {
            // Step 1: create mlcontext to be shared across the model
            // creation workflow objects
            mlContext = new MLContext();
            lblStep1.Content = "MLContext is created successful";


        }

        private void step2_Click(object sender, RoutedEventArgs e)
        {
            // Step 2: Read the training data which will be used to train
            // the movie recommendation model. The shema for training data
            // is defined by type 'TInput' in LoadFromTextFile<TInput>() method.
            IDataView dataView = mlContext.Data.LoadFromTextFile<MovieRating>("D:\\Zoom\\MovieRecommendation\\MovieRecommendation\\Data\\ml-latest-small\\ratings.csv", hasHeader: true, separatorChar: ',');
            double trainRate = double.Parse(txtTrain.Text)/100;
            double testRate = 1- trainRate;
            trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: testRate);
            lblStep2.Content = "Created train test data sucessful";

        }

        private void step3_Click(object sender, RoutedEventArgs e)
        {
            // Step 3: Trainform your data by encoding the two features UserId and MovieID.
            // These encoded features will be provided as input
            // to our MatrixFactorizationTrainer
            dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey
                (outputColumnName: "userIdEncoded", inputColumnName: nameof(MovieRating.userId)).
                Append(mlContext.Transforms.Conversion.MapValueToKey
                (outputColumnName: "movieIdEncoded", inputColumnName: nameof(MovieRating.movieId)));
            // Specify the options for matrixfactorization trainer
            options = new MatrixFactorizationTrainer.Options();
            options.MatrixColumnIndexColumnName = "userIdEncoded";
            options.MatrixRowIndexColumnName = "movieIdEncoded";
            options.LabelColumnName = "Label";
            options.NumberOfIterations = 20;
            options.ApproximationRank = 100;
            lblStep3.Content = "Transform data was successful";
                
        }

        private void step4_Click(object sender, RoutedEventArgs e)
        {
            // Step 4: Create the training pipeline
            trainingPipeLine = dataProcessingPipeline.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            lblStep4.Content = "Created the training pipeline successful";
        }

        private void step5_Click(object sender, RoutedEventArgs e)
        {
            // step 5: train the model fitting to the dataset
            trainedModel = trainingPipeLine.Fit(trainTestData.TrainSet);
            lblStep5.Content = "Trained model successful!";
        }

        private void step6_Click(object sender, RoutedEventArgs e)
        {
            // step 6: Evaluate the model performance
            IDataView prediction = trainedModel.Transform(trainTestData.TestSet);
            RegressionMetrics metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            txtlossSquare.Text = metrics.LossFunction.ToString();
            txtMAE.Text = metrics.MeanAbsoluteError.ToString();
            txtMSE.Text = metrics.MeanSquaredError.ToString();
            txtRMSE.Text = metrics.RootMeanSquaredError.ToString();
            txtRsquare.Text = metrics.RSquared.ToString();
        }

        private void step7_Click(object sender, RoutedEventArgs e)
        {
            // Step 7: Test a single prediction by predicting a single movie rating for a specific user
            PredictionEngine<MovieRating, MovieRatingPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(trainedModel);
            /* Make a single movie rating prediction, the score are for particular user 
             * and will range from 1-5. the higher the score the higher the 
             * likelyhood of a user liking a particular movie
             * you can recommend a movie to a user if say rating > 3.5 */
            int predictionuserId = int.Parse(txtUserid.Text);
            int predictionmovieId = int.Parse(txtmovieID.Text);
            MovieRatingPrediction movieRatingPrediction = predictionEngine.Predict(new MovieRating()
            { userId = predictionuserId, movieId = predictionmovieId });
            Movie movieService = new Movie();
            FlowDocument mcFlowDoc = new FlowDocument();
            // Create a paragraph with text
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run("For userId: "));
            para.Inlines.Add(new Bold(new Run(predictionuserId + " ")));
            para.Inlines.Add(new Run("movie rating prediction ( 1 - 5 stars) for movie: "));
            para.Inlines.Add(new Bold(new Run(movieService.Get(predictionmovieId).MovieTitle)));
            para.Inlines.Add(new Run(" is: "));
            para.Inlines.Add(new Bold(new Run(Math.Round(movieRatingPrediction.Score, 1) + "")));
            // Add the paragraph to blocks of paragraph
            mcFlowDoc.Blocks.Add(para);
            txtRichResult.Document = mcFlowDoc;

        }
    }
}
