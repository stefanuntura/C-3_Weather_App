﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    // Class to hold forcast data from OWM
    public class WeatherForecastData
    {
            public string cod { get; set; }
            public int message { get; set; }
            public int cnt { get; set; }
            public List<WeatherData.Root> list { get; set; }
    }
}
