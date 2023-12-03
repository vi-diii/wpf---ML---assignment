using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingModel
{
    public class TimeSeries
    {
        public string Name { get;}
        public Observation[] Observations { get;}
        public TimeSpan Interval { get;}
        public string Group { get;}
        public TimeSeries(
            string name,
            IEnumerable<Observation> observations,
            TimeSpan interval,
            string group
            ) 
        {
           Name = name;
           Observations = observations.ToArray();
           Interval = interval;
           Group = group;
        }
        
    }
}
