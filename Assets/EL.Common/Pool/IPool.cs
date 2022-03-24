namespace EL.Common.Pool
{
    public interface IPool<T>
    {
        bool CanTake();
        T Take();
        void Release(T item);
    }
}