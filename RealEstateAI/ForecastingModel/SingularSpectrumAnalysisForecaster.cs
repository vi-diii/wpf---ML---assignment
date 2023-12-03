using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingModel
{
    public class SingularSpectrumAnalysisForecaster
    {
        public static Observation[] Forecast(
            ICollection<Observation> observations,
            int horizon,
            TimeSpan interval
            )
        {
            // create a new Ml context
            var ml = new MLContext();
            // convert data to IDataView.
            IDataView dataview = ml.Data.LoadFromEnumerable(observations);
            // setup arguments.
            var inputColumnName = nameof(Observation.Value);
            var outputColumnName = nameof(SingularSpectrumAnalysisForecastResult.Forecast);
            int windownSize   = Math.Min(50, observations.Count/2-1);
            int seriesLength = Math.Min(110, observations.Count);

            var model = ml.Forecasting.ForecastBySsa(
                outputColumnName: outputColumnName,
                inputColumnName: inputColumnName,
               windowSize: windownSize,
                seriesLength: seriesLength,
                trainSize: observations.Count,
                horizon: horizon * 6
                );
            // Train.
            var trainformer = model.Fit(dataview);
            // 
            using var forecastEngine = trainformer.CreateTimeSeriesEngine<Observation, SingularSpectrumAnalysisForecastResult>(ml);
            var forecast = forecastEngine.Predict();
            var predictions = new List<Observation>();
            DateTime currentTime = observations.Last().Date;
            foreach (var predictedValue in forecast.Forecast)
            {
                currentTime = currentTime.Add(interval);
                predictions.Add(new Observation()
                {
                    Date = currentTime,
                    Value = predictedValue
                });
            }
            return predictions.ToArray();


        }
    }
}
