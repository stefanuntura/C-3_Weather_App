using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.Effects;

namespace WeatherApp
{
    internal class DbManager
    {
        private static DbManager dbManager;
        private string connectionString;
        internal SqlConnection conn{ get; set; }

        private DbManager()
        {
            connectionString = (App.Current as App).ConnectionString;
        }


        public static DbManager getInstance()
        {
            if (dbManager == null)
            {
                dbManager = new DbManager();
            }
            return dbManager;
        }


        // Converts the retrieved data from the database into a WeatherData.Root object
        public WeatherData.Root convertSqlToObject(SqlDataReader reader)
        {
            Trace.WriteLine("SQLtoObject");

            //initialize required classes
            WeatherData.Root root = new WeatherData.Root();
            root.main = new WeatherData.Main();
            root.weather = new List<WeatherData.Weather>();
            WeatherData.Weather weather = new WeatherData.Weather();
            root.weather.Add(weather);
            root.coord = new WeatherData.Coord();
            root.wind = new WeatherData.Wind();
            root.sys = new WeatherData.Sys();
            WeatherData.Weather weatherObj = new WeatherData.Weather();


            root.dt = (int)Utilities.unixTimeStampFromDate(reader.GetDateTime(0));

            if (!reader.IsDBNull(2))
            {
                root.coord.lon = reader.GetDouble(2);
            }

            if (!reader.IsDBNull(3))
            {
                root.coord.lat = reader.GetDouble(3);
            }

            if (!reader.IsDBNull(4))
            {
                weatherObj.id = reader.GetInt32(4);
            }

            if (!reader.IsDBNull(5))
            {
                weatherObj.main = reader.GetString(5);
            }

            if (!reader.IsDBNull(6))
            {
                weatherObj.description = reader.GetString(6);
            }

            if (!reader.IsDBNull(7))
            {
                weatherObj.icon = reader.GetString(7);
            }

            if (!reader.IsDBNull(8))
            {
                root.main.temp = reader.GetDouble(8);
            }

            if (!reader.IsDBNull(9))
            {
                root.main.feels_like = reader.GetDouble(9);
            }

            if (!reader.IsDBNull(10))
            {
                root.main.temp_min = reader.GetDouble(10);
            }

            if (!reader.IsDBNull(11))
            {
                root.main.temp_max = reader.GetDouble(11);
            }

            if (!reader.IsDBNull(12))
            {
                root.main.pressure = reader.GetInt32(12);
            }

            if (!reader.IsDBNull(13))
            {
                root.main.humidity = reader.GetInt32(13);
            }

            if (!reader.IsDBNull(14))
            {
                root.wind.speed = reader.GetDouble(14);
            }

            if (!reader.IsDBNull(15))
            {
                root.wind.deg = reader.GetInt32(15);
            }

            if (!reader.IsDBNull(16))
            {
                root.wind.gust = reader.GetDouble(16);
            }

            //17 is unit

            if (!reader.IsDBNull(18))
            {
                root.sys.type = reader.GetInt32(18);
            }

            if (reader.IsDBNull(20))
            {
                root.sys.sunrise = reader.GetInt32(20);
            }

            if (reader.IsDBNull(21))
            {
                root.sys.sunset = reader.GetInt32(21);
            }

            if (reader.IsDBNull(22))
            {
                root.timezone = reader.GetInt32(22);
            }

            root.weather.Add(weatherObj);

            Trace.WriteLine(root.ToString());
            return root;
        }

        //------------------------------------ MULTITHREADING METHODS (PLINQ) ------------------------------------


        // Accepts a list of WeatherData.Root objects
        public void insertMulti(WeatherForecastData list)
        {
            {
                list.list.AsParallel().ForAll(data =>
                {
                    insertMulti(data);
                });
            }
        }

        // Accepts single WeatherDate.Root object 
        public void insertMulti(WeatherData.Root item)
        {
            item.weather.AsParallel().ForAll(weather =>
            {
                executeInsertQuery(item, weather);
            });
        }

        public void updateMulti(WeatherData.Root data)
        {
            data.weather.AsParallel().ForAll(weather =>
            {
                executeUpdateQuery(data, weather);
            });

        }



        //------------------------------------ REGULAR METHODS ------------------------------------

        //Insert for WeatherForecastData
        public void insertSingle(WeatherForecastData list)
        {
            foreach (var item in list.list)
            {
                insertSingle(item);
            }
        }

        //Insert for single WeatherData.Root Object
        public void insertSingle(WeatherData.Root item)
        {
            foreach (var weather in item.weather)
            {
                executeInsertQuery(item, weather);
            }
        }

