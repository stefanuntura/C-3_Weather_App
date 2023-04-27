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
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void MetricsButton_Checked(object sender, RoutedEventArgs e)
        {
            MetricsButton.Content = "Celsius";
            Global_Variables.units = "metric";
        }

        private void MetricsButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MetricsButton.Content = "Fahrenheit";
            Global_Variables.units = "imperial";
        }
        private void MultiThreadingButton_Checked(object sender, RoutedEventArgs e)
        {
            MultiThreadingButton.Content = "Multi Threaded";
            Global_Variables.isMultiThreaded = true;
        }

        private void MultiThreadingButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MultiThreadingButton.Content = "Single Threaded";
            Global_Variables.isMultiThreaded = false;
        }
    }
}
