using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO DO

//Draw line or line renderer


public class MovingPlatformBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuyBehavior;
    public GameObject LittleGuy;

    private float Speed;
    public float BaseSpeed = 0.001f;
    public float MaxSpeed = 2;
    public float Acceleration = 1;
    public float ReturnSpeed=1;

    public float WaitTime=0.15f;
    public float BoostDelay = 0.05f;

    private Vector3 StartingPoint;
    public Vector3 EndingPoint = new Vector3(0,0,0);
    public Vector2 EndingBoostForce = new Vector2(100,100);

    private float TimeOfChange=0;
    public bool MovingForwards = false;
    public bool MovingBackwards = false;

    public bool TouchingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        StartingPoint=gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Putting things into motion
        if(!MovingBackwards && !MovingForwards && TouchingPlayer && Time.time - TimeOfChange >= WaitTime)
        {
            Debug.Log("Going up");
            MovingForwards=true;
            TimeOfChange=Time.time;
        }

        if(MovingForwards)
        {
            //Speed gets faster
            Speed = Mathf.Lerp(BaseSpeed,MaxSpeed,(Time.time-TimeOfChange)/Acceleration);
            gameObject.transform.position = Vector2.Lerp(StartingPoint,EndingPoint,Speed);

            //Stop the platform to go backwards
            if(gameObject.transform.position == EndingPoint)
            {
                MovingForwards = false;
                MovingBackwards = true;
                TimeOfChange=Time.time;

                //Jump!
                if(TouchingPlayer)
                {
                    LittleGuyBehavior.CanJump=false;
                    LittleGuyBehavior.PlayerRB.AddForce(EndingBoostForce);
                }
                LittleGuy.gameObject.transform.parent = null;

                Debug.Log("Going Down");
            }
        }
        //Move downwards after WaitTime
        else if(MovingBackwards && Time.time-TimeOfChange >= WaitTime)
        {
            
            Speed = Mathf.Lerp(BaseSpeed/2,MaxSpeed/2,(Time.time-TimeOfChange)/ReturnSpeed);
            gameObject.transform.position = Vector2.Lerp(gameObject.transform.position,StartingPoint,Speed);
            
            Vector2 pos = gameObject.transform.position*10;

            //Come to a stop
            if(Mathf.Round(pos.x) == Mathf.Round(StartingPoint.x*10) && Mathf.Round(pos.y) == Mathf.Round(StartingPoint.y*10))
            {
                MovingBackwards=false;
                TimeOfChange=Time.time;
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TimeOfChange=Time.time;
        LittleGuy.gameObject.transform.SetParent(gameObject.transform,true);

        if(collision.gameObject.tag=="Player")
        {
            TouchingPlayer = true;
            //LittleGuy.transform.SetParent(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LittleGuy.gameObject.transform.parent = null;

        if(collision.gameObject.tag=="Player")
        {
            TouchingPlayer = false;
            //LittleGuy.transform.SetParent(gameObject,false);
        }
    }
}
