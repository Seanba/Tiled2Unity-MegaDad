using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// A simple class that goes through our tagged appearing blocks and controls when they're active
// Takes for granted we have 3 such groups of blocks, which is fine for a demo.
class AppearingBlockManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(BlockRoutine());
    }

    private IEnumerator BlockRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        const float TimeToWait = 1.05f;
        while (true)
        {
            // Group 1 appears
            MakeBlocksAppear(GetBlockControllers(1));
            yield return new WaitForSeconds(TimeToWait);

            // Group 2 appears
            MakeBlocksAppear(GetBlockControllers(2));
            yield return new WaitForSeconds(TimeToWait);

            // Group 1 dissappears, Group 3 appears
            MakeBlocksDisappear(GetBlockControllers(1));
            MakeBlocksAppear(GetBlockControllers(3));
            yield return new WaitForSeconds(TimeToWait);

            // Group 2 disappears
            MakeBlocksDisappear(GetBlockControllers(2));
            yield return new WaitForSeconds(TimeToWait);

            // Group 3 dissappears
            MakeBlocksDisappear(GetBlockControllers(3));
        }
    }

    IList<AppearingBlockController> GetBlockControllers(int groupNumber)
    {
        List<AppearingBlockController> controllers = new List<AppearingBlockController>();

        string tag = String.Format("AppearingBlocks{0}", groupNumber);
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);

        if (gameObjects != null)
        {
            foreach (GameObject go in gameObjects)
            {
                AppearingBlockController[] comps = go.GetComponentsInChildren<AppearingBlockController>();
                controllers.AddRange(comps);
                if (comps != null)
                {
                    controllers.AddRange(comps);
                }
            }
        }

        return controllers;
    }

    private void MakeBlocksAppear(IList<AppearingBlockController> blocks)
    {
        foreach (var bc in blocks)
        {
            bc.Appear();
        }

        if (blocks.Count() > 0)
        {
            this.GetComponent<AudioSource>().Play();
        }

    }

    private void MakeBlocksDisappear(IList<AppearingBlockController> blocks)
    {
        foreach (var bc in blocks)
        {
            bc.Disappear();
        }
    }

}

