using System;
namespace Bot.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string UserID { get; set; }
        public string Message_side { get; set; }
    }
}
