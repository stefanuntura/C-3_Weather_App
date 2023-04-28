using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
    public sealed partial class Historical : Page
    {
        Object startDate;
        Object endDate;
        List<WeatherHistoricalData.Root> weatherHistoricalData;
        SemaphoreSlim semaphoreSlim;
        public Historical()
        {
            this.InitializeComponent();
        }

        private void Startdate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            startDate = Startdate.Date;
        }

        private async void EndDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            endDate = Startdate.Date;

            if (startDate != null && startDate != endDate)
            {
                weatherHistoricalData = await Utilities.extractHistoricalWeatherData();

                // Synchronous collection itteration & table data population
                // WARNING!!! Running method of popualting UI synchronously WILL crash your PC :)
                if (!Global_Variables.isMultiThreaded)
                {
                    for(int i = 0; i < weatherHistoricalData.Count; i++)
                    {
                        populateHistroicalDataTable(i, weatherHistoricalData[i]);
                    }
                }
                // Asynchronous collection itteration & table data population 
                else
                {
                    semaphoreSlim = new SemaphoreSlim(15);

                    for (int i = 0; i < weatherHistoricalData.Count - 176000; i++)
                    {
                        Debug.WriteLine("Current obj index: " + i);
                        Debug.WriteLine("Left to process: " + (weatherHistoricalData.Count - 176000 - i).ToString());
                        populateHistroicalDataTableAsync(i);
                    }

                    await Task.Run(() => semaphoreSlim.Wait());
                }
            }
        }

        private void populateHistroicalDataTable(int tableEntryIndex, WeatherHistoricalData.Root historyEntryData)
        {
            if (HistoricalDataTable.Items.Count >= tableEntryIndex)
            {
                //Prepare data to enter in table
                string historyEntryDate = Utilities.unixTimeStampToDate(historyEntryData.dt);
                string historyEntryDescription = historyEntryData.weather[0].description;
                string historyEntryTemp = Utilities.prepareTempForUI(historyEntryData.main.temp);
                string historyEntryFeelsLikeTemp = Utilities.prepareTempForUI(historyEntryData.main.feels_like);
                string historyEntryPressure = historyEntryData.main.pressure.ToString();
                string historyEntryHumidity = historyEntryData.main.humidity.ToString();

                StackPanel myStack = new StackPanel();
                myStack.HorizontalAlignment = HorizontalAlignment.Stretch;
                myStack.VerticalAlignment = VerticalAlignment.Stretch;
                myStack.Orientation = Orientation.Horizontal;
                

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv1 = new ListViewItem();
                lv1.Content = historyEntryDate;
                lv1.Height = 100;
                lv1.Width = 200;
                lv1.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv1.VerticalAlignment = VerticalAlignment.Stretch;

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv2 = new ListViewItem();
                lv2.Content = historyEntryDescription;
                lv2.Height = 100;
                lv2.Width = 400;
                lv2.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv2.VerticalAlignment = VerticalAlignment.Stretch;

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv3 = new ListViewItem();
                lv3.Content = historyEntryTemp;
                lv3.Height = 100;
                lv3.Width = 100;
                lv3.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv3.VerticalAlignment = VerticalAlignment.Stretch;

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv4 = new ListViewItem();
                lv4.Content = historyEntryFeelsLikeTemp;
                lv4.Height = 100;
                lv4.Width = 150;
                lv4.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv4.VerticalAlignment = VerticalAlignment.Stretch;

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv5 = new ListViewItem();
                lv5.Content = historyEntryPressure;
                lv5.Height = 100;
                lv5.Width = 125;
                lv5.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv5.VerticalAlignment = VerticalAlignment.Stretch;

                // Create new StackPanel "Child" elements with alignment and width
                ListViewItem lv6 = new ListViewItem();
                lv6.Content = historyEntryHumidity;
                lv6.Height = 100;
                lv6.Width = 125;
                lv6.HorizontalAlignment = HorizontalAlignment.Stretch;
                lv6.VerticalAlignment = VerticalAlignment.Stretch;

                // Add "Child" elements for the new StackPanel
                myStack.Children.Add(lv1);
                myStack.Children.Add(lv2);
                myStack.Children.Add(lv3);
                myStack.Children.Add(lv4);
                myStack.Children.Add(lv5);
                myStack.Children.Add(lv6);

                // Add the new StackPanel as a ListViewItem control
                HistoricalDataTable.Items.Insert(tableEntryIndex, myStack);
            }
        }       

        private async void populateHistroicalDataTableAsync(int index)
        {
            await semaphoreSlim.WaitAsync();

            ThreadPool.QueueUserWorkItem(async _ =>
            {
                try
                {
                    // Insert the data into the UI thread
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        populateHistroicalDataTable(index, weatherHistoricalData[index]);
                    });
                }
                finally
                {
                    // Release the semaphore
                    semaphoreSlim.Release();
                }
            });
        }
    }
}
