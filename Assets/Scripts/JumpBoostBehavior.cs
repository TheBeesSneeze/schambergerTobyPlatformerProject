using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;
    public GameController GC;

    public Renderer SpriteRenderer;

    public Vector2 BoostForce = new Vector2(500,500);

    public AudioSource BoostSound;

    public bool BoostReady = true;

    public float BoostResetTime = 3;

    public float TimeOfCollision;

    public Vector4 OnColor = new Vector4 (0, 250, 0, 1);
    public Vector4 OffColor = new Vector4 (250, 0, 0, 1);

    private bool Inside = false;

    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameController>();
        SpriteRenderer = gameObject.GetComponent<Renderer>();

        GameObject LittleGuyGameObject=GameObject.FindGameObjectWithTag("Player");
        LittleGuy = LittleGuyGameObject.GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - TimeOfCollision >= BoostResetTime && !BoostReady)
        {
            ToggleBoost(true);
        }
        //if player jumps while inside boost area, it boosts (wow)
        if(Inside)
        {
            if(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.E) ||LittleGuy.GroundPounding)
            {
                Boost();
            }
        }

    }

    //When player interracts with [this thing], the player is launched up a little, their double jump is refilled, [this thing] is destroyed
    private void OnTriggerEnter2D(Collider2D collider)
    {//would it be more efficient to move this into the player?
                //no
        if(collider.gameObject.tag=="Player")
        {
            Inside = true;
            LittleGuy.LittleGuyAnimator.SetBool("Jumping",true);
            LittleGuy.LittleGuyAnimator.SetBool("Dashing",false);
            LittleGuy.LittleGuyAnimator.SetBool("Running",false);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag=="Player")
        {
            Inside = false;
            LittleGuy.LittleGuyAnimator.SetBool("Jumping",false);
            LittleGuy.LittleGuyAnimator.SetBool("Dashing",true);
        }
    }

    private Vector2 MirrorMultiplier = new Vector2(-1,1);
    //actually jump
    public void Boost()
    {
        Debug.Log("Jump Boost");

        LittleGuy.Dashing=false;
        LittleGuy.Jumping=false;
        LittleGuy.CanDoubleJump = false;
        LittleGuy.GroundPounding=false;
        LittleGuy.JumpBoosting=true;

        ToggleBoost(false);
        if(LittleGuy.FacingRight)
        {
            LittleGuy.PlayerRB.velocity=BoostForce;
        }
        else
        {
            LittleGuy.PlayerRB.velocity=BoostForce*MirrorMultiplier;
        }
        Invoke("EndBoost",0.5f);
        LittleGuy.LittleGuyAnimator.SetBool("Jumping",true);
        LittleGuy.LittleGuyAnimator.SetBool("Dashing",true);
        if(GC.SoundPlaying)
        {
            BoostSound.Play();
        }
        
    }

    //turns on/off the boost
    public void ToggleBoost(bool TurnOn)
    {
        GetComponent<Collider2D>().enabled=TurnOn;

        if(TurnOn)
        {
            BoostReady=true;
        }
        else
        {
            TimeOfCollision = Time.time;
            BoostReady=false;

        }
    }

    //Stops the player mid air!
    public void EndBoost()
    {
        ToggleBoost(true);
        LittleGuy.Jumping=true;
        LittleGuy.CanDoubleJump = true;
        LittleGuy.JumpBoosting=false;
        
    }
}
