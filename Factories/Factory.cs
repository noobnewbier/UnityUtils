using System;

namespace UnityUtils.Factories
{
    public static class Factory
    {
        public static IFactory<TOut> Create<TOut>(Func<TOut> factoryMethod) => new AnonymousFactory<TOut>(factoryMethod);

        public static IFactory<TArg, TOut> Create<TArg, TOut>(Func<TArg, TOut> factoryMethod) =>
            new AnonymousFactory<TArg, TOut>(factoryMethod);

        public static IFactory<TArg1, TArg2, TOut> Create<TArg1, TArg2, TOut>(Func<TArg1, TArg2, TOut> factoryMethod) =>
            new AnonymousFactory<TArg1, TArg2, TOut>(factoryMethod);

        private class AnonymousFactory<TOut> : IFactory<TOut>
        {
            private readonly Func<TOut> _factoryMethod;

            public AnonymousFactory(Func<TOut> factoryMethod)
            {
                _factoryMethod = factoryMethod;
            }

            public TOut Create() => _factoryMethod.Invoke();
        }

        private class AnonymousFactory<TArg, TOut> : IFactory<TArg, TOut>
        {
            private readonly Func<TArg, TOut> _factoryMethod;

            public AnonymousFactory(Func<TArg, TOut> factoryMethod)
            {
                _factoryMethod = factoryMethod;
            }

            public TOut Create(TArg arg) => _factoryMethod.Invoke(arg);
        }

        private class AnonymousFactory<TArg1, TArg2, TOut> : IFactory<TArg1, TArg2, TOut>
        {
            private readonly Func<TArg1, TArg2, TOut> _factoryMethod;

            public AnonymousFactory(Func<TArg1, TArg2, TOut> factoryMethod)
            {
                _factoryMethod = factoryMethod;
            }

            public TOut Create(TArg1 arg1, TArg2 arg2) => _factoryMethod.Invoke(arg1, arg2);
        }
    }
}