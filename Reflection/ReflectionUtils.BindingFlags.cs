using System.Collections.Generic;
using System.Reflection;

namespace UnityUtils
{
    public static partial class ReflectionUtils
    {
        public static BindingFlags GetFlags(this MemberInfo memberInfo) => MemberBindingFlagsCache.GetOrFind(memberInfo);

        public static bool Contains(this BindingFlags flags, BindingFlags bindingFlags) =>
            (flags & bindingFlags) == bindingFlags;

        public static bool MatchesExactly(this BindingFlags flags, BindingFlags bindingFlags) =>
            flags == bindingFlags;

        public static bool HasIntersects(this BindingFlags flags, BindingFlags bindingFlags) => (flags & bindingFlags) != 0;

        public static bool MatchesPartly(this BindingFlags flags, BindingFlags bindingFlags) =>
            (flags & bindingFlags) != 0;
        //origin: https://stackoverflow.com/a/56258378

        private static class MemberBindingFlagsCache
        {
            private static readonly Dictionary<MemberInfo, BindingFlags> Cache = new();

            public static BindingFlags GetOrFind(MemberInfo memberInfo)
            {
                if (!Cache.TryGetValue(memberInfo, out var bindingFlags))
                {
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    Cache[memberInfo] = bindingFlags =
                        (BindingFlags)memberInfo.GetType()
                            .GetProperty("BindingFlags", flags)! // we must have something here, otherwise the only fix is to re-write this reflection code 
                            .GetValue(memberInfo);
                }

                return bindingFlags;
            }

            public static void Clear()
            {
                Cache.Clear();
            }
        }
    }
}