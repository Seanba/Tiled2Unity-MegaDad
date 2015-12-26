using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class SplashController : MonoBehaviour
{
    public static SplashController InstantiateSplash(Vector2 pos)
    {
        // We don't instantiate a splash unless we cross a water boundary
        // It's possible we've already (just barely) crossed the water boundary by now, so take some slop into account
        const float Slop = 16.0f;
        Vector2 start_w = pos + Vector2.up * Slop;
        Vector2 end_w = pos - Vector2.up * Slop;

        RaycastHit2D hit = Physics2D.Linecast(start_w, end_w, 1 << LayerMask.NameToLayer("Water"));
        if (hit)
        {
            // Push the splash into the water a bit
            const int Offset_y = -2;

            pos.y = hit.point.y;
            pos.y += Offset_y;

            // Create the object and return our controller
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Splash"), pos, Quaternion.identity);
            return go.GetComponent<SplashController>();
        }

        return null;
    }

    public void OnSplashComplete()
    {
        // Simply destroy us
        GameObject.Destroy(this.gameObject);
    }
}

