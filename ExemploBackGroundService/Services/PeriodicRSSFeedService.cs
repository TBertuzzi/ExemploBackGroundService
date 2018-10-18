using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExemploBackGroundService.Models;
using Matcha.BackgroundService;
using Microsoft.Toolkit.Parsers.Rss;
using MonkeyCache.SQLite;
using Xamarin.Forms;

namespace ExemploBackGroundService.Services
{
    public class PeriodicRSSFeedService : IPeriodicTask
    {
        private string _key = "rssFeed"; //Chave com o nome do objeto que armazena os dados.

        public PeriodicRSSFeedService(int minutes)
        {
            Interval = TimeSpan.FromMinutes(minutes);
        }

        public TimeSpan Interval { get; set; }

        public async Task StartJob()
        {
            bool existeNoticiaNova = false;
            string feed = null;

            var existingList = Barrel.Current.Get<List<RssData>>(_key) ?? new List<RssData>();

            using (var client = new HttpClient())
            {

                feed = await client.GetStringAsync("https://medium.com/feed/@bertuzzi");

            }

            var parser = new RssParser();
            var rss = parser.Parse(feed).OrderByDescending(e => e.PublishDate).ToList(); ;

            if (feed != null)
            {
                foreach (var rssSchema in rss)
                {
                    var isExist = existingList.Any(e => e.Guid == rssSchema.InternalID);

                    var rssdata = new RssData
                    {
                        Title = rssSchema.Title,
                        PubDate = rssSchema.PublishDate,
                        Link = rssSchema.FeedUrl,
                        Guid = rssSchema.InternalID,
                        Author = rssSchema.Author,
                        Thumbnail = string.IsNullOrWhiteSpace(rssSchema.ImageUrl) ? $"https://placeimg.com/80/80/nature" : rssSchema.ImageUrl,
                        Description = rssSchema.Summary
                    };

                    if (!isExist)
                    {
                        //Se Existiu alguma noticia nova, pelo menos 1 vez
                        existeNoticiaNova = true;
                        existingList.Add(rssdata);
                    }
                }

                if (existeNoticiaNova)
                    MessagingCenter.Send(existingList, "Update");
            }

            existingList = existingList.OrderByDescending(e => e.PubDate).ToList();

            Barrel.Current.Add(_key, existingList, TimeSpan.FromDays(30));
        }
    }
}
