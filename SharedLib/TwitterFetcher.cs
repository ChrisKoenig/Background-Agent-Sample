using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace SharedLib
{
    public class TwitterFetcher
    {
        private const string twitterSearchUri = "http://search.twitter.com/search.json?q={0}";

        public void GetTweets(string searchText, Action<IQueryable<Tweet>> func)
        {
            var twitterSearchString = string.Format(twitterSearchUri, HttpUtility.UrlEncode(searchText));
            var request = HttpWebRequest.CreateHttp(twitterSearchString) as HttpWebRequest;
            request.BeginGetResponse((cb) =>
            {
                string results = "";
                using (var response = request.EndGetResponse(cb) as HttpWebResponse)
                using (var stream = response.GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                    results = streamReader.ReadToEnd();

                var result = JsonConvert.DeserializeObject<JResponse>(results);
                var tweets = result.results.Select(r => new Tweet()
                {
                    Sender = r.from_user,
                    SenderPictureUrl = r.profile_image_url,
                    Text = r.text,
                    SentDate = DateTime.Now,
                });
                func(tweets.AsQueryable());
            }, null);
        }
    }
}