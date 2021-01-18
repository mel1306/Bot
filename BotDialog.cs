using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;

namespace Bot
{
    public class BotDialog : Dialog
    {
        private readonly CsvService _csv;

        public BotDialog()
        {
            _csv = new CsvService();
        }

        [Expression("@hex")]
        public void CheckStock(Context context, Result result)
        {
            var text = result.Request.Text;
            var stockCode = text.Split("=")[1];
            var stock = _csv.GetStock(stockCode);
            var message = string.Empty;

            if (stock != null)
                message = stock.ToString();
            else
                message = $"{stockCode} was not found.";

            result.SendResponse(message);
        }
    }
}

//  /stock=aapl.us