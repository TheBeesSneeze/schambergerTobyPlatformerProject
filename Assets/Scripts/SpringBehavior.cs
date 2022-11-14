using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO DO

//Make the player jump higher if they hold space

public class SpringBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public bool BounceUp=false;
    public bool BounceDown=false;
    public bool BounceLeft=false;
    public bool BounceRight=false;

    public Vector2 PityForce = new Vector2(0,100);

    public float JumpWindow = 0.5f;
    public float TimeOfBounce;

    public float VelocityMultiplier=1.3f;

    public float VelocityMultiplyThreshold=15;
    public float MinVelocityOutput=25;
    public float MaxVelocityOutput=35;

    // Start is called before the first frame update
    void Start()
    {
        GameObject LittleGuyGameObject=GameObject.FindGameObjectWithTag("Player");
        LittleGuy = LittleGuyGameObject.GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - JumpWindow >= TimeOfBounce)
        {
            //LittleGuy.OnSpring=false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            Debug.Log("Spring");

            Vector2 vel = collision.relativeVelocity; //LittleGuy.PlayerRB.velocity;
            Debug.Log(vel);

            //the fun part

            if(BounceUp)
            {
                
                //just in case the player is to the side of the spring
                if(LittleGuy.PlayerRB.position.y >= gameObject.transform.position.y)
                {
                    Debug.Log("Spring up");

                    TimeOfBounce = Time.time;
                    
                    LittleGuy.CanJump=false;
                    LittleGuy.OnSpring=true;
                    LittleGuy.CanGroundPound = true;
                    
                    //if(!LittleGuy.Dashing)
                    //{
                        LittleGuy.CanDoubleJump=true;
                    //}

                    //if player is going faster than BounceVelY, their new velocity will be in the middle of their old one and BounceVelocity
                    float NewVel = MinVelocityOutput;

                    //Ground pounding make em go zooom
                    if(LittleGuy.GroundPounding)
                    {
                        NewVel = MaxVelocityOutput;
                        LittleGuy.GroundPounding=false;
                    }
                    
                    //player can jump higher if theyre already going faster than min velocity
                    else if(Mathf.Abs(vel.y)>=MinVelocityOutput)
                    {   
                        NewVel = -vel.y * VelocityMultiplier;

                        if(NewVel>MaxVelocityOutput)
                        {
                            NewVel=MaxVelocityOutput;
                        }
                        else if(NewVel<MinVelocityOutput)
                        {
                            NewVel = MinVelocityOutput;
                        }
                    }
                    
                    //if they arent going fast enough, they will still jump at MinVelocityOutput

                    Debug.Log(NewVel);
                    LittleGuy.PlayerRB.AddForce(PityForce);
                    LittleGuy.PlayerRB.velocity = new Vector2(vel.x, NewVel);
                }
            }
            
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            LittleGuy.OnSpring=false;
        }
    }
}
