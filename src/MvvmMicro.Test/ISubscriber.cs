namespace PlaidSoft.MvvmMicro.Test
{
    public interface ISubscriber<T>
    {
        void Callback(T message);
    }
}
