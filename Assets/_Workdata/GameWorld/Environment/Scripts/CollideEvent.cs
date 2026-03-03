using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class CollideEvent : MonoBehaviour
{
    [Separator("Trigger Settings")]
    [SerializeField, Tag] private string tagToCompare;
    
    public UnityEvent OnCollisionDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToCompare))
        {
            OnCollisionDetected?.Invoke();
            Prefs.SetKey(Prefs.KEY_TYPES.FIRST_GAME, false);
        }
    }
}
