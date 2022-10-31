using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public Renderer SpriteRenderer;

    public Vector2 BoostForce = new Vector2(500,500);

    public bool BoostReady = true;

    public float BoostResetTime = 3;

    public float TimeOfCollision;

    public Vector4 OnColor = new Vector4 (0, 250, 0, 1);
    public Vector4 OffColor = new Vector4 (250, 0, 0, 1);

    private bool Inside = false;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = gameObject.GetComponent<Renderer>();

        GameObject LittleGuyGameObject=GameObject.FindGameObjectWithTag("Player");
        LittleGuy = LittleGuyGameObject.GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - TimeOfCollision >= BoostResetTime && !BoostReady)
        {
            ToggleBoost();
        }
        //if player jumps while inside boost area, it boosts (wow)
        if(Inside)
        {
            if(Input.GetKey(KeyCode.Space)||Input.GetKeyDown(KeyCode.Space))
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
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.gameObject.tag=="Player")
        {
            Inside = false;
        }
    }

    private Vector2 MirrorMultiplier = new Vector2(-1,1);
    //actually jump
    public void Boost()
    {
        Debug.Log("Jump Boost");

        LittleGuy.Dashing=false;
        LittleGuy.Jumping=true;
        LittleGuy.CanDoubleJump = true;

        ToggleBoost();
        if(LittleGuy.FacingRight)
        {
            LittleGuy.PlayerRB.velocity*=BoostForce;
        }
        else
        {
            LittleGuy.PlayerRB.velocity=BoostForce*MirrorMultiplier;
        }
        
    }

    //turns on/off the boost
    public void ToggleBoost()
    {
        BoostReady = !BoostReady;
        GetComponent<Collider2D>().enabled=BoostReady;

        if(BoostReady)
        {
            SpriteRenderer.material.SetColor("_Color", Color.green);
        }
        else
        {
            TimeOfCollision = Time.time;
            SpriteRenderer.material.SetColor("_Color", Color.grey);

        }
    }
}
