using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// Helper class for collecting trigger collisions
// This is needed because OnTriggerExit2D is not reliable
// So, we have to poll for trigger collision status each frame
// Note: This is not reliable either. Don't use unless you don't mind skipping the odd trigger on per-frame basis
// (It's okay to use for a one-shot response to being in a trigger)
class TriggerTest : MonoBehaviour
{
    private List<Collider2D> triggerColliders = new List<Collider2D>();

    public T GetTriggerCollider<T>() where T : Collider2D
    {
        var colliders = this.triggerColliders.OfType<T>();
        if (colliders.Count() > 0)
        {
            return colliders.First();
        }
        return null;
    }

    public bool IsInTrigger()
    {
        return this.triggerColliders.Count() > 0;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!this.triggerColliders.Contains(collider))
        {
            this.triggerColliders.Add(collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!this.triggerColliders.Contains(collider))
        {
            this.triggerColliders.Add(collider);
        }
    }

    private void LateUpdate()
    {
        // Clear the triggers at the end of every frame
        // They will get picked up again in a OnTriggerStay2D call
        this.triggerColliders.Clear();
    }
}

