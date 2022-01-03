namespace UnityUtils.Factories
{
    public interface IFactory<in TArg1, in TArg2, in TArg3, out TOut>
    {
        TOut Create(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface IFactory<in TArg1, in TArg2, out TOut>
    {
        TOut Create(TArg1 arg1, TArg2 arg2);
    }

    public interface IFactory<in TArg, out TOut>
    {
        TOut Create(TArg arg);
    }

    public interface IFactory<out TOut>
    {
        TOut Create();
    }
}