using System.Text.RegularExpressions;
using Bot.Models;
using Newtonsoft.Json;
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
            var message = stock != null ? stock.ToString() : $"{stockCode} was not found.";
            result.SendResponse(message);
        }

        [Expression("@all")]
        public void AllExpressions(Context context, Result result)
        {
            var messageJson = JsonConvert.DeserializeObject<Message>(result.Request.Text);
            var regex = new Regex("(\\/stock=|\\/STOCK=)+([a-z|A-Z]+\\.)+[a-z|A-Z]{1,5}$");
            if (regex.IsMatch(messageJson.Text))
            {
                var text = messageJson.Text;
                var stockCode = text.Split("=")[1];
                var stock = _csv.GetStock(stockCode);
                var message = stock != null ? stock.ToString() : $"{stockCode} was not found.";
                result.SendResponse(message);
            }
            else
            {
                result.SendResponse(new Response
                {
                    Type = "NotCommand",
                    Text = result.Request.Text
                });
            }
        }
    }
}