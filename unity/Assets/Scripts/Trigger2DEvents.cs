using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// Keep in mind that OnTriggerExit2D is not always invoked due to a bug in Unity
// We need to be able to live with that for now
class Trigger2DEvents : MonoBehaviour
{
    public delegate void OnTriggerEnter2DHandler(Collider2D collider);
    public event OnTriggerEnter2DHandler OnTriggerEnter2DEvent;

    public delegate void OnTriggerExit2DHandler(Collider2D collider);
    public event OnTriggerExit2DHandler OnTriggerExit2DEvent;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (this.OnTriggerEnter2DEvent != null)
        {
            OnTriggerEnter2DEvent(collider);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (this.OnTriggerExit2DEvent != null)
        {
            OnTriggerExit2DEvent(collider);
        }
    }


}

