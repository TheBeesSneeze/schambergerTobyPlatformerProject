using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public SpriteRenderer Ones;
    public SpriteRenderer Tens;
    public SpriteRenderer Hundreds;
    public SpriteRenderer Thousands;

    public Sprite Zero;
    public Sprite One;
    public Sprite Two;
    public Sprite Three;
    public Sprite Four;
    public Sprite Five;
    public Sprite Six;
    public Sprite Seven;
    public Sprite Eight;
    public Sprite Nine;

    public float Delay = 0.5f;

    public int time;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTimer",3,Delay);
    }

    // Update is called once per frame
    public void UpdateTimer()
    {
        time = (int)Time.time;
        
        UpdateDisplay(Ones,time%10);
        time = (time-time%10)/10;
        UpdateDisplay(Tens,time%10);
        time = (time-time%10)/10;
        UpdateDisplay(Hundreds,time%10);
        time = (time-time%10)/10;
        UpdateDisplay(Thousands,time%10);
        
    }

    public void UpdateDisplay(SpriteRenderer Display, int n)
    {
        //because i couldnt figure out how to do sprite arrays
             if(n==0) Display.sprite = Zero;
        else if(n==1) Display.sprite = One;
        else if(n==2) Display.sprite = Two;
        else if(n==3) Display.sprite = Three;
        else if(n==4) Display.sprite = Four;
        else if(n==5) Display.sprite = Five;
        else if(n==6) Display.sprite = Six;
        else if(n==7) Display.sprite = Seven;
        else if(n==8) Display.sprite = Eight;
        else if(n==9) Display.sprite = Nine;
    }
}
