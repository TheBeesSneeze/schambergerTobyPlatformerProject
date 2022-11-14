using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLightBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public SpriteRenderer ThisSprite;
    
    public Sprite AllImg; //Jumping
    public Sprite RedImg; //Still
    public Sprite YelImg; //Walking Slow
    public Sprite GrnImg; //Walking Fast
    public Sprite OffImg; //Stunned

    private float AvgPlayerSpeed;

    private float UpdateDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject LittleGuyGameObject=GameObject.FindGameObjectWithTag("Player");
        LittleGuy = LittleGuyGameObject.GetComponent<PlayerBehavior>();

        AvgPlayerSpeed = (LittleGuy.WalkSpeed + LittleGuy.RunSpeed)/2;

        float RandomOffset = Random.Range(0, 10.0f) / 100;
        InvokeRepeating("UpdateColor",0,UpdateDelay + RandomOffset);

        ThisSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public void UpdateColor()
    {
        //check for color changes
        if(LittleGuy.Jumping || LittleGuy.Dashing || LittleGuy.GroundPounding) ThisSprite.sprite = AllImg;
        else if(LittleGuy.Speed > AvgPlayerSpeed) ThisSprite.sprite = GrnImg;
        else if (LittleGuy.Speed > LittleGuy.WalkSpeed) ThisSprite.sprite = YelImg;
        else if (LittleGuy.Stunned) ThisSprite.sprite = OffImg;
        else ThisSprite.sprite = RedImg;
    }
}
