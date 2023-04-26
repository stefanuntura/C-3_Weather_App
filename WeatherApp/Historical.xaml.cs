using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
        Semaphore semaphore;
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
                    semaphore = new Semaphore(initialCount: 0, maximumCount: 5);
                        
                    
                    Thread[] threadList = new Thread[20];
                    semaphoreSlim = new SemaphoreSlim(5);

                    for (int i = 0; i < threadList.GetLength(0); i++)
                    {
                        //We use localI to prevent the delay of creating thread to interfere with the treadname
                        int localI = i;
                        //Create the thread with a lambda
                        threadList[i] = new Thread(() => populateHistroicalDataTableAsync(localI));
                        //Start the thread
                        threadList[i].Start(); 
                    }
                }
            }
        }

        private void populateHistroicalDataTable(int tableEntryIndex, WeatherHistoricalData.Root historyEntryData)
        {
            //Prepare data to enter in table
            string historyEntryDate = historyEntryData.dt.ToString();
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
            lv1.Width = 200;
            lv1.HorizontalAlignment = HorizontalAlignment.Stretch;
            lv1.VerticalAlignment = VerticalAlignment.Stretch;

            // Create new StackPanel "Child" elements with alignment and width
            ListViewItem lv2 = new ListViewItem();
            lv2.Content = historyEntryDescription;
            lv2.Width = 400;
            lv2.HorizontalAlignment = HorizontalAlignment.Stretch;
            lv2.VerticalAlignment = VerticalAlignment.Stretch;

            // Create new StackPanel "Child" elements with alignment and width
            ListViewItem lv3 = new ListViewItem();
            lv3.Content = historyEntryTemp;
            lv3.Width = 100;
            lv3.HorizontalAlignment = HorizontalAlignment.Stretch;
            lv3.VerticalAlignment = VerticalAlignment.Stretch;

            // Create new StackPanel "Child" elements with alignment and width
            ListViewItem lv4 = new ListViewItem();
            lv4.Content = historyEntryFeelsLikeTemp;
            lv4.Width = 150;
            lv4.HorizontalAlignment = HorizontalAlignment.Stretch;
            lv4.VerticalAlignment = VerticalAlignment.Stretch;

            // Create new StackPanel "Child" elements with alignment and width
            ListViewItem lv5 = new ListViewItem();
            lv5.Content = historyEntryPressure;
            lv5.Width = 125;
            lv5.HorizontalAlignment = HorizontalAlignment.Stretch;
            lv5.VerticalAlignment = VerticalAlignment.Stretch;

            // Create new StackPanel "Child" elements with alignment and width
            ListViewItem lv6 = new ListViewItem();
            lv6.Content = historyEntryHumidity;
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

        private void populateHistroicalDataTableAsync(int index)
        {
            semaphoreSlim.Wait();
            populateHistroicalDataTable(index, weatherHistoricalData[index]);
            semaphoreSlim.Release();
        }
    }
}
