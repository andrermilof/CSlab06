using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lab06
{
    public class Weather
    {
        public static int lat { get { return (new Random()).Next(-90, 90); } } 
        public static int lon { get { return (new Random()).Next(-180, 180); } }

        public static string key = "b3aa469ecf9912946416bcda21666d49";
        public string Country { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Temp { get; set; }
    }
    public class Program
    {
        static async Task Main(string[] args)
        {
            List<Weather> lw = new List<Weather>();
            string url;
            using var client = new HttpClient();
            HttpResponseMessage result;
            string res;
            Regex rCountry = new Regex("(\"country\":\"(?<Country>[A-Z]*)\")|(\"name\":\"(?<Name>[A-Za-z]*)\")|(\"temp\":(?<Temp>[0-9.]*))|(\"description\":\"(?<Desc>[A-Za-z\\s]*)\")");
            MatchCollection matches;

            
            for (int i = 0; i < 50;)
            {
                url = $"https://api.openweathermap.org/data/2.5/weather?lat={Weather.lat}&lon={Weather.lon}&appid={Weather.key}";
                result = await client.GetAsync(url);
                res = await result.Content.ReadAsStringAsync();
                matches = rCountry.Matches(res);

                if (matches.Count == 4)
                    if (matches[3].Groups["Name"].Value != "" && matches[2].Groups["Country"].Value != "")   
                    {
                        i++;
                        lw.Add(new Weather()
                        {
                            Country = matches[2].Groups["Country"].Value,
                            Name = matches[3].Groups["Name"].Value,
                            Description = matches[0].Groups["Desc"].Value,
                            Temp = double.Parse(matches[1].Groups["Temp"].Value.Replace('.', ','))
                        });
                    }
            }

            foreach(var i in lw)
            {
                Console.WriteLine(i.Name);
                Console.WriteLine(i.Country);
                Console.WriteLine(i.Temp);
                Console.WriteLine(i.Description);
                Console.WriteLine();
            }

            var min = lw.Select(i => i.Temp).Min();
            var max = lw.Select(i => i.Temp).Max();
            var avr = lw.Select(i => i.Temp).Average();
            var count = lw.Select(i => i.Country).Distinct().Count();
            var cntr = lw.First(i => i.Description == "clear sky" || i.Description == "rain" || i.Description == "few clouds");

            Console.WriteLine(count);
            Console.WriteLine(min);
            Console.WriteLine(max);
            Console.WriteLine(avr);
            Console.WriteLine(cntr.Country + '\n' + cntr.Name);
        }
    }

}