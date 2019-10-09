using UnityEngine;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Utils
{
    [RequireComponent(typeof(Collider))]
    public class CollisionBridge : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _delegate;

        //todo: include other events
        private void OnCollisionEnter(Collision collision)
        {
            if (_delegate == null)
            {
                return;
            }

            if (_delegate is ICollisionEnterDelegate collisionEnterDelegate)
            {
                collisionEnterDelegate.OnCollisionEnterCalled(collision);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_delegate == null)
            {
                return;
            }

            if (_delegate is ITriggerExitDelegate triggerExitDelegate)
            {
                triggerExitDelegate.OnTriggerExitCalled(other);
            }
        }
    }

    public interface ICollisionEnterDelegate
    {
        void OnCollisionEnterCalled(Collision collision);
    }

    public interface ITriggerExitDelegate
    {
        void OnTriggerExitCalled(Collider other);
    }
}