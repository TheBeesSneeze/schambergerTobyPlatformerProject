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

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - TimeOfCollision >= BoostResetTime && !BoostReady)
        {
            ToggleBoost();
        }
    }

    //When player interracts with [this thing], the player is launched up a little, their double jump is refilled, [this thing] is destroyed
    private void OnTriggerEnter2D(Collider2D collider)
    {//would it be more efficient to move this into the player?
        
        if(collider.gameObject.tag=="Player")
        {
            Debug.Log("Jump Boost");
            LittleGuy.CanDoubleJump = true;
            LittleGuy.PlayerRB.AddForce(BoostForce);
            ToggleBoost();
        }
    }

    //turns on/off the boost
    public void ToggleBoost()
    {
        BoostReady = !BoostReady;
        GetComponent<Collider2D>().enabled=BoostReady;

        if(BoostReady)
        {
            
        }
        else
        {
            TimeOfCollision = Time.time;
        }
    }
}
