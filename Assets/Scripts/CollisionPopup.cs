using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPopup : MonoBehaviour
{
    public Sprite OffSprite;
    public Sprite OnSprite;

    public SpriteRenderer ThisSprite;

    public float TurnOffTime=0;

    void Start()
    {
        ThisSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            ThisSprite.sprite=OnSprite;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            Invoke("TurnOff",TurnOffTime);
        }
    
    }

    public void TurnOff()
    {
        ThisSprite.sprite=OffSprite;
    }

    
    
}
