using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Benutzersteuerelement" wird unter https://go.microsoft.com/fwlink/?LinkId=234236 dokumentiert.

namespace WeatherApp
{
    public sealed partial class NavBar : UserControl
    {
        public NavBar()
        {
            this.InitializeComponent();
        }

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
                Console.WriteLine(CitySearchBox.Text);
                //TO DO: Code to save city to global variable
            }
        }
    }
}
