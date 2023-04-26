using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class Forecast : Page
    {
        public Forecast()
        {
            this.InitializeComponent();
            WeakReferenceMessenger.Default.Register<NavSearch>(this, OnSearchTermReceived);
        }

        private void NavBar_Loaded(object sender, RoutedEventArgs e)
        {
            lock (Global_Variables.lockObj)
            {
                getForecastData();
                CityName.Text = Global_Variables.cityName;
            }
        }

        private void OnSearchTermReceived(object recipient, NavSearch message)
        {
            lock (Global_Variables.lockObj)
            {
                getForecastData();
                CityName.Text = Global_Variables.cityName;
            }
        }

        private async void getForecastData()
        {
            WeatherForecastData weatherForecastData = await ApiCalls.fetchFiveDaysWeatherForecast();

            if(weatherForecastData != null)
            {
                populateDays(weatherForecastData);
            }
        }
        
        // TO DO: Add DB insert part
        private void populateDays(WeatherForecastData forecast)
        {/*
            DbManager dbManager = DbManager.getInstance();

            if (Global_Variables.isMultiThreaded)
            {
                dbManager.insertMulti(weatherForecastData);
            }
            else
            {
                dbManager.insertSingle(weatherForecastData.list);
            }*/

            for(int i  = 0; i <= 4; i++)
            {
                WeatherData.Root weather = forecast.list[i];
                List<TextBlock> blockList = selectTextBlock(i);
                if (blockList != null)
                {
                    //Description of weather
                    blockList[1].Text = weather.weather[0].description;

                    //Temperatures
                    blockList[0].Text = Utilities.prepareTempForUI(weather.main.temp);
                    blockList[2].Text = "Feels like: " + Utilities.prepareTempForUI(weather.main.feels_like);

                    //Pressure
                    blockList[3].Text = "Pressure: " + weather.main.pressure.ToString();

                    //Humidity
                    blockList[4].Text = "Humidity: " + weather.main.humidity.ToString();
                }
            }
        }

        private List<TextBlock> selectTextBlock(int i)
        {
            List<TextBlock> list = new List<TextBlock>();
            
            switch (i)
            {
                case 0:
                    list.Add(OneTemp);
                    list.Add(OneDesc);
                    list.Add(OneFeelsLike);
                    list.Add(OnePressure);
                    list.Add(OneHumidity);
                    return list;
                case 1:
                    list.Add(TwoTemp);
                    list.Add(TwoDesc);
                    list.Add(TwoFeelsLike);
                    list.Add(TwoPressure);
                    list.Add(TwoHumidity);
                    return list;
                case 2:
                    list.Add(ThreeTemp);
                    list.Add(ThreeDesc);
                    list.Add(ThreeFeelsLike);
                    list.Add(ThreePressure);
                    list.Add(ThreeHumidity);
                    return list;
                case 3:
                    list.Add(FourTemp);
                    list.Add(FourDesc);
                    list.Add(FourFeelsLike);
                    list.Add(FourPressure);
                    list.Add(FourHumidity);
                    return list;
                case 4:
                    list.Add(FiveTemp);
                    list.Add(FiveDesc);
                    list.Add(FiveFeelsLike);
                    list.Add(FivePressure);
                    list.Add(FiveHumidity);
                    return list;
            }
            return list;
        }
    }
}
