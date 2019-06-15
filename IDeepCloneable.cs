namespace Utils
{
    public interface IDeepCloneable<out T>
    {
        T DeepCopy();
    }
}