using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingModel
{
    public class ForecastScorer
    {
        public static RegressionMetrics Evaluate(Observation[] actual, Observation[] forecast)
        {
            IEnumerable<Comparison> comparisons = BuildComparisons(actual, forecast);
            var context = new MLContext();
            IDataView predictions = context.Data.LoadFromEnumerable(comparisons);
            RegressionMetrics regressionMetrics = context.Regression.Evaluate(
                data: predictions,
                labelColumnName: nameof(Comparison.Actual),
                scoreColumnName: nameof(Comparison.Predicted)
                );
            return regressionMetrics;
                
        }

        private static IEnumerable<Comparison> BuildComparisons(Observation[] actual, Observation[] forecast)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                var comparison = new Comparison()
                {
                    Actual = actual[i].Value,
                    Predicted = forecast[i].Value
                };
                yield return comparison;
            }
            
        }
    }
}
