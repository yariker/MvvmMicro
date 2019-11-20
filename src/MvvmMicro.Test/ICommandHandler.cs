namespace Takesoft.MvvmMicro.Test
{
    public interface ICommandHandler
    {
        bool CanExecute();
        void Execute();
    }      
}
