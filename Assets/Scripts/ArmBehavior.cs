using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public float ArmBounceDistance = 0.75f;
    public float ArmBounceAscentSpeed = 0.0035f;
    public float ArmRecenterSpeed = 2;

    Vector2 BaseLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        BaseLocalPosition = gameObject.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateArm();
    }

    public bool FirstRodeo = true;
    public Vector2 PreviousPosition;
    float TimeStartedDescending;
    //arm kinda bounces up and down 
    public void AnimateArm()
    {
        //Move arm down when jumping
        if(LittleGuy.Jumping)
        {
            //move arm down when going up
            if(LittleGuy.PlayerRB.velocity.y > 0)
            {
                gameObject.transform.localPosition = Vector2.Lerp(BaseLocalPosition, new Vector2(BaseLocalPosition.x,-ArmBounceDistance), Time.time-LittleGuy.TimeStartedJumping);
            }
            //move arm up when falling
            else
            {
                float tempAscentSpeed = ArmBounceAscentSpeed * LittleGuy.PlayerRB.velocity.y / -10;
                float yPos = gameObject.transform.localPosition.y + ArmBounceAscentSpeed;
                yPos = Mathf.Clamp(yPos, -ArmBounceDistance,ArmBounceDistance);
                gameObject.transform.localPosition = new Vector2(BaseLocalPosition.x,yPos);

                //gameObject.transform.localPosition = Vector2.Lerp(gameObject.transform.localPosition, Vector2.zero, Time.time-TimeStartedDescending);
            }
        }
        //recenter arm after jumping
        else
        {
            //THIS MIGHT NEED OPTIMIZING!
            gameObject.transform.localPosition = Vector2.Lerp(gameObject.transform.localPosition, BaseLocalPosition, (Time.time-LittleGuy.TimeStoppedJumping)/ArmRecenterSpeed);
        }
    }
}
