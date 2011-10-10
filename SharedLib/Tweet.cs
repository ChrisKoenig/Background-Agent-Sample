using System;

namespace SharedLib
{
    public class Tweet
    {
        public string Sender { get; set; }

        public string SenderPictureUrl { get; set; }

        public string Text { get; set; }

        public DateTime SentDate { get; set; }
    }
}