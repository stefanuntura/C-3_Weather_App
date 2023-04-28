using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Diagnostics;

namespace WeatherApp
{
    //Utility class, has methods that are used throughout the application
    internal static class Utilities
    {
        public static string prepareCurrentWeatherDataApiUrl()
        {
            string preparedUrl = String.Format(Constant_Variables.API_URL_CURRENT_WEATHER_BY_LOCATION, Global_Variables.cityName, Constant_Variables.API_KEY, Global_Variables.units);

            return preparedUrl;
        }

        public static string prepareFiveDaysWeatherForecastDataApiUrl()
        {
            string preparedUrl = String.Format(Constant_Variables.API_URL_FIVE_DAY_WEATHER_FORECAST_BY_LOCATION, Global_Variables.cityName, Constant_Variables.API_KEY, Global_Variables.units);

            return preparedUrl;
        }

        public static string prepareWeatherIconUrl(string iconId) 
        {
            string preparedurl = String.Format(Constant_Variables.API_URL_ICON, iconId);

            return preparedurl;
        }

        public static String unixTimeStampToDate(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime().Date;

            return dateTimeToDateString(dateTime);
        }

        public static long unixTimeStampFromDate(DateTime dateTime)
        {
            DateTime unixStart = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            long timeStamp = (long)Math.Floor((dateTime.ToUniversalTime() - unixStart).TotalSeconds);

            return timeStamp;
        }

        public static String dateTimeToDateString(DateTime dateTime)
        {
            string[] dateTimeBits = dateTime.ToString().Split(' ');
            string date = dateTimeBits[0];

            return date;
        }

        public static List<string> extractUniqueDatesFromWeatherData(List<WeatherData.Root> weatherDataList)
        {
            List<string> uniqueDates = new List<string>();

            foreach (WeatherData.Root weatherData in weatherDataList)
            {
                //Gets date form time stamp
                string date = Utilities.unixTimeStampToDate(weatherData.dt);

                //Adds unique date to the array of dates to display
                if (!uniqueDates.Contains(date)) uniqueDates.Add(date);
            }

            return uniqueDates;
        }

        public async static Task<List<WeatherHistoricalData.Root>> extractHistoricalWeatherData()
        {
            var storageFile = await Package.Current.InstalledLocation.TryGetItemAsync("emmen_historical_weather.txt") as StorageFile;

            string content = await FileIO.ReadTextAsync(storageFile);

            return JsonConvert.DeserializeObject<List<WeatherHistoricalData.Root>>(content);
        }

        public static string prepareTempForUI(double temperature)
        {
            return Math.Round(temperature).ToString() + convertMeasurementUnitSymbol();
        }

        public static string convertMeasurementUnitSymbol()
        {
            return Global_Variables.units == "metric" ? "\u2103" : "\u2109";
        }
    }
}
