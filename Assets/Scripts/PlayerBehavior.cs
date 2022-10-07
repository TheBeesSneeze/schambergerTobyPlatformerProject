using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public ArmBehavior Arm;
    public Rigidbody2D PlayerRB;

    //Variables for as far as the eye can see...

    public float Speed = 0;
    public float WalkSpeed = 8;
    public float RunSpeed = 20;

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
    public bool Jumping = false;
    public bool Dashing = false;
    bool FacingRight = true;

    public float BaseMass = 2.5f;
    public float BaseGravityScale = 1.5f;

    public float SlowAscentModifier = 0.8f;
    public float SlowDescentModifier = 0.8f;

    public float FastAscentModifier = 1.3f;
    public float FastDescentModifier = 1.4f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRB = GetComponent<Rigidbody2D>();

        BaseScale = gameObject.transform.localScale;

        Speed = WalkSpeed;
        TimeStartedMoving = Time.time;

        ResetCharacter();
    }

    // Update is called once per frame
    void Update()
    {   
        Running();

        //Reset Speed when change directions
        if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))&&CanDoubleJump)
        {
            TimeStartedMoving = Time.time;
            Speed = WalkSpeed;
        }
        FaceDirection();

        //Jump
            // When the player starts to descend, their mass will increase. If the player holds down space however, their mass will not increase and they will fall a bit faster
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jumps();
        }
        AnimateJump();

        //Speed up descent
        AdjustJump();
    }

    void FixedUpdate()
    {
        //walk
        if(!Dashing)
        {
            float xMove = Input.GetAxis("Horizontal");

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
        if(CanJump)
        {
            Debug.Log("Jump");
            Jumping = true;
            CanJump = false;

            PlayerRB.AddForce(JumpForce);

            //animation stuff
            TimeStartedJumping = Time.time;
            PreviousScale = gameObject.transform.localScale;
        }
        //Double Jump
        else if (CanDoubleJump)
        {
            Debug.Log("Dash");

            Jumping = false;
            Dashing = true;
            CanDoubleJump = false;
            
            Vector2 DashVelocity = new Vector2(DashVelocityX*Speed/WalkSpeed,DashVelocityY);
            // negative X value if facing left
            if(!FacingRight)
            {
                DashVelocity = new Vector2(DashVelocity.x*-1,DashVelocityY);
            }

            PlayerRB.velocity=DashVelocity;

            //animation stuff
            TimeStartedJumping = Time.time;
            PreviousScale = gameObject.transform.localScale;
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
            gameObject.transform.localScale = Vector3.Lerp(PreviousScale, new Vector3(gameObject.transform.localScale.x*1.3f,0.85f,1), (Time.time-TimeStartedJumping)/JumpAnimationSpeed);
        }
        //when they on the ground
        else if(gameObject.transform.localScale != BaseScale)
        {
            gameObject.transform.localScale = Vector3.Lerp(PreviousScale, BaseScale, (Time.time-TimeStoppedJumping)*5);
        }
    }

    //Essentially, it lets the player control the height / speed of their jump by holding space
    public void AdjustJump()
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Floor")
        {
            ResetCharacter();
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
        Jumping = false;
        Dashing = false;

        //animation stuff
        PreviousScale = gameObject.transform.localScale;
        TimeStoppedJumping = Time.time;
        //gameObject.transform.localScale = BaseScale;
        
        Arm.FirstRodeo = true;
    }
}
