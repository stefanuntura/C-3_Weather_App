using System.Collections.Generic;

namespace WeatherApp
{
    public class WeatherData
    {
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }

            override public string ToString()
            {
                string res = "\tcoords: {\n";
                res += "\t\tlat: " + lat + "\n";
                res += "\t\tlon: " + lon + "\n";
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

        public class Main
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }

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

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }

            public override string ToString()
            {
                string res = "\tsys: {\n";
                res += "\t\ttype: " + type + "\n";
                res += "\t\tid: " + id + "\n";
                res += "\t\tcountry: " + country + "\n";
                res += "\t\tsunrise: " + sunrise + "\n";
                res += "\t\tsunset: " + sunset + "\n";
                res += "\t}\n";
                return res;
            }
        }

        public class Root
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int timezone { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }

            override public string ToString()
            {
                string res = "root: {\n";
                res += coord != null ? coord.ToString() : "";
                res += "\tweather: {\n";

                foreach(WeatherData.Weather w in weather)
                {
                    res += w.ToString();
                }

                res += "\t}\n";
                res += "\tbase: " + @base + "\n";
                res += main != null ? main.ToString() : "";
                res += "\tvisibility: " + visibility + "\n";
                res += wind != null ? wind.ToString() : "";
                res += clouds != null ? clouds.ToString() : "";
                res += "\tdt: " + dt + "\n";
                res += sys != null ? sys.ToString() : "";
                res += "\ttimezone: " + timezone + "\n";
                res += "\tid: " + id + "\n";
                res += "\tname: " + name + "\n";
                res += "\tcod: " + cod + "\n";
                res += "}\n";
                return res;
            }
        }
    }
}
