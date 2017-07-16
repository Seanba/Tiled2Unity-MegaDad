using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class BubbleController : MonoBehaviour
{
    private static readonly float RisingSpeed = 0.5f * 60.0f;

    private static readonly float TimeToMove_dx = 0.25f;
    private float step_dx = 2.0f;
    private float timer = 0.0f;

    private bool isBubbleInWater = false;

    private void Start()
    {
        this.timer = 0;
        this.isBubbleInWater = false;
        this.GetComponent<Renderer>().enabled = false;
    }

    private void FixedUpdate()
    {
        if (this.isBubbleInWater)
        {
            FixedUpdateBubble();
        }
        else
        {
            FixedUpdatePlayer();
        }
    }

    private void FixedUpdatePlayer()
    {
        // We're checking for the player to be in water
        // Once he is, then our bubble is in water too and ready to go
        Transform playerMouth = GameObject.FindGameObjectWithTag("Player").transform.Find("Animator/Mouth");
        this.isBubbleInWater = Physics2D.OverlapPoint(playerMouth.position, 1 << LayerMask.NameToLayer("Water"));

        if (this.isBubbleInWater)
        {
            // Move to the player's position
            Vector3 pos = playerMouth.position;
            pos.x = Mathf.Round(pos.x);
            this.transform.position = pos;
            this.GetComponent<Renderer>().enabled = true;
        }
    }

    private void FixedUpdateBubble()
    {
        // Is the bubble still in water?
        this.isBubbleInWater = Physics2D.OverlapPoint(this.transform.position, 1 << LayerMask.NameToLayer("Water"));

        if (!this.isBubbleInWater)
        {
            this.GetComponent<Renderer>().enabled = false;
            this.timer = 0.0f;
        }
        else
        {
            this.timer += Time.deltaTime;

            float dx = 0;
            float dy = RisingSpeed * Time.deltaTime;

            if (this.timer >= TimeToMove_dx)
            {
                this.timer = 0;
                dx = this.step_dx;
                this.step_dx = -this.step_dx;
            }

            // Move the bubble up (and perhaps oscillate back and forth) 
            this.transform.Translate(dx, dy, 0);
        }
    }

}

