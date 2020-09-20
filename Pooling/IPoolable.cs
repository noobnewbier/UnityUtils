namespace UnityUtils.Pooling
{
    public interface IPoolable<out T> where T : class
    {
        T GetPooledInstance();
        void ReturnToPool();
    }
}