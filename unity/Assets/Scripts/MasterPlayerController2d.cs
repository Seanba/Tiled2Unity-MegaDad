using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[RequireComponent(typeof(PlayerPlatformController2d), typeof(PlayerLadderController2d))]
class MasterPlayerController2d : MonoBehaviour
{
    public PlayerPlatformController2d PlatformController { get; private set; }
    public PlayerLadderController2d LadderController { get; private set; }

    // We our sprite animation on a child object because transforming the localScale (for flipping) breaks triggers
    // Any colliders not centered on on the sprite should be a child of this object
    public GameObject SpriteAnimator { get; private set; }

    public TriggerTest LadderGrabTest { get; private set; }
    public TriggerTest LadderDownTest { get; private set; }


    public void GotoLadderState_FromSpine(EdgeCollider2D ladderSpine)
    {
        // Snap to the ladder
        Vector3 pos = this.transform.position;
        float ladder_x = ladderSpine.points[0].x;
        this.transform.position = new Vector3(ladder_x, pos.y, pos.z);

        SwitchFacingDirection();

        // The platform controller is not active while on a ladder, but our ladder controller is
        this.PlatformController.SetPlayerControllerEnabled(false);
        this.LadderController.SetPlayerControllerEnabled(true);
    }

    public void GotoLadderState_FromTop(EdgeCollider2D ladderSpine)
    {
        // We need to snap to the ladder plus move down past the ladder top so we don't collide with it
        // (We take for granted that the edge collider points for the spine go from down-to-up)
        Vector3 pos = this.transform.position;
        float ladder_x = ladderSpine.points[0].x;
        float ladder_y = ladderSpine.points[1].y;
        this.transform.position = new Vector3(ladder_x, ladder_y - 2.0f, pos.z);

        SwitchFacingDirection();

        // The platform controller is not active while on a ladder, but our ladder controller is
        this.PlatformController.SetPlayerControllerEnabled(false);
        this.LadderController.SetPlayerControllerEnabled(true);
    }

    public void GotoPlatformState()
    {
        // Platform controller is now enabled. Other player controllers are disabled.
        this.LadderController.SetPlayerControllerEnabled(false);
        this.PlatformController.SetPlayerControllerEnabled(true);
    }

    public bool FaceDirection(float dx)
    {
        Vector3 scale = this.SpriteAnimator.transform.localScale;
        if (Mathf.Sign(dx) != Mathf.Sign(scale.x))
        {
            this.SpriteAnimator.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            return true;
        }
        return false;
    }

    public void SwitchFacingDirection()
    {
        Vector3 scale = this.SpriteAnimator.transform.localScale;
        this.SpriteAnimator.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    private void Awake()
    {
        this.PlatformController = this.gameObject.GetComponent<PlayerPlatformController2d>();
        this.LadderController = this.gameObject.GetComponent<PlayerLadderController2d>();

        this.SpriteAnimator = this.transform.Find("Animator").gameObject;

        this.LadderGrabTest = this.transform.Find("LadderGrabTest").GetComponent<TriggerTest>();
        this.LadderDownTest = this.transform.Find("LadderDownTest").GetComponent<TriggerTest>();

        // Ladder Controller starts of disabled
        this.LadderController.SetPlayerControllerEnabled(false);
        this.PlatformController.SetPlayerControllerEnabled(true);
    }
}

