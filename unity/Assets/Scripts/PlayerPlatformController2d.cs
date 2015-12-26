using UnityEngine;

using System.Collections;

[RequireComponent(typeof(MasterPlayerController2d), typeof(CharacterController2D))]
class PlayerPlatformController2d : PlayerController2d
{
    // Note: Physics data is normalized against 60 fps, not that we're required to maintain that framerate.
    // Gravity is 0.25 pixels per frame squared.
    public static readonly float GravityPerSecondSquared = -0.25f * 60.0f * 60.0f;
    public static readonly float GroundVelocityPerSecond = 1.375f * 60.0f;
    public static readonly float AirVelocityPerSecond = 1.3125f * 60.0f;
    public static readonly float JumpVelocityPerSecond = 4.875f * 60.0f;

    private static readonly float GravityModifierInWater = 0.375f;

    public static readonly float AbsorbGroundedInputTime = 0.0675f;

    private Vector2 velocity;
    private float gravityModifier = 1.0f;
    private float absorbGroundedInputTimer = 0.0f;

    protected override void OnAwake()
    {
        this.velocity = Vector2.zero;

        Trigger2DEvents triggerEvents = this.transform.Find("WaterTrigger").GetComponent<Trigger2DEvents>();
        triggerEvents.OnTriggerEnter2DEvent += new Trigger2DEvents.OnTriggerEnter2DHandler(Water_OnTriggerEnter2DEvent);
        triggerEvents.OnTriggerExit2DEvent += new Trigger2DEvents.OnTriggerExit2DHandler(Water_OnTriggerExit2DEvent);
    }

    void Water_OnTriggerEnter2DEvent(Collider2D collider)
    {
        // Create a splash where we enter the water
        SplashController.InstantiateSplash(this.transform.position);

        // When we hit the water, we interrupt our fall
        this.gravityModifier = GravityModifierInWater;

        // Slow us down initially as we enter water
        this.Controller2d.velocity.y *= 0.25f;
        this.Controller2d.velocity.x *= 0.25f;
    }

    void Water_OnTriggerExit2DEvent(Collider2D collider)
    {
        // Create a splash where we exit the water
        SplashController.InstantiateSplash(this.transform.position);

        // Gravity has full effect now outside water
        this.gravityModifier = 1.0f;
    }

    protected override void OnControllerEnabled(bool enabled)
    {
    }

    protected override void OnUpdate()
    {
        // Need to reset this every update due to a bug in Unity
        this.Animator.speed = 1.0f;

        this.velocity = this.Controller2d.velocity;
        if (this.Controller2d.isGrounded)
        {
            this.velocity.y = 0;
            this.Animator.SetBool("Grounded", true);
        }
        else
        {
            this.Animator.SetBool("Grounded", false);
        }

        UpdateHorizontalInput();

        if (LadderCheck())
        {
            // We're on a ladder now. We're done with this update.
            return;
        }

        if (this.Controller2d.isGrounded && Input.GetButtonDown("Jump"))
        {
            // Jumping
            this.velocity.y = JumpVelocityPerSecond;

            // Raise our MegaDad dude up a bit to start the jump off
            // Otherwise, his leg starts off a bit too deep into the ground
            this.transform.Translate(0, 4, 0);
        }
        else if (this.Controller2d.isGrounded == false && this.velocity.y > 0.0f)
        {
            // We are in the middle of a jump but we only continue jumping as the button is down
            if (Input.GetButton("Jump") == false)
            {
                this.velocity.y = 0.0f;
            }
        }
        
        // Apply gravity before moving
        this.velocity.y += GravityPerSecondSquared * Time.deltaTime * this.gravityModifier;

        // Move our character
        this.Controller2d.move(this.velocity * Time.deltaTime);
    }

    private void UpdateHorizontalInput()
    {
        this.absorbGroundedInputTimer -= Time.deltaTime;
        this.absorbGroundedInputTimer = Mathf.Max(0, this.absorbGroundedInputTimer);

        float running = Input.GetAxisRaw("Horizontal");

        if (running != 0)
        {
            // If we change facing direction then don't move
            if (this.Master.FaceDirection(running))
            {
                running = 0;
                this.absorbGroundedInputTimer = AbsorbGroundedInputTime;
            }
        }

        // Set our horizontal velocity
        float modifier = this.Controller2d.isGrounded ? GroundVelocityPerSecond : AirVelocityPerSecond;
        this.velocity.x = running * modifier;

        // If we're absorbing grounded input then velocity.x is zero
        if (this.Controller2d.isGrounded && this.absorbGroundedInputTimer > 0)
        {
            this.velocity.x = 0;
        }

        // Set the animation control
        this.Animator.SetFloat("Speed", Mathf.Abs(this.velocity.x));
    }

    private bool LadderCheck()
    {
        float climbing = Input.GetAxisRaw("Vertical");
        if (climbing != 0)
        {
            // Do we have a ladder to grab?
            EdgeCollider2D ladderGrabEdgeSpine = this.Master.LadderGrabTest.GetTriggerCollider<EdgeCollider2D>();
            EdgeCollider2D ladderDownEdgeSpine = this.Master.LadderDownTest.GetTriggerCollider<EdgeCollider2D>();

            if (climbing > 0)
            {
                if (ladderGrabEdgeSpine != null)
                {
                    this.Master.GotoLadderState_FromSpine(ladderGrabEdgeSpine);
                    return true;
                }
            }
            else if (climbing < 0)
            {
                // If there is already a ladder for us to grab onto without looking past a ladder top, than grab it
                // (In that case, we require the same ladder to be below us too)
                if (ladderGrabEdgeSpine != null && ladderDownEdgeSpine != null)
                {
                    this.Master.GotoLadderState_FromSpine(ladderGrabEdgeSpine);
                    return true;
                }

                // Try to climb down a ladder (past a ladder top edge)
                if (ladderDownEdgeSpine != null)
                {
                    this.Master.GotoLadderState_FromTop(ladderDownEdgeSpine);
                    return true;
                }
            }
        }
        return false;
    }

}
