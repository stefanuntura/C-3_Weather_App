using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WeatherApp
{
    public static class ApiCalls
    {
        public static async Task<WeatherData.Root> fetchCurrentWeather()
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareCurrentWeatherDataApiUrl();

            try
            {
                string response = await client.GetStringAsync(url);

                if (response != null && response != "")
                {
                     return parseCurrentWeatherJson(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops! Request has failed with the following error: " + e);


                //TO DO: Add error visual display for the UI

                return null;
            }
            return null;
        }

        public static async Task<WeatherForecastData> fetchFiveDaysWeatherForecast()
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareFiveDaysWeatherForecastDataApiUrl();

            try
            {
                string response = await client.GetStringAsync(url);

                if (response != null && response != "")
                {
                    return parseWeatherForecastJson(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops! Request has failed with the following error: " + e);

                return null;

                //TO DO: Add error visual display for the UI

            }

            return null;
        }

        public static async Task<WeatherHistoricalData> fetchHistoricalWeather()
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareHistoricalWeatherDataApiUrl();

            Console.WriteLine("Url: " + url);

            try
            {
                string response = await client.GetStringAsync(url);

                Console.WriteLine("Response: " + response);


                if (response != null && response != "")
                {
                    Console.WriteLine("Response: " + response);

                    return parseWeatherHistoricalJson(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops! Request has failed with the following error: " + e);

                return null;

                //TO DO: Add error visual display for the UI

            }

            return null;
        }

        public static async Task<List<ReverseGeoLocation>> fetchGeoLocation()
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareReverseGeoLocationApiUrl();

            Console.WriteLine("Url: " + url);

            try
            {
                string response = await client.GetStringAsync(url);

                Console.WriteLine("Response: " + response);


                if (response != null && response != "")
                {
                    Console.WriteLine("Response: " + response);

                    return parseGeoLocationJson(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops! Request has failed with the following error: " + e);

                return null;

                //TO DO: Add error visual display for the UI

            }

            return null;
        }

        //Deserializes json response to WeatherData.Root class
        //Uses NewtonSoft.Json nugget package
        private static WeatherData.Root parseCurrentWeatherJson(String json)
        {
            return JsonConvert.DeserializeObject<WeatherData.Root>(json);
        }

        private static WeatherForecastData parseWeatherForecastJson(String json)
        {
            return JsonConvert.DeserializeObject<WeatherForecastData>(json);
        }

        private static WeatherHistoricalData parseWeatherHistoricalJson(String json)
        {
            return JsonConvert.DeserializeObject<WeatherHistoricalData>(json);
        }

        private static List<ReverseGeoLocation> parseGeoLocationJson(String json) 
        {
            return JsonConvert.DeserializeObject <List<ReverseGeoLocation>>(json);
        }

    }
}