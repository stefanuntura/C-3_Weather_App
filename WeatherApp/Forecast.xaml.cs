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

        private void OnSearchTermReceived(object recipient, NavSearch message)
        {
            lock (Global_Variables.lockObj)
            {
                getForecastData();
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
                    blockList[0].Text = Math.Round(weather.main.temp).ToString() + (Global_Variables.units == "metric" ? "\u2103" : "\u2109");
                    blockList[1].Text = weather.weather[0].description;
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
                    return list;
                case 1:
                    list.Add(TwoTemp);
                    list.Add(TwoDesc);
                    return list;
                case 2:
                    list.Add(ThreeTemp);
                    list.Add(ThreeDesc);
                    return list;
                case 3:
                    list.Add(FourTemp);
                    list.Add(FourDesc);
                    return list;
                case 4:
                    list.Add(FiveTemp);
                    list.Add(FiveDesc);
                    return list;
            }
            return list;
        }

    }
}
