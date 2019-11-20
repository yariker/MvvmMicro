using System.Threading.Tasks;

namespace Takesoft.MvvmMicro.Test
{
    public interface IAsyncCommandHandler<T>
    {
        bool CanExecute(T parameter);
        Task ExecuteAsync(T parameter);
    }
}
