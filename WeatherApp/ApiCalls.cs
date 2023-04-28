using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherApp
{
    //API call class, has various different API calles needed from OpenWeatherMap
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
                return null;
            }
            return null;
        }

        //Sync way to get WeatherData
        public static WeatherData.Root fetchCurrentWeatherSync()
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareCurrentWeatherDataApiUrl();

            try
            {
                //.Result makes the response sync
                string response = client.GetStringAsync(url).Result;

                if (response != null && response != "")
                {
                    return parseCurrentWeatherJson(response);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }

        // async method for fetching forecast
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
                return null;
            }

            return null;
        }

        //Sync method for forcast data
        public static WeatherForecastData fetchFiveDayWeatherForecastSync() 
        {
            HttpClient client = new HttpClient();

            String url = Utilities.prepareFiveDaysWeatherForecastDataApiUrl();

            try
            {
                string response = client.GetStringAsync(url).Result;

                if (response != null && response != "")
                {
                    return parseWeatherForecastJson(response);
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        //async method for fetching icon url
        public static async Task<HttpResponseMessage> fetchWeatherIcon(string url)
        {
            HttpClient client = new HttpClient();

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                var content = response.Content;

                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception e)
            {
                return null;
                //TO DO: Add error visual display for the UI
            }

            return null;
        }

        //Sync method for fetching icon
        public static HttpResponseMessage fetchWeatherIconSync(string url)
        {
            HttpClient client = new HttpClient();

            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                var content = response.Content;

                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception e)
            {
                return null;
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
    }
}