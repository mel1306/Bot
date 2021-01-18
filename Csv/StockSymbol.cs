namespace Bot
{
    public class StockSymbol
    {
        public string Symbol { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public override string ToString()
        {
            return $"{Symbol?.ToUpper() ?? string.Empty} quote is $ {Close} per share";
        }
    }
}
