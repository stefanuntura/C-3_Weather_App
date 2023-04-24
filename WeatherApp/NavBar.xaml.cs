using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


namespace WeatherApp
{
    public sealed partial class NavBar : UserControl
    {
        public event EventHandler<string> SearchTextChanged;
        public NavBar()
        {
            this.InitializeComponent();
        }

        // Event for when enter is pressed in the search bar 
        private void CitySearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Global_Variables.cityName = CitySearchBox.Text.Trim();
                var searchTerm = CitySearchBox.Text;
                var message = new NavSearch { SearchTerm = searchTerm };
                WeakReferenceMessenger.Default.Send(message);
            }
        }

        // Page navigation for the NavBar tabs
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
    }
}
