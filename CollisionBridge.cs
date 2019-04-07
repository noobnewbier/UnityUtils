using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionBridge : MonoBehaviour
{
    [SerializeField] MonoBehaviour _delegate;

    //todo: include other events
    private void OnCollisionEnter(Collision collision)
    {
        var collisionEnterDelegate = _delegate as ICollisionEnterDelegate;
        if (_delegate != null)
            collisionEnterDelegate.OnCollisionEnterCalled(collision);
    }

    private void OnTriggerExit(Collider other)
    {
        var triggerExitDelegate = _delegate as ITriggerExitDelegate;
        if (_delegate != null)
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



