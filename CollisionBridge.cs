using System.Linq;
using UnityEngine;

// ReSharper disable SuspiciousTypeConversion.Global

namespace UnityUtils
{
    [RequireComponent(typeof(Collider))]
    public class CollisionBridge : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] delegates;

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var @delegate in delegates)
            {
                if (@delegate == null) return;

                if (@delegate is ICollisionEnterDelegate collisionEnterDelegate)
                    collisionEnterDelegate.OnCollisionEnterCalled(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            foreach (var @delegate in delegates)
            {
                if (@delegate == null) return;

                if (@delegate is ICollisionExitDelegate collisionStayDelegate)
                    collisionStayDelegate.OnCollisionExitCalled(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (var @delegate in delegates)
            {
                if (@delegate == null) return;

                if (@delegate is ICollisionStayDelegate collisionStayDelegate)
                    collisionStayDelegate.OnCollisionStayCalled(collision);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (var @delegate in delegates)
            {
                if (@delegate == null) return;

                if (@delegate is ITriggerExitDelegate triggerExitDelegate)
                    triggerExitDelegate.OnTriggerExitCalled(other);
            }
        }

        private void OnValidate()
        {
            if (!delegates.Any())
            {
                Debug.Log($"{gameObject.name}'s collision bridge : delegates has no element");
                return;
            }

            for (var i = 0; i < delegates.Length; i++)
            {
                if (!IsAssigned(delegates[i]))
                {
                    Debug.Log($"{gameObject.name}'s collision bridge : delegate not assigned");

                    return;
                }

                if (!IsValidDelegate(delegates[i]))
                {
                    Debug.Log(
                        $"{gameObject.name}'s collision bridge :  {delegates[i].name} does not implement the necessary interface"
                    );

                    delegates[i] = null;
                }
            }
        }

        private static bool IsValidDelegate(MonoBehaviour mono) =>
            mono is ICollisionEnterDelegate ||
            mono is ICollisionStayDelegate ||
            mono is ICollisionExitDelegate ||
            mono is ITriggerExitDelegate;

        private static bool IsAssigned(MonoBehaviour mono) => (object) mono != null;
    }

    public interface ICollisionEnterDelegate
    {
        void OnCollisionEnterCalled(Collision collision);
    }

    public interface ITriggerExitDelegate
    {
        void OnTriggerExitCalled(Collider other);
    }

    public interface ICollisionStayDelegate
    {
        void OnCollisionStayCalled(Collision collision);
    }

    public interface ICollisionExitDelegate
    {
        void OnCollisionExitCalled(Collision collision);
    }
}