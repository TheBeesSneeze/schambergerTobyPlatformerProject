using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragilePlatformBehavior : MonoBehaviour
{
    public Collider2D collider;
    public SpriteRenderer SpriteRender;
    public Vector4 BaseColor;

    public float TimeToBreak=0.5f;
    public float TimeToComeBack=5;
    private float TimeOfCollision;

    public bool WillBreak = false;
    public bool Broken = false;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        SpriteRender = gameObject.GetComponent<SpriteRenderer>();
        BaseColor=SpriteRender.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(WillBreak)
        {
            if(TimeOfCollision + TimeToBreak <= Time.time)
            {
                WillBreak = false;
                Broken = true;

                //SpriteRender.material.color = new Vector4(0,0,0,0);
                SpriteRender.enabled=false;
                collider.enabled=false;
            }
        }
        if(Broken)
        {
            if(TimeOfCollision + TimeToBreak + TimeToComeBack <=Time.time)
            {
                Broken = false;

                //SpriteRender.material.color = new Vector4(0,0,0,0);
                SpriteRender.enabled=true;
                collider.enabled=true;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="Player" && !WillBreak)
        {
            WillBreak = true;
            TimeOfCollision = Time.time;
        }
    }
}
