using MessageQueue;
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

        //public BotService(IMessageQueue messageQueue)
        public BotService(RabbitMQInfo info)
        {
            var regex = new Regex("(\\/stock=|\\/STOCK=)+([a-z|A-Z]+\\.)+[a-z|A-Z]{1,5}$");
            _rabbitMQ = new RabbitMQService(info);
            _bot = new OscovaBot();
            _bot.CreateRecognizer("hex", regex);
            //_bot.Dialogs.Add(new BotDialog(messageQueue));
            _bot.Dialogs.Add(new BotDialog());
            _bot.Trainer.StartTraining();

            _bot.MainUser.ResponseReceived += (sender, eventArgs) =>
            {
                var csvResponse = eventArgs.Response.Text;
                //Console.WriteLine(eventArgs.Response.Text);
                //Aqui envio el mensaje al chat por medio de RabbitMQ
                SendMessageToRabbitMQ(csvResponse);
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
    }
}