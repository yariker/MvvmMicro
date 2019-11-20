using System.Threading.Tasks;

namespace Takesoft.MvvmMicro.Test
{
    public interface IAsyncCommandHandler
    {
        bool CanExecute();
        Task ExecuteAsync();
    }    
}
