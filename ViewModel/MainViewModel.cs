#define DEBUG_AGENT

using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Scheduler;
using SharedLib;

namespace BackgroundAgentSample.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Tweet> _Tweets;
        private PeriodicTask periodicTask;
        private const string periodicTaskName = "ScheduledAgent";
        private bool AgentIsEnabled;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                LoadTweets();
                periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            }
        }

        public string SearchKey
        {
            get
            {
                return "dfwwpdev";
            }
        }

        public bool IsEnabled
        {
            get { return periodicTask == null ? false : periodicTask.IsEnabled; }
        }

        public ObservableCollection<Tweet> Tweets
        {
            get { return _Tweets; }
            set
            {
                if (_Tweets != value)
                {
                    _Tweets = value;
                    RaisePropertyChanged(() => Tweets);
                }
            }
        }

        public void LoadTweets()
        {
            var f = new TwitterFetcher();
            f.GetTweets(SearchKey, list =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Tweets = new ObservableCollection<Tweet>(list);
                });
            });
        }

        public void StartPeriodicAgent()
        {
            AgentIsEnabled = true;

            // If this task already exists, remove it
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
            {
                RemoveAgent();
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = "This demonstrates a periodic task.";

            try
            {
                ScheduledActionService.Add(periodicTask);

#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(
                    periodicTaskName,
                    TimeSpan.FromSeconds(30));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    AgentIsEnabled = false;
                }
                else
                {
                    throw;
                }
            }
        }

        public void RemoveAgent()
        {
            try
            {
                ScheduledActionService.Remove(periodicTaskName);
            }
            catch
            {
                //noop
            }
        }
    }
}