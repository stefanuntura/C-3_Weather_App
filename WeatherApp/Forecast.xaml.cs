using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


namespace WeatherApp
{
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
        
        private void populateDays(WeatherForecastData forecast)
        {
            DbManager dbManager = DbManager.getInstance();

            if (Global_Variables.isMultiThreaded)
            {
                dbManager.insertMulti(forecast);
            }
            else
            {
                dbManager.insertSingle(forecast);
            }


            for (int i  = 0; i <= 4; i++)
            {
                WeatherData.Root weather = forecast.list[i];
                List<TextBlock> blockList = selectTextBlock(i);
                if(blockList != null)
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

                    BitmapImage bitmap = new BitmapImage(new Uri(Utilities.prepareWeatherIconUrl(weather.weather[0].icon)));
                    ImageSource imageSource = bitmap as ImageSource;

                    switch (i)
                    {
                        case 0:
                            OneImage.Source = imageSource;
                            break;
                        case 1:
                            TwoImage.Source = imageSource;
                            break;
                        case 2:
                            ThreeImage.Source = imageSource;
                            break;
                        case 3:
                            FourImage.Source = imageSource;
                            break;
                        case 4:
                            FiveImage.Source = imageSource;
                            break;
                    }
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
