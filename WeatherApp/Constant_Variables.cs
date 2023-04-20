namespace WeatherApp
{
    internal static class Constant_Variables
    {
        //TO DO: Move secret api key to variable in .env file
        public const string API_KEY = "347d866c9b3fce34a550fd9feb2de1c1";
        public const string API_URL_CURRENT_WEATHER_BY_LOCATION = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units={2}";
        public const string API_URL_FIVE_DAY_WEATHER_FORECAST_BY_LOCATION = "https://api.openweathermap.org/data/2.5/forecast?q={0}&appid={1}&units={2}";
        public const string API_URL_HISTORICAL_WEATHER_BY_LOCATION = "https://history.openweathermap.org/data/2.5/history/city?q={0},{1}&appid={2}&type=hour&start={3}&end={4}&units={5}";
        public const string API_REVERSE_GEOLOCATION = "https://api.openweathermap.org/geo/1.0/reverse?appid={0}&lat={1}&lon={2}&limit=1";

        public const string NL_PROVINCE_JSON = @"
         {
            ""North Brabant"": 3558,
            ""Utrecht"": 3559,
            ""South Holland"": 3560,
            ""Gelderland"": 915,
            ""Groningen"": 918,
            ""Limburg"": 916,
            ""North Holland"": 3557,
            ""Overijssel"": 917,
            ""Drenthe"": 914,
            ""Zeeland"": 1508,
            ""Flevoland"" : 1511,
            ""Friesland"" : 913,
            ""Frisia"" : 913
         }";
    }
}
