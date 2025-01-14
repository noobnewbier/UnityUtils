using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

namespace UnityUtils
{
    /// <summary>
    /// Check NonebNi's NonebEditorUpdatableSystem for details.
    /// </summary>
    public static class PlayerLoopHelpers
    {
        /// <summary>
        /// Usage:
        /// ```
        /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        /// public static void Initialize()
        /// {
        ///     PlayerLoopHelpers.AddRunnerToPlayerLoop(typeof(Update), typeof(NonebEditorUpdatableSystem), UpdateFunc);
        /// }
        /// ```
        /// </summary>
        public static void AddRunnerToPlayerLoop(Type intoLoopType, Type runnerType, PlayerLoopSystem.UpdateFunction loopUpdate)
        {
            var newPlayerLoopRoot = PlayerLoop.GetCurrentPlayerLoop();

            InsertLoop(newPlayerLoopRoot, intoLoopType, runnerType, loopUpdate);
            PlayerLoop.SetPlayerLoop(newPlayerLoopRoot);
        }

        private static int FindLoopSystemIndex(IList<PlayerLoopSystem> playerLoopList, Type systemType)
        {
            for (var i = 0; i < playerLoopList.Count; i++)
                if (playerLoopList[i].type == systemType)
                    return i;

            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }

        private static PlayerLoopSystem[] InsertRunner(
            PlayerLoopSystem loopSystem,
            Type loopRunnerType,
            PlayerLoopSystem.UpdateFunction loopRunnerDelegate)
        {
            var source = loopSystem.subSystemList;

            var dest = new PlayerLoopSystem[source.Length + 1];
            Array.Copy(source, 0, dest, 0, source.Length);

            dest[^1].type = loopRunnerType;
            dest[^1].updateDelegate = loopRunnerDelegate;

            return dest;
        }

        private static void InsertLoop(
            PlayerLoopSystem toLoopSystem,
            Type intoLoopType,
            Type newRunnerType,
            PlayerLoopSystem.UpdateFunction newRunnerDelegate)
        {
            var subSystemList = toLoopSystem.subSystemList;
            var i = FindLoopSystemIndex(subSystemList, intoLoopType);

            subSystemList[i].subSystemList = InsertRunner(
                subSystemList[i],
                newRunnerType,
                newRunnerDelegate
            );
        }
    }
}