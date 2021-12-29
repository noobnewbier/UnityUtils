using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityUtils.TestUtils
{
    public class WaitForSceneLoaded : CustomYieldInstruction
    {
        private readonly string _sceneName;
        private readonly float _startTime;
        private readonly float _timeout;

        public bool TimedOut { get; private set; }

        public override bool keepWaiting
        {
            get
            {
                var scene = SceneManager.GetSceneByName(_sceneName);
                var sceneLoaded = scene.IsValid() && scene.isLoaded;

                if (Time.realtimeSinceStartup - _startTime >= _timeout) TimedOut = true;

                Assert.IsFalse(TimedOut, $"TimedOut when loading {_sceneName}");
                return !sceneLoaded && !TimedOut;
            }
        }

        public WaitForSceneLoaded(string newSceneName, float newTimeout = 10)
        {
            _sceneName = newSceneName;
            _timeout = newTimeout;
            _startTime = Time.realtimeSinceStartup;
        }
    }
}