using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StockCrawler
{
    public class Stock
    {
        public int code;
        public string name;
        public double price;
        public string date;

        public override string ToString()
        {
            return "code = " + code + ", name = " + name + ", price = " + price + ", date = " + date;
        }
    }

    public static class Program
    {
        public static List<Stock> GetStock(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            string dayString = date.ToString("yyyyMMdd");
            List<Stock> allStocks = new List<Stock>();

            string url = string.Format("https://www.twse.com.tw/exchangeReport/MI_INDEX?response=json&date={0}&type=ALLBUT0999", dayString);
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                HttpResponseMessage response = client.SendAsync(request).Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to get data");

                string responseBody = response.Content.ReadAsStringAsync().Result;
                if (responseBody.Length < 1000)
                    return allStocks;

                JObject all = JObject.Parse(responseBody);
                JArray allData = null;

                if (all["fields8"][0].ToString() == "證券代號")
                {
                    allData = all["data8"] as JArray;
                }
                else if (all["fields9"][0].ToString() == "證券代號")
                {
                    allData = all["data9"] as JArray;
                }

                if (allData == null)
                {
                    Trace.WriteLine(all);
                    throw new Exception("Unknown format");
                }

                foreach (JArray j in allData)
                {
                    Stock s = new Stock();

                    if (int.TryParse(j[0].ToString(), out s.code) == false)
                        continue;

                    s.name = j[1].ToObject<string>();

                    if (double.TryParse(j[8].ToString(), out s.price) == false)
                        continue;

                    s.date = dayString;
                    allStocks.Add(s);
                }

                if (allStocks.Count == 0)
                {
                    Trace.WriteLine(all);
                    throw new Exception("Unknown format");
                }

                return allStocks;
            }
        }

        public static void GetStockExample()
        {
            List<Stock> stocks = GetStock(2024, 2, 19);
            foreach (Stock s in stocks)
            {
                if (s.code == 5203 || s.code == 2454)
                {
                    // code = 2454, name = 聯發科, price = 963, date = 20240219
                    // code = 5203, name = 訊連, price = 92.8, date = 20240219
                    Console.WriteLine(s);
                }
            }
        }

        public static void Main()
        {
            GetStockExample();
        }
    }
}