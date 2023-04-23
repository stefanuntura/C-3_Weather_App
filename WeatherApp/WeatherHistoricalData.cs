using System.Collections.Generic;

namespace WeatherApp
{
    public class WeatherHistoricalData
    {
        public class Main
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double dew_point { get; set; }
            public override string ToString()
            {
                string res = "\tmain: {\n";
                res += "\t\ttemp: " + temp + "\n";
                res += "\t\tfeels_like: " + feels_like + "\n";
                res += "\t\ttemp_min: " + temp_min + "\n";
                res += "\t\ttemp_max: " + temp_max + "\n";
                res += "\t\tpressure: " + pressure + "\n";
                res += "\t\thumídity: " + humidity + "\n";
                res += "\t}\n";
                return res;
            }
        }

        public class Clouds
        {
            public int all { get; set; }
            public override string ToString()
            {
                string res = "\tclouds: {\n";
                res += "\t\tall: " + all + "\n";
                res += "\t}\n";
                return res;
            }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
            public override string ToString()
            {
                string res = "\t\t{\n";
                res += "\t\t\tid: " + id + "\n";
                res += "\t\t\tmain: " + main + "\n";
                res += "\t\t\tdescription: " + description + "\n";
                res += "\t\t\ticon: " + icon + "\n";
                res += "\t\t}\n";
                return res;
            }
        }

        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
            public double gust { get; set; }
            public override string ToString()
            {
                string res = "\twind: {\n";
                res += "\t\tspeed: " + speed + "\n";
                res += "\t\tdeg: " + deg + "\n";
                res += "\t\tgust: " + gust + "\n";
                res += "\t}\n";
                return res;
            }
        }

        public class Root
        {
            public int dt { get; set; }
            public string dt_iso { get; set; }
            public int timezone { get; set; }
            public Main main { get; set; }
            public Clouds clouds { get; set; }
            public List<Weather> weather { get; set; }
            public Wind wind { get; set; }
            public double lat { get; set; }
            public string city_name { get; set; }
            public double lon { get; set; }
        }
    }
}
