#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define T2U_IS_UNITY_4
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class AppearingBlockController : MonoBehaviour
{
    // How far we can go up to get our of a collision
    public float stepUp = 16.0f;

    private Animator animator = null;

    public void Appear()
    {
        this.GetComponent<Collider2D>().enabled = true;
        this.GetComponent<Renderer>().enabled = true;

        this.animator.SetTrigger("Appear");
        CheckForPlayerCollision();
    }

    public void Disappear()
    {
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<Renderer>().enabled = false;
    }


    private void Awake()
    {
        this.animator = GetComponent<Animator>();
        Disappear();
    }


    private Rect GetBoxCollider2DRect(GameObject go)
    {
        BoxCollider2D box2d = go.GetComponent<BoxCollider2D>();
        Vector2 pos = box2d.transform.position;
        Rect rect = new Rect();

#if T2U_IS_UNITY_4
        rect.xMin = pos.x + box2d.center.x - box2d.size.x * 0.5f;
        rect.xMax = pos.x + box2d.center.x + box2d.size.x * 0.5f;
        rect.yMin = pos.y + box2d.center.y - box2d.size.y * 0.5f;
        rect.yMax = pos.y + box2d.center.y + box2d.size.y * 0.5f;
#else
        rect.xMin = pos.x + box2d.offset.x - box2d.size.x * 0.5f;
        rect.xMax = pos.x + box2d.offset.x + box2d.size.x * 0.5f;
        rect.yMin = pos.y + box2d.offset.y - box2d.size.y * 0.5f;
        rect.yMax = pos.y + box2d.offset.y + box2d.size.y * 0.5f;
#endif
        return rect;
    }

    private Rect MinkowskiSum(Rect rc1, Rect rc2)
    {
        Rect rc = rc1;
        rc.xMin -= rc2.width * 0.5f;
        rc.xMax += rc2.width * 0.5f;
        rc.yMin -= rc2.height * 0.5f;
        rc.yMax += rc2.height * 0.5f;
        return rc;
    }

    private void CheckForPlayerCollision()
    {
        // Does the player overlap with this appearing block?
        // If so, push the player out of the way
        // Prefer to put the player on top of the appearing block if you can
        // Because the player's box collider is disabled (he uses raycasts for collisions) then we can't rely on OnCollisionEnter type functions.

        GameObject player = (GameObject)GameObject.FindGameObjectWithTag("Player");
        Rect rcPlayer = GetBoxCollider2DRect(player);
        Rect rcBlock = GetBoxCollider2DRect(this.gameObject);

        // If there's an overlap than we must push the player out of the way
        // Prefer to push him up if we can, otherwise push left/right (never push down)
        if (rcBlock.Overlaps(rcPlayer))
        {
            // How much we move the player out of the way
            Vector2 playerDelta = Vector2.zero;

            // Reduce the player to a point
            Vector2 ptPlayer = rcPlayer.center;

            // Do a Minkowski sum on the block
            rcBlock = MinkowskiSum(rcBlock, rcPlayer);

            float up = rcBlock.yMax - ptPlayer.y;
            float right = rcBlock.xMax - ptPlayer.x;
            float left = ptPlayer.x - rcBlock.xMin;
            if (up <= this.stepUp)
            {
                // Prefer to push up
                playerDelta.y = up;
            }
            else if (left < right)
            {
                // Push left
                playerDelta.x = -left;
            }
            else
            {
                // Push right
                playerDelta.x = right;
            }

            // Move the player out of the way
            player.transform.Translate(playerDelta);
        }
    }

} // end class

