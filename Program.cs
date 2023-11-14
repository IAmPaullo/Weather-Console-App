using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherApp2
{
    class Program
    {
        const string API_KEY_FILE = "apiKey.txt";
        //static string filePath = Path.Combine(Environment.CurrentDirectory, @"Data\", API_KEY_FILE);
        static string filePath = Path.Combine(Environment.CurrentDirectory, @"Data\", API_KEY_FILE);

        const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather?";
        static string url;
        static string API_KEY;
        static string CITY;


        static async Task Main(string[] args)
        {
            SetUpAPI();
            Console.WriteLine("API Key will be removed from Console Screen shortly");
            Thread.Sleep(3000);
            for (int i = 0; i < 5; i++)
            {

                Console.WriteLine("Clearing API Key from Console...");
                Thread.Sleep(500);
                Console.Clear();
                Thread.Sleep(100);
            }
            Console.WriteLine("What City do you want to know the Weather?");
            CITY = Console.ReadLine();

            url = BASE_URL + "appid=" + API_KEY + "&q=" + CITY;
            try
            {
                WeatherData weatherData = await GetWeatherDataAsync();
                Console.WriteLine($"Currently, it's {weatherData.Temperature} °C in {CITY}!");
                Console.WriteLine($"The Weather is {weatherData.WeatherCondition}!");
                Console.WriteLine(GetResponseOnWeather(weatherData.Temperature));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void SetUpAPI()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error reading API Key: File does not Exist");
                throw new Exception($"Error reading API Key: File does not Exist");
            }

            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            string apiKey = streamReader.ReadLine();
            API_KEY = apiKey;
            if (string.IsNullOrEmpty(API_KEY))
            {
                Console.WriteLine("Error reading API Key: File is Empty");
                throw new Exception($"Error reading API Key: File is Empty");
            }
            Console.WriteLine($"Sucessfully read API Key: {apiKey}");

            fileStream.Close();
            streamReader.Close();

            //old reading file method
            //string apiKey = File.ReadAllText(filePath);
            //API_KEY = apiKey;
        }

        static async Task<WeatherData> GetWeatherDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    return ParseWeatherData(data);
                }
                else
                {
                    throw new Exception($"Failed to fetch data. Status code: {response.StatusCode}");
                }
            }
        }

        private static WeatherData ParseWeatherData(string _data)
        {
            JObject jsonResponse = JObject.Parse(_data);
            double temperatureKelvin = jsonResponse["main"]["temp"].Value<double>();
            double temperatureCelsius = temperatureKelvin - 273.15;
            string weatherCondition = jsonResponse["weather"]?[0]?["main"]?.ToString() ?? "Unknown";

            return new WeatherData
            {
                Temperature = temperatureCelsius,
                WeatherCondition = weatherCondition
                
            };
        }

        private static string GetResponseOnWeather(double _temperature)
        {
            if (_temperature >= 30)
            {
                return "It's so HOT!";
            }
            else if (_temperature >= 15 && _temperature < 30)
            {
                return "Such a nice Weather!";
            }
            else if (_temperature <= 14 && _temperature >= 5)
            {
                return "Cold! But it's ok";
            }
            else if (_temperature <= 4)
            {
                return "Brrrr... It's freezing";
            }
            else
            {
                return "Dont even know what to say";
            }
        }
    }
}

class WeatherData
{
    public double Temperature { get; set; }
    public string WeatherCondition { get; set; }
}

