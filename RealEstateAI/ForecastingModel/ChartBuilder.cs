using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlot.Plotly;

namespace ForecastingModel
{
    public class ChartBuilder
    {
        public static PlotlyChart BuidChart(TimeSeriesAnalysis analysis)
        {
            IEnumerable<Trace> traces = BuidTraces(analysis);
            PlotlyChart chart = BuidPlotlyChart(analysis.TimeSeries.Name, traces);
            return chart;
        }

        private static PlotlyChart BuidPlotlyChart(string charTitle, IEnumerable<Trace> traces)
        {
            PlotlyChart chart = Chart.Plot(traces);
            var layout = new Layout.Layout { title = charTitle };
            chart.WithLayout(layout);
            chart.Width = 800;
            chart.Height = 600;
            chart.WithXTitle("Date");
            chart.WithYTitle("Price - Million VND/m2");
            chart.WithLegend(true);
            return chart;
        }

        private static IEnumerable<Trace> BuidTraces(TimeSeriesAnalysis analysis)
        {
            yield return BuidTrace("Historical", analysis.Historical);
            yield return BuidTrace("Actual", analysis.Actual);
            foreach (ForecastDetails forecastDetails in analysis.Forecasts)
            {
                string name = $"{forecastDetails.AlgorithmName} Forecast";
                yield return BuidTrace(name, forecastDetails.Forecast);
            }
        }

        private static Trace BuidTrace(string name, IEnumerable<Observation> observations)
        {
            DateTime[] dates = observations.Select(s => s.Date).ToArray();
            float[] values = observations.Select(s =>s.Value).ToArray();
            return new Scatter()
            {
                name = name,
                x = dates,
                  y = values
            };
        }
    }
}
