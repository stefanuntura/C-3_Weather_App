using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace WeatherApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // TODO: move these to appropriate classes when rebased
        public const string bingAPIKey = "auanlCtPy8GI2VO7KqPs~F4e-Om5LsRk5vQRoLBNfdQ~AggZV9E06diYWeqfAl3RiFQiivReb_Z3LOyp8tBw5VLRY8DSisSSJmWg4mJrmExR";
        public const string owmAPIKey = "17004d86814c438ea51d196cf301efc0";
        public const double currentLat = 47;
        public const double currentLon = -122;

        public MainPage()
        {
            this.InitializeComponent();
            weatherMap.MapServiceToken = bingAPIKey;
            weatherMap.Style = MapStyle.Road;

            // Set the map zoom level to show the entire world
            weatherMap.ZoomLevel = 1;
        }

        //TO DO: Figure out why two of the cases throw errors
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //  to the app settings page
                Frame.Navigate(typeof(Settings));
            }
            else
            {
                // Navigate to the selected page based on the Tag property
                string tag = args.InvokedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "Map":
                        Frame.Navigate(typeof(Map));
                        break;
                    case "Details":
                        Frame.Navigate(typeof(Details));
                        break;
                    case "Forecast":
                        Frame.Navigate(typeof(Forecast));
                        break;
                    case "Historical":
                        Frame.Navigate(typeof(Historical));
                        break;
                }

            }
        }

        private void CitySearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                SearchCity(CitySearchBox.Text.Trim());
            }
        }

        private async void SearchCity(string cityName)
        {
            // Use the OpenWeatherMap API to retrieve the geographic coordinates and current weather conditions for the city
            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&units=metric&appid={owmAPIKey}";

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON data to a dynamic object
                    dynamic data = JsonConvert.DeserializeObject(json);

                    // Get the geographic coordinates of the city
                    double latitude = data.coord.lat;
                    double longitude = data.coord.lon;
                    Geopoint cityLocation = new Geopoint(new BasicGeoposition { Latitude = latitude, Longitude = longitude });

                    // Center the MapControl to the city location
                    weatherMap.Center = cityLocation;
                    weatherMap.ZoomLevel = 12;

                    // Remove any existing MapIcons from the map
                    var existingPins = weatherMap.MapElements.Where(x => x is MapIcon).ToList();
                    foreach (var pin in existingPins)
                    {
                        weatherMap.MapElements.Remove(pin);
                    }

                    // Get the weather icon code and use it to construct the URL of the weather icon image
                    string weatherIcon = data.weather[0].icon;
                    string weatherIconUrl = $"http://openweathermap.org/img/w/{weatherIcon}.png";

                    // Create a new MapIcon to represent the city location and current weather conditions
                    var mapIcon = new MapIcon
                    {
                        Location = cityLocation,
                        Title = cityName,
                        NormalizedAnchorPoint = new Point(0.5, 1.0),
                        Image = RandomAccessStreamReference.CreateFromUri(new Uri(weatherIconUrl))
                    };

                    // Add the MapIcon to the MapControl
                    weatherMap.MapElements.Add(mapIcon);

                    // Get the current weather conditions for the city
                    string weatherDescription = data.weather[0].description;
                    double temperature = data.main.temp;
                    double windSpeed = data.wind.speed;

                    // Display the weather information in a message box (or any other UI element)
                    weatherDescriptionBox.Text = $"The current weather in {cityName} is {weatherDescription}, with a temperature of {temperature} °C and a wind speed of {windSpeed} m/s.";
                    tempCurrentBox.Text = temperature.ToString();
                    windSpeedBox.Text = windSpeed.ToString();
                   
                }
                else
                {
                    // Display an error message if the API request fails
                    string errorMessage = $"Could not retrieve weather information for {cityName}. Please check spelling or search a valid city.";
                    MessageDialog dialog = new MessageDialog(errorMessage);
                    await dialog.ShowAsync();
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            List<WeatherHistoricalData.Root> historicalData = await Utilities.extractHistoricalWeatherData();
        }
    }
}
