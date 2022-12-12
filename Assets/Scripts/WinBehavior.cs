using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinBehavior : MonoBehaviour
{
    //public PlayerBehavior LittleGuy;

    public GameController GC;

    public int SceneToLoad = 0;
    public AudioSource WinSound;
    
    //i dont know how to get this code working man

    void Start()
    {
        GC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameController>();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag=="Player")
        {
            Debug.Log("Win condition met");

            if(GC.SoundPlaying)
            {
                WinSound.Play();
            }
            Invoke("MoveOn",2f);
            //SceneManager.MoveGameObjectToScene(LittleGuy.gameObject, SceneToLoad);
        }
    }

    public void MoveOn()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
