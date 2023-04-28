using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public static class Global_Variables
    {
        public static bool isMultiThreaded = true;
        public static string cityName = "Emmen";
        public static double cityLat = 52.77;
        public static double cityLon = 6.90;
        public static string units = "metric";
        public static object lockObj = new object();
    }
}
