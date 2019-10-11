namespace UnityUtils
{
    public interface IDeepCloneable<out T>
    {
        T DeepCopy();
    }
}