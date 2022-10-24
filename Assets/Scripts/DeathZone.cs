using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public Vector2 RespawnPosition = new Vector2(0,0);
    public float RespawnDelay = 0.5f;

    private float TimeOfDeath;
    private bool Dead = false;

    

    void OnCollisionEnter2D(Collision2D collision)
    {
        //KILL
        collision.gameObject.SetActive(false);

        if(collision.gameObject.tag=="Player")
        {
            //i mean, ideally, there shouldve been a wait here yknow
            LittleGuy.gameObject.SetActive(true);
            LittleGuy.gameObject.transform.position = RespawnPosition;

        }
        
    }

}
