namespace EL.Common.Pool
{
    public interface IObjectPool
    {
        void Register<T>(IPool<T> pool);

        bool CanTakeFromPool<T>();

        T Take<T>();
        void Release<T>(T item);

        IPool<T> GetPoll<T>();
    }
}