using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// Base class for handling player controller state
abstract class PlayerController2d : PlayerController2d_ForSealing
{
    protected MasterPlayerController2d Master { get; private set; }
    protected Animator Animator { get ; private set; }
    protected CharacterController2D Controller2d { get; private set; }
    protected bool PlayerControllerEnabled { get; private set; }

    protected override sealed void Awake()
    {
        this.PlayerControllerEnabled = false;

        this.Master = this.gameObject.GetComponent<MasterPlayerController2d>();
        this.Animator = this.transform.Find("Animator").gameObject.GetComponent<Animator>();
        this.Controller2d = this.gameObject.GetComponent<CharacterController2D>();

        OnAwake();
    }

    protected override sealed void Update()
    {
        if (this.PlayerControllerEnabled)
        {
            DebugUpdate();
            OnUpdate();
        }
    }

    public void SetPlayerControllerEnabled(bool enabled)
    {
        this.PlayerControllerEnabled = enabled;
        OnControllerEnabled(enabled);
    }

    abstract protected void OnAwake();
    abstract protected void OnUpdate();
    abstract protected void OnControllerEnabled(bool enabled);

    private void DebugUpdate()
    {
        // Allow user to cheat and move the player around with a mouse click
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Ray ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            this.transform.position = new Vector3(ray.origin.x, ray.origin.y, this.transform.position.z);
            this.Controller2d.velocity = Vector2.zero;
        }
    }
}

// C# trick to avoid function hiding
// Because Unity uses reflection to call common functions we need to make sure the right methods are called
public abstract class PlayerController2d_ForSealing : MonoBehaviour
{
    abstract protected void Awake();
    abstract protected void Update();
}


