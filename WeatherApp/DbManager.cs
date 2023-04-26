﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    internal class DbManager
    {
        private static DbManager dbManager;
        internal SqlConnection connection { get; set; }

        private DbManager()
        {
            string connectionString = "Data Source=LAPTOP-9M0QVSK5\\SQLEXPRESS01;Initial Catalog=WeatherDb;Integrated Security=True";
            connection = new SqlConnection(connectionString);
            connection.Open();
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
            WeatherData.Root root = new WeatherData.Root();
            root.main = new WeatherData.Main();
            root.weather = new List<WeatherData.Weather>();
            WeatherData.Weather weather = new WeatherData.Weather();
            root.weather.Add(weather);
            root.coord = new WeatherData.Coord();
            root.wind = new WeatherData.Wind();
            WeatherData.Weather weatherObj = new WeatherData.Weather();

            if (!reader.IsDBNull(4))
            {
                root.main.pressure = reader.GetInt32(4);
            }

            root.main.temp = reader.GetDouble(2);
            weatherObj.description = reader.GetString(3);
            root.dt = (int)Utilities.unixTimeStampFromDate(reader.GetDateTime(0));

            if (!reader.IsDBNull(4))
            {
                root.main.pressure = reader.GetInt32(4);
            }

            if (!reader.IsDBNull(5))
            {
                root.main.humidity = reader.GetInt32(5);
            }

            if (!reader.IsDBNull(7))
            {
                root.main.feels_like = reader.GetDouble(7);
            }

            if (!reader.IsDBNull(8))
            {
                root.wind.speed = reader.GetDouble(8);
            }

            if (!reader.IsDBNull(9))
            {
                root.wind.deg = reader.GetInt32(9);
            }

            root.coord.lat = reader.GetDouble(12);
            root.coord.lon = reader.GetDouble(13);
            root.weather.Add(weatherObj);

            Trace.WriteLine(root.ToString());
            return root;
        }

        //------------------------------------ MULTITHREADING METHODS (PLINQ) ------------------------------------


        // Accepts a list of WeatherData.Root objects
        public void insertMulti(WeatherForecastData list)
        {
            list.list.AsParallel().ForAll(data => {
                insertMulti(data);
            });
        }

        // Accepts single WeatherDate.Root object 
        public void insertMulti(WeatherData.Root item)
        {
            item.weather.AsParallel().ForAll(weather => {
                executeInsertQuery(item, weather);
            });
        }

        public void updateMulti(WeatherData.Root data)
        {
            data.weather.AsParallel().ForAll(weather => {
                executeUpdateQuery(data, weather);
            });

        }



        //------------------------------------ REGULAR METHODS ------------------------------------

        //Insert for WeatherForecastData
        public void insertSingle(List<WeatherData.Root> list)
        {
            foreach (var item in list)
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
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO weather_data " +
                "(day, city, temp, description, pressure, humidity, feels_like, wind_speed, wind_deg, lat, lon, unit) " +
                "VALUES (@day, @city, @temp, @description, @pressure, @humidity, @feels, @windSpeed, @windDirect, @lat, @lon, @unit)";

            String dbDate = Utilities.unixTimeStampToDate(item.dt);

            command.Parameters.AddWithValue("@day", dbDate);
            command.Parameters.AddWithValue("@temp", item.main != null ? item.main.temp : (object)DBNull.Value);
            command.Parameters.AddWithValue("@description", weather != null ? weather.description : (object)DBNull.Value);
            command.Parameters.AddWithValue("@pressure", item.main != null ? item.main.pressure : (object)DBNull.Value);
            command.Parameters.AddWithValue("@humidity", item.main != null ? item.main.humidity : (object)DBNull.Value);
            command.Parameters.AddWithValue("@feels", item.main != null ? item.main.feels_like : (object)DBNull.Value);
            command.Parameters.AddWithValue("@windSpeed", item.wind != null ? item.wind.speed : (object)DBNull.Value);
            command.Parameters.AddWithValue("@windDirect", item.wind != null ? item.wind.deg : (object)DBNull.Value);
            command.Parameters.AddWithValue("@lat", item.coord != null ? item.coord.lat : (object)DBNull.Value);
            command.Parameters.AddWithValue("@lon", item.coord != null ? item.coord.lon : (object)DBNull.Value);

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
            SqlCommand command = connection.CreateCommand();
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

        // Select Forecast from current daté to 5 days in the future
        public List<WeatherData.Root> selectForecast(String city)
        {
            string dateNow = DateTime.Now.ToString("dd.MM.yyyy");
            string dateThen = DateTime.Now.AddDays(5).ToString("dd.MM.yyyy");
            SqlCommand command = connection.CreateCommand();
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

        public void update(WeatherData.Root data)
        {
            foreach (var weather in data.weather)
            {
                executeUpdateQuery(data, weather);
            }

        }

        public void executeUpdateQuery(WeatherData.Root data, WeatherData.Weather weather)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE weather_data " +
                "SET city = @city, temp = @temp, description = @description, " +
                "pressure = @pressure, humidity = @humidity,  " +
                "feels_like = @feels, wind_speed = @windSpeed, wind_direction = @windDirect, " +
                " lat = @lat, lon = @lon, unit = @unit " +
                "WHERE city = @city AND day = @date";

            String dbDate = Utilities.unixTimeStampToDate(data.dt);
            command.Parameters.AddWithValue("@day", dbDate);
            command.Parameters.AddWithValue("@city", Global_Variables.cityName);
            command.Parameters.AddWithValue("@temp", data.main.temp);
            command.Parameters.AddWithValue("@description", weather.description);
            command.Parameters.AddWithValue("@pressure", data.main.pressure);
            command.Parameters.AddWithValue("@humidity", data.main.humidity);
            command.Parameters.AddWithValue("@feels", data.main.feels_like);
            command.Parameters.AddWithValue("@windSpeed", data.wind.speed);
            command.Parameters.AddWithValue("@windDirect", data.wind.deg);
            command.Parameters.AddWithValue("@lat", data.coord.lat);
            command.Parameters.AddWithValue("@lon", data.coord.lon);
            command.Parameters.AddWithValue("@unit", Global_Variables.units);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public void delete(string data, DateTime date)
        {
            SqlCommand command = connection.CreateCommand();
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

        public void deleteAllEntriesPerCity(string city)
        {
            SqlCommand command = connection.CreateCommand();
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

        //------------------------------------ HISTORICAL DATA METHODS (PLINQ) ------------------------------------


        // TO DO: Rewrite all historical data stuff to fit new class structure
        /*public void insertHistoricalBulkMulti(WeatherHistoricalData data)
        {
            data.weather.AsParallel().ForAll(item =>
            {
                item.weather.AsParallel().ForAll(weather =>
                {

                });
            });
        }*/

        public void executeHistoricalInsert(WeatherData.Root item, WeatherData.Weather weather)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO historical_data" +
            "(timestamp, temp,  feels_like, pressure, humidity, temp_min, temp_max, wind_speed, wind_deg, main, description, icon) " +
            "VALUES (@time, @temp,  @feels, @pressure, @humidity, @min, @max, @windSpeed, @windDirect,  @main, @description, @icon)";

            command.Parameters.AddWithValue("@time", item.dt);
            command.Parameters.AddWithValue("@temp", item.main.temp);
            command.Parameters.AddWithValue("@pressure", item.main.pressure);
            command.Parameters.AddWithValue("@humidity", item.main.humidity);
            command.Parameters.AddWithValue("@feels", item.main.feels_like);
            command.Parameters.AddWithValue("@min", item.main.temp_min);
            command.Parameters.AddWithValue("@max", item.main.temp_max);
            command.Parameters.AddWithValue("@windSpeed", item.wind.speed);
            command.Parameters.AddWithValue("@windDirect", item.wind.deg);
            command.Parameters.AddWithValue("@description", "joa"); //weather.description
            command.Parameters.AddWithValue("@main", "ioefa"); //weather.main
            command.Parameters.AddWithValue("@icon", "ihafio"); //weather.icon

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
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

        public List<WeatherData.Root> selectHistoricalDataByDate(int start, int end)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM historical_data WHERE timestamp BETWEEN @now AND @then";
            command.Parameters.AddWithValue("@now", start);
            command.Parameters.AddWithValue("@then", end);

            List<WeatherData.Root> list = new List<WeatherData.Root>();

            try
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WeatherData.Root res = convertHistoricalSqlToObject(reader);
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

        public WeatherData.Root convertHistoricalSqlToObject(SqlDataReader reader)
        {
            WeatherData.Root root = new WeatherData.Root();
            root.main = new WeatherData.Main();
            root.weather = new List<WeatherData.Weather>();
            root.wind = new WeatherData.Wind();
            WeatherData.Weather weatherObj = new WeatherData.Weather();


            root.dt = reader.GetInt32(0);
            root.main.temp = reader.GetDouble(1);

            if (!reader.IsDBNull(2))
            {
                root.main.feels_like = reader.GetDouble(2);
            }

            if (!reader.IsDBNull(3))
            {
                root.main.pressure = reader.GetInt32(3);
            }

            if (!reader.IsDBNull(4))
            {
                root.main.humidity = reader.GetInt32(4);
            }

            if (!reader.IsDBNull(5))
            {
                root.main.temp_min = reader.GetDouble(5);
            }

            if (!reader.IsDBNull(6))
            {
                root.main.temp_max = reader.GetDouble(6);
            }

            if (!reader.IsDBNull(7))
            {
                root.wind.speed = reader.GetDouble(7);
            }

            if (!reader.IsDBNull(8))
            {
                root.wind.deg = reader.GetInt32(8);
            }

            if (!reader.IsDBNull(9))
            {
                weatherObj.main = reader.GetString(9);
            }

            if (!reader.IsDBNull(10))
            {
                weatherObj.description = reader.GetString(10);
            }

            if (!reader.IsDBNull(11))
            {
                weatherObj.icon = reader.GetString(11);
            }


            root.weather.Add(weatherObj);

            Trace.WriteLine(root.ToString());
            return root;
        }
    }
}