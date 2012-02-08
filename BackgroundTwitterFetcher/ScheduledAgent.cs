#define DEBUG_AGENT

using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using SharedLib;

namespace BackgroundTwitterFetcher
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            var fetcher = new TwitterFetcher();
            fetcher.GetTweets("#30tolaunch", (list) =>
            {
                var tweet = list.FirstOrDefault();
                if (tweet != null)
                {
                    UpdateTile(tweet);
                }
#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(
                    "ScheduledAgent",
                    TimeSpan.FromSeconds(30));
#endif
                NotifyComplete();
            });
        }

        private void UpdateTile(Tweet tweet)
        {
            var tile = ShellTile.ActiveTiles.First();
            var data = new StandardTileData()
            {
                BackTitle = tweet.Sender,
                BackContent = tweet.Text,
            };
            tile.Update(data);
        }
    }
}