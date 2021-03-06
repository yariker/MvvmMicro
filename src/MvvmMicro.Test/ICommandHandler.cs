namespace MvvmMicro.Test
{
    public interface ICommandHandler
    {
        bool CanExecute();
        void Execute();
    }      
}
