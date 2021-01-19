using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Linq;

namespace Bot
{
    public class CsvService
    {
        public StockSymbol GetStock(string symbol)
        {
            StockSymbol stock = null;
            try
            {
                var url = $"https://stooq.com/q/l/?s={symbol}&f=sd2t2ohlcv&h&e=csv";
                var client = new WebClient();
                var stream = client.OpenRead(url);
                using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<StockSymbol>();
                stock = records.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return stock;
        }
    }
}
