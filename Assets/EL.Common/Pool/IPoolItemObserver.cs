namespace EL.Common.Pool
{
    public interface IPoolItemObserver<T, in TS>
    {
        T Create(TS source);
        void AfterTake(T item);
        void AfterReturn(T item);
    }
}