        public void executeInsertQuery(WeatherData.Root item, WeatherData.Weather weather)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if(conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "INSERT INTO weather_data " +
                    "(day, city,lon, lat, id, main, description, icon, temp, feels_like, temp_min, temp_max, pressure, humidity,  wind_speed, wind_deg,  gust, unit, sys_type, country, sunrise, sunset, timezone) " +
                    "VALUES (@day, @city, @lat, @lon, @id, @main, @description, @icon, @temp, @feels, @min, @max, @pressure, @humidity, @windSpeed, @windDirect, @gust, @unit, @sys, @country, @sunrise, @sunset, @timezone)";

                    String dbDate = Utilities.unixTimeStampToDate(item.dt);

                    command.Parameters.AddWithValue("@day", dbDate);
                    command.Parameters.AddWithValue("@temp", item.main != null ? item.main.temp : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", weather != null && !string.IsNullOrEmpty(weather.description) ? weather.description : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@pressure", item.main != null ? item.main.pressure : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@humidity", item.main != null ? item.main.humidity : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@feels", item.main != null ? item.main.feels_like : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@windSpeed", item.wind != null ? item.wind.speed : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@windDirect", item.wind != null ? item.wind.deg : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@lat", item.coord != null ? item.coord.lat : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@lon", item.coord != null ? item.coord.lon : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", weather != null ? weather.id : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@main", weather != null && !string.IsNullOrEmpty(weather.main) ? weather.main : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@icon", weather != null && !string.IsNullOrEmpty(weather.icon) ? weather.icon : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@min", item.main != null ? item.main.temp_min : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@max", item.main != null ? item.main.temp_max : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@gust", item.wind != null ? item.wind.gust : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@sys", item.sys != null ? item.sys.type : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@sunset", item.sys != null ? item.sys.sunset : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@sunrise", item.sys != null ? item.sys.sunrise : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@country", item.sys != null && !string.IsNullOrEmpty(item.sys.country) ? item.sys.country : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@timezone", item.timezone != null ? item.timezone : (object)DBNull.Value);



                    lock (Global_Variables.lockObj)
                    {
                        command.Parameters.AddWithValue("@city", Global_Variables.cityName != null ? Global_Variables.cityName : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@unit", Global_Variables.units != null ? Global_Variables.units : (object)DBNull.Value);
                    }

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }


        public WeatherData.Root selectCurrentDay(String city)
        {
            string dateString = DateTime.Now.ToString("dd.MM.yyyy");
            return selectDay(city, dateString);
        }

        public WeatherData.Root selectDay(String city, DateTime time)
        {
            string dateString = time.ToString("dd.MM.yyyy");
            return selectDay(city, dateString);
        }

        public WeatherData.Root selectDay(String city, String date)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM weather_data WHERE city = @city AND day = @day";
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.AddWithValue("@day", date);
                    WeatherData.Root res = new WeatherData.Root();

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            res = convertSqlToObject(reader);
                        }

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }

