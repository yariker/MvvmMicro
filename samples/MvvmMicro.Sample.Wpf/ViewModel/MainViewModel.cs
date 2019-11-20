using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Takesoft.MvvmMicro.Sample.NetFx.Model;
using Takesoft.MvvmMicro.Sample.NetFx.Services;

namespace Takesoft.MvvmMicro.Sample.NetFx.ViewModel
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

        /// <summary>
        /// Gets the command that loads more items into the <see cref="Facts"/> collection.
        /// </summary>
        public ICommand LoadCommand { get; }

        /// <summary>
        /// Gets the command that cancels the <see cref="LoadCommand"/> if it's running.
        /// </summary>
        public ICommand CancelCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            private set => Set(ref _isLoading, value);
        }

        public ObservableCollection<Fact> Facts { get; } = new ObservableCollection<Fact>();

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
                Fact[] facts = await _catFactFeed.GetFactsAsync(5, _cancellationTokenSource.Token);
                foreach (Fact fact in facts)
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
            CommandManager.InvalidateRequerySuggested();
        }
    }
}