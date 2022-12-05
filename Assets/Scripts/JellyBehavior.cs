using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyBehavior : MonoBehaviour
{
    public float TargetVelocity = 2;
    public float TimeToSchloooop = 3; //im fucking delusional rn
    public float OldVelocity;
    public float TimeOfSchloooop;
    public bool InThere = false;

    public float VelocityReducer = 2; //divides velocity on impact

    public float VelY;
    private float Modifier;    

    public PlayerBehavior LittleGuy;

    // Update is called once per frame
    void Start()
    {
        
    }

    // Start is called before the first frame update
    void Update()
    {
        //push player up
        if(!LittleGuy.GroundPounding)
        {
            if(InThere)
            {
                if(LittleGuy.PlayerRB.velocity.y < TargetVelocity)
                {
                    VelY = Mathf.Lerp(OldVelocity,TargetVelocity + Modifier,(Time.time-TimeOfSchloooop)/TimeToSchloooop);
                }
                LittleGuy.PlayerRB.velocity = new Vector2(LittleGuy.PlayerRB.velocity.x*0.95f,VelY);
            } 
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Modifier = 0.2f;
            }
            if(Input.GetKeyUp(KeyCode.Space))
            {
                Modifier = 0;
            }
        }
    }

    public void PushPlayerUp()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            InThere = true;

            if(LittleGuy.PlayerRB.velocity.y < -3)
            {
                //LittleGuy.PlayerRB.velocity= new Vector2(LittleGuy.PlayerRB.velocity.x,LittleGuy.PlayerRB.velocity.y/VelocityReducer);
            }

            LittleGuy.PlayerRB.velocity /= VelocityReducer;

            OldVelocity = LittleGuy.PlayerRB.velocity.y;
            TimeOfSchloooop = Time.time;
            LittleGuy.InsideJelly = true;


            LittleGuy.Dashing=false;
            LittleGuy.CanJump=false;
            LittleGuy.CanDoubleJump=false;
            LittleGuy.CanGroundPound=false;
            LittleGuy.LittleGuyAnimator.SetBool("Jumping",true);
            LittleGuy.LittleGuyAnimator.SetBool("Dashing",false);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            InThere = false;
            LittleGuy.InsideJelly = false;
            LittleGuy.CanDoubleJump=true;
            LittleGuy.CanJump=false;
            LittleGuy.CanGroundPound=true;
            

            if(Input.GetKey(KeyCode.Space) && VelY > 1)
            {
                LittleGuy.Jump();
            }
        }
    }
}
