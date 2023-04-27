using CommunityToolkit.Mvvm.Messaging;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace WeatherApp
{
    public sealed partial class NavBar : UserControl
    {
        public event EventHandler<string> SearchTextChanged;
        private int currentNavItemIndex = 0;

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

        // Method to navigate through the Nav Bar
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // Determines which slide transition to use depending which index of the Nav Bar the user is on
            int newIndex = sender.MenuItems.IndexOf(args.InvokedItemContainer);

            SlideNavigationTransitionEffect effect = GetTransitionEffect(newIndex);

            currentNavItemIndex = newIndex;

            if (args.IsSettingsInvoked)
            {
                NavigateToSettingsPage(effect);
            }
            else
            {
                string tag = args.InvokedItemContainer.Tag.ToString();
                NavigateToPageWithTag(tag, effect);

                // Send the HideGridMessage message
                WeakReferenceMessenger.Default.Send(new HidePageElements());
            }


        }

        // Determines the transition effect
        private SlideNavigationTransitionEffect GetTransitionEffect(int newIndex)
        {
            if (newIndex > currentNavItemIndex)
            {
                return SlideNavigationTransitionEffect.FromRight;
            }
            else
            {
                return SlideNavigationTransitionEffect.FromLeft;
            }
        }

        // To the app settings page
        private void NavigateToSettingsPage(SlideNavigationTransitionEffect effect)
        {
            Frame.Navigate(typeof(Settings), null, new SlideNavigationTransitionInfo() { Effect = effect });
        }

        // Navigate to the selected page based on the tag property
        private void NavigateToPageWithTag(string tag, SlideNavigationTransitionEffect effect)
        {
            switch (tag)
            {
                case "Map":
                    Frame.Navigate(typeof(Map), null, new SlideNavigationTransitionInfo() { Effect = effect });
                    break;
                case "Forecast":
                    Frame.Navigate(typeof(Forecast), null, new SlideNavigationTransitionInfo() { Effect = effect });
                    break;
                case "Historical":
                    Frame.Navigate(typeof(Historical), null, new SlideNavigationTransitionInfo() { Effect = effect });
                    break;
            }
        }

    }
}
