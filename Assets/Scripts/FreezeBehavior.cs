using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBehavior : MonoBehaviour
{
    public GameController GC;
    //Player can enter a freeze box by pressing space or e, dashing inside, or shooting in from another freeze box
    public SpriteRenderer ThisRenderer;
    public Sprite OffSprite;
    public Sprite OnSprite;

    public PlayerBehavior LittleGuy;

    private SpriteRenderer Arrow_Renderer;
    private SpriteRenderer E_Renderer;

    public GameObject StaticOverlay;

    public AudioSource BackgroundMusic;
    private float MusicPitch=1;
    public float ReversePitch=-0.35f;

    public Vector2 StartingPoint;
    public Vector2 EndingPoint;

    public float Speed=1;

    public float TimeOfFreeze;
    public float TimeOfLaunch;
    public float TimeOfPlayerPull;
    public bool LaunchingPlayer=false;

    public Vector2 LaunchForce = new Vector2(10,0);
    public float LaunchSeconds = 1;
    public float PullInPlayerSeconds = 1;

    public enum FreezeState
    {
        Idle,
        IdleWithPlayer,
        Moving, //moving only happens after launching. it stops after it gets back to its start point
        MovingPlayer,
        PullingInPlayer
    }
    public FreezeState MyState;

    // Start is called before the first frame update
    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameController>();
        LittleGuy=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();

        //Get sprite renderers of those silly little prompts
        ThisRenderer = GetComponent<SpriteRenderer>();
        Arrow_Renderer = this.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        E_Renderer = this.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();

        Arrow_Renderer.enabled = false;
        E_Renderer.enabled = false;

        StartingPoint = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(MyState!=FreezeState.Idle)
        {
            //move without player
            if(MyState==FreezeState.Moving)
            {
                MoveAcross();
                float x = gameObject.transform.position.x;
                float y = gameObject.transform.position.y;

                //Stop when back at starting point
                if(Mathf.Approximately(x,StartingPoint.x)&&Mathf.Approximately(y,StartingPoint.y))
                {
                    MyState=FreezeState.Idle;
                    UpdateSprite();
                }
            }
            //move with player and suck them in
            else if(MyState==FreezeState.MovingPlayer)
            {
                MoveAcross();
            }
            else if(MyState==FreezeState.PullingInPlayer)
            {
                LittleGuy.transform.position = Vector2.MoveTowards(LittleGuy.transform.position,transform.position,0.03f);
                LittleGuy.PlayerRB.velocity=Vector2.zero;

                if(LittleGuy.transform.position == transform.position && Time.time - TimeOfPlayerPull >= PullInPlayerSeconds)
                {
                    TimeOfFreeze=Time.time;
                    MyState=FreezeState.MovingPlayer;
                    UpdateSprite();
                }
            }

            if(LaunchingPlayer)
            {
                Launching();
            }
            else if(Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.E))
            {
                //Start the freeze
                if(MyState==FreezeState.IdleWithPlayer)
                {
                    MyState=FreezeState.PullingInPlayer;
                    UpdateSprite();
                    FreezePlayer();
                    Arrow_Renderer.enabled = false;
                    LittleGuy.JumpBoosting=true;
                    TimeOfPlayerPull=Time.time;
                }

                //Launch that little fucker
                else if(MyState==FreezeState.MovingPlayer)
                {
                    TimeOfLaunch=Time.time;
                    LaunchPlayer();
                    LittleGuy.LittleGuyAnimator.enabled=true;
                    MyState=FreezeState.Moving;
                    LittleGuy.JumpBoosting=true;
                    Invoke("UpdateSprite",LaunchSeconds+0.1f);
                }
            }
            UpdateMusic();
        }
        //mental fog
    }

    //Moves from start point to end point. uses a sin wave to kinda slow down towards the enda it
    public float Point; //public because i wanna see it :)
    public void MoveAcross()
    {
        //every time i try to add complicated math, it always explodes in my face, but here we are, not learning from our lessons

        //point is a number between -1.05 and 1.05
        Point = Mathf.Cos(Speed*Mathf.PI*(Time.time-TimeOfFreeze)) * 1.05f;
        //Scale point to a number between 0 and 1
        Point = (Point-1)/-2;
        transform.position = Vector2.Lerp(StartingPoint,EndingPoint,Point);
        //i had to open up desmos to figure this one out. unbelievable
    }


    //Stops player in their tracks. true for frozen. false for unfreeze
    public void FreezePlayer()
    {
        Debug.Log("Freeze");
        LittleGuy.LittleGuyAnimator.enabled=false;
        //LittleGuy.PlayerRB.simulated=!Frozen;

        //Parent player to box
        LittleGuy.PlayerRB.bodyType=RigidbodyType2D.Kinematic;
        LittleGuy.PlayerRB.useFullKinematicContacts=true;
        LittleGuy.gameObject.transform.SetParent(gameObject.transform,true);
        LittleGuy.Frozen=true;
    }

    private Vector2 OldPosition;
    //Starts launch, should only be called once
    public void LaunchPlayer()
    {
        Debug.Log("Launch. Force: " + LaunchForce.ToString());
        OldPosition = LittleGuy.gameObject.transform.position;
        LittleGuy.gameObject.transform.parent = null;

        LaunchingPlayer = true;
    }

    public void Launching()
    {
        if(LittleGuy.JumpBoosting)
        {
            float LTime = (Time.time-TimeOfLaunch) / LaunchSeconds;
            if(LTime <= 1)
            {
                LittleGuy.gameObject.transform.position = Vector2.Lerp(OldPosition,OldPosition+LaunchForce,LTime);
            }
            //Ends the launch
            else
            {
                Debug.Log("Ending Launch");
                LittleGuy.PlayerRB.bodyType=RigidbodyType2D.Dynamic;
                LittleGuy.PlayerRB.useFullKinematicContacts=false;

                LittleGuy.PlayerRB.AddForce(LaunchForce*100);
                LittleGuy.JumpBoosting=false;
                LittleGuy.CanDoubleJump = false;
                LittleGuy.Dashing=true;
                LittleGuy.Frozen=false; 
                
                Arrow_Renderer.enabled = false;

                LaunchingPlayer=false;

                UpdateSprite();
            }
        }
    }

    public void UpdateSprite()
    {
        
        if(MyState==FreezeState.MovingPlayer || MyState==FreezeState.PullingInPlayer)
        {
            ThisRenderer.sprite = OnSprite;
        }
        else
        {
            ThisRenderer.sprite = OffSprite;
        }

        if(MyState==FreezeState.MovingPlayer)
        {
            Arrow_Renderer.enabled=(true);
            E_Renderer.enabled = true;
            StaticOverlay.SetActive(true);
        }
        else 
        {
            Arrow_Renderer.enabled=(false);
            E_Renderer.enabled = false;

            if(!LittleGuy.JumpBoosting && !LittleGuy.Frozen) //because sometimes player falls into another box
            {
                StaticOverlay.SetActive(false);
            }
            
        }

        MusicPitch = BackgroundMusic.pitch; //little command that has no business here but fits nicely anyways
    }

    public void UpdateMusic()
    {
        //if(Little)
        if(GC.MusicPlaying)
        {
            if(MyState==FreezeState.MovingPlayer || MyState==FreezeState.PullingInPlayer)
            {
                BackgroundMusic.pitch = Mathf.Lerp(MusicPitch,ReversePitch, (Time.time-TimeOfFreeze)/2);
            }
            else if(LaunchingPlayer)
            {
                BackgroundMusic.pitch = Mathf.Lerp(MusicPitch,2.0f, (Time.time-TimeOfLaunch));
            }
            else if(MyState!=FreezeState.Idle)
            {
                BackgroundMusic.pitch = Mathf.Lerp(MusicPitch,1, Time.time%1);
            }
            else
            {
                BackgroundMusic.pitch=1;
            }
        }
    }

    //player cant jump while in the uh thing
    public bool CouldJump = false;
    public bool CouldDash = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {        
        if (collider.gameObject.tag == "Player")
        {
            CouldJump = LittleGuy.CanJump;
            CouldDash = LittleGuy.CanDoubleJump;

            LittleGuy.CanJump = false;
            LittleGuy.CanDoubleJump = false;

            //if player already boosting, they in get freezed in there
            if((LittleGuy.JumpBoosting || LittleGuy.Dashing) && MyState==FreezeState.Idle)
            {
                Debug.Log("Caught!");
                MyState=FreezeState.PullingInPlayer;
                FreezePlayer();
                LittleGuy.JumpBoosting=false;
            }

            if(MyState==FreezeState.Idle)
            {
                MyState=FreezeState.IdleWithPlayer;
            }

            
        }

        
    
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {

            if(MyState==FreezeState.IdleWithPlayer)
            {
                MyState=FreezeState.Idle;
                
                if(GC.MusicPlaying)
                {
                    if(!LittleGuy.JumpBoosting) BackgroundMusic.pitch = 1;
                }

                LittleGuy.CanJump = CouldJump;
                LittleGuy.CanDoubleJump = CouldDash;
            }
        }

    }
}
