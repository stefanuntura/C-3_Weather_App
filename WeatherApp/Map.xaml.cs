﻿using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace WeatherApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Map : Page
    {
        public const string bingAPIKey = "auanlCtPy8GI2VO7KqPs~F4e-Om5LsRk5vQRoLBNfdQ~AggZV9E06diYWeqfAl3RiFQiivReb_Z3LOyp8tBw5VLRY8DSisSSJmWg4mJrmExR";
        public const string owmAPIKey = "17004d86814c438ea51d196cf301efc0";
        public const double currentLat = 47;
        public const double currentLon = -122;

        public Map()
        {
            this.InitializeComponent();
            weatherMap.MapServiceToken = bingAPIKey;
            weatherMap.Style = MapStyle.Road;
            WeakReferenceMessenger.Default.Register<NavSearch>(this, OnSearchTermReceived);
            // Set the map zoom level to show the entire world
            weatherMap.ZoomLevel = 1;
        }

        private void OnSearchTermReceived(object recipient, NavSearch message)
        {
            Global_Variables.cityName = message.SearchTerm;
            SearchCity(Global_Variables.cityName);
        }

        public async void SearchCity(string cityName)
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
    }
}
