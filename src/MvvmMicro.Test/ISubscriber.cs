namespace MvvmMicro.Test
{
    public interface ISubscriber<T>
    {
        void Callback(T message);
    }
}
