using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public ArmBehavior Arm;
    public Rigidbody2D PlayerRB;
    public SpriteRenderer SpriteRender;
    public SwingingBehavior SwingBehavior;

    // Variables for as far as the eye can see...

    public bool DebugMode=false;
    
    public float Speed = 0;
    public float WalkSpeed = 8;
    public float RunSpeed = 20;
    public float StunDuration = 0.75f;

    public float EdgeGrabRadius = 2;

    public float TimeStartedMoving;
    public float TimeStartedJumping;
    public float TimeStoppedJumping;
    public Vector3 BaseScale;

    public Vector2 JumpForce = new Vector2(0,100);
    public float DashVelocityX = 1300;
    public float DashVelocityY = 150;
    public Vector2 AddedJumpForce = new Vector2(0,1.5f);//the amount the player increased each frame bc of holding space

    public bool CanJump = true;
    public bool CanDoubleJump = true;
    public bool CanGroundPound = true;
    public bool Jumping = false;
    public bool Dashing = false;
    public bool GroundPounding = false;
    public bool FacingRight = true;

    public bool Stunned = false;
    public float TimeStunned;

    public float BaseMass = 2.5f;
    public float BaseGravityScale = 1.5f;
    public float DashGravityScale = 1f;

    public float DashGravityAdjustSpeed = 40; //yep those are certainly words in a variable name
    public float VerticalGroundPoundVelocityMultiplier=1.1f;
    public float HorizontalGroundPoundVelocity=0.2f;

    public float SlowAscentModifier = 0.8f;
    public float SlowDescentModifier = 0.8f;

    public float FastAscentModifier = 1.3f;
    public float FastDescentModifier = 1.4f;

    public bool InsideJumpBoost = false;
    public bool OnSpring = false;

    private float xMove=0;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRB = GetComponent<Rigidbody2D>();
        SpriteRender = gameObject.GetComponent<SpriteRenderer>();

        BaseScale = gameObject.transform.localScale;

        Speed = WalkSpeed;
        TimeStartedMoving = Time.time;

        ResetCharacter();
    }

    // Update is called once per frame
    void Update()
    {   
        Running();
        
        //xmove is calculated in BeStunned
        BeStunned();

        //Reset Speed when change directions
        if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))&&CanDoubleJump)
        {
            TimeStartedMoving = Time.time;
            Speed = WalkSpeed;
        }
        FaceDirection();

        GroundPound();

        //Jump
            // When the player starts to descend, their mass will increase. If the player holds down space however, their mass will not increase and they will fall a bit faster
        if(Input.GetKeyDown(KeyCode.Space) && !Stunned && !SwingBehavior.Swinging && !InsideJumpBoost)
        {
            Jumps();
        }

        AnimateJump();

        //Speed up descent
        AdjustJump();

        //LedgeGrab();

    }

    void FixedUpdate()
    {
        //walk
        if(!Dashing)
        {
            Vector2 NewPos = gameObject.transform.position;

            NewPos.x += xMove * Speed * Time.deltaTime;
            gameObject.transform.position = NewPos;
        }

    }

    public void FaceDirection()
    {
        //Flip the sprite pretty much
        //                                   Turn to left                                                              Turn to right
        if(((Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && FacingRight) || (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !FacingRight)) && CanDoubleJump ) //turn left
        {
            FacingRight=!FacingRight;
            BaseScale[0] *= -1;
            PreviousScale = BaseScale;
            gameObject.transform.localScale = BaseScale;
        }
    }
    
    Vector3 PreviousScale = new Vector3(1,1,1);
    //Manages Jumps and Double Jumps. is triggered when user presses space
    public void Jumps()
    {
        //Regular Jump
        if(CanJump && !OnSpring )
        {
            if(DebugMode) Debug.Log("Jump");
    
            Jumping = true;
            CanJump = false;
            GroundPounding = false;
            CanGroundPound = true;

            PlayerRB.AddForce(JumpForce);

            //animation stuff
            TimeStartedJumping = Time.time;
            PreviousScale = gameObject.transform.localScale;
        }
        //Double Jump
        else if (CanDoubleJump)
        {
            if (DebugMode) Debug.Log("Dash");

            Jumping = false;
            Dashing = true;
            CanDoubleJump = false;
            GroundPounding = false;
            CanGroundPound = true;
            
            Vector2 DashVelocity = new Vector2(DashVelocityX*Speed/WalkSpeed,DashVelocityY);
            // negative X value if facing left
            if(!FacingRight)
            {
                DashVelocity = new Vector2(DashVelocity.x*-1,DashVelocityY);
            }

            PlayerRB.velocity=DashVelocity;
            PlayerRB.gravityScale=DashGravityScale;

            //animation stuff
            TimeStartedJumping = Time.time;
            PreviousScale = gameObject.transform.localScale;
        }
    }

    public void BeStunned()
    {   
        //Normal
        if(!Stunned) 
        {
            xMove = Input.GetAxis("Horizontal");
        }

        //Stops being Stunned
        else if (TimeStunned+StunDuration<=Time.time)
        {
            Stunned = false;
            SpriteRender.enabled=true;
        }

        //Runs when stunned
        else 
        {
            xMove=0;
            //have the player flash between visibile and not
            if(Time.frameCount%120>=90)
            {
                SpriteRender.enabled=false;
            }
            else
            {
                SpriteRender.enabled=true;
            }
        }
    }

    public float JumpAnimationSpeed = 10;
    public void AnimateJump()
    {
        if(Jumping)
        {
            gameObject.transform.localScale = Vector3.Lerp(PreviousScale, new Vector3(gameObject.transform.localScale.x,1.08f,1), (Time.time-TimeStartedJumping)/JumpAnimationSpeed);
        }
        else if(Dashing)
        {
            gameObject.transform.localScale = Vector3.Lerp(PreviousScale, new Vector3(Mathf.Abs(gameObject.transform.localScale.x)/gameObject.transform.localScale.x*1.1f,0.85f,1), (Time.time-TimeStartedJumping)/JumpAnimationSpeed);
        }
        //when they on the ground
        else if(gameObject.transform.localScale != BaseScale)
        {
            gameObject.transform.localScale = Vector3.Lerp(PreviousScale, BaseScale, (Time.time-TimeStoppedJumping)*5);
        }
    }

    //Lock x movement and just go down
    //Player can almost always ground pound if they arent on the ground
    public void GroundPound()
    {
        //Start ground pound
        if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.S)) && CanGroundPound )
        {
            GroundPounding=true;
            Dashing=false;
            Jumping=false;
            SwingBehavior.Swinging=false;

            PlayerRB.velocity *= new Vector2(0,1.1f);
        }

        if(GroundPounding)
        {
            Speed = HorizontalGroundPoundVelocity;
        }
        
        
    }

    //Essentially, it lets the player control the height / speed of their jump by holding space
    public void AdjustJump()
    {
        if(Jumping && !InsideJumpBoost)
        {
            //going down
            if(PlayerRB.velocity.y<0)
            {
                //Fast descent
                if(!Input.GetKey(KeyCode.Space))
                {
                    PlayerRB.mass = BaseMass * FastDescentModifier;
                    PlayerRB.gravityScale = BaseGravityScale * FastDescentModifier;
                }
                //Slow descent
                else
                {
                    PlayerRB.mass = BaseMass * SlowDescentModifier;
                    PlayerRB.gravityScale = BaseGravityScale * SlowDescentModifier;
                }
            }

            //going up
            else
            {
                //Debug.Log(PlayerRB.position);
                //player jumps higher
                if(Input.GetKey(KeyCode.Space))
                {
                    PlayerRB.AddForce(AddedJumpForce);
                }
                //player jumps a little lower and faster
                else
                {
                    PlayerRB.mass = BaseMass*SlowAscentModifier;
                    PlayerRB.gravityScale = BaseGravityScale*SlowAscentModifier;
                }
            }
        }
        if(Dashing)
        {
            //Dash gravity gets progressively heavier 
            PlayerRB.gravityScale=Mathf.Lerp(DashGravityScale,BaseGravityScale,(Time.time-TimeStartedJumping)/DashGravityAdjustSpeed);
        }
    }

    public void LedgeGrab()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(!CanJump || Dashing)
            {
                if (DebugMode) Debug.Log("Edge Grab");
                //Get objects of nearby
                List<Collider2D> Ledges = new List<Collider2D>();
                Collider2D[] NearbyObjects = Physics2D.OverlapCircleAll(gameObject.transform.position,EdgeGrabRadius,1 << LayerMask.NameToLayer("Player"));
                
                //Weeeeeed em out 
                for (int i = 0; i < NearbyObjects.Length; i++)
                {
                    GameObject Object = NearbyObjects[i].gameObject;
                    
                    //Item cant be grabbed if:
                    //it isnt a platform
                    //it is below the player
                    if(Object.transform.position.y<gameObject.transform.position.y-0.5f)
                    {
                        //NearbyObjects.RemoveAt(i);
                        //i--;
                        continue;
                    }
                    else if( !(Object.tag == "Floor" || Object.tag == "Platform" || Object.tag == "Moving Platform"))
                    {
                        //NearbyObjects.RemoveAt(i);
                        //i--;
                        continue;
                    }
                    if (DebugMode) Debug.Log(Object.tag);
                    Ledges.Add(NearbyObjects[i]);
                }
                Debug.Log(Ledges);

                if(Ledges.Count!=0)
                {
                    if (DebugMode) Debug.Log(Ledges[0].gameObject.name);
                }
            }
        }
        
    }

    public void Running()
    {
        if(CanJump)
        {
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                TimeStartedMoving = Time.time;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                Speed=Mathf.Lerp(WalkSpeed,RunSpeed,(Time.time-TimeStartedMoving)/4);
            }
            //user is not pressing A or D, decrease speed
            else
            {
                Speed = WalkSpeed;
            }
        }
        
    }

    //resets the gravity scale and mass and all that
    public void ResetCharacter()
    {
        PlayerRB.mass = BaseMass;
        PlayerRB.gravityScale = BaseGravityScale;

        CanDoubleJump = true;
        CanJump = true;
        CanGroundPound = false;
        Jumping = false;
        Dashing = false;
        GroundPounding = false;

        //animation stuff
        PreviousScale = gameObject.transform.localScale;
        TimeStoppedJumping = Time.time;
        //gameObject.transform.localScale = BaseScale;
        
        Arm.FirstRodeo = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Floor" || collision.gameObject.tag=="Moving Platform")
        {
            ResetCharacter();
        }
        if (collision.gameObject.tag == "Booster")
        {
            InsideJumpBoost=true;
        }
        if(collision.gameObject.tag == "Platform")
        {
            Dashing=false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Booster")
        {
            InsideJumpBoost=true;
        }
    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Booster")
        {
            InsideJumpBoost=false;
        }
    }
}

    

    
