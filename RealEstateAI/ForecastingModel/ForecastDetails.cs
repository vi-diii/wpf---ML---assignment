using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace ForecastingModel
{
    public class ForecastDetails
    {
        public string AlgorithmName { get; }
        public Observation[] Forecast { get; }
        public RegressionMetrics RegressionMetrics { get; }
        public ForecastDetails(
            string algorithmName,
            IEnumerable<Observation> forcast,
            RegressionMetrics regressionMetrics )
        {
            AlgorithmName = algorithmName;
            Forecast =  Forecast.ToArray();
            RegressionMetrics = regressionMetrics;
        }
    }
}
