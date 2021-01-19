using MessageQueue;
using Microsoft.Extensions.Options;
using RabbitMQ;
using Syn.Bot.Oscova;
using System;
using System.Text.RegularExpressions;

namespace Bot
{
    public class BotService
    {
        private readonly OscovaBot _bot;
        private readonly RabbitMQService _rabbitMQ;
        private readonly object completedEventLock = new object();
        private EventHandler<NotCommandEventArgs> _notCommand;
        public event EventHandler<NotCommandEventArgs> NotCommand
        {
            add
            {
                if (_notCommand == null)
                {
                    _notCommand += value;
                }
            }
            remove
            {
                _notCommand -= value;
            }
        }

        public BotService(IOptions<RabbitMQInfo> info) : this(info.Value) { }

        public BotService(RabbitMQInfo info)
        {
            var regex = new Regex("(\\/stock=|\\/STOCK=)+([a-z|A-Z]+\\.)+[a-z|A-Z]{1,5}$");
            var allRegex = new Regex(".");

            _rabbitMQ = new RabbitMQService(info);
            _bot = new OscovaBot();
            _bot.CreateRecognizer("hex", regex);
            _bot.CreateRecognizer("all", allRegex);
            _bot.Dialogs.Add(new BotDialog());
            _bot.Trainer.StartTraining();

            _bot.MainUser.ResponseReceived += (sender, eventArgs) =>
            {
                var response = eventArgs.Response.Text;
                if (eventArgs.Response.Type.Equals("NotCommand"))
                {
                    var args = new NotCommandEventArgs { Text = response, Date = DateTime.Now };
                    OnNotCommand(args);
                }
                else
                {
                    SendMessageToRabbitMQ(response);
                }
            };
        }

        private void SendMessageToRabbitMQ(string text)
        {
            _rabbitMQ.PublishMessage(text);
        }

        public void CheckStocksCommand(string command)
        {
            var evaluationResult = _bot.Evaluate(command);
            evaluationResult.Invoke();
        }

        protected virtual void OnNotCommand(NotCommandEventArgs e)
        {
            var handler = _notCommand;
            handler?.Invoke(this, e);
        }
    }
}