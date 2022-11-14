using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public Vector2 JumpForce = new Vector2(0,4000f);
    public bool TouchingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        LittleGuy = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        //if game gets too laggy make this an invoke repeating or something
        if(TouchingPlayer)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                LittleGuy.PlayerRB.velocity = Vector2.zero;
                LittleGuy.PlayerRB.AddForce(JumpForce);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if(collision.gameObject.tag=="Player")
        {
            LittleGuy.CanJump=false;
            TouchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            TouchingPlayer = false;
        }
    }
}
