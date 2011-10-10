#define DEBUG_AGENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BackgroundAgentSample.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

namespace BackgroundAgentSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel viewModel;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                viewModel = new MainViewModel();
                this.DataContext = viewModel;
            };
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            viewModel.LoadTweets();
        }

        private void PeriodicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.StartPeriodicAgent();
        }

        private void PeriodicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveAgent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
        }
    }
}