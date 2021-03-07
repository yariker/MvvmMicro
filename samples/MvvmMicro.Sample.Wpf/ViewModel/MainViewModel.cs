using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using MvvmMicro.Sample.Wpf.Model;
using MvvmMicro.Sample.Wpf.Services;

namespace MvvmMicro.Sample.Wpf.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICatFactFeed _catFactFeed;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isLoading = true;

        public MainViewModel(IMessenger messenger, ICatFactFeed catFactFeed)
            : base(messenger)
        {
            _catFactFeed = catFactFeed ?? throw new ArgumentNullException(nameof(catFactFeed));
            LoadCommand = new AsyncRelayCommand<ScrollChangedEventArgs>(OnLoadAsync, CanLoad);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);

            if (IsInDesignMode)
            {
                LoadCommand.Execute(null);
                IsLoading = true;
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => Set(ref _isLoading, value);
        }

        public ObservableCollection<Fact> Facts { get; } = new();

        /// <summary>
        /// Gets the command that loads more items into the <see cref="Facts"/> collection.
        /// </summary>
        public AsyncRelayCommand<ScrollChangedEventArgs> LoadCommand { get; }

        /// <summary>
        /// Gets the command that cancels the <see cref="LoadCommand"/> if it's running.
        /// </summary>
        public RelayCommand CancelCommand { get; }

        private bool CanLoad(ScrollChangedEventArgs e)
        {
            // Execute the command only if the list is empty or it's scrolled all the way down.
            return Facts.Count == 0 || e != null && Math.Round(e.VerticalOffset) >= Math.Round(e.ExtentHeight - e.ViewportHeight);
        }

        private async Task OnLoadAsync(ScrollChangedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IsLoading = true;

            try
            {
                var facts = await _catFactFeed.GetFactsAsync(5, _cancellationTokenSource.Token);
                foreach (var fact in facts)
                {
                    Facts.Add(fact);
                }
            }
            catch (OperationCanceledException)
            {
                if (Facts.Count == 0)
                {
                    Messenger.Publish(Notifications.CloseWindow);
                }
            }
            finally
            {
                IsLoading = false;
                _cancellationTokenSource.Dispose(); 
            }
        }

        private bool CanCancel()
        {
            return IsLoading && _cancellationTokenSource?.IsCancellationRequested == false;
        }

        private void OnCancel()
        {
            _cancellationTokenSource.Cancel();
            CancelCommand.RaiseCanExecuteChanged();
        }
    }
}