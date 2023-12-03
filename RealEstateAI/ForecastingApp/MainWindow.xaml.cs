using ForecastingModel;
using Microsoft.ML.Data;
using Microsoft.Win32;
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
using XPlot.Plotly;

namespace ForecastingApp
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

        private void btnPickDataset_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "csv format|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {

                txtDataset.Text = openFileDialog.FileName;

            }
        }

        private async void btnProcessForecasting_Click(object sender, RoutedEventArgs e)
        {
            TimeSeries timeSeries = WaitTimeLoader.Load(txtDataset.Text);

            int horizon = int.Parse(txtHorizon.Text);

            Observation[] observations = GapFiller.FillGaps(timeSeries.Observations, timeSeries.Interval).ToArray(); Observation[] historical = observations.Take(observations.Length - horizon).ToArray(); Observation[]

            actual = observations.Skip(observations.Length - horizon).ToArray();

            var forecasts = new List<ForecastDetails>();

            Observation[] linearRegressionForecast = LinearRegressionForecaster

            .Forecast(historical, horizon, timeSeries.Interval); RegressionMetrics linearRegressionMetrics =

            ForecastScorer.Evaluate(actual, linearRegressionForecast);

            forecasts.Add(new ForecastDetails("Linear Regression", linearRegressionForecast, linearRegressionMetrics));

            Observation[] ssaForecast = SingularSpectrumAnalysisForecaster

            .Forecast(historical, horizon, timeSeries.Interval);

            RegressionMetrics ssaMetrics = ForecastScorer.Evaluate(actual, ssaForecast); forecasts.Add(new ForecastDetails("SSA", ssaForecast, ssaMetrics));

            var analysisResult = new TimeSeriesAnalysis(timeSeries, historical, actual, forecasts);

            analysisResult.LinearRegressionMetric = linearRegressionMetrics;

            analysisResult.SingularSpectrumMetric = ssaMetrics;

            PlotlyChart chart = ChartBuilder.BuidChart(analysisResult); chart.WithTitle("Real Estate forecasting");

            await myWebBrowser.EnsureCoreWebView2Async();

            myWebBrowser.NavigateToString(chart.GetHtml());

            lblLinearMAE.Content = $"{analysisResult.LinearRegressionMetric.MeanAbsoluteError:F2}"; lblLinearRMSE.Content = $"{analysisResult.LinearRegressionMetric.RootMeanSquaredError:F2}";

            lblSingularMAE.Content = $"{analysisResult.SingularSpectrumMetric.MeanAbsoluteError:F2}";

            lblSingularRMSE.Content = $"{analysisResult.SingularSpectrumMetric.RootMeanSquaredError:F2}";
        }
    }
}
