using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public class WeatherHistoricalData
    {
        public string cod { get; set; }
        public string message { get; set; }
        public int city_id { get; set; }
        public double calctime { get; set; }
        public int cnt { get; set; }
        public List<WeatherData.Root> list { get; set; }
    }
}
