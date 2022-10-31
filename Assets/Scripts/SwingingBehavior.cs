using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PlayerBehavior is getting WAY too cluttered. i just wanna make another script

public class SwingingBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public float SwingSpeed;
    public float ExpandSpeed=2;
    public float MaxSpeed = 50;
    public float MinSpeed = 35;
    
    public float MaxDistance = 12;
    public Vector3 Direction = Vector3.forward;

    public bool Swinging = false;
    public Vector2 RelativePosition; //Relative Position is just the distance between the player x and the swing x

    private GameObject Swing;
    public Vector2 SwingLocation; // its more of a slider along the x axis, determines speed
    public float DisanceFromSwing;
 
     void Update()
     {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(transform.position.x<=SwingLocation.x)
            {
                Direction = Vector3.forward;
            }
            else
            {
                Direction = Vector3.back;
            }
            Swing = FindClosestSwing();
            SwingLocation = Swing.transform.position;
            DisanceFromSwing = Vector2.Distance(SwingLocation, gameObject.transform.position);

            //Start swinging
            if(DisanceFromSwing <= MaxDistance)
            {
                Swinging = true;
                Physics2D.autoSimulation = false;

                LittleGuy.CanJump=false;
                LittleGuy.CanDoubleJump=false;
                LittleGuy.Dashing=false;
                LittleGuy.GroundPounding=false;
                LittleGuy.CanGroundPound=true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && Swinging)
        {
            Physics2D.autoSimulation = true;
            SwingJump();
            
            Swinging = false;
        }

        if(Swinging)
        {
            //Speed gets slower if you go up, goes faster if youre in the bottom middle bit

            // relative position is number -1 => 1
            // 1 means far to left or right, 0 is the center
            RelativePosition = GetRelativePosition();
            SwingSpeed = ((MaxSpeed-MinSpeed)*(1-Mathf.Abs(RelativePosition.x))) + MinSpeed; // binary stretch!

            transform.RotateAround(SwingLocation, Direction, SwingSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0,0,0);

            //reverse direction
            if(transform.position.y+(MaxDistance/DisanceFromSwing)>=SwingLocation.y)
            {
                //if on left
                if(SwingLocation.x-gameObject.transform.position.x>=0)
                {
                    Direction = Vector3.forward;
                }
                else
                {
                    Direction = Vector3.back;
                }
            }

            //move away from swing
            if(DisanceFromSwing<MaxDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, SwingLocation, -1 * ExpandSpeed * Time.deltaTime);
                DisanceFromSwing = Vector2.Distance(SwingLocation, gameObject.transform.position);
            }
            
        }

        
    }

    public void SwingJump()
    {
        //Output is the x and y values of relative positions swapped
        Vector2 Output = new Vector2(-RelativePosition.y,-RelativePosition.x);
        Output *= Direction.z;
        //Output /= MinSpeed/SwingSpeed;

        Output *= 25 * SwingSpeed;
        Output.y = Mathf.Abs(Output.y);
        LittleGuy.PlayerRB.AddForce(Output);

        Debug.Log(RelativePosition);
        Debug.Log(Output);

        LittleGuy.CanDoubleJump=true;
        LittleGuy.Dashing=false;
    }

    //Returns the closest swing, does not take into account max position
    public GameObject FindClosestSwing()
    {
        GameObject[] Swings;
        Swings = GameObject.FindGameObjectsWithTag("Swing");

        GameObject Closest = null;

        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject Swing in Swings)
        {
            Vector3 diff = Swing.transform.position - position;
            float CurrentDistance = diff.sqrMagnitude;

            if (CurrentDistance < distance)
            {
                Closest = Swing;
                distance = CurrentDistance;
            }
        }
        return Closest;
    }

    public Vector2 GetRelativePosition()
    {
        float x = gameObject.transform.position.x-SwingLocation.x; 
        x = (x / DisanceFromSwing); 

        float y = gameObject.transform.position.y- SwingLocation.y; 
        y = (y / DisanceFromSwing); 

        return new Vector2 (x,y);
    }
}
