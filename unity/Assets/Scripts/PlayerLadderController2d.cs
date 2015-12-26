using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// Controls and animates the player while he is on a ladder
class PlayerLadderController2d : PlayerController2d
{
    public static readonly float LadderVelocityPerSecond = 1.0f * 60.0f;

    // If we encounter a ladder top between these points then we are "bending over" a ladder
    public Transform Linecast1 { get; private set; }
    public Transform Linecast2 { get; private set; }

    protected override void OnAwake()
    {
        this.Controller2d.onControllerCollidedEvent += new Action<RaycastHit2D>(controller2d_onControllerCollidedEvent);

        this.Linecast1 = this.transform.Find("LadderLinecast1");
        this.Linecast2 = this.transform.Find("LadderLinecast2");
    }

    void controller2d_onControllerCollidedEvent(RaycastHit2D obj)
    {
        // Untagged collisions are the terrain around us
        // If we collide with anything that isn't a ladder then we leave this state
        if (this.PlayerControllerEnabled && obj.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            this.Animator.SetTrigger("ClimbedOffLadder");
            this.Master.GotoPlatformState();
        }
    }

    protected override void OnControllerEnabled(bool enabled)
    {
        if (enabled)
        {
            this.Animator.SetTrigger("OnLadder");
        }
    }

    protected override void OnUpdate()
    {
        // We only animate the player while he is moving up/down the ladder
        float climbing = Input.GetAxisRaw("Vertical");
        this.Animator.speed = Mathf.Abs(climbing);

        // Are we jumping? If so, we leave this state.
        if (climbing == 0 && Input.GetButtonDown("Jump"))
        {
            this.Animator.SetTrigger("JumpedOffLadder");
            this.Master.GotoPlatformState();
            return;
        }

        // If we linecast across a ladder top than we are either climbing off of or onto a ladder (bending over a ladder)
        RaycastHit2D ladderTopRay = Physics2D.Linecast(this.Linecast1.position, this.Linecast2.position, 1 << LayerMask.NameToLayer("LadderTops"));
        this.Animator.SetFloat("LadderLinecast", ladderTopRay ? 1.0f : 0.0f);

        // If we're moving up a ladder and our "top half" no longer crosses a ladder top then we climb off the ladder
        if (ladderTopRay && climbing > 0)
        {
            if (!Physics2D.Linecast(this.Linecast1.position, this.transform.position, 1 << LayerMask.NameToLayer("LadderTops")))
            {
                this.Animator.SetTrigger("ClimbedOffLadder");

                // To "pop off" the ladder we have to abruptly move to our "foot" 
                // The bottom of our linecast is made to represent our foot position
                // And our foot needs to rest on the ladder top we're crossing.
                Vector3 position = this.transform.position;
                float feet_y = this.Linecast2.transform.position.y;
                float ladderTop_y = ladderTopRay.point.y;

                position.y = ladderTop_y + (position.y - feet_y);
                this.transform.position = position;

                // We may be just above the ladder top and we don't want to fall for a split second
                // So, move our character controller back down to the ground. He should collide with the ladder top we just popped past.
                this.Controller2d.move(new Vector3(0, -0.25f, 0));

                // Go back to the platform state
                this.Master.GotoPlatformState();
                return;
            }
        }

        // If we're moving down a ladder then it's possible to fall off it (because it comes to an end)
        if (climbing < 0)
        {
            // Check if the "top half" of our player is crossing a "LadderBottom"
            RaycastHit2D ladderBottomRay =
                Physics2D.Linecast(this.Linecast1.position, this.transform.position, 1 << LayerMask.NameToLayer("LadderBottoms"));
            if (ladderBottomRay)
            {
                this.Animator.SetTrigger("JumpedOffLadder");
                this.Master.GotoPlatformState();
            }
        }

        // Move our character up/down the ladder
        // Note: There is no graivty applied while on the ladder
        if (climbing != 0)
        {
            Vector2 velocity = new Vector2(0, Mathf.Sign(climbing) * LadderVelocityPerSecond * Time.deltaTime);
            this.Controller2d.move(velocity);
        }
    }
}

