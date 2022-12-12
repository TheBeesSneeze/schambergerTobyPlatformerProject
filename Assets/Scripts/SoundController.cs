using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public GameController GC;

    public SpriteRenderer ThisSprite;
    
    public Sprite On;
    public Sprite Off;

    public bool Pressed = false;

    public enum ButtonType
    {
        Sound,
        Music
    }
    public ButtonType ThisButton;


    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Pressed = !Pressed;
            
            //manage sprite
            if(!Pressed)
            {
                ThisSprite.sprite=On;
            }
            else
            {
                ThisSprite.sprite=Off;
            }

            //disable sounds
            if(ThisButton==ButtonType.Sound)
            {
                GC.SoundPlaying=!Pressed;
            }
            //pause / unpause music
            else
            {
                GC.MusicPlaying=!Pressed;
                if(GC.MusicPlaying)
                {
                    GC.BackgroundMusic.pitch=1;
                }
                else
                {
                    GC.BackgroundMusic.pitch=0;
                }
            }
        }
    }
}
