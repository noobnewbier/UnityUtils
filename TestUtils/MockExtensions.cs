using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;

namespace UnityUtils.TestUtils
{
    public static class MockExtensions
    {
        // https://stackoverflow.com/a/19711560
        public static void SetUpCallInOrder<T>(this Mock<T> mock, params Expression<Action<T>>[] expressions) where T : class
        {
            // All closures have the same instance of sharedCallCount
            var sharedCallCount = 0;
            for (var i = 0; i < expressions.Length; i++)
            {
                // make sure it's called
                var expression = expressions[i];
                // mock.Verify(expression);

                // make sure it's called in order
                var expectedCallCount = i;
                mock.Setup(expression).Callback(
                    () =>
                    {
                        Assert.AreEqual(expectedCallCount, sharedCallCount);
                        sharedCallCount++;
                    }
                ).Verifiable(Times.Once);
            }
        }
    }
}