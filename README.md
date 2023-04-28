# C-3_Weather_App

To ensure that the application runs as intended, there are some things that need to be set up beforehand. First the Database application requires the latest SQL Server and Visual Studio 2022 version to run. For this application SQLEXPRESS 2022 was used. Please follow the subsequent instructions:

#### SQL Server Setup and database connection 

- Update Visual Studio 2022 to the latest version
- Download SQL Server 2022 Express
- Restart your PC
- Open the Project Solution and check that the Database Project is set to SQL 2022
  - Right-click the Database Project ('WeatherDb') in the Solution Explorer
  - Select Properties
  - Ensure that the target platform is set to SQL Server 2022
- Publish the Database Project
  - Right-click the Database Project ('WeatherDb') again in the Solution Explorer
  - Click Publish
  - Click Edit to alter the database connection
  - In the new pop-up select your SQL Server 2022 Express instance and test the connection before selecting 'Ok'
  - In the remaining pop-up window set the database name to 'WeatherDb'
  - Copy your Data the connection string up until the first semi-colon (right before Initial Catalog)
- Change the connection String
  - Open the App.xaml.cs from the Solution Explorer
  - Replace the connection string with what you copied in the last step
  - Specify the port number 1433 like so : Data Source = YourServerName\Instance ,1433 ; Initial Catalog
- Download Sql Server Configuration Menu 2022 if you do not have it
- Click Sql Server Network configuration
  - Enabled Named Pipes
  - Enabled TCP / IP
- Click SQL Server Services
  - Right click SQL Server (SQLEXPRESS), Click restart 
  - Right click Server Browser, Click restart
    - If you cannot restart it: Right click Server browser -> Properties -> Service -> Change Start Mode to automatic and restart



<h4>Remarks </h4>

The historical data tab in the application processes a file consisting of bulk historical weather data of Emmen dating back to 1979, due to a 100mb file limit with github, this file was cut down by about half. So the file dates back to February 2003. 

Furthermore, If that part of the application is used when the application runs synchronously, it WILL crash the program (and probably your pc). 

