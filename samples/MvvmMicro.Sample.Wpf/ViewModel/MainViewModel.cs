using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MvvmMicro.Sample.Wpf.Model;
using MvvmMicro.Sample.Wpf.Services;

namespace MvvmMicro.Sample.Wpf.ViewModel;

public class MainViewModel : ViewModelBase
{
    private readonly ICatFactFeed _catFactFeed;

    public MainViewModel(IMessenger messenger, ICatFactFeed catFactFeed)
        : base(messenger)
    {
        _catFactFeed = catFactFeed ?? throw new ArgumentNullException(nameof(catFactFeed));

        LoadCommand = new AsyncRelayCommand<ScrollChangedEventArgs>(OnLoadAsync, CanLoad);
        CancelCommand = new RelayCommand(LoadCommand.Cancel, () => LoadCommand.IsExecuting);

        if (IsInDesignMode)
        {
            LoadCommand.Execute(null);
        }
    }

    public ObservableCollection<Fact> Facts { get; } = new();

    /// <summary>
    /// Gets the command that loads more items into the <see cref="Facts"/> collection.
    /// </summary>
    public IAsyncCommand<ScrollChangedEventArgs> LoadCommand { get; }

    /// <summary>
    /// Gets the command that cancels the <see cref="LoadCommand"/> if it's running.
    /// </summary>
    public ICommand CancelCommand { get; }

    private bool CanLoad(ScrollChangedEventArgs e)
    {
        // Execute the command only if the list is empty or it's scrolled all the way down.
        return Facts.Count == 0 || e != null && Math.Round(e.VerticalOffset) >= Math.Round(e.ExtentHeight - e.ViewportHeight);
    }

    private async Task OnLoadAsync(ScrollChangedEventArgs e, CancellationToken cancellationToken)
    {
        try
        {
            var facts = await _catFactFeed.GetFactsAsync(5, cancellationToken);
            foreach (var fact in facts)
            {
                Facts.Add(fact);
            }
        }
        catch (OperationCanceledException)
        {
            if (Facts.Count == 0)
            {
                Messenger.Send(Notifications.CloseWindow);
            }
        }
    }
}
