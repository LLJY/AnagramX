using AnagramX.Repositories;
using AnagramX.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AnagramX
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        private readonly MainViewModel _viewModel;
        public MainPage()
        {
            InitializeComponent();
            // inject a repository instance, I'm lazy to use a DI framework
            _viewModel = new MainViewModel(new MainRepository());
           Initialize();
        }
        private async void Initialize()
        {
            HideUiForLoading(true);
            var task = _viewModel.DownloadDictionary();
            await task;
            HideUiForLoading(false);
        }
        /// <summary>
        /// Click event handler to generate random 10 characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomButton_Clicked(object sender, EventArgs e)
        {
            AnagramText.Text = _viewModel.GetRandomCharacters();
        }
        /// <summary>
        /// Event handler for finding anagrams
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GenerateButton_Clicked(object sender, EventArgs e)
        {
            // check if the user has not clicked generate without entering text
            if (!string.IsNullOrWhiteSpace(AnagramText.Text))
            {
                HideUiForLoading(true);
                // start a stopwatch to benchmark elapsed time (for benchmarking fastest algorithm and languages)
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                // get the list of anagrams for the user input
                var anagrams = await Task.Run(() => _viewModel.GetAnagrams(AnagramText.Text));
                HideUiForLoading(false);
                progressText.Text = $"Elapsed Time: {stopWatch.ElapsedMilliseconds}ms\nAnagrams Found:{anagrams.Count} ";
                progressText.IsVisible = true;
                // we don't need the stopwatch anymore
                stopWatch.Stop();
                AnagramsListView.ItemsSource = anagrams;
            }
        }
        /// <summary>
        /// Hides certain ui elements while the anagrams are being calculated
        /// </summary>
        /// <param name="hide">
        /// Determines to hide or unhide elements</param>
        private void HideUiForLoading(bool hide)
        {
            AnagramsListView.IsVisible = !hide;
            RandomButton.IsVisible = !hide;
            GenerateButton.IsVisible = !hide;
            progressCircle.IsVisible = hide;
            progressCircle.IsRunning = hide;
            progressText.IsVisible = hide;
            progressText.Text = "Loading...";
        }
    }
}
