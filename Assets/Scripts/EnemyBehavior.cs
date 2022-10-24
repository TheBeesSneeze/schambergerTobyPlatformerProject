using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO DO

//Launch player back
//

public class EnemyBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;
    public Collider2D EnemyCollider;
    public SpriteRenderer SpriteRender;
    public Vector4 BaseColor = new Vector4(0,0,0,1);

    public Vector2 MoveBy = new Vector2(0,0);
    private Vector2 StartPosition;
    public float MoveSpeed = 1;

    private float ypos;
    private float xpos;

    public Vector2 BoostForce = new Vector2(1000,1000);
    public Vector2 KnockBack = new Vector2(700,700);

    public bool Dead = false;

    // Start is called before the first frame update
    void Start()
    {
        StartPosition = gameObject.transform.position;

        xpos = StartPosition.x;
        ypos = StartPosition.y;

        EnemyCollider = GetComponent<Collider2D>();
        SpriteRender = gameObject.GetComponent<SpriteRenderer>();
        BaseColor=SpriteRender.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
    }

    //Enemy kinda bobs up and down
    //If frames get too bad you should make it so enemy only bob when on screen
    public void MoveEnemy()
    {
        //Booleans so less math
        if(MoveBy.x!=0)
        {
            xpos = StartPosition.x + Mathf.PingPong(Time.time/MoveSpeed,MoveBy.x);
        }
        if(MoveBy.y!=0)
        {
            ypos = StartPosition.y + Mathf.PingPong(Time.time/MoveSpeed,MoveBy.y);
        }
        gameObject.transform.position = new Vector2(xpos,ypos);
    }

    //This function is literally stolen from JumpBoostBehavior. I dont know how to simply wait oh my god use a prefab
    private Vector2 MirrorMultiplier = new Vector2(-1,1);
    //actually jump
    public void Boost()
    {
        Debug.Log("Jump Boost");

        LittleGuy.Dashing=false;
        LittleGuy.Jumping=true;
        LittleGuy.CanDoubleJump = true;

        if(LittleGuy.FacingRight)
        {
            LittleGuy.PlayerRB.velocity=BoostForce;
        }
        else
        {
            LittleGuy.PlayerRB.velocity=BoostForce*MirrorMultiplier;
        }
        
    }

    //kill enemy
    public void Die()
    {
        Dead = true;
        //SpriteRender.enabled=false;
        SpriteRender.material.color = new Vector4 (0,0,0,0);//Color.clear;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player" && !Dead)
        {
            float y = collision.gameObject.transform.position.y;

            //Player is above the enemy (kill the enemy and jump the player)
            if(y>=ypos+1)
            {
                Boost();
                Die();
            }

            //Player is below the enemy (stun the player)
            else
            {
                LittleGuy.Stunned=true;
                LittleGuy.TimeStunned=Time.time;

                float SpeedMultiplier = LittleGuy.Speed*(LittleGuy.WalkSpeed/2)/LittleGuy.RunSpeed;

                if(LittleGuy.FacingRight)
                {
                    LittleGuy.PlayerRB.AddForce(KnockBack*new Vector2 (-SpeedMultiplier,1));
                }
                else
                {
                    LittleGuy.PlayerRB.AddForce(KnockBack*new Vector2 (SpeedMultiplier,1));
                }

                LittleGuy.Speed = LittleGuy.WalkSpeed;
            }
        }
    }

    void OnBecameInvisible()
    {
        Debug.Log("n");
        Dead = false;
        SpriteRender.material.color=BaseColor;
    }
}
