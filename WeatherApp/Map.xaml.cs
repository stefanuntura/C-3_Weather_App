using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace WeatherApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Map : Page
    {
        private WeatherData.Root weatherData;

        public Map()
        {
            this.InitializeComponent();
            weatherMap.MapServiceToken = Constant_Variables.MAP_SERVICE_TOKEN;
            weatherMap.Style = MapStyle.Road;
            WeakReferenceMessenger.Default.Register<NavSearch>(this, OnSearchTermReceived);
            // Set the map zoom level to show the entire world
            weatherMap.ZoomLevel = 1;
        }

        private void OnSearchTermReceived(object recipient, NavSearch message)
        {
            lock (Global_Variables.lockObj)
            {
                Global_Variables.cityName = message.SearchTerm;
                SearchCity(Global_Variables.cityName);
            }
        }
        private async void SearchCity(string cityName)
        {
            lock (Global_Variables.lockObj)
            {
                Global_Variables.cityName = cityName;
            }

            weatherData = await ApiCalls.fetchCurrentWeather();

            if (weatherData != null)
            {

                Geopoint cityLocation = new Geopoint(new BasicGeoposition { Latitude = weatherData.coord.lat, Longitude = weatherData.coord.lon });

                // Center the MapControl to the city location
                weatherMap.Center = cityLocation;
                weatherMap.ZoomLevel = 12;

                // Remove any existing MapIcons from the map
                var existingPins = weatherMap.MapElements.Where(x => x is MapIcon).ToList();
                foreach (var pin in existingPins)
                {
                    weatherMap.MapElements.Remove(pin);
                }

                // Add the MapIcon to the MapControl
                weatherMap.MapElements.Add(GetMapIcon(cityLocation, cityName));

                setLabels();

            }
            else
            {
                // Display an error message if the API request fails
                string errorMessage = $"Could not retrieve weather information for {cityName}. Please check spelling or search a valid city.";
                MessageDialog dialog = new MessageDialog(errorMessage);
                await dialog.ShowAsync();
            }

        }
        private void setLabels()
        {
            //sets labels

            string weatherDescription = "";

            foreach (var item in weatherData.weather)
            {
                weatherDescription = item.description;
            }

            // dateAndTimeNL.Text = Utilities.unixTimeStampToDate(weatherData.dt);

            //temp
            tempCurrentBox.Text = weatherData.main.temp.ToString().Substring(0, 1) + (Global_Variables.units == "metric" ? "°C" : "°F");
            tempMinMaxBox.Text = weatherData.main.temp_min.ToString().Substring(0, 1) + " / " + weatherData.main.temp_max.ToString().Substring(0, 1) + (Global_Variables.units == "metric" ? "°C" : "°F");
            tempFeelsLikeBox.Text = weatherData.main.feels_like.ToString().Substring(0, 1) + (Global_Variables.units == "metric" ? "°C" : "°F");

            //wind
            windSpeedBox.Text = weatherData.wind.speed.ToString() + " " + (Global_Variables.units == "metric" ? "m/s" : "mi/h");
            windGustBox.Text = weatherData.wind.gust.ToString() + " " + (Global_Variables.units == "metric" ? "m/s" : "mi/h");
            windDegreesBox.Text = weatherData.wind.deg.ToString() + "°";

            //pressure
            pressureCurrentBox.Text = weatherData.main.pressure.ToString() + " hPa";

            //humidity
            humidityCurrentBox.Text = weatherData.main.humidity.ToString() + "%";

            //description
            weatherDescriptionBox.Text = $"The current weather in {Global_Variables.cityName} is {weatherDescription}, with a temperature of {tempCurrentBox.Text} and a wind speed of {windSpeedBox.Text}.";
        }

        private MapIcon GetMapIcon(Geopoint cityLocation, string cityName)
        {
            string weatherIcon = "";

            foreach (var item in weatherData.weather)
            {
                weatherIcon = item.icon;
            }

            // Create a new MapIcon to represent the city location and current weather conditions
            MapIcon mapIcon = new MapIcon
            {
                Location = cityLocation,
                Title = cityName,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                Image = RandomAccessStreamReference.CreateFromUri(new Uri(Utilities.prepareWeatherIconUrl(weatherIcon)))
            };

            return mapIcon;
        }
    }
}
