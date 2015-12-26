using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// A cheap an easy class that animates our Tiled layers
class LayerAnimator : MonoBehaviour
{
    public float TimePerLayer = 0.5f;

    private void Start()
    {
        StartCoroutine(AnimateRoutine());
    }

    private IEnumerator AnimateRoutine()
    {
        bool isLayer1Active = true;
        var layer1Objs = GameObject.FindGameObjectsWithTag("AnimLayer1");
        var layer2Objs = GameObject.FindGameObjectsWithTag("AnimLayer2");

        while (true)
        {
            // We take for granted these objects only have visuals that 
            // are being enabled and disabled
            foreach (var obj1 in layer1Objs)
            {
                obj1.SetActive(isLayer1Active);
            }
            foreach (var obj2 in layer2Objs)
            {
                obj2.SetActive(!isLayer1Active);
            }

            isLayer1Active = !isLayer1Active;
            yield return new WaitForSeconds(TimePerLayer);
        }
    }
}

