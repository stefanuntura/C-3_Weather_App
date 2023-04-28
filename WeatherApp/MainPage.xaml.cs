using CommunityToolkit.Mvvm.Messaging;
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
    // Main Page class of the application. Once a user leaves the main page, they don't navigate back towards it
    public sealed partial class MainPage : Page
    {
         private Stopwatch stopwatch;

        public MainPage()
        {
            this.InitializeComponent();
            WeakReferenceMessenger.Default.Register<HidePageElements>(this, OnHidePageElementsMessageReceived);
        }

        //Method to be called when the HidePageElements class recieves an update
        private void OnHidePageElementsMessageReceived(object recipient, HidePageElements message)
        {
            // Hide the grid
            MainContentGrid.Visibility = Visibility.Collapsed;
        }
    }
}
