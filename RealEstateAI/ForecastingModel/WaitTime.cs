using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingModel
{
    public class WaitTime
    {
        [LoadColumn(0)] public DateTime Date;
        [LoadColumn(1)] public float QueryWaitTime;
        internal Observation ToObservation()
        {
            return new Observation
            {
               Date = Date,
               Value = QueryWaitTime
            };
        }
    }
}
