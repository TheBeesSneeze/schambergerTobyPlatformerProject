using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinBehavior : MonoBehaviour
{
    public PlayerBehavior LittleGuy;

    public string SceneName="Level";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //i dont know how to get this code working man
    public void SendPlayerTo(string scene)
    {
        Scene SceneToLoad = SceneManager.GetSceneByName(scene);
        SceneManager.LoadScene(SceneToLoad.name, LoadSceneMode.Additive);
        SceneManager.LoadScene(SceneToLoad.name);
        SceneManager.MoveGameObjectToScene(LittleGuy.gameObject, SceneToLoad);
                     
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag=="Player")
        {
            Debug.Log("Win condition met");
        }
    }
}
