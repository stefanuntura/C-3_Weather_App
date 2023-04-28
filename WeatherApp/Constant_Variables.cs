namespace WeatherApp
{
    internal static class Constant_Variables
    {
        //TO DO: Move secret api key to variable in .env file
        public const string API_KEY = "347d866c9b3fce34a550fd9feb2de1c1";
        public const string MAP_SERVICE_TOKEN = "auanlCtPy8GI2VO7KqPs~F4e-Om5LsRk5vQRoLBNfdQ~AggZV9E06diYWeqfAl3RiFQiivReb_Z3LOyp8tBw5VLRY8DSisSSJmWg4mJrmExR";
        public const string API_URL_CURRENT_WEATHER_BY_LOCATION = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units={2}";
        public const string API_URL_FIVE_DAY_WEATHER_FORECAST_BY_LOCATION = "https://api.openweathermap.org/data/2.5/forecast?q={0}&appid={1}&units={2}";
        public const string API_URL_ICON = "http://openweathermap.org/img/w/{0}.png";
    }
}
