using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//lets the player press the minesweeper button lol

public class MinesweeperButton : MonoBehaviour
{
    public SpriteRenderer ThisSprite;
    
    public Sprite Pressing;
    public Sprite Sunglasses;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ThisSprite.sprite=Pressing;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ThisSprite.sprite=Sunglasses;
        }
    }
}
