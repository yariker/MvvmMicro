namespace Takesoft.MvvmMicro.Test
{
    public interface ICommandHandler<T>
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }
}
