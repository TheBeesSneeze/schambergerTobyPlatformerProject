using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float Speed = 0;
    public float WalkSpeed = 8;
    public float RunSpeed = 20;

    public float TimeStartedMoving;

    public Vector2 JumpForce = new Vector2(0,100);
    public float DashVelocityX = 1300;
    public float DashVelocityY = 150;
    public Vector2 AddedJumpForce = new Vector2(0,1.5f);//the amount the player increased each frame bc of holding space
    
    Rigidbody2D PlayerRB;

    bool TouchingGround = true;
    bool CanDoubleJump = true;
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
        Speed = WalkSpeed;
        TimeStartedMoving = Time.time;

        ResetCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        //walk
        if(CanDoubleJump)
        {
            float xMove = Input.GetAxis("Horizontal");

            Vector2 NewPos = gameObject.transform.position;

            NewPos.x += xMove * Speed * Time.deltaTime;
            gameObject.transform.position = NewPos;
        }
        

        FaceDirection();
        Running();

        if(PlayerRB.velocity.y == 0)
        {
            CanDoubleJump = true;
            TouchingGround = true;
        }

        //Jump
            // When the player starts to descend, their mass will increase. If the player holds down space however, their mass will not increase and they will fall a bit faster
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jumps();
        }

        //Speed up descent
        AdjustJump();
    }

    public void FaceDirection()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            TimeStartedMoving = Time.time;
            Speed = WalkSpeed;
        }

        //Flip the sprite pretty much
        if(Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && CanDoubleJump) //turn left
        {
            FacingRight=false;
            gameObject.transform.localScale = new Vector3 (1,1,1);
        }
        else if(Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && CanDoubleJump) //turn right
        {
            FacingRight = true;
            gameObject.transform.localScale = new Vector3 (-1,1,1);
        }
    }

    //Manages Jumps and Double Jumps
    public void Jumps()
    {
        //Regular Jump
        if(TouchingGround)
        {
            PlayerRB.AddForce(JumpForce);
            TouchingGround = false;
            gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x,1.1f,1);
        }
        //Double Jump
        else if (CanDoubleJump)
        {
            CanDoubleJump = false;
            Vector2 DashVelocity = new Vector2(DashVelocityX*Speed/WalkSpeed,DashVelocityY);
            // negative X value if facing left
            if(!FacingRight)
            {
                DashVelocity = new Vector2(DashVelocity.x*-1,DashVelocityY);
            }

            //PlayerRB.AddForce(DashVelocity);
            PlayerRB.velocity=DashVelocity;
            gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x, 0.9f,1);

        }
    }

    //Essentially, it lets the player control the height / speed of their jump by holding space
    public void AdjustJump()
    {
        if(!TouchingGround)
        {
            //control their rate of descent
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

            //player can also jump higher by holding space
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
        /*
        if(collision.gameObject.tag == "Platform" && false)
        {
            ResetCharacter();
        }
        */
    }

    public void Running()
    {
        if(TouchingGround)
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
        TouchingGround = true;
        CanDoubleJump = true;
        gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x,1,1);
        
    }
}