                    return res;
                }
            }
        }

        // Select Forecast from current daté to 5 days in the future
        public List<WeatherData.Root> selectForecast(String city)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    string dateNow = DateTime.Now.ToString("dd.MM.yyyy");
                    string dateThen = DateTime.Now.AddDays(5).ToString("dd.MM.yyyy");
                    command.CommandText = "SELECT * FROM weather_data WHERE city = @city AND day BETWEEN @now AND @then";
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.AddWithValue("@now", dateNow);
                    command.Parameters.AddWithValue("@then", dateThen);

                    List<WeatherData.Root> list = new List<WeatherData.Root>();

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WeatherData.Root res = convertSqlToObject(reader);
                                list.Add(res);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }

                    foreach (var item in list)
                    {
                        Trace.WriteLine(item.dt.ToString());
                    }

                    return list;
                }
            }
        }

        public void update(WeatherData.Root data)
        {
            foreach (var weather in data.weather)
            {
                executeUpdateQuery(data, weather);
            }

        }

        public void executeUpdateQuery(WeatherData.Root item, WeatherData.Weather weather)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE weather_data " +
                    "SET city = @city, temp = @temp, description = @description, " +
                    "main = @main, icon = @icon, id = @id, max_temp = @max, min_temp = @min, sunset = @sunset, sunrise = @sunrise," +
                    "pressure = @pressure, humidity = @humidity,  " +
                    "feels_like = @feels, wind_speed = @windSpeed, wind_direction = @windDirect, " +
                    " lat = @lat, lon = @lon, unit = @unit " +
                    "WHERE city = @city AND day = @date";

                    String dbDate = Utilities.unixTimeStampToDate(item.dt);

                    command.Parameters.AddWithValue("@day", dbDate);
                    command.Parameters.AddWithValue("@temp", item.main != null ? item.main.temp : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@description", weather != null && !string.IsNullOrEmpty(weather.description) ? weather.description : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@pressure", item.main != null ? item.main.pressure : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@humidity", item.main != null ? item.main.humidity : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@feels", item.main != null ? item.main.feels_like : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@windSpeed", item.wind != null ? item.wind.speed : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@windDirect", item.wind != null ? item.wind.deg : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@lat", item.coord != null ? item.coord.lat : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@lon", item.coord != null ? item.coord.lon : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@id", weather != null ? weather.id : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@main", weather != null && !string.IsNullOrEmpty(weather.main) ? weather.main : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@icon", weather != null && !string.IsNullOrEmpty(weather.icon) ? weather.icon : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@min", item.main != null ? item.main.temp_min : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@max", item.main != null ? item.main.temp_max : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@sunset", item.sys != null ? item.sys.sunset : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@sunrise", item.sys != null ? item.sys.sunrise : (object)DBNull.Value);



                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void delete(string data, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "DELETE FROM weather_data WHERE city = @city AND day = @date";
                    command.Parameters.AddWithValue("@day", date.ToString("dd.MM.yyyy"));
                    command.Parameters.AddWithValue("@city", data);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }

        public void deleteAllEntriesPerCity(string city)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {

                    conn.Open();
                }
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "DELETE FROM weather_data WHERE city = @city";
                    command.Parameters.AddWithValue("@city", city);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }

        //------------------------------------ HISTORICAL DATA METHODS (PLINQ) ------------------------------------


        public async void insertHistoricalData()
        {
            List<WeatherHistoricalData.Root> rootList = await Utilities.extractHistoricalWeatherData();

            rootList.AsParallel().ForAll(item =>
            {
                insertHistoricalBulkMulti(item);
            });
        }

        public void insertHistoricalBulkMulti(WeatherHistoricalData.Root data)
        {
            data.weather.AsParallel().ForAll(item =>
            {
                executeHistoricalInsert(data, item);
            });
        }

        public void executeHistoricalInsert(WeatherHistoricalData.Root item, WeatherHistoricalData.Weather weather)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {
                    conn.Open();
                }

                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = "INSERT INTO historical_weather" +
                    "(timecode, description, temp, feels_like, pressure, humidity) " +
                    "VALUES (@time, @description, @temp,  @feels, @pressure, @humidity)";

                    command.Parameters.AddWithValue("@time", item.dt);
                    command.Parameters.AddWithValue("@description", weather != null && !string.IsNullOrEmpty(weather.description) ? weather.description : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@temp", item.main != null ? item.main.temp : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@feels", item.main != null ? item.main.feels_like : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@pressure", item.main != null ? item.main.pressure : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@humidity", item.main != null ? item.main.humidity : (object)DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }
            }
        }



        public void insertHistoricalBulk(List<WeatherData.Root> data)
        {
            foreach (var item in data)
            {
                foreach (var weather in item.weather)
                {
                    executeInsertQuery(item, weather);
                }
            }
        }

        public List<WeatherHistoricalData.Root> selectHistoricalDataByDate(int start, int end)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn != null)
                {
                    conn.Open();
                }

                using (SqlCommand command = conn.CreateCommand())
                {

                    Debug.WriteLine(start);
                    Debug.WriteLine(end);
                    command.CommandText = "SELECT * FROM historical_weather WHERE timecode BETWEEN @now AND @then";
                    command.Parameters.AddWithValue("@now", start);
                    command.Parameters.AddWithValue("@then", end);

                    List<WeatherHistoricalData.Root> list = new List<WeatherHistoricalData.Root>();

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WeatherHistoricalData.Root res = convertHistoricalSqlToObject(reader);
                                list.Add(res);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }

                    foreach (WeatherHistoricalData.Root item in list)
                    {
                        Trace.WriteLine(item.main.ToString());
                    }
                    return list;
                }
            }
        }

        public WeatherHistoricalData.Root convertHistoricalSqlToObject(SqlDataReader reader)
        {
            WeatherHistoricalData.Root root = new WeatherHistoricalData.Root();
            root.main = new WeatherHistoricalData.Main();
            root.weather = new List<WeatherHistoricalData.Weather>();
            root.wind = new WeatherHistoricalData.Wind();
            WeatherHistoricalData.Weather weatherObj = new WeatherHistoricalData.Weather();


            root.dt = reader.GetInt32(0);

            if (!reader.IsDBNull(3))
            {
                weatherObj.description = reader.GetString(3);
            }

            if (!reader.IsDBNull(5))
            {
                root.main.temp = reader.GetDouble(5);
            }

            if (!reader.IsDBNull(6))
            {
                root.main.feels_like = reader.GetDouble(6);
            }

            if (!reader.IsDBNull(9))
            {
                root.main.pressure = reader.GetInt32(9);
            }

            if (!reader.IsDBNull(10))
            {
                root.main.humidity = reader.GetInt32(10);
            }

            root.weather.Add(weatherObj);

            Trace.WriteLine(root.main.ToString());
            return root;
        }
    }
}
