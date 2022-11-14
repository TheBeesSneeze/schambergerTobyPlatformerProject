using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public ArmBehavior Arm;
    public Rigidbody2D PlayerRB;
    public SpriteRenderer SpriteRender;
    public SwingingBehavior SwingBehavior;

    public Animator LittleGuyAnimator;

    // Variables for as far as the eye can see...

    public bool DebugMode=false;
    
    public float Speed = 0;
    public float WalkSpeed = 8;
    public float RunSpeed = 20;
    public float StunDuration = 0.75f;

    public Vector2 EdgeGrabArea = new Vector2(2,2);

    public float TimeStartedMoving;
    public float TimeStartedJumping;
    public float TimeStoppedJumping;
    public Vector3 BaseScale;

    public Vector2 JumpForce = new Vector2(0,100);
    public float DashVelocityX = 1300;
    public float DashVelocityY = 150;
    public Vector2 AddedJumpForce = new Vector2(0,1.5f);//the amount the player increased each frame bc of holding space

    //Because I only learned about states after
    public bool CanJump = true;
    public bool CanDoubleJump = true;
    public bool CanGroundPound = true;
    public bool Jumping = false;
    public bool Dashing = false;
    public bool GroundPounding = false;
    public bool JumpBoosting = false;
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
    public bool InsideJelly = false;
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

        layer_mask = LayerMask.GetMask("Platform","Floor");
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
        if(Input.GetKeyDown(KeyCode.Space) && !Stunned && !SwingBehavior.Swinging && !InsideJumpBoost && !JumpBoosting)
        {
            Jumps();
        }
        if(Jumping)
        AnimateJump();

        //Speed up descent
        AdjustJump();

        //LedgeGrab();

        Animate();
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
        if(((Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && FacingRight) || (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !FacingRight)) && (CanDoubleJump || InsideJelly) ) //turn left
        {
            FacingRight=!FacingRight;
            BaseScale[0] *= -1;
            PreviousScale = BaseScale;
            gameObject.transform.localScale = BaseScale;
            LittleGuyAnimator.SetBool("Dashing",false);
        }
    }
    
    public void Animate()
    {
        LittleGuyAnimator.SetBool("Jumping",Jumping);
        LittleGuyAnimator.SetBool("Ground Pounding",GroundPounding);
    }

    Vector3 PreviousScale = new Vector3(1,1,1);
    //Manages Jumps and Double Jumps. is triggered when user presses space
    public void Jumps()
    {
        GroundPounding = false;

        //Regular Jump
        if(CanJump && !OnSpring && !InsideJelly)
        {
            Jump();
        }
        //Double Jump
        else if (CanDoubleJump && !InsideJumpBoost)
        {
            if (DebugMode) Debug.Log("Dash");

            Jumping = false;
            Dashing = true;
            CanDoubleJump = false;
            CanGroundPound = true;
            GroundPounding=false;
            
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

            Invoke("CancelDash",0.5f);
            LittleGuyAnimator.SetBool("Dashing",true);
        }

        CancelDash();
        
    }

    public void Jump()
    {
        if(DebugMode) Debug.Log("Jump");
    
        Jumping = true;
        CanJump = false;
        CanGroundPound = true;

        PlayerRB.velocity = new Vector2(PlayerRB.velocity.x,0);
        PlayerRB.AddForce(JumpForce);

        //animation stuff
        TimeStartedJumping = Time.time;
        PreviousScale = gameObject.transform.localScale;
        LittleGuyAnimator.SetBool("Running",false);
    }

    public void CancelDash()
    {
        if(Dashing)
        {
            Dashing=false;
            CanDoubleJump=false;
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
        if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.S)) && CanGroundPound)
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
        if(!InsideJumpBoost && !InsideJelly)
        {
            if(Jumping)
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
            if(Dashing&&!InsideJumpBoost)
            {
                //Dash gravity gets progressively heavier 
                PlayerRB.gravityScale=Mathf.Lerp(DashGravityScale,BaseGravityScale,(Time.time-TimeStartedJumping)/DashGravityAdjustSpeed);
            }
        }
        else
        {
            PlayerRB.gravityScale = 2;
        }
    }

    public int layer_mask;

    public void LedgeGrab()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(true)
            {

                //Physics2D.BoxCast(transform.position,EdgeGrabArea,0,new Vector2(1,1),1)
                RaycastHit2D Ray = Physics2D.Raycast(transform.position, new Vector2(1,1),2,layer_mask);
                if (Ray.collider!=null)
                {
                    //Output the name of the Collider your Box hit
                    Debug.Log("Hit : " + Ray.collider.name);
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
                LittleGuyAnimator.SetBool("Running",true);
            }
            //user is not pressing A or D, decrease speed
            else
            {
                Speed = WalkSpeed;
                LittleGuyAnimator.SetBool("Running",false);
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
        JumpBoosting = false;

        //animation stuff
        PreviousScale = gameObject.transform.localScale;
        TimeStoppedJumping = Time.time;
        //gameObject.transform.localScale = BaseScale;
        
        Arm.FirstRodeo = true;
        LittleGuyAnimator.SetBool("Dashing",false);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Floor" || collision.gameObject.tag=="Moving Platform" || collision.gameObject.tag=="Platform")
        {
            ResetCharacter();
        }
        if(collision.gameObject.tag == "Platform")
        {
            Dashing=false;
        }
    }
    
    private bool CouldDoubleJump=false;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Booster")
        {
            InsideJumpBoost=true;
            CouldDoubleJump=CanDoubleJump;
            CanDoubleJump=false;
            PlayerRB.gravityScale = 2;
            PlayerRB.velocity = new Vector2(PlayerRB.velocity.x,PlayerRB.velocity.y/4);
        }
    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Booster")
        {
            InsideJumpBoost=false;

            if(CouldDoubleJump)CanDoubleJump=true;
            PlayerRB.gravityScale = BaseGravityScale;
        }
    }
}

    

    
