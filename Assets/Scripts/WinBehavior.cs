using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinBehavior : MonoBehaviour
{
    //public PlayerBehavior LittleGuy;

    public int SceneToLoad = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //i dont know how to get this code working man

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag=="Player")
        {
            Debug.Log("Win condition met");
            SceneManager.LoadScene(SceneToLoad);
            //SceneManager.MoveGameObjectToScene(LittleGuy.gameObject, SceneToLoad);
        }
    }
}
